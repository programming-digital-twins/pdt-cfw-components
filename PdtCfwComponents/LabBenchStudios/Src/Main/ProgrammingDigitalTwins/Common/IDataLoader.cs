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
    public interface IDataLoader : IDataManager
    {
        /// <summary>
		/// Attempts to retrieve the named data instance from the persistence server.
        /// Will return null if there's no data matching the given type with the
		/// given parameters.
        /// 
        /// </summary>
		/// <param name="cacheName"> The name of the cache. The file name will be auto-generated.</param>
		/// <returns type="List<DataCacheEntryContainer>">The data instance(s) associated with the lookup parameters.</returns>
		public List<DataCacheEntryContainer> LoadDataCache(string cacheName);

        /// <summary>
		/// Attempts to retrieve the named data instance from the persistence server.
        /// Will return null if there's no data matching the given type with the
		/// given parameters.
        /// 
        /// </summary>
		/// <param name="cacheName"> The name of the cache. The file name will be auto-generated.</param>
		/// <returns type="string">The text data associated with the lookup parameters.</returns>
		public string LoadTextDataCache(string cacheName);

		/// <summary>
		/// Attempts to retrieve the named data instance from the persistence server.
		/// Will return null if there's no data matching the given type with the
		/// given parameters.
		/// 
		/// </summary>
		/// <param name="cacheName"> The name of the cache. The file name will be auto-generated.</param>
		/// <returns type="string">The text data associated with the lookup parameters.</returns>
		public RequestResponseData LoadRequestResponseData(string cacheName);

        /// <summary>
		/// Attempts to retrieve the named data instance from the persistence server.
        /// Will return null if there's no data matching the given type with the
		/// given parameters.
        /// 
        /// </summary>
		/// <param name="resource"> The resource container with load meta data / additional search criteria.</param>
		/// <param name="startDate"> The start timeStamp.</param>
		/// <param name="endDate"> The end timeStamp.</param>
		/// <returns type="List<ActuatorData>">The data instance(s) associated with the lookup parameters.</returns>
        public List<ActuatorData> LoadActuatorData(ResourceNameContainer resource, DateTime startDate, DateTime endDate);

        /// <summary>
		/// Attempts to retrieve the named data instance from the persistence server.
        /// Will return null if there's no data matching the given type with the
		/// given parameters.
        /// 
        /// </summary>
		/// <param name="resource"> The resource container with load meta data / additional search criteria.</param>
		/// <param name="startDate"> The start timeStamp.</param>
		/// <param name="endDate"> The end timeStamp.</param>
		/// <returns type="List<ConnectionStateData>">The data instance(s) associated with the lookup parameters.</returns>
        public List<ConnectionStateData> LoadConnectionStateData(ResourceNameContainer resource, DateTime startDate, DateTime endDate);

        /// <summary>
		/// Attempts to retrieve the named data instance from the persistence server.
        /// Will return null if there's no data matching the given type with the
		/// given parameters.
        /// 
        /// </summary>
		/// <param name="resource"> The resource container with load meta data / additional search criteria.</param>
		/// <param name="startDate"> The start timeStamp.</param>
		/// <param name="endDate"> The end timeStamp.</param>
		/// <returns type="List<SensorData>">The data instance(s) associated with the lookup parameters.</returns>
        public List<SensorData> LoadSensorData(ResourceNameContainer resource, DateTime startDate, DateTime endDate);

        /// <summary>
		/// Attempts to retrieve the named data instance from the persistence server.
        /// Will return null if there's no data matching the given type with the
		/// given parameters.
        /// 
        /// </summary>
		/// <param name="resource"> The resource container with load meta data / additional search criteria.</param>
		/// <param name="startDate"> The start timeStamp.</param>
		/// <param name="endDate"> The end timeStamp.</param>
		/// <returns type="List<SystemPerformanceData>">The data instance(s) associated with the lookup parameters.</returns>
        public List<SystemPerformanceData> LoadSystemPerformanceData(ResourceNameContainer resource, DateTime startDate, DateTime endDate);

    }
}
