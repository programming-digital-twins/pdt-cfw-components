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

using System.Text;

namespace LabBenchStudios.Pdt.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SensorData : IotDataContext
    {
        [JsonProperty]
        private float value = 0.0f;

        [JsonProperty]
        private float rangeFloor = float.MinValue;

        [JsonProperty]
        private float rangeCeiling = float.MaxValue;

        // necessary for JSON serialization / deserialization
        public SensorData() : base() { }

        public SensorData(string name, string deviceID, int typeCategoryID, int typeID) :
            base(name, deviceID, typeCategoryID, typeID)
        {

        }

        // public methods

        public float GetValue() { return this.value; }

        public float GetRangeFloor() { return this.rangeFloor; }

        public float GetRangeCeiling() {  return this.rangeCeiling; }

        public void SetValue(float val) { this.value = val; base.UpdateTimeStamp(); }

        public void SetRangeFloor(float val) { this.rangeFloor = val; base.UpdateTimeStamp(); }

        public void SetRangeCeiling(float val) { this.rangeCeiling = val; base.UpdateTimeStamp(); }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(',');
            sb.Append(ConfigConst.VALUE_PROP).Append('=').Append(this.value).Append(',');
            sb.Append(ConfigConst.RANGE_FLOOR_PROP).Append('=').Append(this.rangeFloor).Append(',');
            sb.Append(ConfigConst.RANGE_CEILING_PROP).Append('=').Append(this.rangeCeiling).Append(',');

            return sb.ToString();
        }

        public void UpdateData(SensorData data)
        {
            if (data != null)
            {
                base.UpdateData(data);

                this.value = data.GetValue();
                this.rangeFloor = data.GetRangeFloor();
                this.rangeCeiling = data.GetRangeCeiling();

                this.UpdateTimeStamp();
            }
        }
    }
}
