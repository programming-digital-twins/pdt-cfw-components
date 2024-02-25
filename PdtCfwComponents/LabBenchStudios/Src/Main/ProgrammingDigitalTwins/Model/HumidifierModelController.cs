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
using LabBenchStudios.Pdt.Model;

using Newtonsoft.Json;

using System.Text;

namespace LabBenchStudios.Pdt.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HumidifierModelController : DigitalTwinModelState
    {
        [JsonProperty]
        private float relativeHumidity = 0.0f;

        [JsonProperty]
        private float targetHumidity = 0.0f;

        [JsonProperty]
        private float minHumidity = 0.0f;

        [JsonProperty]
        private float maxHumidity = 0.0f;

        // necessary for JSON serialization / deserialization
        public HumidifierModelController() : base() { }

        public HumidifierModelController(string name, string deviceID) :
            base(name, deviceID, ConfigConst.ENV_TYPE_CATEGORY, ConfigConst.HUMIDIFIER_ACTUATOR_TYPE)
        {

        }

        // public methods

        public float GetRelativeHumidity() { return this.relativeHumidity; }

        public float GetTargetHumidity() { return this.targetHumidity; }

        public float GetMinHumidity() { return this.minHumidity; }

        public float GetMaxHumidity() { return this.maxHumidity; }

        public void SetRelativeHumidity(float val) { if (val >= 0 || val <= 100) this.relativeHumidity = val; base.UpdateTimeStamp(); }

        public void SetTargetHumidity(float val) { if (val >= 0 || val <= 100) this.targetHumidity = val; base.UpdateTimeStamp(); }

        public void SetMinHumidity(float val) { if (val >= 0 || val <= 100) this.minHumidity = val; base.UpdateTimeStamp(); }

        public void SetMaxHumidity(float val) { if (val >= 0 || val <= 100) this.maxHumidity = val; base.UpdateTimeStamp(); }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(',');
            sb.Append(ModelConst.RELATIVE_HUMIDITY_PROP_NAME).Append('=').Append(this.relativeHumidity).Append(',');
            sb.Append(ModelConst.TARGET_HUMIDITY_PROP_NAME).Append('=').Append(this.targetHumidity).Append(',');
            sb.Append(ModelConst.MIN_HUMIDITY_PROP_NAME).Append('=').Append(this.minHumidity).Append(',');
            sb.Append(ModelConst.MAX_HUMIDITY_PROP_NAME).Append('=').Append(this.maxHumidity).Append(',');

            return sb.ToString();
        }

    }
}
