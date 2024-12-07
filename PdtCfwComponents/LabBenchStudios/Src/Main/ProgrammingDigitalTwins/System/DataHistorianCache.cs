using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.System
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
    public class DataHistorianCache
    {
        // static

        public enum HistorianCacheState
        {
            Initialized,
            Started,
            Stopped,
            Paused,
            Uninitialized
        }


        // private

        private string cacheName = ConfigConst.NOT_SET;

        private HistorianCacheState state;
        private int curIndex = 0;

        private List<DataCacheEntryContainer> historianCache = null;

        /// <summary>
        /// 
        /// </summary>
        public DataHistorianCache() : this(null)
        {
            // nothing to do - delegates to other constructor
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        public DataHistorianCache(string cacheName) : base()
        {
            if (!string.IsNullOrWhiteSpace(cacheName))
            {
                this.cacheName = cacheName;
            }

            this.historianCache = new List<DataCacheEntryContainer>();
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheEntry"></param>
        public void AddCacheItem(DataCacheEntryContainer cacheEntry)
        {
            if (cacheEntry != null)
            {
                this.historianCache.Add(cacheEntry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheEntry"></param>
        public void AddCacheItems(List<DataCacheEntryContainer> cacheEntries)
        {
            if (cacheEntries != null && cacheEntries.Count > 0)
            {
                this.historianCache.AddRange(cacheEntries);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ClearCache()
        {
            this.historianCache.Clear();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCacheName()
        {
            return this.cacheName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCacheSize()
        {
            return this.historianCache.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HistorianCacheState GetCacheState()
        {
            return this.state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCacheEntryContainer GetCurrentEntry()
        {
            if (this.GetCacheSize() > 0)
            {
                return this.historianCache[this.curIndex];
            } else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCacheEntryContainer GetNextEntry()
        {
            if (this.GetCacheSize() > 0)
            {
                if (++this.curIndex >= this.historianCache.Count)
                {
                    this.curIndex = 0;

                    return null;
                }

                return this.historianCache[this.curIndex];
            } else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCacheEntryContainer GetPreviousEntry()
        {
            if (this.GetCacheSize() > 0)
            {
                if (--this.curIndex < 0)
                {
                    this.curIndex = 0;

                    return null;
                }

                return this.historianCache[this.curIndex];
            } else
            {
                return null;
            }
        }

    }

}
