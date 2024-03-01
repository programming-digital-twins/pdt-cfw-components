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
    public class SystemPerformanceData : IotDataContext
    {
        [JsonProperty]
        private float cpuUtil = 0.0f;

        [JsonProperty]
        private float memUtil = 0.0f;

        [JsonProperty]
        private float diskUtil = 0.0f;

        // necessary for JSON serialization / deserialization
        public SystemPerformanceData() : base() { }

        public SystemPerformanceData(string name, string deviceID) :
            base(name, deviceID, ConfigConst.SYSTEM_TYPE_CATEGORY, ConfigConst.SYSTEM_PERF_TYPE)
        {

        }

        // public methods

        public float GetCpuUtilization() { return this.cpuUtil; }

        public float GetMemoryUtilization() { return this.memUtil; }

        public float GetDiskUtilization() { return this.diskUtil; }

        public void SetCpuUtilization(float val) { if (val >= 0 || val <= 100) this.cpuUtil = val; base.UpdateTimeStamp(); }

        public void SetMemoryUtilization(float val) { if (val >= 0 || val <= 100) this.memUtil = val; base.UpdateTimeStamp(); }

        public void SetDiskUtilization(float val) { if (val >= 0 || val <= 100) this.diskUtil = val; base.UpdateTimeStamp(); }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(',');
            sb.Append(ConfigConst.CPU_UTIL_PROP).Append('=').Append(this.cpuUtil).Append(',');
            sb.Append(ConfigConst.MEM_UTIL_PROP).Append('=').Append(this.memUtil).Append(',');
            sb.Append(ConfigConst.DISK_UTIL_PROP).Append('=').Append(this.diskUtil);

            return sb.ToString();
        }

        public void UpdateData(SystemPerformanceData data)
        {
            if (data != null)
            {
                base.UpdateData(data);

                this.cpuUtil = data.GetCpuUtilization();
                this.memUtil = data.GetMemoryUtilization();
                this.diskUtil = data.GetDiskUtilization();

                this.UpdateTimeStamp();
            }
        }
    }
}
