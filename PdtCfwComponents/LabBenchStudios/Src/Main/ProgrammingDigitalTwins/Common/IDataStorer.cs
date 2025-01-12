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
    public interface IDataStorer : IDataManager
    {
        /// <summary>
		/// Attempts to write the source data instance to the persistence server.
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="dataCache"></param>
        /// <returns type="int">On success, returns the total bytes stored. If no bytes
        /// are written and no errors, returns 0. If errored, returns -1.</returns>
        public int StoreTextDataCache(string cacheName, string dataCache);

        /// <summary>
		/// Attempts to write the source data instance to the persistence server.
        /// </summary>
        /// <param name="historianCache"></param>
        /// <returns type="int">On success, returns the total bytes stored. If no bytes
        /// are written and no errors, returns 0. If errored, returns -1.</returns>
        public int StoreDataCache(IDataHistorianCache historianCache);

        /// <summary>
		/// Attempts to write the source data instance to the persistence server.
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="cache"></param>
        /// <returns type="int">On success, returns the total bytes stored. If no bytes
        /// are written and no errors, returns 0. If errored, returns -1.</returns>
        public int StoreDataCache(string cacheName, List<DataCacheEntryContainer> cache);

        /// <summary>
		/// Attempts to write the source data instance to the persistence server.
        /// </summary>
        /// <param name="data"></param>
        /// <returns type="int">On success, returns the total bytes stored. If no bytes
        /// are written and no errors, returns 0. If errored, returns -1.</returns>
        public bool StoreData(RequestResponseData data);

        /// <summary>
        /// Attempts to write the source data instance to the persistence server.
        /// </summary>
        /// <param name="resource">The target resource name.</param>
        /// <param name="qos">The intended target QoS.</param>
        /// <param name="data">The data instance to store.</param>
        /// <returns type="bool">True on success; false otherwise.</returns>
        public bool StoreData(ResourceNameContainer resource, int qos, ActuatorData data);

        /// <summary>
        /// Attempts to write the source data instance to the persistence server.
        /// </summary>
        /// <param name="resource">The target resource name.</param>
        /// <param name="qos">The intended target QoS.</param>
        /// <param name="data">The data instance to store.</param>
        /// <returns type="bool">True on success; false otherwise.</returns>
        public bool StoreData(ResourceNameContainer resource, int qos, ConnectionStateData data);

        /// <summary>
        /// Attempts to write the source data instance to the persistence server.
        /// </summary>
        /// <param name="resource">The target resource name.</param>
        /// <param name="qos">The intended target QoS.</param>
        /// <param name="data">The data instance to store.</param>
        /// <returns type="bool">True on success; false otherwise.</returns>
        public bool StoreData(ResourceNameContainer resource, int qos, SensorData data);

        /// <summary>
        /// Attempts to write the source data instance to the persistence server.
        /// </summary>
        /// <param name="resource">The target resource name.</param>
        /// <param name="qos">The intended target QoS.</param>
        /// <param name="data">The data instance to store.</param>
        /// <returns type="bool">True on success; false otherwise.</returns>
        public bool StoreData(ResourceNameContainer resource, int qos, SystemPerformanceData data);

    }
}
