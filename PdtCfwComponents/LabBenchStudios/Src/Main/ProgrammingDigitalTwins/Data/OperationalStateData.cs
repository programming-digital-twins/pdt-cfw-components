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
    public class OperationalStateData : IotDataContext
    {
        [JsonProperty]
        private string sysStartTime = ConfigConst.NOT_SET;

        [JsonProperty]
        private int sysStartCount = 0;

        [JsonProperty]
        private int sysStopCount = 0;

        [JsonProperty]
        private int sysOpsCycleCount = 0;

        [JsonProperty]
        private int maintEventCount = 0;

        [JsonProperty]
        private float avgCycleSeconds = 0.0f;

        [JsonProperty]
        private float sysDowntimeSeconds = 0.0f;

        [JsonProperty]
        private float sysUptimeSeconds = 0.0f;

        // empty constructor necessary for JSON serialization / deserialization

        /// <summary>
        /// 
        /// </summary>
        public OperationalStateData() : base() { }

        /// <summary>
        /// Use the OPS_STATE_CATEGORY for the type category. Type ID and name can
        /// be set by the source system, but will default to OPS_STATE_TYPE.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="deviceID"></param>
        public OperationalStateData(string name, string deviceID) :
            base(name, deviceID, ConfigConst.OPS_STATE_CATEGORY, ConfigConst.OPS_STATE_TYPE)
        {

        }

        // public methods

        public string GetSystemStartTime() { return this.sysStartTime; }

        public int GetSystemStartCount() { return this.sysStartCount; }

        public int GetSystemStopCount() { return this.sysStopCount; }

        public void SetSystemStartTime(long startTimeEpoch)
        {
            if (startTimeEpoch > 0l)
            {
                var dto = DateTimeOffset.FromUnixTimeSeconds(startTimeEpoch);
                this.sysStartTime = dto.UtcDateTime.ToString("o");

                base.UpdateTimeStamp();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(',');
            sb.Append(ConfigConst.SYS_START_TIME_PROP).Append('=').Append(this.sysStartTime).Append(',');

            return sb.ToString();
        }

        public void UpdateData(OperationalStateData data)
        {
            if (data != null)
            {
                base.UpdateData(data);

                this.sysStartTime = data.GetSystemStartTime();

                this.UpdateTimeStamp();
            }
        }
    }
}
