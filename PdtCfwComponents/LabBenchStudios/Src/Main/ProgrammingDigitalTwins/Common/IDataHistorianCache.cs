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
        public List<DataCacheEntryContainer> GetCacheEntries();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataCacheEntryContainer GetCacheEntryAtIndex(int index);

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
        public double GetCacheMemoryUsage();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MediaPlayerState.PlaybackState GetPlaybackState();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MediaPlayerState.PlaybackDirection GetPlaybackDirection();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCurrentIndex();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCacheEntryContainer GetCurrentEntry();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetDelayTimeMillis();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCacheEntryContainer GetNextEntry();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetStorageFileName();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetStorageResourceUri();

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
        public void ResetCache();

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
            MediaPlayerState.PlaybackDirection direction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetCacheState(
            MediaPlayerState.PlaybackState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetStartingIndex(int index);

    }

}
