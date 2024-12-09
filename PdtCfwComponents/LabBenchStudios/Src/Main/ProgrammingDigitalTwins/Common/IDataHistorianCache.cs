using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Connection;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Common
{
    public interface IDataHistorianCache
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheEntry"></param>
        public void AddCacheItem(DataCacheEntryContainer cacheEntry);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheEntry"></param>
        public void AddCacheItems(List<DataCacheEntryContainer> cacheEntries);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ClearCache();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCacheName();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCacheSize();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataHistorianState.DataHistorianReplayState GetCacheReplayState();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataHistorianState.DataHistorianReplayDirection GetCacheReplayDirection();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCacheEntryContainer GetCurrentEntry();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCacheEntryContainer GetNextEntry();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCacheEntryContainer GetPreviousEntry();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasCachedEntries();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool LoadDataCache();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeNewEntries"></param>
        /// <returns></returns>
        public bool LoadDataCache(bool storeNewEntries);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool StoreDataCache();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loader"></param>
        public void SetDataLoader(IDataLoader loader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storer"></param>
        public void SetDataStorer(IDataStorer storer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void SetCacheName(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        public void SetCacheAccessDirection(
            DataHistorianState.DataHistorianReplayDirection direction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetCacheState(
            DataHistorianState.DataHistorianReplayState state);

    }

}
