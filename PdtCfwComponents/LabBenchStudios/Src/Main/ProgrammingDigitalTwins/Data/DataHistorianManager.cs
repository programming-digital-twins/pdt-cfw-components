using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using LabBenchStudios.Pdt.Common;

namespace LabBenchStudios.Pdt.Data
{
    public class DataHistorianManager : IDataHistorian
    {
        private int maxItemsPerType = ConfigConst.DEFAULT_MAX_CACHED_ITEMS;
        private long maxCacheSize = ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB;

        private long totalHeapMemory = 0L;

        private IDictionary<string, List<SensorData>> sensorDataCache = null;

        private IDataLoader dataLoader = null;

        public DataHistorianManager() :
            this(ConfigConst.DEFAULT_MAX_CACHED_ITEMS,
                 ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB)
        {
        }

        public DataHistorianManager(int maxItemsPerType, long maxCacheSize) : base()
        {
            if (maxItemsPerType > 0 && maxItemsPerType <= ConfigConst.DEFAULT_MAX_CACHED_ITEMS)
            {
                this.maxItemsPerType = maxItemsPerType;
            }

            if (maxCacheSize > 0L && maxCacheSize <= ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB)
            {
                this.maxCacheSize = maxCacheSize;
            }

            this.sensorDataCache = new Dictionary<string, List<SensorData>>(this.maxItemsPerType);
            this.totalHeapMemory = GC.GetTotalMemory(false);
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
            if (dataLoader != null)
            {
                this.dataLoader = dataLoader;
            }
        }

    }
}
