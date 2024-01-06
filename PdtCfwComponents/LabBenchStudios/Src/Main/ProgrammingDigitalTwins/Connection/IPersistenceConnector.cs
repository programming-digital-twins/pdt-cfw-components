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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Connection
{
    /**
	 * Interface that defines the contract to a data store.
	 * 
	 */
    public interface IPersistenceConnector
    {
		/**
		 * Connects to the persistence server.
		 * 
		 * @return boolean True on success; false otherwise.
		 */
		public Boolean ConnectClient();

		/**
		 * Disconnects from the persistence server.
		 * 
		 * @return boolean True on success; false otherwise.
		 */
		public Boolean DisconnectClient();

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The target resource name.
		 * @param typeID The type ID of the data to retrieve.
		 * @param startDate The start date (null if narrowing is not needed).
		 * @param endDate The end date (null if narrowing is not needed).
		 * @return List<ActuatorData> The data instance(s) associated with the lookup parameters.
		 */
        public List<ActuatorData> GetActuatorData(ResourceNameContainer resource, int typeID, DateTime startDate, DateTime endDate);

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The target resource name.
		 * @param typeID The type ID of the data to retrieve.
		 * @param startDate The start date (null if narrowing is not needed).
		 * @param endDate The end date (null if narrowing is not needed).
		 * @return List<ConnectionStateData> The data instance(s) associated with the lookup parameters.
		 */
        public List<ConnectionStateData> GetConnectionStateData(ResourceNameContainer resource, int typeID, DateTime startDate, DateTime endDate);

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The target resource name.
		 * @param typeID The type ID of the data to retrieve.
		 * @param startDate The start date (null if narrowing is not needed).
		 * @param endDate The end date (null if narrowing is not needed).
		 * @return List<SensorData> The data instance(s) associated with the lookup parameters.
		 */
        public List<SensorData> GetSensorData(ResourceNameContainer resource, int typeID, DateTime startDate, DateTime endDate);

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The target resource name.
		 * @param startDate The start date (null if narrowing is not needed).
		 * @param endDate The end date (null if narrowing is not needed).
		 * @return List<SystemPerformanceData> The data instance(s) associated with the lookup parameters.
		 */
        public List<SystemPerformanceData> GetSystemPerformanceData(ResourceNameContainer resource, DateTime startDate, DateTime endDate);

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, ActuatorData data);

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, ConnectionStateData data);

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, SensorData data);

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, SystemPerformanceData data);

	}
}