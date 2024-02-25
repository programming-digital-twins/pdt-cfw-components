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
    public class ThermostatModelController : DigitalTwinModelState
    {
        [JsonProperty]
        private float currentTemperature = 0.0f;

        [JsonProperty]
        private float targetTemperature = 0.0f;

        [JsonProperty]
        private float minTemperature = 0.0f;

        [JsonProperty]
        private float maxTemperature = 0.0f;

        // necessary for JSON serialization / deserialization
        public ThermostatModelController() : base()
        {

        }

        public ThermostatModelController(string name, string deviceID) :
            base(name, deviceID, ConfigConst.DEFAULT_TYPE_CATEGORY_ID, ConfigConst.DEFAULT_TYPE_ID)
        {

        }

        // public methods

        public float GetCurrentTemperature() { return this.currentTemperature; }

        public float GetTargetTemperature() { return this.targetTemperature; }

        public float GetMinTemperature() { return this.minTemperature; }

        public float GetMaxTemperature() { return this.maxTemperature; }

        public void SetCurrentTemperature(float val)
        {
            if (val >= 0 || val <= 100) this.currentTemperature = val; base.UpdateTimeStamp();
        }

        public void SetTargetTemperature(float val)
        {
            if (val >= 0 || val <= 100) this.targetTemperature = val; base.UpdateTimeStamp();
        }

        public void SetMinTemperature(float val)
        {
            if (val >= 0 || val <= 100) this.minTemperature = val; base.UpdateTimeStamp();
        }

        public void SetMaxTemperature(float val)
        {
            if (val >= 0 || val <= 100) this.maxTemperature = val; base.UpdateTimeStamp();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(',');
            sb.Append(ModelConst.CURRENT_TEMPERATURE_PROP_NAME).Append('=').Append(this.currentTemperature).Append(',');
            sb.Append(ModelConst.TARGET_TEMPERATURE_PROP_NAME).Append('=').Append(this.targetTemperature).Append(',');
            sb.Append(ModelConst.MIN_TEMPERATURE_PROP_NAME).Append('=').Append(this.targetTemperature).Append(',');
            sb.Append(ModelConst.MAX_TEMPERATURE_PROP_NAME).Append('=').Append(this.targetTemperature).Append(',');

            return sb.ToString();
        }

    }
}
