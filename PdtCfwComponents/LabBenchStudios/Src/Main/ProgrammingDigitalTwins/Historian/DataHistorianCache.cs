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
using System.Text.RegularExpressions;
using System.Text;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Util;

namespace LabBenchStudios.Pdt.Historian
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

        public static readonly string DEFAULT_CACHE_NAME = ConfigConst.PRODUCT_NAME + "_DataHistorianCache";

        // private

        private bool enableDebugLog = false;

        private string cacheName = null;
        private string cacheFileName = null;
        private string cacheFilePath = null;

        private MediaPlayerState.PlaybackState replayState;
        private MediaPlayerState.PlaybackDirection replayDirection;

        private int curCacheIndex = 0;
        private int cacheIndexIncrement = 1;
        private int newCacheEntryCount = 0;

        private double cacheMemoryUsage = 0.0d;
        private long delayTimeMillis = 0L;

        private List<DataCacheEntryContainer> dataEntryCache = null;

        private DataCacheEntryContainer curCacheEntry = null;

        private IDataLoader dataLoader = null;
        private IDataStorer dataStorer = null;

        /// <summary>
        /// 
        /// </summary>
        public DataHistorianCache() : base()
        {
            this.dataEntryCache = new List<DataCacheEntryContainer>();

            this.SetCacheName(CreateCacheName());
        }

        /// <summary>
        /// 
        /// </summary>
        public DataHistorianCache(string cacheName) : base()
        {
            this.dataEntryCache = new List<DataCacheEntryContainer>();

            this.SetCacheName(cacheName);
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
                if (this.enableDebugLog)
                {
                    Console.WriteLine($"Adding cache entry: {cacheEntry.GetTimeStamp()}");
                }

                this.dataEntryCache.Add(cacheEntry);
                this.cacheMemoryUsage += cacheEntry.GetApproxByteCount();

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
                foreach (DataCacheEntryContainer cacheEntry in cacheEntries)
                {
                    this.AddCacheItem(cacheEntry, ignoreEntryCount);
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

            this.dataEntryCache.Clear();

            this.cacheMemoryUsage = 0;
            this.newCacheEntryCount = 0;
            this.curCacheIndex = 0;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<DataCacheEntryContainer> GetCacheEntries()
        {
            return this.dataEntryCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataCacheEntryContainer GetCacheEntryAtIndex(int index)
        {
            if (index >= 0 && index < this.dataEntryCache.Count)
            {
                return this.dataEntryCache[index];
            }

            return null;
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
            return this.dataEntryCache.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetCacheMemoryUsage()
        {
            return this.cacheMemoryUsage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MediaPlayerState.PlaybackState GetPlaybackState()
        {
            return this.replayState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MediaPlayerState.PlaybackDirection GetPlaybackDirection()
        {
            return this.replayDirection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCurrentIndex()
        {
            return this.curCacheIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCacheEntryContainer GetCurrentEntry()
        {
            return this.curCacheEntry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetDelayTimeMillis()
        {
            return this.delayTimeMillis;
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

                if (this.curCacheIndex >= this.dataEntryCache.Count ||
                    this.curCacheIndex < 0)
                {
                    this.curCacheIndex = 0;

                    return null;
                }

                this.curCacheEntry = this.dataEntryCache[this.curCacheIndex];

                return this.curCacheEntry;
            } else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetStorageFileName()
        {
            return this.cacheFileName;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetStorageResourceUri()
        {
            return this.cacheFilePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasCachedEntries()
        {
            return (this.GetCacheSize() > 0);
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
            //this.SetCacheState(DataHistorianState.PlaybackState.Stop);
            //this.SetCacheAccessDirection(this.replayDirection);

            if (storeNewEntries)
            {
                this.StoreDataCache();
            }

            if (this.dataLoader != null)
            {
                this.ClearCache();

                List<DataCacheEntryContainer> cacheList = this.dataLoader.LoadDataCache(this.cacheName);

                if (cacheList == null)
                {
                    Console.WriteLine($"No cache exists on file system for cache name: {this.cacheName}. Path: {this.cacheFilePath}");

                    return false;
                } else
                {
                    this.AddCacheItems(cacheList);
                }

                if (this.HasCachedEntries())
                {
                    Console.WriteLine($"Successfully loaded {this.dataEntryCache.Count} items from cache {this.cacheName}.");
                    
                    return true;
                } else
                {
                    Console.WriteLine($"Warning - no cached items for {this.cacheName} loaded from persistence layer.");
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetCache()
        {
            //this.SetCacheState(DataHistorianState.PlaybackState.Stop);

            this.curCacheIndex = 0;
            this.curCacheEntry = this.GetCacheEntryAtIndex(this.curCacheIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool StoreDataCache()
        {
            //this.SetCacheState(DataHistorianState.PlaybackState.Stop);
            //this.SetCacheAccessDirection(this.replayDirection);

            if (this.dataStorer != null && this.HasCachedEntries())
            {
                int resultCode = this.dataStorer.StoreDataCache(this);

                if (resultCode > 0)
                {
                    Console.WriteLine($"Successfully stored {resultCode} items to cache {this.cacheName}.");

                    //this.newCacheEntryCount = 0;
                } else if (resultCode == 0)
                {
                    Console.WriteLine($"Warning - no cached items from {this.cacheName} stored to persistence layer.");
                } else
                {
                    Console.WriteLine($"Error - failed to store cache {this.cacheName} to persistence layer.");
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loader"></param>
        public void SetDataLoader(IDataLoader loader)
        {
            if (loader != null)
            {
                this.dataLoader = loader;

                Console.WriteLine($"Setting data loader: {this.cacheFileName}");
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
                this.dataStorer = storer;

                Console.WriteLine($"Setting data storer: {this.cacheFileName}");
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
                try
                {
                    name = name.Trim(ConfigConst.JSON_EXT.ToCharArray());
                } catch (Exception e)
                {
                    // ignore
                }

                this.cacheName = name;

                Console.WriteLine($"Setting historian cache name: {this.cacheName}");

                this.InitCacheProperties();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        public void SetCacheAccessDirection(
            MediaPlayerState.PlaybackDirection direction)
        {
            this.replayDirection = direction;

            switch (this.replayDirection)
            {
                case MediaPlayerState.PlaybackDirection.Forward:
                    this.cacheIndexIncrement = 1;
                    break;

                case MediaPlayerState.PlaybackDirection.Reverse:
                    this.cacheIndexIncrement = -1;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetCacheState(
            MediaPlayerState.PlaybackState state)
        {
            this.replayState = state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetStartingIndex(int index)
        {
            if (index >= 0 && index < this.dataEntryCache.Count)
            {
                this.curCacheIndex = index;
                this.curCacheEntry = this.GetCacheEntryAtIndex(this.curCacheIndex);

                return true;
            }

            return false;
        }


        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string CreateCacheName()
        {
            byte[] guidBytes = Guid.NewGuid().ToByteArray();
            string base64Guid = Convert.ToBase64String(guidBytes);
            string shortGuid = Regex.Replace(base64Guid, "[/+=]", "");

            StringBuilder builder = new StringBuilder(DataHistorianCache.DEFAULT_CACHE_NAME);
            builder.Append("_");
            builder.Append(shortGuid);

            string cacheName = builder.ToString();

            Console.WriteLine($"Created data historian cache name: {cacheName}");

            return cacheName;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitCacheProperties()
        {
            this.cacheFileName = FileUtil.CreateDataHistorianFileName(this.cacheName);
            this.cacheFilePath = Path.GetDirectoryName(this.cacheFileName);

            Console.WriteLine($"Cache props: Name = {this.cacheName}; File = {this.cacheFileName}; Path = {this.cacheFilePath}");
        }

    }

}
