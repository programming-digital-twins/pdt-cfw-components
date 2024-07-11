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

using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

using Task = System.Threading.Tasks.Task;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using InfluxDB.Client.Core.Flux.Domain;
using System.Linq;

namespace LabBenchStudios.Pdt.Connection
{
    public class InfluxClientConnector : IPersistenceConnector
    {
        // static consts

        public const string DEFAULT_HOST = "localhost";
        public const int DEFAULT_PORT = 8086;

        // private member vars

        private string serverHost = DEFAULT_HOST;
        private int serverPort = DEFAULT_PORT;
        private string clientToken = "test";
        private string orgID = "test";
        private bool isConnected = false;
        private bool autoReconnect = true;

        private InfluxDBClient dbClient = null;

        private ISystemStatusEventListener eventListener = null;

        private ConnectionStateData connStateData = null;

        // constructors

        public InfluxClientConnector(
            string clientToken, string orgID,
            ISystemStatusEventListener eventListener) : 
            this(DEFAULT_HOST, DEFAULT_PORT, clientToken, orgID, eventListener)
        {
            // nothing to do
        }

        public InfluxClientConnector(
            string serverHost, int serverPort, string clientToken, string orgID,
            ISystemStatusEventListener eventListener)
        {
            if (serverHost != null)
            {
                this.serverHost = serverHost;
            }

            if (serverPort > 0 && serverPort < 65535)
            {
                this.serverPort = serverPort;
            }

            if (clientToken != null)
            {
                this.clientToken = clientToken;
            }

            if (orgID != null)
            {
                this.orgID = orgID;
            }

            this.eventListener = eventListener;
            this.connStateData = new ConnectionStateData("PDT", "UUID", this.serverHost, this.serverPort);
        }

        // public methods

        public bool ConnectClient()
        {
            if (this.dbClient == null)
            {
                this.InitConnector();
            }

            if (this.dbClient != null)
            {
                this.connStateData.SetMessage("Connecting...");
                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());

                _ = this.CheckConnection();

                return true;
            }

            return false;
        }

        public bool DisconnectClient()
        {
            if (this.dbClient != null)
            {
                this.connStateData.SetMessage("Disconnecting...");
                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());

                return true;
            }

            return false;
        }

        public bool IsClientConnected()
        {
            return this.isConnected;
        }

        /**
		 * Attempts to retrieve the named data instance list from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param typeID The type ID of the data to retrieve. Ignored if invalid (e.g., < 0).
		 * @param startDate The start date. If null, the current time less 1 hour will be used.
		 * @param endDate The end date. If null, the current time will be used.
		 * @return List<ActuatorData> The data instance(s) associated with the lookup parameters.
		 */
        public List<ActuatorData> LoadActuatorData(ResourceNameContainer resource, int typeID, DateTime startDate, DateTime endDate)
        {
            _ = this.CheckConnection();

            string bucketName = ConfigConst.CMD_DATA_PERSISTENCE_NAME;

            if (resource != null && resource.PersistenceName != null)
            {
                bucketName = resource.PersistenceName;
            }

            return null;
        }

        /**
		 * Attempts to retrieve the named data instance list from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start date. If null, the current time less 1 hour will be used.
		 * @param endDate The end date. If null, the current time will be used.
		 * @return List<ActuatorData> The data instance(s) associated with the lookup parameters.
		 */
        public List<ConnectionStateData> LoadConnectionStateData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            _ = this.CheckConnection();

            string bucketName = ConfigConst.SYS_DATA_PERSISTENCE_NAME;

            if (resource != null && resource.PersistenceName != null)
            {
                bucketName = resource.PersistenceName;
            }

            long startTime = DateUtil.ConvertDateTimeToMillis(startDate);

            //string fluxQuery = "from(bucket:\"" + bucketName + "\") |> range(start: " + startTime + ")";
            string fluxQuery = "from(bucket:\"" + bucketName + "\") |> range(start: 0)";

            Console.WriteLine("Query: " + fluxQuery);

            List<ConnectionStateData> dataList = this.GetConnectionStateDataRecords(fluxQuery);

            return dataList;
        }

        /**
		 * Attempts to retrieve the named data instance list from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param typeID The type ID of the data to retrieve. Ignored if invalid (e.g., < 0).
		 * @param startDate The start date. If null, the current time less 1 hour will be used.
		 * @param endDate The end date. If null, the current time will be used.
		 * @return List<ActuatorData> The data instance(s) associated with the lookup parameters.
		 */
        public List<SensorData> LoadSensorData(ResourceNameContainer resource, int typeID, DateTime startDate, DateTime endDate)
        {
            _ = this.CheckConnection();

            string bucketName = ConfigConst.SENSOR_DATA_PERSISTENCE_NAME;

            if (resource != null && resource.PersistenceName != null)
            {
                bucketName = resource.PersistenceName;
            }

            long startTime = DateUtil.ConvertDateTimeToMillis(startDate);

            string fluxQuery = "from(bucket:\"" + bucketName + "\") |> range(start: 0)";

            List<SensorData> dataList = this.GetSensorDataRecords(bucketName, 8);

            return dataList;
        }

        /**
		 * Attempts to retrieve the named data instance list from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start date. If null, the current time less 1 hour will be used.
		 * @param endDate The end date. If null, the current time will be used.
		 * @return List<ActuatorData> The data instance(s) associated with the lookup parameters.
		 */
        public List<SystemPerformanceData> LoadSystemPerformanceData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            _ = this.CheckConnection();

            string bucketName = ConfigConst.SYS_DATA_PERSISTENCE_NAME;

            if (resource != null && resource.PersistenceName != null)
            {
                bucketName = resource.PersistenceName;
            }

            long startTime = DateUtil.ConvertDateTimeToMillis(startDate);

            string fluxQuery = "from(bucket:\"" + bucketName + "\") |> range(start: " + startTime + ")";

            List<SystemPerformanceData> dataList = this.GetSystemPerformanceDataRecords(fluxQuery);

            return dataList;
        }

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, ActuatorData data)
        {
            _ = this.CheckConnection();

            if (data != null)
            {
                using (var writeApi = this.dbClient.GetWriteApi())
                {
                    PointData pd = this.CreateDataPoint(data);
                    writeApi.WritePoint(pd, resource.DeviceName, this.orgID);
                }

                return true;
            }

            return false;
        }

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, ConnectionStateData data)
        {
            _ = this.CheckConnection();

            if (data != null)
            {
                using (var writeApi = this.dbClient.GetWriteApi())
                {
                    PointData pd = this.CreateDataPoint(data);
                    writeApi.WritePoint(pd, resource.DeviceName, this.orgID);
                }

                return true;
            }

            return false;
        }

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, SensorData data)
        {
            _ = this.CheckConnection();

            if (data != null)
            {
                using (var writeApi = this.dbClient.GetWriteApi())
                {
                    PointData pd = this.CreateDataPoint(data);
                    writeApi.WritePoint(pd, resource.DeviceName, this.orgID);
                }

                return true;
            }

            return false;
        }

        /**
		 * Attempts to write the source data instance to the persistence server.
		 * 
		 * @param resource The target resource name.
		 * @param qos The intended target QoS.
		 * @param data The data instance to store.
		 * @return boolean True on success; false otherwise.
		 */
        public bool StoreData(ResourceNameContainer resource, int qos, SystemPerformanceData data)
        {
            _ = this.CheckConnection();

            if (data != null)
            {
                using (var writeApi = this.dbClient.GetWriteApi())
                {
                    PointData pd = this.CreateDataPoint(data);
                    writeApi.WritePoint(pd, resource.DeviceName, this.orgID);
                }

                return true;
            }

            return false;
        }


        // private

        /**
         * Retrieves records for the given type.
         * 
         * @return List<ConnectionStateData> The list of data objects retrieved,
         * or an empty list if no data objects are retrieved (due to error or
         * a lack of data).
         */
        private List<ConnectionStateData> GetConnectionStateDataRecords(string fluxQuery)
        {
            List<ConnectionStateData> dataList = new List<ConnectionStateData>();

            // TODO: change this to async, send callback when query completes
            List<FluxTable> tableList = this.dbClient.GetQueryApiSync().QuerySync(fluxQuery, this.orgID);

            if (tableList != null && tableList.Count > 0)
            {
                tableList.ForEach(tableList =>
                {
                    List<FluxRecord> recordList = tableList.Records;

                    recordList.ForEach(recordList =>
                    {
                        object name = ConfigConst.NOT_SET;
                        object deviceID = ConfigConst.NOT_SET;
                        object locationID = ConfigConst.NOT_SET;
                        object typeID = ConfigConst.DEFAULT_TYPE_ID;
                        object typeCategoryID = ConfigConst.DEFAULT_TYPE_CATEGORY_ID;
                        object hostName = ConfigConst.DEFAULT_HOST;
                        object hostPort = ConfigConst.DEFAULT_MQTT_PORT;
                        object diskUtil = ConfigConst.DEFAULT_VAL;

                        recordList.Values.TryGetValue(ConfigConst.NAME_PROP, out name);
                        recordList.Values.TryGetValue(ConfigConst.DEVICE_ID_PROP, out deviceID);
                        recordList.Values.TryGetValue(ConfigConst.LOCATION_ID_PROP, out locationID);
                        recordList.Values.TryGetValue(ConfigConst.TYPE_ID_PROP, out typeID);
                        recordList.Values.TryGetValue(ConfigConst.TYPE_CATEGORY_ID_PROP, out typeCategoryID);

                        ConnectionStateData data =
                            new ConnectionStateData(name.ToString(), deviceID.ToString(), hostName.ToString(), Convert.ToInt32(hostPort));

                        data.SetLocationID(locationID.ToString());

                        dataList.Add(data);
                    });
                });
            }

            return dataList;
        }

        /**
         * Retrieves records for the given type.
         * 
         * @return List<SensorData> The list of data objects retrieved,
         * or an empty list if no data objects are retrieved (due to error or
         * a lack of data).
         */
        private List<SensorData> GetSensorDataRecords(string bucket, int totHours)
        {
            List<SensorData> dataList = new List<SensorData>();

            string totHoursStr = (totHours * -1).ToString() + "h";

            string fluxQuery = "from(bucket: \"" + bucket + "\") |> range(start: " + totHoursStr + ")";

            Console.WriteLine("Query: " + fluxQuery);

            var tableList = this.dbClient.GetQueryApiSync().QuerySync(fluxQuery, this.orgID);

            foreach (var record in tableList.SelectMany(table => table.Records))
            {
                object name = record.GetMeasurement();

                object deviceID = ConfigConst.NOT_SET;
                object locationID = ConfigConst.NOT_SET;
                object typeID = ConfigConst.DEFAULT_TYPE_ID;
                object typeCategoryID = ConfigConst.DEFAULT_TYPE_CATEGORY_ID;
                object dataVal = ConfigConst.DEFAULT_VAL;
                object timeVal = ConfigConst.NOT_SET;
                object val = null;

                timeVal = record.GetTime();

                if (record.GetField().Equals("value")) dataVal = record.GetValue();

                if (record.Values.TryGetValue(ConfigConst.DEVICE_ID_PROP, out val)) deviceID = val.ToString();
                if (record.Values.TryGetValue(ConfigConst.LOCATION_ID_PROP, out val)) locationID = val.ToString();
                if (record.Values.TryGetValue(ConfigConst.TYPE_ID_PROP, out val)) typeID = val;
                if (record.Values.TryGetValue(ConfigConst.TYPE_CATEGORY_ID_PROP, out val)) typeCategoryID = val;

                SensorData data =
                    new SensorData(name.ToString(), deviceID.ToString(), Convert.ToInt32(typeCategoryID), Convert.ToInt32(typeID));

                data.SetLocationID(locationID.ToString());
                data.SetValue((float) Convert.ToDouble(dataVal));
                data.OverrideTimeStamp(timeVal.ToString());
                
                dataList.Add(data);
            }

            return dataList;
        }

        /**
         * Retrieves records for the given type.
         * 
         * @return List<SystemPerformanceData> The list of data objects retrieved,
         * or an empty list if no data objects are retrieved (due to error or
         * a lack of data).
         */
        private List<SystemPerformanceData> GetSystemPerformanceDataRecords(string fluxQuery)
        {
            List<SystemPerformanceData> dataList = new List<SystemPerformanceData>();

            // TODO: change this to async, send callback when query completes
            List<FluxTable> tableList = this.dbClient.GetQueryApiSync().QuerySync(fluxQuery, this.orgID);

            if (tableList != null && tableList.Count > 0)
            {
                tableList.ForEach(tableList =>
                {
                    List<FluxRecord> recordList = tableList.Records;

                    recordList.ForEach(recordList =>
                    {
                        object name = ConfigConst.NOT_SET;
                        object deviceID = ConfigConst.NOT_SET;
                        object locationID = ConfigConst.NOT_SET;
                        object typeID = ConfigConst.DEFAULT_TYPE_ID;
                        object typeCategoryID = ConfigConst.DEFAULT_TYPE_CATEGORY_ID;
                        object cpuUtil = ConfigConst.DEFAULT_VAL;
                        object memUtil = ConfigConst.DEFAULT_VAL;
                        object diskUtil = ConfigConst.DEFAULT_VAL;

                        recordList.Values.TryGetValue(ConfigConst.NAME_PROP, out name);
                        recordList.Values.TryGetValue(ConfigConst.DEVICE_ID_PROP, out deviceID);
                        recordList.Values.TryGetValue(ConfigConst.LOCATION_ID_PROP, out locationID);
                        recordList.Values.TryGetValue(ConfigConst.TYPE_ID_PROP, out typeID);
                        recordList.Values.TryGetValue(ConfigConst.TYPE_CATEGORY_ID_PROP, out typeCategoryID);
                        recordList.Values.TryGetValue(ConfigConst.CPU_UTIL_PROP, out cpuUtil);
                        recordList.Values.TryGetValue(ConfigConst.MEM_UTIL_PROP, out memUtil);
                        recordList.Values.TryGetValue(ConfigConst.DISK_UTIL_PROP, out diskUtil);

                        SystemPerformanceData data =
                            new SystemPerformanceData(name.ToString(), deviceID.ToString());

                        data.SetLocationID(locationID.ToString());
                        data.SetCpuUtilization((float)Convert.ToDouble(cpuUtil));
                        data.SetMemoryUtilization((float)Convert.ToDouble(memUtil));
                        data.SetDiskUtilization((float)Convert.ToDouble(diskUtil));

                        dataList.Add(data);
                    });
                });
            }

            return dataList;
        }

        /**
         * Creates an InfluxDB PointData instance for the given type.
         * 
         * @return PointData The PointData instance that represents the given
         * data type container.
         */
        private PointData CreateDataPoint(ActuatorData data)
        {
            long millis = DateUtil.ConvertIso8601TimeStampToMillis(data.GetTimeStamp());

            PointData point =
                PointData
                    .Measurement(data.GetName())
                    .Tag(ConfigConst.DEVICE_ID_PROP, data.GetDeviceID())
                    .Tag(ConfigConst.LOCATION_ID_PROP, data.GetLocationID())
                    .Tag(ConfigConst.TYPE_ID_PROP, data.GetTypeID().ToString())
                    .Tag(ConfigConst.TYPE_CATEGORY_ID_PROP, data.GetTypeCategoryID().ToString())
                    .Field(ConfigConst.COMMAND_PROP, data.GetCommand())
                    .Field(ConfigConst.STATE_DATA_PROP, data.GetStateData())
                    .Field(ConfigConst.STATUS_CODE_PROP, data.GetStatusCode())
                    .Field(ConfigConst.VALUE_PROP, data.GetValue())
                    .Timestamp(millis, WritePrecision.Ms);

            return point;
        }

        /**
         * Creates an InfluxDB PointData instance for the given type.
         * 
         * @return PointData The PointData instance that represents the given
         * data type container.
         */
        private PointData CreateDataPoint(ConnectionStateData data)
        {
            long millis = DateUtil.ConvertIso8601TimeStampToMillis(data.GetTimeStamp());

            PointData point =
                PointData
                    .Measurement(data.GetName())
                    .Tag(ConfigConst.DEVICE_ID_PROP, data.GetDeviceID())
                    .Tag(ConfigConst.LOCATION_ID_PROP, data.GetLocationID())
                    .Tag(ConfigConst.TYPE_ID_PROP, data.GetTypeID().ToString())
                    .Tag(ConfigConst.TYPE_CATEGORY_ID_PROP, data.GetTypeCategoryID().ToString())
                    .Field(ConfigConst.HOST_NAME_PROP, data.GetHostName())
                    .Field(ConfigConst.PORT_KEY, data.GetHostPort())
                    .Field(ConfigConst.MESSAGE_IN_COUNT_PROP, data.GetMessageInCount())
                    .Field(ConfigConst.MESSAGE_OUT_COUNT_PROP, data.GetMessageOutCount())
                    .Field(ConfigConst.IS_CONNECTING_PROP, data.IsClientConnecting())
                    .Field(ConfigConst.IS_CONNECTED_PROP, data.IsClientConnected())
                    .Field(ConfigConst.IS_DISCONNECTED_PROP, data.IsClientDisconnected())
                    .Timestamp(millis, WritePrecision.Ms);

            return point;
        }

        /**
         * Creates an InfluxDB PointData instance for the given type.
         * 
         * @return PointData The PointData instance that represents the given
         * data type container.
         */
        private PointData CreateDataPoint(SensorData data)
        {
            long millis = DateUtil.ConvertIso8601TimeStampToMillis(data.GetTimeStamp());

            PointData point =
                PointData
                    .Measurement(data.GetName())
                    .Tag(ConfigConst.DEVICE_ID_PROP, data.GetDeviceID())
                    .Tag(ConfigConst.LOCATION_ID_PROP, data.GetLocationID())
                    .Tag(ConfigConst.TYPE_ID_PROP, data.GetTypeID().ToString())
                    .Tag(ConfigConst.TYPE_CATEGORY_ID_PROP, data.GetTypeCategoryID().ToString())
                    .Field(ConfigConst.VALUE_PROP, data.GetValue())
                    .Timestamp(millis, WritePrecision.Ms);

            return point;
        }

        /**
         * Creates an InfluxDB PointData instance for the given type.
         * 
         * @return PointData The PointData instance that represents the given
         * data type container.
         */
        private PointData CreateDataPoint(SystemPerformanceData data)
        {
            long millis = DateUtil.ConvertIso8601TimeStampToMillis(data.GetTimeStamp());

            PointData point =
                PointData
                    .Measurement(data.GetName())
                    .Tag(ConfigConst.DEVICE_ID_PROP, data.GetDeviceID())
                    .Tag(ConfigConst.LOCATION_ID_PROP, data.GetLocationID())
                    .Tag(ConfigConst.TYPE_ID_PROP, data.GetTypeID().ToString())
                    .Tag(ConfigConst.TYPE_CATEGORY_ID_PROP, data.GetTypeCategoryID().ToString())
                    .Field(ConfigConst.CPU_UTIL_PROP, data.GetCpuUtilization())
                    .Field(ConfigConst.MEM_UTIL_PROP, data.GetMemoryUtilization())
                    .Field(ConfigConst.DISK_UTIL_PROP, data.GetDiskUtilization())
                    .Timestamp(millis, WritePrecision.Ms);

            return point;
        }


        // private

        private async Task CheckConnection()
        {
            this.isConnected = await this.dbClient.PingAsync();

            if (this.autoReconnect && ! this.isConnected)
            {
                this.InitConnector();
            }

            this.isConnected = await this.dbClient.PingAsync();
        }

        private void InitConnector()
        {
            UriBuilder builder = new UriBuilder();

            builder.Scheme = "http";
            builder.Host = this.serverHost;
            builder.Port = this.serverPort;

            this.dbClient = new InfluxDBClient(builder.Uri.ToString(), this.clientToken);
        }

        private void OnConnectSuccess()
        {
            this.isConnected = true;

            this.eventListener?.LogDebugMessage("InfluxDB client connection complete.");

            this.connStateData.SetMessage("Connected!");
            this.connStateData.SetIsClientConnectedFlag(true);
            this.connStateData.SetStatusCode(ConfigConst.CONN_SUCCESS_STATUS_CODE);

            this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
        }

        private void OnConnectFailure()
        {
            this.isConnected = false;

            this.eventListener?.LogDebugMessage("InfluxDB client connection failed.");

            EventMessage msg = new EventMessage();
            msg.StatusCode = ConfigConst.CONN_FAILURE_STATUS_CODE;

            this.connStateData.SetMessage("Conn Failed!");
            this.connStateData.SetIsClientDisconnectedFlag(true);
            this.connStateData.SetStatusCode(ConfigConst.CONN_FAILURE_STATUS_CODE);

            this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
        }

        private void OnDisconnectSuccess()
        {
            this.isConnected = false;

            this.eventListener?.LogDebugMessage("InfluxDB client disconnect complete.");

            this.connStateData.SetMessage("Disconnected!");
            this.connStateData.SetIsClientDisconnectedFlag(true);
            this.connStateData.SetStatusCode(ConfigConst.DISCONN_SUCCESS_STATUS_CODE);

            this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
        }

        private ConnectionStateData GetConnectionStateCopy()
        {
            ConnectionStateData updatedConnStateData =
                new ConnectionStateData(
                    this.connStateData.GetName(),
                    this.connStateData.GetDeviceID(),
                    this.connStateData.GetHostName(),
                    this.connStateData.GetHostPort());

            updatedConnStateData.UpdateData(this.connStateData);

            return updatedConnStateData;
        }

    }
}
