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
        [JsonProperty]
        private string deviceID = ConfigConst.NOT_SET;

        [JsonProperty]
        private int typeID = ConfigConst.DEFAULT_TYPE_ID;

        [JsonProperty]
        private int typeCategoryID = ConfigConst.DEFAULT_TYPE_CATEGORY_ID;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string name = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string typeName = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string typeCategoryName = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string deviceUUID = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string locationID = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float latitude = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float longitude = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float elevation = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float heading = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float timeOffsetSeconds = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int statusCode = 0;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string timeStamp = ConfigConst.NOT_SET;

        private bool hasError = false;

        // necessary for JSON serialization / deserialization
        public IotDataContext()
        {
            this.UpdateTimeStamp();
        }

        public IotDataContext(string name, string deviceID, string locationID)
        {
            if (!string.IsNullOrEmpty(name)) { this.name = name; }
            if (!string.IsNullOrEmpty(deviceID)) { this.deviceID = deviceID; }
            if (!string.IsNullOrEmpty(locationID)) { this.locationID = locationID; }

            this.UpdateTimeStamp();
        }

        public IotDataContext(string name, string deviceID, int typeCategoryID, int typeID)
        {
            if (! string.IsNullOrEmpty(name)) { this.name = name; }
            if (! string.IsNullOrEmpty(deviceID)) { this.deviceID = deviceID; }

            if (typeCategoryID >= 0) { this.typeCategoryID = typeCategoryID; }

            // timestamp will be updated with this setter call
            this.SetTypeID(typeID);
        }

        // public methods

        public string GetName() { return this.name; }

        public string GetDeviceID() { return this.deviceID; }

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

        public string GetTypeName() { return this.typeName; }

        public string GetTypeCategoryName() { return this.typeCategoryName; }

        public void OverrideTimeStamp(string timeStamp) { this.timeStamp = timeStamp; }

        public void SetLatitude(float val) { this.latitude = val; this.UpdateTimeStamp(); }

        public void SetLongitude(float val) { this.longitude = val; this.UpdateTimeStamp(); }

        public void SetElevation(float val) { this.elevation = val; this.UpdateTimeStamp(); }

        public void SetHeading(float val) { this.heading = val; this.UpdateTimeStamp(); }

        public void SetLocationID(string name) { if (! string.IsNullOrEmpty(name)) this.locationID = name; this.UpdateTimeStamp(); }

        public void SetStatusCode(int val) { this.statusCode = val; this.UpdateTimeStamp(); }

        public void SetDeviceID(string name) { if (!string.IsNullOrEmpty(name)) { this.deviceID = name; }; this.UpdateTimeStamp(); }

        public void SetName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                string prevName = this.name;
                this.name = name;

                if (!prevName.Equals(this.name))
                {
                    this.OnNameUpdate(prevName);
                    this.UpdateTimeStamp();
                }
            }
        }

        public void SetTypeCategoryID(int val)
        {
            if (val >= 0)
            {
                this.typeCategoryID = val;
                this.UpdateTimeStamp();
            }
        }

        public void SetTypeID(int val)
        {
            if (val >= 0)
            {
                this.typeID = val;
                this.UpdateTimeStamp();
            }
        }

        public void SetTypeName(string name)
        {
            if (! string.IsNullOrEmpty(name))
            {
                this.typeName = name;
                this.UpdateTimeStamp();
            }
        }

        public void SetTypeCategoryName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                this.typeCategoryName = name;
                this.UpdateTimeStamp();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(ConfigConst.NAME_PROP).Append('=').Append(this.name).Append(',');
            sb.Append(ConfigConst.DEVICE_ID_PROP).Append('=').Append(this.deviceID).Append(',');
            sb.Append(ConfigConst.TYPE_NAME_PROP).Append('=').Append(this.typeName).Append(',');
            sb.Append(ConfigConst.TYPE_ID_PROP).Append('=').Append(this.typeID).Append(',');
            sb.Append(ConfigConst.TYPE_CATEGORY_ID_PROP).Append('=').Append(this.typeCategoryID).Append(',');
            sb.Append(ConfigConst.TYPE_CATEGORY_NAME_PROP).Append('=').Append(this.typeCategoryName).Append(',');
            sb.Append(ConfigConst.TIMESTAMP_PROP).Append('=').Append(this.timeStamp).Append(',');
            sb.Append(ConfigConst.STATUS_CODE_PROP).Append('=').Append(this.statusCode).Append(',');
            sb.Append(ConfigConst.HAS_ERROR_PROP).Append('=').Append(this.hasError).Append(',');
            sb.Append(ConfigConst.LOCATION_ID_PROP).Append('=').Append(this.locationID).Append(',');
            sb.Append(ConfigConst.LATITUDE_PROP).Append('=').Append(this.latitude).Append(',');
            sb.Append(ConfigConst.LONGITUDE_PROP).Append('=').Append(this.longitude).Append(',');
            sb.Append(ConfigConst.ELEVATION_PROP).Append('=').Append(this.elevation);

            return sb.ToString();
        }

        public void UpdateData(IotDataContext data)
        {
            if (data != null)
            {
                this.name = data.GetName();
                this.deviceID = data.GetDeviceID();
                this.typeID = data.GetTypeID();
                this.typeName = data.GetTypeName();
                this.typeCategoryID = data.GetTypeCategoryID();
                this.typeCategoryName = data.GetTypeCategoryName();
                this.statusCode = data.GetStatusCode();
                this.hasError = data.hasError;
                this.locationID = data.GetLocationID();
                this.latitude = data.GetLatitude();
                this.longitude = data.GetLongitude();
                this.elevation = data.GetElevation();

                this.UpdateTimeStamp();
            }
        }

        // protected methods

        protected virtual void OnNameUpdate(string prevName)
        {
            // nothing to do - sub-class can override
        }

        protected void UpdateTimeStamp()
        {
            this.timeStamp = DateTime.Now.ToUniversalTime().ToString("o");
        }
    }
}
