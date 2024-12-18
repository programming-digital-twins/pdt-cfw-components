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

namespace LabBenchStudios.Pdt.Common
{
    public interface IDataHistorian
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDataHistorianPlayer CreateDataHistorianPlayer();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetLoadableCacheList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetLoadedCacheNames();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCacheFilePath();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetRootFilePath();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public IDataHistorianPlayer GetDataHistorianPlayer(string cacheName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetMaxCacheSize();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetTotalMemoryUsage();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public bool IsCacheReplaying(string cacheName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public void SetEventListener(ISystemStatusEventListener listener);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool SetRootFilePath(string filePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="direction"></param>
        public void SetReplayDirection(string cacheName, MediaPlayerState.PlaybackDirection direction);

    }
}
