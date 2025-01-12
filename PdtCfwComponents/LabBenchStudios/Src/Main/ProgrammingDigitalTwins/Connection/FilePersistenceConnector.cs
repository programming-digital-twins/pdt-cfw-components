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
using System.Runtime.InteropServices;
using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Util;
using MQTTnet.Server;

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

        private string persistenceTypePath = null;

        private string objectStorePath = null;
        private string historianCachePath = null;
        private string predictionCachePath = null;
        private string textCachePath = null;


        private bool isEncoded = false;
        private bool isPathInitialized = false;
        private bool isConnected = false;
        private bool areIncomingMessagesPaused = false;

        private FileUtil.PersistenceDataTypeEnum persistenceDataType = FileUtil.PersistenceDataTypeEnum.IotData;


        // constructors

        /// <summary>
        /// The system user's default temp path will be used.
        /// </summary>
        /// <param name="persistenceDataType"></param>"
        public FilePersistenceConnector(FileUtil.PersistenceDataTypeEnum persistenceDataType) :
            this(null, ConfigConst.PRODUCT_NAME, persistenceDataType, null)
        {
            // nothing to do
        }

        /// <summary>
        /// If storagePath is invalid, the system user's default temp path will be used.
        /// </summary>
        /// <param name="storagePath"></param>
        /// <param name="persistenceDataType"></param>"
        public FilePersistenceConnector(string storagePath, FileUtil.PersistenceDataTypeEnum persistenceDataType) :
            this(storagePath, ConfigConst.PRODUCT_NAME, persistenceDataType, null)
        {
            // nothing to do
        }

        /// <summary>
        /// If storagePath is invalid, the system user's default temp path will be used.
        /// </summary>
        /// <param name="storagePath"></param>
        /// <param name="productName"></param>
        /// <param name="eventListener"></param>
        /// <param name="persistenceDataType"></param>"
        public FilePersistenceConnector(
            string storagePath, string productName, FileUtil.PersistenceDataTypeEnum persistenceDataType, ISystemStatusEventListener eventListener) :
            base(productName, persistenceDataType, ConfigConst.FILE_SYSTEM_TYPE, eventListener)
        {
            // quick init of primary storage path
            if (string.IsNullOrWhiteSpace(storagePath)) {
                storagePath = Path.GetTempPath();
            }

            this.primaryStoragePath = storagePath;

            this.InitStoragePaths();
        }


        // protected methods

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
        protected override string HandleCreateHistorianCacheName(string cacheName)
        {
            return FileUtil.CreateDataCacheFileName(cacheName, this.historianCachePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected override string HandleCreatePredictionCacheName(string cacheName)
        {
            return FileUtil.CreateDataCacheFileName(cacheName, this.predictionCachePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected override string HandleCreateTextCacheName(string cacheName)
        {
            return FileUtil.CreateDataCacheFileName(cacheName, this.textCachePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected override string HandleCreateDataStoreName(string cacheName)
        {
            return FileUtil.CreateDataCacheFileName(cacheName, this.objectStorePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string HandleGetHistorianCacheUri()
        {
            return this.historianCachePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string HandleGetPredictionCacheUri()
        {
            return this.predictionCachePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string HandleGetTextCacheUri()
        {
            return this.textCachePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string HandleGetObjectStoreUri()
        {
            return this.objectStorePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected override List<DataCacheEntryContainer> HandleLoadDataCache(string cacheName)
        {
            string fileName = FileUtil.CreateDataCacheFileName(cacheName, this.historianCachePath);
            int bytesRead = 0;

            Console.WriteLine($"Loading data cache {cacheName} from location {fileName}.");

            try
            {
                string jsonData = FileUtil.ReadDataFromFile(fileName);

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
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected override string HandleLoadTextDataCache(string cacheName)
        {
            string fileName = FileUtil.CreateDataCacheFileName(cacheName, this.textCachePath);
            int bytesRead = 0;

            Console.WriteLine($"Loading data cache {cacheName} from location {fileName}.");

            try
            {
                string data = File.ReadAllText(fileName);

                bytesRead = data.Length;

                if (data != null)
                {
                    Console.WriteLine($"Successfully loaded data cache {cacheName} from location {fileName}. Total bytes: {bytesRead}.");
                } else
                {
                    Console.WriteLine($"No data loaded for data cache {cacheName} from location {fileName}.");
                }

                return data;

            } catch (Exception e)
            {
                Console.WriteLine($"Failed to read data cache {cacheName} from file {fileName}. Error: {e.Message}");
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected override RequestResponseData HandleLoadRequestResponseData(string cacheName)
        {
            string fileName = FileUtil.CreateDataCacheFileName(cacheName, this.predictionCachePath);
            int bytesRead = 0;

            Console.WriteLine($"Loading data cache {cacheName} from location {fileName}.");

            try
            {
                string jsonData = File.ReadAllText(fileName);

                bytesRead = jsonData.Length;

                RequestResponseData data = DataUtil.JsonToRequestResponseData(jsonData);

                if (data != null)
                {
                    Console.WriteLine($"Successfully loaded data cache {cacheName} from location {fileName}. Total bytes: {bytesRead}.");
                } else
                {
                    Console.WriteLine($"No data loaded for data cache {cacheName} from location {fileName}.");
                }

                return data;

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
            string fileName = FileUtil.CreateDataCacheFileName(cacheName, this.objectStorePath, true, true);

            Console.WriteLine($"Storing data cache entries at location {fileName}.");

            return this.HandleStoreDataCache(cacheName, fileName, dataCache, true);
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

            Console.WriteLine($"Storing historian data cache at location {fileName}.");

            List<DataCacheEntryContainer> dataCache = historianCache.GetCacheEntries();

            return this.HandleStoreDataCache(cacheName, fileName, dataCache, true);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dataCache"></param>
        /// <returns></returns>
        protected override int HandleStoreTextDataCache(string fileName, string dataCache)
        {
            Console.WriteLine($"Storing text data cache at location {fileName}.");

            return FileUtil.WriteDataToFile(fileName, dataCache, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="qos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool HandleStoreData(RequestResponseData data)
        {
            if (data != null)
            {
                string fileName = FileUtil.CreateDataCacheFileName(data.GetSessionID(), this.predictionCachePath);

                Console.WriteLine($"Storing prediction data cache at location {fileName}.");

                string jsonData = DataUtil.RequestResponseDataToJson(data);

                int bytesWritten = FileUtil.WriteDataToFile(fileName, jsonData, true);

                if (bytesWritten > 0)
                {
                    return true;
                } else
                {
                    Console.WriteLine($"Failed to write bytes to file {fileName}.");
                }
            } else
            {
                Console.WriteLine("Failed to write RequestResponseData to filesystem. Data object is null.");
            }

            return false;
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
        /// <param name="cacheName"></param>
        /// <param name="fileName"></param>
        /// <param name="dataCache"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        private int HandleStoreDataCache(string cacheName, string fileName, List<DataCacheEntryContainer> dataCache, bool overwrite)
        {
            string jsonData = DataUtil.DataCacheEntryListToJson(dataCache);

            Console.WriteLine($"Storing {jsonData.Length} bytes to data cache {cacheName} at location {fileName}.");

            int bytesWritten = FileUtil.WriteDataToFile(fileName, jsonData, overwrite);

            return bytesWritten;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitStoragePaths()
        {
            // init primary path
            string cachePathName = "";

            switch (base.GetPersistenceDataType())
            {
                case FileUtil.PersistenceDataTypeEnum.Historian:
                    cachePathName = "historian cache";
                    this.historianCachePath = FileUtil.CreateHistorianCacheFilePath(this.primaryStoragePath);

                    break;

                case FileUtil.PersistenceDataTypeEnum.Prediction:
                    cachePathName = "prediction cache";
                    this.predictionCachePath = FileUtil.CreatePredictionCacheFilePath(this.primaryStoragePath);

                    break;

                case FileUtil.PersistenceDataTypeEnum.Text:
                    cachePathName = "text data cache";
                    this.textCachePath = FileUtil.CreateTextCacheFilePath(this.primaryStoragePath);

                    break;

                case FileUtil.PersistenceDataTypeEnum.IotData:
                    cachePathName = "iot data store";
                    this.objectStorePath = FileUtil.CreateObjectStoreFilePath(this.primaryStoragePath);

                    break;
            }

            Console.WriteLine($"Initialized {cachePathName} cache path: {this.persistenceTypePath}");
            this.isPathInitialized = true;
        }

    }

}
