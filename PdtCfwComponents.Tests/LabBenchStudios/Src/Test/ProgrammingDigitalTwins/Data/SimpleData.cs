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

namespace LabBenchStudios.Pdt.Test.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SimpleData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        string Name { get; set; } = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        string DeviceID { get; set; } = ConfigConst.NOT_SET;

        [JsonProperty]
        int TypeID { get; set; } = ConfigConst.DEFAULT_TYPE_ID;

        [JsonProperty]
        int TypeCategoryID { get; set; } = ConfigConst.DEFAULT_TYPE_CATEGORY_ID;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        string TimeStamp = ConfigConst.NOT_SET;

        // necessary for JSON serialization / deserialization
        public SimpleData()
        {
            this.UpdateTimeStamp();
        }

        public SimpleData(string name, string deviceID, int typeCategoryID, int typeID)
        {
            if (! string.IsNullOrEmpty(name)) { this.Name = name; }
            if (! string.IsNullOrEmpty(deviceID)) { this.DeviceID = deviceID; }

            if (typeCategoryID >= 0) { this.TypeCategoryID = typeCategoryID; }
            if (typeID >= 0) { this.TypeID = typeID; }

            this.UpdateTimeStamp();
        }

        // public methods

        public string GetName() { return this.Name; }

        public string GetDeviceID() { return this.DeviceID; }

        public int GetDeviceType() { return this.TypeID; }

        public int GetDeviceCategory() { return this.TypeCategoryID; }

        public string GetTimeStamp() { return this.TimeStamp; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(ConfigConst.NAME_PROP).Append('=').Append(this.Name).Append(',');
            sb.Append(ConfigConst.TYPE_ID_PROP).Append('=').Append(this.TypeID).Append(',');
            sb.Append(ConfigConst.TYPE_CATEGORY_ID_PROP).Append('=').Append(this.TypeCategoryID).Append(',');
            sb.Append(ConfigConst.TIMESTAMP_PROP).Append('=').Append(this.TimeStamp);

            return sb.ToString();
        }

        // private methods

        private void UpdateTimeStamp()
        {
            this.TimeStamp = DateTime.Now.ToUniversalTime().ToString("o");
        }
    }
}
