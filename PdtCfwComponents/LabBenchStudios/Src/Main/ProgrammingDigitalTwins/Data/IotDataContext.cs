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

using LabBenchStudios.Pdt.Common;

using Newtonsoft.Json;

using System;
using System.Text;

namespace LabBenchStudios.Pdt.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IotDataContext
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string name = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string deviceID = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string modelID = ModelConst.IOT_DATA_CONTEXT_MODEL_ID;

        [JsonProperty]
        private int typeID = ConfigConst.DEFAULT_TYPE_ID;

        [JsonProperty]
        private int typeCategoryID = ConfigConst.DEFAULT_TYPE_CATEGORY_ID;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string deviceUUID = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string locationID = ConfigConst.NOT_SET;

        [JsonProperty]
        private float latitude = 0.0f;

        [JsonProperty]
        private float longitude = 0.0f;

        [JsonProperty]
        private float elevation = 0.0f;

        [JsonProperty]
        private float heading = 0.0f;

        [JsonProperty]
        private float timeOffsetSeconds = 0.0f;

        [JsonProperty]
        private int statusCode = 0;

        [JsonProperty]
        private bool hasError = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string timeStamp = ConfigConst.NOT_SET;

        // necessary for JSON serialization / deserialization
        public IotDataContext()
        {
            this.UpdateTimeStamp();
        }

        public IotDataContext(string name, string deviceID, int typeCategoryID, int typeID)
        {
            if (! string.IsNullOrEmpty(name)) { this.name = name; }
            if (! string.IsNullOrEmpty(deviceID)) { this.deviceID = deviceID; }

            if (typeCategoryID >= 0) { this.typeCategoryID = typeCategoryID; }
            if (typeID >= 0) { this.typeID = typeID; }

            // set the DTMI (modelID) - only for those that may be deserialized from the EDA
            // all others will keep the default of ModelConst.IOT_DATA_CONTEXT_MODEL_ID
            //
            // for now, this will suffice as a simple 'mapping' table, and also mitigate
            // any need to update the EDA with knowledge of DTMI naming conventions
            switch (typeID)
            {
                case ConfigConst.HUMIDIFIER_ACTUATOR_TYPE:
                    this.modelID = ModelConst.HUMIDIFIER_CONTROLLER_MODEL_ID; break;

                case ConfigConst.HVAC_ACTUATOR_TYPE:
                    this.modelID = ModelConst.THERMOSTAT_CONTROLLER_MODEL_ID; break;

                case ConfigConst.SYSTEM_PERF_TYPE:
                    this.modelID = ModelConst.DEVICE_SYS_PERF_TELEMETRY_MODEL_ID; break;

                case ConfigConst.HUMIDITY_SENSOR_TYPE:
                    this.modelID = ModelConst.ENV_SENSORS_TELEMETRY_MODEL_ID; break;

                case ConfigConst.TEMP_SENSOR_TYPE:
                    this.modelID = ModelConst.ENV_SENSORS_TELEMETRY_MODEL_ID; break;

                case ConfigConst.PRESSURE_SENSOR_TYPE:
                    this.modelID = ModelConst.ENV_SENSORS_TELEMETRY_MODEL_ID; break;

                case ConfigConst.FLUID_RATE_SENSOR_TYPE:
                    this.modelID = ModelConst.FLUID_PUMP_TELEMETRY_MODEL_ID; break;

                case ConfigConst.WIND_SYSTEM_TYPE:
                    this.modelID = ModelConst.POWER_WINDMILL_TELEMETRY_MODEL_ID; break;
            }

            this.UpdateTimeStamp();
        }

        // public methods

        public string GetName() { return this.name; }

        public string GetDeviceID() { return this.deviceID; }

        public string GetModelID() {  return this.modelID; }

        public int GetDeviceType() { return this.typeID; }

        public int GetDeviceCategory() { return this.typeCategoryID; }

        public float GetLatitude() { return this.latitude; }

        public float GetLongitude() { return this.longitude; }

        public float GetElevation() { return this.elevation; }

        public float GetHeading() { return this.heading; }

        public string GetLocationID() { return this.locationID; }

        public int GetStatusCode() { return this.statusCode; }

        public string GetTimeStamp() { return this.timeStamp; }

        public int GetTypeID() { return this.typeID; }

        public int GetTypeCategoryID() { return this.typeCategoryID; }

        public void OverrideTimeStamp(string timeStamp) { this.timeStamp = timeStamp; }

        public void SetLatitude(float val) { this.latitude = val; }

        public void SetLongitude(float val) { this.longitude = val; }

        public void SetElevation(float val) { this.elevation = val; }

        public void SetHeading(float val) { this.heading = val; }

        public void SetLocationID(string name) { if (! string.IsNullOrEmpty(name)) this.locationID = name; }

        public void SetStatusCode(int val) {  this.statusCode = val; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(ConfigConst.NAME_PROP).Append('=').Append(this.name).Append(',');
            sb.Append(ConfigConst.DEVICE_ID_PROP).Append('=').Append(this.deviceID).Append(',');
            sb.Append(ConfigConst.MODEL_ID_PROP).Append('=').Append(this.modelID).Append(',');
            sb.Append(ConfigConst.TYPE_ID_PROP).Append('=').Append(this.typeID).Append(',');
            sb.Append(ConfigConst.TYPE_CATEGORY_ID_PROP).Append('=').Append(this.typeCategoryID).Append(',');
            sb.Append(ConfigConst.TIMESTAMP_PROP).Append('=').Append(this.timeStamp).Append(',');
            sb.Append(ConfigConst.STATUS_CODE_PROP).Append('=').Append(this.statusCode).Append(',');
            sb.Append(ConfigConst.HAS_ERROR_PROP).Append('=').Append(this.hasError).Append(',');
            sb.Append(ConfigConst.LOCATION_ID_PROP).Append('=').Append(this.locationID).Append(',');
            sb.Append(ConfigConst.LATITUDE_PROP).Append('=').Append(this.latitude).Append(',');
            sb.Append(ConfigConst.LONGITUDE_PROP).Append('=').Append(this.longitude).Append(',');
            sb.Append(ConfigConst.ELEVATION_PROP).Append('=').Append(this.elevation);

            return sb.ToString();
        }

        // private methods

        private void UpdateTimeStamp()
        {
            this.timeStamp = DateTime.Now.ToUniversalTime().ToString("o");
        }
    }
}
