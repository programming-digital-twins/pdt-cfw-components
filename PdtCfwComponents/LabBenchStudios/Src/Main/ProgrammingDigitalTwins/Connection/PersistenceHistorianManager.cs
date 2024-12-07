using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Connection
{
    /// <summary>
    /// This class is responsible for managing all locally accessible historical
    /// data, and uses an IPersistenceConnector implementation as the underlying
    /// store / load functionality.
    /// 
    /// As this class will manage storing, loading, and ultimately caching of
    /// data, there are some limits that need to be established. The following
    /// assumptions are based on simple simulation runs using the Edge Device
    /// App when generating no more than two messages for each poll cycle.
    /// 
    /// Anticipated storage needs for a single IotDataContext object, when converted
    /// to JSON, is ~500 bytes. A typical simulator will generate a single sensor
    /// payload (about 500 bytes) and a single system performance payload (also about
    /// 500 bytes) every poll cycle. Storage consumption can therefore be estimated
    /// as follows on a per simulator basis (with the given assumptions):
    ///   - poll cycle: 5 seconds
    ///   - sensor data: ~500 bytes
    ///   - perf data: ~500 bytes
    ///   - data / poll cycle: ~1000 bytes
    ///   - poll cycles / min: 12
    ///   - data / min: 12 * 1000 = 12,000 bytes
    ///   - data / hour: 60 * 12,000 bytes = 720,000 bytes
    ///   - data / day: 17,280,000 bytes
    /// 
    /// In summary, a single simulator operating at a 5 second poll rate for an entire
    /// day should generate (and required storage for) approx. 16.8 MB
    /// 
    /// Using this figure for a sample run of a single simulator yields the following:
    ///   - ~0.7 MB / hr per simulator
    ///   - ~16.8 MB / day per simulator
    ///   - ~117.6 MB / week per simulator
    ///   ~ ~510 MB / month per simulator
    ///   - ~6.12 GB / year per simulator
    ///   
    /// Using this figure for a sample run of 5 simulators yields the following:
    ///   - ~3.5 MB / hr for 5 simulators
    ///   - ~84.0 MB / day for 5 simulators
    ///   - ~588.0 MB / week for 5 simulators
    ///   ~ ~2.55 GB / month for 5 simulators
    ///   - ~30.6 GB / year for 5 simulators
    ///   
    /// While storage durations are configurable within the coded properties configuration,
    /// the default functionality for this class (and its underlying in memory cache) will
    /// center on the following rules:
    ///   - 1 data file (.dat) per device (simulator) per load request (1 day's worth of data)
    ///   - Max of 10 unique devices (simulators) in cache at any given time
    ///   - Estimated memory requirements for operation of the internal cache: ~168 MB
    /// </summary>
    public class PersistenceHistorianManager : IDataHistorian
    {
        private bool initializeBackingFileStore = true;
        private string backingFilePath = ConfigConst.DEFAULT_FILE_STORAGE_PATH;

        private int maxItemsPerType = ConfigConst.DEFAULT_MAX_CACHED_ITEMS;
        private long maxCacheSize = ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB;

        private long totalHeapMemory = 0L;

        private IDictionary<string, List<ActuatorData>> actuatorDataCache = null;
        private IDictionary<string, List<SensorData>> sensorDataCache = null;
        private IDictionary<string, List<SystemPerformanceData>> sysPerfDataCache = null;

        private ISystemStatusEventListener eventListener = null;

        private IPersistenceConnector persistenceConnector = null;

        /// <summary>
        /// 
        /// </summary>
        public PersistenceHistorianManager() :
            this(null, null,
                 ConfigConst.DEFAULT_MAX_CACHED_ITEMS,
                 ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB)
        {
            // nothing to do - delegates to other constructor
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="listener"></param>
        /// <param name="maxItemsPerType"></param>
        /// <param name="maxCacheSize"></param>
        public PersistenceHistorianManager(
            string filePath, ISystemStatusEventListener listener,
            int maxItemsPerType, long maxCacheSize) : base()
        {
            if (maxItemsPerType > 0 && maxItemsPerType <= ConfigConst.DEFAULT_MAX_CACHED_ITEMS)
            {
                this.maxItemsPerType = maxItemsPerType;
            }

            if (maxCacheSize > 0L && maxCacheSize <= ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB)
            {
                this.maxCacheSize = maxCacheSize;
            }

            this.actuatorDataCache = new Dictionary<string, List<ActuatorData>>(this.maxItemsPerType);
            this.sensorDataCache = new Dictionary<string, List<SensorData>>(this.maxItemsPerType);
            this.sysPerfDataCache = new Dictionary<string, List<SystemPerformanceData>>(this.maxItemsPerType);

            this.totalHeapMemory = GC.GetTotalMemory(false);

            if (this.initializeBackingFileStore) {
                this.InitFileStorage();
            }
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetTotalMemory()
        {
            return this.totalHeapMemory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetMaxCacheSize()
        {
            return this.maxCacheSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public string FillSensorDataCache(string bucketName)
        {
            DateTime startDate = DateTime.Now;
            startDate.AddDays(-1);

            DateTime endDate = DateTime.Now;

            return this.FillSensorDataCache(bucketName, startDate, endDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public string FillSensorDataCache(string bucketName, DateTime startDate, DateTime endDate)
        {
            string cacheName = ConfigConst.SENSOR_DATA_PERSISTENCE_NAME;

            if (this.sensorDataCache.ContainsKey(cacheName))
            {
                this.sensorDataCache.Remove(cacheName);
            }

            if (this.persistenceConnector != null)
            {
                ResourceNameContainer resource = new ResourceNameContainer();
                resource.PersistenceName = bucketName;
                
                List<SensorData> sensorDataList =
                    this.persistenceConnector.LoadSensorData(resource, ConfigConst.DEFAULT_TYPE_ID, startDate, endDate);

                if (sensorDataList != null && sensorDataList.Count > 0)
                {
                   this.sensorDataCache.Add(cacheName, sensorDataList);
                }
            }

            return cacheName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public bool IsCacheReplaying(string cacheName)
        {
            // TODO: implement this

            return false;
        }

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param typeID The type ID of the data to retrieve.
		 * @param startDate The start date (null if narrowing is not needed).
		 * @param endDate The end date (null if narrowing is not needed).
		 * @return List<ActuatorData> The data instance(s) associated with the lookup parameters.
		 */
        public List<ActuatorData> LoadActuatorData(ResourceNameContainer resource, int typeID, DateTime startDate, DateTime endDate)
        {
            if (this.persistenceConnector != null) {
                Console.WriteLine($"Loading actuator data. Start: {startDate}. End: {endDate}.");
                return this.persistenceConnector.LoadActuatorData(resource, typeID, startDate, endDate);
            } else {
                Console.WriteLine($"No persistence connector. Can't load actuator data. Start: {startDate}. End: {endDate}.");
                return null;
            }
        }

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start date (null if narrowing is not needed).
		 * @param endDate The end date (null if narrowing is not needed).
		 * @return List<ConnectionStateData> The data instance(s) associated with the lookup parameters.
		 */
        public List<ConnectionStateData> LoadConnectionStateData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            if (this.persistenceConnector != null) {
                Console.WriteLine($"Loading connection state data. Start: {startDate}. End: {endDate}.");
                return this.persistenceConnector.LoadConnectionStateData(resource, startDate, endDate);
            } else {
                Console.WriteLine($"No persistence connector. Can't load connection state data. Start: {startDate}. End: {endDate}.");
                return null;
            }
        }

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param typeID The type ID of the data to retrieve.
		 * @param startDate The start date (null if narrowing is not needed).
		 * @param endDate The end date (null if narrowing is not needed).
		 * @return List<SensorData> The data instance(s) associated with the lookup parameters.
		 */
        public List<SensorData> LoadSensorData(ResourceNameContainer resource, int typeID, DateTime startDate, DateTime endDate)
        {
            if (this.persistenceConnector != null) {
                Console.WriteLine($"Loading sensor data. Start: {startDate}. End: {endDate}.");
                return this.persistenceConnector.LoadSensorData(resource, typeID, startDate, endDate);
            } else {
                Console.WriteLine($"No persistence connector. Can't load sensor data. Start: {startDate}. End: {endDate}.");
                return null;
            }
        }

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start date (null if narrowing is not needed).
		 * @param endDate The end date (null if narrowing is not needed).
		 * @return List<SystemPerformanceData> The data instance(s) associated with the lookup parameters.
		 */
        public List<SystemPerformanceData> LoadSystemPerformanceData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            if (this.persistenceConnector != null) {
                Console.WriteLine($"Loading system performance data. Start: {startDate}. End: {endDate}.");
                return this.persistenceConnector.LoadSystemPerformanceData(resource, startDate, endDate);
            } else {
                Console.WriteLine($"No persistence connector. Can't load system performance data. Start: {startDate}. End: {endDate}.");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="speed"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public string StartReplayCache(string cacheName, float speed, IDataContextEventListener listener)
        {
            // TODO: implement this

            return cacheName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public string StopReplayCache(string cacheName)
        {
            // TODO: implement this

            return cacheName;
        }


        // private methods

        /// <summary>
        /// 
        /// </summary>
        private void InitFileStorage()
        {
            this.persistenceConnector = new FilePersistenceConnector();
        }
    }
}
