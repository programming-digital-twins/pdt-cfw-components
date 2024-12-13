/**
 * MIT License
 * 
 * Copyright (c) 2024 Andrew D. King
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Connection
{
    /// <summary>
    /// This class provides a simple file persistence connector that can store
    /// and retrieve chunks of IotDataContext objects (and its derivatives) to
    /// and from the filesystem.
    /// 
    /// File structures are as follows:
    /// 
    /// Example 1 (sensor data):
    ///   {primaryStoragePath}/{productName}/dataStore/{deviceID}/{dataType}/{DayAndYear}.json
    ///     -- or --
    ///   {primaryStoragePath}/{productName}/dataStore/{deviceID}/{dataType}/{DayAndYear}.bin
    /// 
    ///   Sample:
    ///     /mnt/pdt/dataStore/edgedevice001/SensorData/2024Dec08.json
    /// 
    /// Example 2 (data cache, which can be comprised of actuator data, sensor data, etc.):
    ///   {primaryStoragePath/{productName}/dataCache/{cacheName}_{DayAndYear}.json
    ///   
    ///   Sample:
    ///     /mnt/pdt/dataCache/SimulatedTrainingExercise01_2024Dec08.json
    ///     
    /// To avoid an over-abundance of I/O operations on any given file and
    /// the filesystem in general, storage requests will be queued and then
    /// written to the data store at the rate of approx. once per minute.
    /// If a read request is issued before the queue can drain and write
    /// any items to the data store, a write will be forced, and the read
    /// will then commence against the stored file.
    /// 
    /// If any incoming read operation is attempting to retrieve the latest
    /// item only, it will be read from the internal cache, which will
    /// store only one instance of the latest object type.
    /// For instance, the latest written SensorData will be kept on hand,
    /// as will the latest SystemPerformanceData and ActuatorData. Should
    /// a load request meet the parameters of that which is cached, no
    /// disk I/O will be incurred.
    /// </summary>
    public class FilePersistenceConnector : BasePersistenceConnector
    {
        // static consts

        public const string DATE_TIME_FORMAT = "ddMMMyyyy";

        // enum declaration

        /// <summary>
        /// 
        /// </summary>
        public enum SerializerTypeEnum
        {
            Binary,
            Json
        }

        // private member vars

        private SerializerTypeEnum serializerType = SerializerTypeEnum.Json;

        private int maxUniqueDevicesLoadCount = 10;
        private int maxHoursPerDeviceLoadCount = 24;

        private string primaryStoragePath = null;
        private string productName = ConfigConst.PRODUCT_NAME;

        private string dataStorePathPrefix = null;
        private string dataCachePathPrefix = null;

        private bool isEncoded = false;
        private bool isPathInitialized = false;
        private bool isConnected = false;
        private bool areIncomingMessagesPaused = false;

        private ISystemStatusEventListener eventListener = null;

        private ConnectionStateData connStateData = null;

        // constructors

        /// <summary>
        /// The system user's default temp path will be used.
        /// </summary>
        public FilePersistenceConnector() : this(null, ConfigConst.PRODUCT_NAME, null)
        {
            // nothing to do
        }

        /// <summary>
        /// If storagePath is invalid, the system user's default temp path will be used.
        /// </summary>
        /// <param name="storagePath"></param>
        public FilePersistenceConnector(string storagePath) : this(storagePath, ConfigConst.PRODUCT_NAME, null)
        {
            // nothing to do
        }

        /// <summary>
        /// If storagePath is invalid, the system user's default temp path will be used.
        /// </summary>
        /// <param name="storagePath"></param>
        /// <param name="productName"></param>
        /// <param name="eventListener"></param>
        public FilePersistenceConnector(string storagePath, string productName, ISystemStatusEventListener eventListener)
        {
            // quick init of primary storage path
            if (string.IsNullOrWhiteSpace(storagePath)) {
                storagePath = Path.GetTempPath();
            }

            this.primaryStoragePath = storagePath;

            this.InitStoragePaths();

            // set the event listener (for system status events)
            this.eventListener = eventListener;

            // create the initial conn state message and send to listener (if non-null)
            this.connStateData = new ConnectionStateData();
            this.connStateData.SetTypeCategoryID(ConfigConst.SYSTEM_TYPE_CATEGORY);
            this.connStateData.SetTypeID(ConfigConst.FILE_SYSTEM_TYPE);
            this.connStateData.SetResourcePrefix(storagePath);
            this.connStateData.SetMessage($"Default file persistence connector initialized.");
            this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
        }


        // public methods



        // protected

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override int GetPersistenceSystemTypeID()
        {
            return ConfigConst.FILE_SYSTEM_TYPE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool HandleConnect()
        {
            if (!this.isPathInitialized)
            {
                this.InitStoragePaths();

                if (!this.isPathInitialized)
                {
                    Console.WriteLine($"Can't initialize file path: {this.primaryStoragePath}. File persistence is disabled.");

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool HandleDisconnect()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected override string HandleCreateCacheFileName(string cacheName)
        {
            return this.CreateAbsFileName(cacheName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected override List<DataCacheEntryContainer> HandleLoadDataCache(string cacheName)
        {
            string fileName = this.CreateAbsFileName(cacheName);
            int bytesRead = 0;

            Console.WriteLine($"Loading data cache {cacheName} from location {fileName}.");

            try
            {
                StreamReader reader = new StreamReader(fileName);
                string jsonData = reader.ReadToEnd();

                bytesRead = jsonData.Length;

                List<DataCacheEntryContainer> dataCache = DataUtil.JsonToDataCacheEntryList(jsonData);

                if (dataCache != null && dataCache.Count > 0)
                {
                    Console.WriteLine($"Successfully loaded data cache {cacheName} from location {fileName}. Total bytes: {bytesRead}.");
                } else
                {
                    Console.WriteLine($"No data loaded for data cache {cacheName} from location {fileName}.");
                }

                return dataCache;

            } catch (Exception e)
            {
                Console.WriteLine($"Failed to read data cache {cacheName} from file {fileName}. Error: {e.Message}");
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected override List<ActuatorData> HandleLoadActuatorData(ResourceNameContainer resource, TimeDuration duration)
        {
            Console.WriteLine("HandleLoadActuatorData not yet implemented.");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected override List<ConnectionStateData> HandleLoadConnectionStateData(ResourceNameContainer resource, TimeDuration duration)
        {
            Console.WriteLine("HandleLoadConnectionStateData not yet implemented.");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected override List<SensorData> HandleLoadSensorData(ResourceNameContainer resource, TimeDuration duration)
        {
            Console.WriteLine("HandleLoadSensorData not yet implemented.");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected override List<SystemPerformanceData> HandleLoadSystemPerformanceData(ResourceNameContainer resource, TimeDuration duration)
        {
            Console.WriteLine("HandleLoadSystemPerformanceData not yet implemented.");

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="dataCache"></param>
        /// <returns></returns>
        protected override int HandleStoreDataCache(string cacheName, List<DataCacheEntryContainer> dataCache)
        {
            string fileName = this.CreateAbsFileName(cacheName, true, true);

            return this.HandleStoreDataCache(cacheName, fileName, dataCache);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="historianCache"></param>
        /// <returns></returns>
        protected override int HandleStoreDataCache(IDataHistorianCache historianCache)
        {
            string cacheName = historianCache.GetCacheName();
            string fileName = historianCache.GetStorageFileName();

            List<DataCacheEntryContainer> dataCache = historianCache.GetCacheEntries();

            return this.HandleStoreDataCache(cacheName, fileName, dataCache);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="qos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool HandleStoreData(ResourceNameContainer resource, int qos, ActuatorData data)
        {
            Console.WriteLine("HandleStoreData for ActuatorData not yet implemented.");
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="qos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool HandleStoreData(ResourceNameContainer resource, int qos, ConnectionStateData data)
        {
            Console.WriteLine("HandleStoreData for ConnectionStateData not yet implemented.");
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="qos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool HandleStoreData(ResourceNameContainer resource, int qos, SensorData data)
        {
            Console.WriteLine("HandleStoreData for SensorData not yet implemented.");
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="qos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool HandleStoreData(ResourceNameContainer resource, int qos, SystemPerformanceData data)
        {
            Console.WriteLine("HandleStoreData for SystemPerformanceData not yet implemented.");
            return true;
        }


        // private

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        private string CreateAbsFileName(ResourceNameContainer resource)
        {
            string deviceID = resource.DeviceName;
            string dataType = ConfigConst.NOT_SET;

            if (resource.IsActuationResource)
            {
                dataType = nameof(ActuatorData);
            } else if (resource.IsConnStateResource)
            {
                dataType = nameof(ConnectionStateData);
            } else if (resource.IsSensingResource)
            {
                dataType = nameof(SensorData);
            } else if (resource.IsSystemResource)
            {
                dataType = nameof(SystemPerformanceData);
            }

            string fileName = DateTime.UtcNow.ToString(DATE_TIME_FORMAT);
            string absFileName = Path.GetFullPath(Path.Combine(this.dataStorePathPrefix, deviceID, dataType, fileName));

            return absFileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        private string CreateAbsFileName(string cacheName)
        {
            return this.CreateAbsFileName(cacheName, false, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="useDate"></param>
        /// <returns></returns>
        private string CreateAbsFileName(string cacheName, bool useDate)
        {
            return this.CreateAbsFileName(cacheName, useDate, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="useDate"></param>
        /// <param name="useJsonExt"></param>
        /// <returns></returns>
        private string CreateAbsFileName(string cacheName, bool useDate, bool useJsonExt)
        {
            string fileName = cacheName;

            if (useDate)
            {
                fileName = fileName + "_" + DateTime.UtcNow.ToString(DATE_TIME_FORMAT);
            }

            if (useJsonExt)
            {
                fileName = fileName + ConfigConst.JSON_EXT;
            }

            string absFileName = Path.GetFullPath(Path.Combine(this.dataCachePathPrefix, cacheName, fileName));

            return absFileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="cacheFileName"></param>
        /// <param name="dataCache"></param>
        /// <returns></returns>
        private int HandleStoreDataCache(string cacheName, string fileName, List<DataCacheEntryContainer> dataCache)
        {
            string jsonData = DataUtil.DataCacheEntryListToJson(dataCache);
            int bytesWritten = 0;

            Console.WriteLine($"Storing {jsonData.Length} bytes to data cache {cacheName} at location {fileName}.");

            try
            {
                StreamWriter writer = new StreamWriter(fileName);
                writer.Write(jsonData);

                bytesWritten = jsonData.Length;

                if (bytesWritten > 0)
                {
                    Console.WriteLine($"Successfully stored data cache {cacheName} to location {fileName}. Total bytes: {bytesWritten}.");
                } else
                {
                    Console.WriteLine($"No data stored for data cache {cacheName} to location {fileName}.");
                }
            } catch (Exception e)
            {
                bytesWritten = -1;

                Console.WriteLine($"Failed to write data cache {cacheName} to file {fileName}. Error: {e.Message}");
            }

            return bytesWritten;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitStoragePaths()
        {
            // init primary path
            Console.WriteLine($"File persistence - using primary storage path: {this.primaryStoragePath}");

            this.isPathInitialized = this.InitStoragePath(this.primaryStoragePath);

            if (this.isPathInitialized)
            {
                Console.WriteLine($"Initialized primary data path {this.primaryStoragePath}");
            } else
            {
                Console.WriteLine($"Failed to initialize primary data path {this.primaryStoragePath}");
            }

            // init data cache and data store paths (sub-dirs of primary path)
            this.dataCachePathPrefix = Path.Combine(this.primaryStoragePath, ConfigConst.DATA_CACHE_NAME);
            this.dataStorePathPrefix = Path.Combine(this.primaryStoragePath, ConfigConst.DATA_STORE_NAME);

            // init data historian cache path
            if (this.InitStoragePath(this.dataCachePathPrefix))
            {
                Console.WriteLine($"Initialized data historian cache path {this.dataCachePathPrefix}");
            } else
            {
                Console.WriteLine($"Failed to initialize data historian cache path {this.dataCachePathPrefix}");
            }

            // init data store path
            if (this.InitStoragePath(this.dataStorePathPrefix))
            {
                Console.WriteLine($"Initialized data store path {this.dataStorePathPrefix}");
            } else
            {
                Console.WriteLine($"Failed to initialize data store path {this.dataStorePathPrefix}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool InitStoragePath(string path)
        {
            // make sure the path exists
            if (!Directory.Exists(path))
            {
                // path doesn't exist - try to create it
                try
                {
                    DirectoryInfo dirInfo = Directory.CreateDirectory(path);

                    Console.WriteLine($"File persistence - path created: {path}. Info: {dirInfo}");
                } catch (Exception e)
                {
                    Console.WriteLine($"Failed to create storage path {path}. Error: {e.Message}");

                    return false;
                }
            } else
            {
                // path already exists - try to access it
                string pathInfo = Directory.GetDirectoryRoot(path);

                Console.WriteLine($"File persistence - path exists: {path}. Info: {pathInfo}");
            }

            return true;
        }

    }

}
