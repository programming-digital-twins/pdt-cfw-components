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
    /// {primaryStoragePath}/{productName}/dataStore/{deviceID}/{dataType}/{DayAndYear}.json
    ///   -- or --
    /// {primaryStoragePath}/{productName}/dataStore/{deviceID}/{dataType}/{DayAndYear}.bin
    /// 
    /// For example:
    /// /mnt/pdt/dataStore/edgedevice001/ActuatorData/2022-10-29.json
    /// /mnt/pdt/dataStore/edgedevice001/SensorData/2022-10-29.json
    /// /mnt/pdt/dataStore/edgedevice001/SystemPerformanceData/2022-10-29.json
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

        private bool isEncoded = false;
        private bool isPathInitialized = false;
        private bool isConnected = false;
        private bool areIncomingMessagesPaused = false;

        private ISystemStatusEventListener eventListener = null;

        private ConnectionStateData connStateData = null;

        // constructors

        public FilePersistenceConnector() : this(null, ConfigConst.PRODUCT_NAME, null)
        {
            // nothing to do
        }

        public FilePersistenceConnector(string storagePath, string productName, ISystemStatusEventListener eventListener)
        {
            if (string.IsNullOrEmpty(storagePath)) {
                storagePath = ConfigConst.DEFAULT_FILE_STORAGE_PATH;
            }

            this.primaryStoragePath = storagePath;
            this.eventListener = eventListener;

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
                this.InitStoragePath();

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
        /// <param name="resource"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected override List<ActuatorData> HandleLoadActuatorData(ResourceNameContainer resource, TimeDuration duration)
        {
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
            return null;
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

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitStoragePath()
        {
            // make sure the path exists
            if (!Directory.Exists(this.primaryStoragePath)) {
                // path doesn't exist - try to create it
                try {
                    DirectoryInfo dirInfo = Directory.CreateDirectory(this.primaryStoragePath);
                    this.isPathInitialized = true;

                    Console.WriteLine($"File persistence - primary path created: {this.primaryStoragePath}. Info: {dirInfo}");
                } catch (Exception e) {
                    this.isPathInitialized = false;
                    Console.WriteLine($"Failed to create storage path {this.primaryStoragePath}. Error: {e.Message}");
                }
            } else {
                // path already exists - try to access it
                string pathInfo = Directory.GetDirectoryRoot(this.primaryStoragePath);
                this.isPathInitialized = true;

                Console.WriteLine($"File persistence - primary path exists: {this.primaryStoragePath}. Info: {pathInfo}");
            }
        }

    }

}
