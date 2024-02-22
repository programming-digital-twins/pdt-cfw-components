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
using LabBenchStudios.Pdt.Data;

using Newtonsoft.Json;

using System.Text;

namespace LabBenchStudios.Pdt.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SampleDataModel : IotDataContext
    {
        [JsonProperty]
        private float changeMe = 0.0f;

        [JsonProperty]
        private float changeMe2 = 0.0f;

        // necessary for JSON serialization / deserialization
        public SampleDataModel() : base() { }

        public SampleDataModel(string name, string deviceID) :
            base(name, deviceID, ConfigConst.DEFAULT_TYPE_CATEGORY_ID, ConfigConst.DEFAULT_TYPE_ID)
        {

        }

        // public methods

        public float GetChangeMeValue() { return this.changeMe; }

        public float GetChangeMe2Value() { return this.changeMe2; }

        public void SetChangeMeValue(float val) { if (val >= 0 || val <= 100) this.changeMe = val; }

        public void SetChangeMe2Value(float val) { if (val >= 0 || val <= 100) this.changeMe2 = val; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(',');
            sb.Append(ConfigConst.CPU_UTIL_PROP).Append('=').Append(this.changeMe).Append(',');
            sb.Append(ConfigConst.MEM_UTIL_PROP).Append('=').Append(this.changeMe2).Append(',');

            return sb.ToString();
        }

    }
}
