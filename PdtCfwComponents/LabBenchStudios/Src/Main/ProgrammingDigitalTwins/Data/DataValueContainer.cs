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
    public class DataValueContainer
    {
        [JsonProperty]
        private string propName = ConfigConst.NOT_SET;

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
        private float maxMeasuredDelta = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float rangeMaxFloor = float.MinValue;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float rangeNominalFloor = float.MinValue;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float rangeMaxCeiling = float.MaxValue;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float rangeNominalCeiling = float.MaxValue;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float minReading = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float maxReading = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int floorCrossings = 0;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int ceilingCrossings = 0;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int maxDeltaCrossings = 0;

        // local instance state
        private bool areConstraintsInitialized = false;

        // necessary for JSON serialization / deserialization

        /// <summary>
        /// 
        /// </summary>
        public DataValueContainer() : base()
        {
            this.InitDeltaRanges();
        }


        // public methods

        public string GetPropertyName() { return this.propName; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetUnit() { return this.unit; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetValue() { return this.value; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetMaxReading() { return this.maxReading; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetMinReading() { return this.minReading; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetTargetValue() { return this.targetValue; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetNominalDeltaValue() { return this.nominalValueDelta; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetMaxDeltaValue() { return this.maxValueDelta; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetMaxMeasuredDeltaValue() { return this.maxMeasuredDelta; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetMaxCeilingCrossings() { return this.ceilingCrossings; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetMaxDeltaCrossings() { return this.maxDeltaCrossings; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetMaxFloorCrossings() { return this.floorCrossings; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetRangeMaxFloor() { return this.rangeMaxFloor; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetRangeMaxCeiling() {  return this.rangeMaxCeiling; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetRangeNominalFloor() { return this.rangeNominalFloor; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetRangeNominalCeiling() { return this.rangeNominalCeiling; }

        /// <summary>
        /// This is destructive as it will reset all crossing counters.
        /// Only invoke this if the intent is to really perform this action.
        /// </summary>
        public void ResetCrossingCounters()
        {
            this.ceilingCrossings = 0;
            this.floorCrossings = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void SetPropertyName(string name) { if (!string.IsNullOrEmpty(name)) this.propName = name; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        public void SetUnit(string unit) { if (!string.IsNullOrEmpty(unit)) this.unit = unit; }

        /// <summary>
        /// Sets the current value. If it's < minReading, or > maxReading, the
        /// appropriate value will also be updated.
        /// </summary>
        /// <param name="val"></param>
        public void SetValue(float val)
        {
            this.value = val;

            if (val > this.maxReading) { this.maxReading = val; }

            if (val < this.minReading) { this.minReading = val; }

            if (val > this.rangeMaxCeiling) { this.ceilingCrossings++; }

            if (val < this.rangeMaxFloor) { this.floorCrossings++; }

            this.maxMeasuredDelta = Math.Abs(this.maxReading - this.minReading);

            if (this.maxMeasuredDelta > this.maxValueDelta) { this.maxDeltaCrossings++; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetTargetValue(float val) { this.targetValue = val; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetNominalValueDelta(float val) { if (val > 0.0f) this.nominalValueDelta = val; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetMaxValueDelta(float val) { if (val > 0.0f) this.maxValueDelta = val; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetRangeMaxFloor(float val) { this.rangeMaxFloor = val; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetRangeMaxCeiling(float val) { this.rangeMaxCeiling = val; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetRangeNominalFloor(float val) { this.rangeNominalFloor = val; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetRangeNominalCeiling(float val) { this.rangeNominalCeiling = val; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // TODO: add the calculated values

            sb.Append(ConfigConst.NAME_PROP).Append('=').Append(this.propName).Append(',');
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraints"></param>
        public void UpdateConstraints(DataValueContainer constraints)
        {
            if (constraints != null)
            {
                this.propName = constraints.GetPropertyName();
                this.unit = constraints.GetUnit();
                this.targetValue = constraints.GetTargetValue();
                this.nominalValueDelta = constraints.GetNominalDeltaValue();
                this.maxValueDelta = constraints.GetMaxDeltaValue();
                this.rangeNominalFloor = constraints.GetRangeNominalFloor();
                this.rangeMaxFloor = constraints.GetRangeMaxFloor();
                this.rangeNominalCeiling = constraints.GetRangeNominalCeiling();
                this.rangeMaxCeiling = constraints.GetRangeMaxCeiling();

                this.areConstraintsInitialized = true;

                this.InitDeltaRanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void UpdateData(DataValueContainer data)
        {
            if (data != null)
            {
                // make sure constraints are set - it's possible the caller will
                // only invoke this UpdateData() method and assume constraints are
                // already applied - if not, invoke UpdateConstraints() as well
                if (! this.areConstraintsInitialized)
                {
                    this.UpdateConstraints(data);
                }

                // update dynamic data (values, min / max readings, etc.)
                this.SetValue(data.GetValue());

                this.minReading = data.GetMinReading();
                this.maxReading = data.GetMaxReading();
                this.maxMeasuredDelta = data.GetMaxMeasuredDeltaValue();
                this.maxDeltaCrossings = data.GetMaxDeltaCrossings();
                this.ceilingCrossings = data.GetMaxCeilingCrossings();
                this.floorCrossings = data.GetMaxFloorCrossings();
            }
        }

        // private

        /// <summary>
        /// Calculate the appropriate delta between nominal floor and ceiling
        /// and the max floor and ceiling.
        /// 
        /// NOTE: If the nominalValueDelta or maxValueDelta are > 0.0f,
        /// no action is taken.
        /// 
        /// Both nominalValueDelta and maxValueDelta should be positive float values.
        /// </summary>
        private void InitDeltaRanges()
        {
            if (this.nominalValueDelta <= 0.0f)
            {
                if (this.rangeNominalCeiling < this.rangeNominalFloor)
                {
                    (this.rangeNominalCeiling, this.rangeNominalFloor) = (this.rangeNominalFloor, this.rangeNominalCeiling);
                }

                if (this.rangeNominalFloor < 0.0f && this.rangeNominalCeiling >= 0.0f)
                {
                    this.nominalValueDelta = this.rangeNominalCeiling + Math.Abs(this.rangeNominalFloor);
                } else
                {
                    this.nominalValueDelta = Math.Abs(this.rangeNominalCeiling) - Math.Abs(this.rangeNominalFloor);
                }
            }

            if (this.maxValueDelta <= 0.0f)
            {
                if (this.rangeMaxCeiling < this.rangeMaxFloor)
                {
                    (this.rangeMaxCeiling, this.rangeMaxFloor) = (this.rangeMaxFloor, this.rangeMaxCeiling);
                }

                if (this.rangeMaxFloor < 0.0f && this.rangeMaxCeiling >= 0.0f)
                {
                    this.maxValueDelta = this.rangeMaxCeiling + Math.Abs(this.rangeMaxFloor);
                } else
                {
                    this.maxValueDelta = Math.Abs(this.rangeMaxCeiling) - Math.Abs(this.rangeMaxFloor);
                }
            }
        }
    }
}
