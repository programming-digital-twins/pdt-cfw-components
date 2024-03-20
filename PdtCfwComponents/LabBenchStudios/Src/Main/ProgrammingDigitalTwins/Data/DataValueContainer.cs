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
    public class DataValueContainer
    {
        [JsonProperty]
        private string unit = ConfigConst.NOT_SET;

        [JsonProperty]
        private float value = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float targetValue = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float nominalValueDelta = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float maxValueDelta = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float rangeMaxFloor = float.MinValue;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float rangeNominalFloor = float.MinValue;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float rangeMaxCeiling = float.MaxValue;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float rangeNominalCeiling = float.MaxValue;

        // necessary for JSON serialization / deserialization
        public DataValueContainer() : base() { }


        // public methods

        public string GetUnit() {  return unit; }

        public float GetValue() { return this.value; }

        public float GetTargetValue() { return this.targetValue; }

        public float GetNominalDeltaValue() { return this.nominalValueDelta; }

        public float GetMaxDeltaValue() { return this.maxValueDelta; }

        public float GetRangeMaxFloor() { return this.rangeMaxFloor; }

        public float GetRangeMaxCeiling() {  return this.rangeMaxCeiling; }

        public float GetRangeNominalFloor() { return this.rangeNominalFloor; }

        public float GetRangeNominalCeiling() { return this.rangeNominalCeiling; }

        public void SetUnit(string unit) { this.unit = unit; }

        public void SetValue(float val) { this.value = val; }

        public void SetTargetValue(float val) { this.targetValue = val; }

        public void SetNominalValueDelta(float val) { this.nominalValueDelta = val; }

        public void SetMaxValueDelta(float val) { this.maxValueDelta = val; }

        public void SetRangeMaxFloor(float val) { this.rangeMaxFloor = val; }

        public void SetRangeMaxCeiling(float val) { this.rangeMaxCeiling = val; }

        public void SetRangeNominalFloor(float val) { this.rangeNominalFloor = val; }

        public void SetRangeNominalCeiling(float val) { this.rangeNominalCeiling = val; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(ConfigConst.UNIT_PROP).Append('=').Append(this.unit).Append(',');
            sb.Append(ConfigConst.VALUE_PROP).Append('=').Append(this.value).Append(',');
            sb.Append(ConfigConst.TARGET_VALUE_PROP).Append('=').Append(this.targetValue).Append(',');
            sb.Append(ConfigConst.NOMINAL_VALUE_DELTA_PROP).Append('=').Append(this.nominalValueDelta).Append(',');
            sb.Append(ConfigConst.MAX_VALUE_DELTA_PROP).Append('=').Append(this.maxValueDelta).Append(',');
            sb.Append(ConfigConst.RANGE_NOMINAL_FLOOR_PROP).Append('=').Append(this.rangeNominalFloor).Append(',');
            sb.Append(ConfigConst.RANGE_NOMINAL_CEILING_PROP).Append('=').Append(this.rangeNominalCeiling).Append(',');
            sb.Append(ConfigConst.RANGE_MAX_FLOOR_PROP).Append('=').Append(this.rangeMaxFloor).Append(',');
            sb.Append(ConfigConst.RANGE_MAX_CEILING_PROP).Append('=').Append(this.rangeMaxCeiling);

            return sb.ToString();
        }

        public void UpdateData(DataValueContainer data)
        {
            if (data != null)
            {
                this.unit = data.GetUnit();
                this.value = data.GetValue();
                this.targetValue = data.GetTargetValue();
                this.nominalValueDelta = data.GetNominalDeltaValue();
                this.maxValueDelta = data.GetMaxDeltaValue();
                this.rangeNominalFloor = data.GetRangeNominalFloor();
                this.rangeMaxFloor = data.GetRangeMaxFloor();
                this.rangeNominalCeiling = data.GetRangeNominalCeiling();
                this.rangeMaxCeiling = data.GetRangeMaxCeiling();
            }
        }
    }
}
