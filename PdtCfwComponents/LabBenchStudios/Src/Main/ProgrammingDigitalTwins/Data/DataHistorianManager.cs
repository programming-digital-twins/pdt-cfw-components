using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Connection;

namespace LabBenchStudios.Pdt.Data
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
    public class DataHistorianManager : IDataHistorian
    {
        private bool initializeBackingFileStore = true;

        private int maxItemsPerType = ConfigConst.DEFAULT_MAX_CACHED_ITEMS;
        private long maxCacheSize = ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB;

        private long totalHeapMemory = 0L;

        private IDictionary<string, List<ActuatorData>> actuatorDataCache = null;
        private IDictionary<string, List<SensorData>> sensorDataCache = null;
        private IDictionary<string, List<SystemPerformanceData>> sysPerfDataCache = null;

        private ISystemStatusEventListener eventListener = null;

        private IPersistenceConnector persistenceConnector = null;
        private IDataLoader dataLoader = null;

        public DataHistorianManager() :
            this(null, null,
                 ConfigConst.DEFAULT_MAX_CACHED_ITEMS,
                 ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB)
        {
        }

        public DataHistorianManager(
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

        public long GetTotalMemory()
        {
            return this.totalHeapMemory;
        }

        public long GetMaxCacheSize()
        {
            return this.maxCacheSize;
        }

        public string FillSensorDataCache(string bucketName)
        {
            DateTime startDate = DateTime.Now;
            startDate.AddDays(-1);

            DateTime endDate = DateTime.Now;

            return this.FillSensorDataCache(bucketName, startDate, endDate);
        }

        public string FillSensorDataCache(string bucketName, DateTime startDate, DateTime endDate)
        {
            string cacheName = ConfigConst.SENSOR_DATA_PERSISTENCE_NAME;

            if (this.sensorDataCache.ContainsKey(cacheName))
            {
                this.sensorDataCache.Remove(cacheName);
            }

            if (this.dataLoader != null)
            {
                ResourceNameContainer resource = new ResourceNameContainer();
                resource.PersistenceName = bucketName;
                
                List<SensorData> sensorDataList =
                    this.dataLoader.LoadSensorData(resource, ConfigConst.DEFAULT_TYPE_ID, startDate, endDate);

                if (sensorDataList != null && sensorDataList.Count > 0)
                {
                   this.sensorDataCache.Add(cacheName, sensorDataList);
                }
            }

            return cacheName;
        }

        public bool IsCacheReplaying(string cacheName)
        {
            // TODO: implement this

            return false;
        }

        public string StartReplayCache(string cacheName, float speed, IDataContextEventListener listener)
        {
            // TODO: implement this

            return cacheName;
        }

        public string StopReplayCache(string cacheName)
        {
            // TODO: implement this

            return cacheName;
        }

        public void SetDataLoader(IDataLoader dataLoader)
        {
            if (this.dataLoader == null) {
                if (dataLoader != null) {
                    Console.WriteLine($"Setting data loader: {dataLoader}");
                    this.dataLoader = dataLoader;
                } else {
                    Console.WriteLine("Data loader parameter is null. Ignoring data loader set request.");
                }
            } else {
                Console.WriteLine("Data loader already initialized. Ignoring data loader set request.");
            }
        }


        // private methods

        private void InitFileStorage()
        {
            this.persistenceConnector = new FilePersistenceConnector();
        }
    }
}
