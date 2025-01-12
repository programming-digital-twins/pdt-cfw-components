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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

using LabBenchStudios.Pdt.Util;
namespace LabBenchStudios.Pdt.Connection
{
    /// <summary>
    /// This class provides a simple generic persistence functions that
    /// the sub-class can use for generalized validation and processing
    /// of persistence data.
    /// 
    /// The underlying sub-class will provide the persistence layer
    /// connectivity and interaction logic.
    /// 
    /// As a general feature irrespective of the underlying data store,
    /// and to avoid an over-abundance of I/O operations on the persistence
    /// layer connection, storage requests will be queued and then
    /// written to the data store at the rate of approx. once per minute.
    /// If a read request is issued before the queue can drain and write
    /// any items to the data store, a write will be forced, and the read
    /// will then commence against the stored file.
    /// 
    /// If any incoming read operation is attempting to retrieve the latest
    /// item only, it will be read from the internal cacheList, which will
    /// store only one instance of the latest object type.
    /// For instance, the latest written SensorData will be kept on hand,
    /// as will the latest SystemPerformanceData and ActuatorData. Should
    /// a load request meet the parameters of that which is cached, no
    /// disk I/O will be incurred.
    /// 
    /// Both of these capabilities can be disabled via the underlying sub-class.
    /// 
    /// </summary>
    public abstract class BasePersistenceConnector : IPersistenceConnector
    {
        // static consts


        // private member vars

        private string productName = ConfigConst.PRODUCT_NAME;

        private bool isConnected = false;

        private int typeID = ConfigConst.DEFAULT_TYPE_ID;

        private FileUtil.PersistenceDataTypeEnum persistenceDataType = FileUtil.PersistenceDataTypeEnum.IotData;

        private ISystemStatusEventListener eventListener = null;
        private IDataContextEventListener dataListener = null;

        private ConnectionStateData connStateData = null;

        // constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="persistenceDataType"></param>"
        /// <param name="typeID"></param>
        public BasePersistenceConnector(FileUtil.PersistenceDataTypeEnum persistenceDataType, int typeID) :
            this(ConfigConst.PRODUCT_NAME, persistenceDataType, typeID, null)
        {
            // nothing to do
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="persistenceDataType"></param>"
        /// <param name="typeID"></param>
        /// <param name="eventListener"></param>
        public BasePersistenceConnector(
            string productName, FileUtil.PersistenceDataTypeEnum persistenceDataType, int typeID, ISystemStatusEventListener eventListener)
        {
            if (!string.IsNullOrWhiteSpace(productName))
            {
                this.productName = productName;
            }

            this.persistenceDataType = persistenceDataType;
            this.typeID = typeID;

            this.SetEventListener(eventListener);
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ConnectClient()
        {
            this.isConnected = this.HandleConnect();

            return this.isConnected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool DisconnectClient()
        {
            if (this.HandleDisconnect())
            {
                this.isConnected = false;
            }

            return this.isConnected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsClientConnected()
        {
            return this.isConnected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public string CreateDataStoreName(string cacheName)
        {
            switch (this.persistenceDataType)
            {
                case FileUtil.PersistenceDataTypeEnum.Historian:
                    return this.HandleCreateHistorianCacheName(cacheName);

                case FileUtil.PersistenceDataTypeEnum.Prediction:
                    return this.HandleCreatePredictionCacheName(cacheName);

                case FileUtil.PersistenceDataTypeEnum.IotData:
                    return this.HandleCreateDataStoreName(cacheName);

                case FileUtil.PersistenceDataTypeEnum.Text:
                    return this.HandleCreateTextCacheName(cacheName);
            }

            // should never get here
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDataStoreUri()
        {
            switch (this.persistenceDataType)
            {
                case FileUtil.PersistenceDataTypeEnum.Historian:
                    return this.HandleGetHistorianCacheUri();

                case FileUtil.PersistenceDataTypeEnum.Prediction:
                    return this.HandleGetPredictionCacheUri();

                case FileUtil.PersistenceDataTypeEnum.IotData:
                    return this.HandleGetObjectStoreUri();

                case FileUtil.PersistenceDataTypeEnum.Text:
                    return this.HandleGetTextCacheUri();
            }

            // should never get here
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDataContextEventListener GetDataListener()
        {
            return this.dataListener;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ISystemStatusEventListener GetEventListener()
        {
            return this.eventListener;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FileUtil.PersistenceDataTypeEnum GetPersistenceDataType()
        {
            return this.persistenceDataType;
        }

        /// <summary>
		/// Attempts to retrieve the named data instance from the persistence server.
        /// Will return null if there's no data matching the given type with the
		/// given parameters.
        /// 
        /// </summary>
		/// <param name="cacheName"> The name of the cache to load. The filename will be auto-generated from the name.</param>
		/// <returns type="List<DataCacheEntryContainer>">The data instance(s) associated with the lookup parameters.</returns>
		public List<DataCacheEntryContainer> LoadDataCache(string cacheName)
        {
            if (! string.IsNullOrWhiteSpace(cacheName))
            {
                return this.HandleLoadDataCache(cacheName);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
		/// <param name="cacheName"> The name of the cache to load. The filename will be auto-generated from the name.</param>
        /// <returns></returns>
        public string LoadTextDataCache(string cacheName)
        {
            Console.WriteLine($"Attempting to load text data. Name: {cacheName}.");

            return this.HandleLoadTextDataCache(cacheName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"> The name of the cache to load. The filename will be auto-generated from the name.</param>
        /// <returns></returns>
        public RequestResponseData LoadRequestResponseData(string cacheName)
        {
            Console.WriteLine($"Attempting to load request response data. Name: {cacheName}.");

            return this.HandleLoadRequestResponseData(cacheName);
        }

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start timeStamp. This will be validated by the TimeDuration class.
		 * @param endDate The end timeStamp. This will be validated by the TimeDuration class.
		 * the current time is used.
		 * @return List<ActuatorData> The data instance(s) associated with the lookup parameters.
		 */
        public List<ActuatorData> LoadActuatorData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            TimeDuration duration = new TimeDuration(startDate, endDate);

            Console.WriteLine($"Attempting to load actuator data. Play: {duration.GetStartTime()}. End: {duration.GetEndTime()}.");

            return this.HandleLoadActuatorData(resource, duration);
        }

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start timeStamp. This will be validated by the TimeDuration class.
		 * @param endDate The end timeStamp. This will be validated by the TimeDuration class.
		 * @return List<ConnectionStateData> The data instance(s) associated with the lookup parameters.
		 */
        public List<ConnectionStateData> LoadConnectionStateData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            TimeDuration duration = new TimeDuration(startDate, endDate);

            Console.WriteLine($"Attempting to load connection playbackState data. Play: {duration.GetStartTime()}. End: {duration.GetEndTime()}.");

            return this.HandleLoadConnectionStateData(resource, duration);
        }

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start timeStamp. This will be validated by the TimeDuration class.
		 * @param endDate The end timeStamp. This will be validated by the TimeDuration class.
		 * @return List<SensorData> The data instance(s) associated with the lookup parameters.
		 */
        public List<SensorData> LoadSensorData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            TimeDuration duration = new TimeDuration(startDate, endDate);

            Console.WriteLine($"Attempting to load sensor data. Play: {duration.GetStartTime()}. End: {duration.GetEndTime()}.");

            return this.HandleLoadSensorData(resource, duration);
        }

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start timeStamp. This will be validated by the TimeDuration class.
		 * @param endDate The end timeStamp. This will be validated by the TimeDuration class.
		 * @return List<SystemPerformanceData> The data instance(s) associated with the lookup parameters.
		 */
        public List<SystemPerformanceData> LoadSystemPerformanceData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            TimeDuration duration = new TimeDuration(startDate, endDate);

            Console.WriteLine($"Attempting to load system performance data. Play: {duration.GetStartTime()}. End: {duration.GetEndTime()}.");

            return this.HandleLoadSystemPerformanceData(resource, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public void SetDataListener(IDataContextEventListener listener)
        {
            if (listener != null)
            {
                this.dataListener = listener;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public void SetEventListener(ISystemStatusEventListener listener)
        {
            if (listener != null)
            {
                this.eventListener = listener;

                this.connStateData = new ConnectionStateData();
                this.connStateData.SetTypeCategoryID(ConfigConst.SYSTEM_TYPE_CATEGORY);
                this.connStateData.SetTypeID(this.typeID);
                this.connStateData.SetMessage($"Persistence connector initialized for {this.productName}: {this.persistenceDataType.ToString()}.");

                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
            }
        }

        /// <summary>
		/// Attempts to write the source data instance to the persistence server.
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="dataCache"></param>
        /// <returns type="int">On success, returns the total bytes stored. If no bytes
        /// are written and no errors, returns 0. If errored, returns -1.</returns>
        public int StoreTextDataCache(string cacheName, string dataCache)
        {
            if (!string.IsNullOrEmpty(dataCache))
            {
                string fileName = this.CreateDataStoreName(cacheName);

                return this.HandleStoreTextDataCache(fileName, dataCache);
            } else {
                Console.WriteLine($"No cacheList data to write to cacheList name {cacheName}. Ignoring store request.");
            }

            return 0;
        }

        /// <summary>
		/// Attempts to write the source data instance to the persistence server.
        /// </summary>
        /// <param name="historianCache"></param>
        /// <returns type="int">On success, returns the total bytes stored. If no bytes
        /// are written and no errors, returns 0. If errored, returns -1.</returns>
        public int StoreDataCache(IDataHistorianCache historianCache)
        {
            if (historianCache != null)
            {
                string cacheName = historianCache.GetCacheName();
                string fileName  = historianCache.GetStorageFileName();

                List<DataCacheEntryContainer> cacheEntries = historianCache.GetCacheEntries();

                if (cacheEntries.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        return this.HandleStoreDataCache(cacheName, cacheEntries);
                    } else
                    {
                        return this.HandleStoreDataCache(historianCache);
                    }
                } else
                {
                    Console.WriteLine($"No cacheList data to write to cacheList name {cacheName}. Ignoring store request.");
                }
            }

            return 0;
        }

        /// <summary>
		/// Attempts to write the source data instance to the persistence server.
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="cacheList"></param>
        /// <returns type="int">On success, returns the total bytes stored. If no bytes
        /// are written and no errors, returns 0. If errored, returns -1.</returns>
        public int StoreDataCache(string cacheName, List<DataCacheEntryContainer> cacheList)
        {
            if (!string.IsNullOrWhiteSpace(cacheName))
            {
                if (cacheList != null && cacheList.Count > 0)
                {
                    return this.HandleStoreDataCache(cacheName, cacheList);
                }
            }

            Console.WriteLine($"No cacheList data to write to cacheList name {cacheName}. Ignoring store request.");

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool StoreData(RequestResponseData data)
        {
            if (data != null && data.HasSessionID())
            {
                return this.HandleStoreData(data);
            } else
            {
                Console.WriteLine($"Request response data is null or does not have a session ID (needed for file name generation).");
            }

            return false;
        }

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, ActuatorData data)
        {
            if (data != null)
            {
                qos = this.GetValidQosLevel(qos);

                return this.HandleStoreData(resource, qos, data);
            } else
            {
                Console.WriteLine("ActuatorData reference is null. Ignoring store request.");
                return false;
            }
        }

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, ConnectionStateData data)
        {
            if (data != null)
            {
                qos = this.GetValidQosLevel(qos);

                return this.HandleStoreData(resource, qos, data);
            } else
            {
                Console.WriteLine("ActuatorData reference is null. Ignoring store request.");
                return false;
            }
        }

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, SensorData data)
        {
            if (data != null)
            {
                qos = this.GetValidQosLevel(qos);

                return this.HandleStoreData(resource, qos, data);
            } else
            {
                Console.WriteLine("ActuatorData reference is null. Ignoring store request.");
                return false;
            }
        }

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, SystemPerformanceData data)
        {
            if (data != null)
            {
                qos = this.GetValidQosLevel(qos);

                return this.HandleStoreData(resource, qos, data);
            } else
            {
                Console.WriteLine("ActuatorData reference is null. Ignoring store request.");
                return false;
            }
        }


        // protected

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected ConnectionStateData GetConnectionStateCopy()
        {
            ConnectionStateData updatedConnStateData =
                new ConnectionStateData(
                    this.connStateData.GetName(),
                    this.connStateData.GetDeviceID(),
                    this.connStateData.GetHostName(),
                    this.connStateData.GetHostPort());

            updatedConnStateData.UpdateData(this.connStateData);

            return updatedConnStateData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qos"></param>
        /// <returns></returns>
        protected int GetValidQosLevel(int qos)
        {
            if (qos < 0 || qos > 3)
            {
                qos = 0;
            }

            return qos;
        }

        // protected template methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract int GetPersistenceSystemTypeID();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract bool HandleConnect();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract bool HandleDisconnect();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected abstract string HandleCreateHistorianCacheName(string cacheName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected abstract string HandleCreatePredictionCacheName(string cacheName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected abstract string HandleCreateTextCacheName(string cacheName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected abstract string HandleCreateDataStoreName(string cacheName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract string HandleGetHistorianCacheUri();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract string HandleGetPredictionCacheUri();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract string HandleGetTextCacheUri();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract string HandleGetObjectStoreUri();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected abstract string HandleLoadTextDataCache(string cacheName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected abstract List<DataCacheEntryContainer> HandleLoadDataCache(string cacheName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        protected abstract RequestResponseData HandleLoadRequestResponseData(string cacheName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected abstract List<ActuatorData> HandleLoadActuatorData(ResourceNameContainer resource, TimeDuration duration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected abstract List<ConnectionStateData> HandleLoadConnectionStateData(ResourceNameContainer resource, TimeDuration duration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected abstract List<SensorData> HandleLoadSensorData(ResourceNameContainer resource, TimeDuration duration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected abstract List<SystemPerformanceData> HandleLoadSystemPerformanceData(ResourceNameContainer resource, TimeDuration duration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dataCache"></param>
        /// <returns></returns>
        protected abstract int HandleStoreTextDataCache(string fileName, string dataCache);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        protected abstract int HandleStoreDataCache(string cacheName, List<DataCacheEntryContainer> dataCache);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        protected abstract int HandleStoreDataCache(IDataHistorianCache historianCache);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract bool HandleStoreData(RequestResponseData data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="qos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract bool HandleStoreData(ResourceNameContainer resource, int qos, ActuatorData data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="qos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract bool HandleStoreData(ResourceNameContainer resource, int qos, ConnectionStateData data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="qos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract bool HandleStoreData(ResourceNameContainer resource, int qos, SensorData data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="qos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract bool HandleStoreData(ResourceNameContainer resource, int qos, SystemPerformanceData data);

    }

}
