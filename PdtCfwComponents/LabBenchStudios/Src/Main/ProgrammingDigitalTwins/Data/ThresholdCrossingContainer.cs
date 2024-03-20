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
using System.Diagnostics;
using System.Text;

namespace LabBenchStudios.Pdt.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ThresholdCrossingContainer
    {
        [JsonProperty]
        private int totalMaxCrossingEvents = 0;

        [JsonProperty]
        private int totalMinCrossingEvents = 0;

        [JsonProperty]
        private long totalMillisBetweenMaxCrossingEvents = 0;

        [JsonProperty]
        private long totalMillisBetweenMinCrossingEvents = 0;

        [JsonProperty]
        private float greatestMaxThresholdCrossingValue = 0.0f;

        [JsonProperty]
        private float greatestMinThresholdCrossingValue = 0.0f;

        private Stopwatch maxCrossingsDuration = new Stopwatch();
        private Stopwatch minCrossingsDuration = new Stopwatch();

        public ThresholdCrossingContainer() : base() { }


        // public methods

        public long GetAvgDurationBetweenEachMaxThresholdCrossingEvents()
        {
            if (this.totalMaxCrossingEvents > 0 && this.totalMillisBetweenMaxCrossingEvents > 0)
            {
                return (this.totalMillisBetweenMaxCrossingEvents / this.totalMaxCrossingEvents);
            }
            else
            {
                return 0;
            }
        }

        public long GetAvgDurationBetweenEachMinThresholdCrossingEvents()
        {
            if (this.totalMinCrossingEvents > 0 && this.totalMillisBetweenMinCrossingEvents > 0)
            {
                return (this.totalMillisBetweenMinCrossingEvents / this.totalMinCrossingEvents);
            }
            else
            {
                return 0;
            }
        }

        public long GetDurationBetweenMaxThresholdCrossingEvents()
        {
            return this.totalMillisBetweenMaxCrossingEvents;
        }

        public long GetDurationBetweenMinThresholdCrossingEvents()
        {
            return this.totalMillisBetweenMinCrossingEvents;
        }

        public long GetDurationSinceLastMaxThresholdCrossingEvent()
        {
            if (this.maxCrossingsDuration.IsRunning)
            {
                return (this.maxCrossingsDuration.ElapsedMilliseconds - this.totalMillisBetweenMaxCrossingEvents);
            }
            else
            {
                return 0;
            }
        }

        public long GetDurationSinceLastMinThresholdCrossingEvent()
        {
            if (this.minCrossingsDuration.IsRunning)
            {
                return (this.minCrossingsDuration.ElapsedMilliseconds - this.totalMillisBetweenMinCrossingEvents);
            }
            else
            {
                return 0;
            }
        }

        public float GetGreatestMaxThresholdCrossingValue()
        {
            return this.greatestMaxThresholdCrossingValue;
        }

        public float GetGreatestMinThresholdCrossingValue()
        {
            return this.greatestMinThresholdCrossingValue;
        }

        public int GetTotalMaxThresholdCrossingEvents() {  return this.totalMaxCrossingEvents; }

        public int GetTotalMinThresholdCrossingEvents() { return this.totalMinCrossingEvents; }

        public void IncrementMaxThresholdCrossingEvents(float val)
        {
            if (Math.Abs(val) > Math.Abs(this.greatestMaxThresholdCrossingValue))
            {
                this.greatestMaxThresholdCrossingValue = val;
            }

            if (this.totalMaxCrossingEvents == 0)
            {
                this.maxCrossingsDuration.Start();
            }
            else
            {
                this.totalMillisBetweenMaxCrossingEvents = this.maxCrossingsDuration.ElapsedMilliseconds;
            }

            ++this.totalMaxCrossingEvents;
        }

        public void IncrementMinThresholdCrossingEvents(float val)
        {
            if (Math.Abs(val) > Math.Abs(this.greatestMinThresholdCrossingValue))
            {
                this.greatestMinThresholdCrossingValue = val;
            }

            if (this.totalMinCrossingEvents == 0)
            {
                this.minCrossingsDuration.Start();
            }
            else
            {
                this.totalMillisBetweenMinCrossingEvents = this.minCrossingsDuration.ElapsedMilliseconds;
            }

            ++this.totalMinCrossingEvents;
        }

        public void ResetMaxThresholdCrossingEvents()
        {
            this.greatestMaxThresholdCrossingValue = 0.0f;
            this.totalMaxCrossingEvents = 0;
            this.totalMillisBetweenMaxCrossingEvents = 0;

            this.maxCrossingsDuration.Reset();
        }

        public void ResetMinThresholdCrossingEvents()
        {
            this.greatestMinThresholdCrossingValue = 0.0f;
            this.totalMinCrossingEvents = 0;
            this.totalMillisBetweenMinCrossingEvents = 0;

            this.minCrossingsDuration.Reset();
        }

        public void ResetAllThresholdCrossingEvents()
        {
            this.ResetMaxThresholdCrossingEvents();
            this.ResetMinThresholdCrossingEvents();
        }

        public void StopTrackingMaxThresholdCrossingEvents()
        {
            this.maxCrossingsDuration.Stop();
        }

        public void StopTrackingMinThresholdCrossingEvents()
        {
            this.minCrossingsDuration.Stop();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            /*
            sb.Append(ConfigConst.UNIT_PROP).Append('=').Append(this.unit).Append(',');
            sb.Append(ConfigConst.VALUE_PROP).Append('=').Append(this.value).Append(',');
            sb.Append(ConfigConst.TARGET_VALUE_PROP).Append('=').Append(this.targetValue).Append(',');
            sb.Append(ConfigConst.NOMINAL_VALUE_DELTA_PROP).Append('=').Append(this.nominalValueDelta).Append(',');
            sb.Append(ConfigConst.MAX_VALUE_DELTA_PROP).Append('=').Append(this.maxValueDelta).Append(',');
            sb.Append(ConfigConst.RANGE_NOMINAL_FLOOR_PROP).Append('=').Append(this.rangeNominalFloor).Append(',');
            sb.Append(ConfigConst.RANGE_NOMINAL_CEILING_PROP).Append('=').Append(this.rangeNominalCeiling).Append(',');
            sb.Append(ConfigConst.RANGE_MAX_FLOOR_PROP).Append('=').Append(this.rangeMaxFloor).Append(',');
            sb.Append(ConfigConst.RANGE_MAX_CEILING_PROP).Append('=').Append(this.rangeMaxCeiling);
            */

            return sb.ToString();
        }

    }

}
