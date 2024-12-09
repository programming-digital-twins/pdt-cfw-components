using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Connection;
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
    public class DataHistorianCache : IDataHistorianCache
    {
        // static


        // private

        private string cacheName = "DefaultDataHistorianCache";

        private DataHistorianState.DataHistorianReplayState replayState;
        private DataHistorianState.DataHistorianReplayDirection replayDirection;

        private int curCacheIndex = 0;
        private int cacheIndexIncrement = 1;
        private int newCacheEntryCount = 0;

        private List<DataCacheEntryContainer> historianCache = null;

        private IDataLoader dataLoader = null;
        private IDataStorer dataStorer = null;

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
            this.SetCacheName(cacheName);

            this.historianCache = new List<DataCacheEntryContainer>();
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheEntry"></param>
        public void AddCacheItem(DataCacheEntryContainer cacheEntry)
        {
            this.AddCacheItem(cacheEntry, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheEntry"></param>
        /// <param name="ignoreEntryCount"></param>
        public void AddCacheItem(DataCacheEntryContainer cacheEntry, bool ignoreEntryCount)
        {
            if (cacheEntry != null)
            {
                this.historianCache.Add(cacheEntry);

                if (!ignoreEntryCount)
                {
                    this.newCacheEntryCount++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheEntries"></param>
        public void AddCacheItems(List<DataCacheEntryContainer> cacheEntries)
        {
            this.AddCacheItems(cacheEntries, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheEntries"></param>
        /// <param name="ignoreEntryCount"></param>
        public void AddCacheItems(List<DataCacheEntryContainer> cacheEntries, bool ignoreEntryCount)
        {
            if (cacheEntries != null && cacheEntries.Count > 0)
            {
                this.historianCache.AddRange(cacheEntries);

                if (!ignoreEntryCount)
                {
                    this.newCacheEntryCount += cacheEntries.Count;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ClearCache()
        {
            Console.WriteLine($"Clearing all cached entries from cache {this.cacheName}.");

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
        public DataHistorianState.DataHistorianReplayState GetCacheReplayState()
        {
            return this.replayState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataHistorianState.DataHistorianReplayDirection GetCacheReplayDirection()
        {
            return this.replayDirection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCacheEntryContainer GetCurrentEntry()
        {
            if (this.GetCacheSize() > 0)
            {
                return this.historianCache[this.curCacheIndex];
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
                this.curCacheIndex += this.cacheIndexIncrement;

                if (this.curCacheIndex >= this.historianCache.Count ||
                    this.curCacheIndex < 0)
                {
                    this.curCacheIndex = 0;

                    return null;
                }

                return this.historianCache[this.curCacheIndex];
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
                this.curCacheIndex += this.cacheIndexIncrement;

                if (this.curCacheIndex < 0 ||
                    this.curCacheIndex >= this.historianCache.Count)
                {
                    this.curCacheIndex = 0;

                    return null;
                }

                return this.historianCache[this.curCacheIndex];
            } else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasCachedEntries()
        {
            return (this.historianCache.Count > 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool LoadDataCache()
        {
            return this.LoadDataCache(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeNewEntries"></param>
        /// <returns></returns>
        public bool LoadDataCache(bool storeNewEntries)
        {
            this.SetCacheState(DataHistorianState.DataHistorianReplayState.Stop);
            this.SetCacheAccessDirection(this.replayDirection);

            if (storeNewEntries)
            {
                this.StoreDataCache();
            }

            if (this.dataLoader != null)
            {
                this.ClearCache();

                this.historianCache = this.dataLoader.LoadDataCache(this.cacheName);

                if (this.historianCache != null)
                {
                    if (this.historianCache.Count > 0)
                    {
                        Console.WriteLine($"Successfully loaded {this.historianCache.Count} items from cache {this.cacheName}.");
                    } else
                    {
                        Console.WriteLine($"Warning - no cached items for {this.cacheName} loaded from persistence layer.");
                    }
                } else
                {
                    Console.WriteLine($"Error - failed to load cache {this.cacheName} from persistence layer.");
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool StoreDataCache()
        {
            this.SetCacheState(DataHistorianState.DataHistorianReplayState.Stop);
            this.SetCacheAccessDirection(this.replayDirection);

            if (this.dataStorer != null)
            {
                int resultCode = this.dataStorer.StoreDataCache(this.cacheName, this.historianCache);

                if (resultCode > 0)
                {
                    Console.WriteLine($"Successfully stored {resultCode} items to cache {this.cacheName}.");

                    this.newCacheEntryCount = 0;
                } else if (resultCode == 0)
                {
                    Console.WriteLine($"Warning - no cached items from {this.cacheName} stored to persistence layer.");
                } else
                {
                    Console.WriteLine($"Error - failed to store cache {this.cacheName} to persistence layer.");
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loader"></param>
        public void SetDataLoader(IDataLoader loader)
        {
            if (loader != null)
            {
                Console.WriteLine("Setting data loader...");
                this.dataLoader = loader;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storer"></param>
        public void SetDataStorer(IDataStorer storer)
        {
            if (storer != null)
            {
                Console.WriteLine("Setting data storer...");
                this.dataStorer = storer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void SetCacheName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                this.cacheName = name;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        public void SetCacheAccessDirection(
            DataHistorianState.DataHistorianReplayDirection direction)
        {
            this.replayDirection = direction;

            switch (this.replayDirection)
            {
                case DataHistorianState.DataHistorianReplayDirection.Forward:
                    this.cacheIndexIncrement = 1;
                    break;

                case DataHistorianState.DataHistorianReplayDirection.Reverse:
                    this.cacheIndexIncrement = -1;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetCacheState(
            DataHistorianState.DataHistorianReplayState state)
        {
            this.replayState = state;
        }

    }

}
