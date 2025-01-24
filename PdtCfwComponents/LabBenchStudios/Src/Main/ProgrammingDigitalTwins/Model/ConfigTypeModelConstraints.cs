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

using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using LabBenchStudios.Pdt.Data;
using Newtonsoft.Json;

/// 
/// 
/// 
namespace LabBenchStudios.Pdt.Model
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ConfigTypeModelConstraints : DataValueContainer
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private bool enableConstraints = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private bool enableDutyCycle = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float dutyCycleSeconds = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float minDutyCycle = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float maxDutyCycle = 100.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float optimalDutyCycle = 50.0f;


        // necessary for JSON serialization / deserialization

        /// <summary>
        /// 
        /// </summary>
        public ConfigTypeModelConstraints() : base()
        {
            // nothing to do
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool AreConstraintsEnabled()
        {
            return this.enableConstraints;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAllDataAsTable()
        {
            Dictionary<string, string> dataTable = new Dictionary<string, string>();

            dataTable.Add("propName", base.GetPropertyName());
            dataTable.Add("dutyCycleSeconds", this.dutyCycleSeconds.ToString());
            dataTable.Add("unit", base.GetUnit());
            dataTable.Add("maxReading", base.GetMaxReading().ToString());
            dataTable.Add("minReading", base.GetMinReading().ToString());
            dataTable.Add("targetVal", base.GetTargetValue().ToString());
            dataTable.Add("nomDutyCycle", this.optimalDutyCycle.ToString());
            dataTable.Add("minDutyCycle", this.minDutyCycle.ToString());
            dataTable.Add("maxDutyCycle", this.maxDutyCycle.ToString());
            dataTable.Add("rangeNomCeiling", base.GetRangeNominalCeiling().ToString());
            dataTable.Add("rangeNomFloor", base.GetRangeNominalFloor().ToString());
            dataTable.Add("rangeMaxCeil", base.GetRangeMaxCeiling().ToString());
            dataTable.Add("rangeMaxFloor", base.GetRangeMaxFloor().ToString());
            dataTable.Add("maxCeilCrossings", base.GetMaxCeilingCrossings().ToString());
            dataTable.Add("maxFloorCrossings", base.GetMaxFloorCrossings().ToString());
            dataTable.Add("maxDeltaCrossings", base.GetMaxDeltaCrossings().ToString());
            dataTable.Add("nomDeltaVal", base.GetNominalDeltaValue().ToString());
            dataTable.Add("maxDeltaVal", base.GetMaxDeltaValue().ToString());
            dataTable.Add("maxMeasuredDeltaVal", base.GetMaxMeasuredDeltaValue().ToString());

            return dataTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetDutyCycleSeconds()
        {
            return this.dutyCycleSeconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetMinDutyCycle()
        {
            return this.minDutyCycle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetMaxDutyCycle()
        {
            return this.maxDutyCycle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetOptimalDutyCycle()
        {
            return this.optimalDutyCycle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsDutyCycleEnabled()
        {
            return this.enableDutyCycle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void SetEnableConstraints(bool enable)
        {
            this.enableConstraints = enable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void SetEnableDutyCycle(bool enable)
        {
            this.enableDutyCycle = enable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetDutyCycleSeconds(float val)
        {
            this.dutyCycleSeconds = val;

            if (this.dutyCycleSeconds > 0.0f)
            {
                this.enableDutyCycle = true;
            } else
            {
                this.enableDutyCycle = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetMinDutyCycle(float val)
        {
            if (val >= 0.0f && val <= 100.0f)
            {
                this.minDutyCycle = val;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetMaxDutyCycle(float val)
        {
            if (val >= 0.0f && val <= 100.0f)
            {
                this.maxDutyCycle = val;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetOptimalDutyCycle(float val)
        {
            if (val >= 0.0f && val <= 100.0f)
            {
                this.optimalDutyCycle = val;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraints"></param>
        public void UpdateData(ConfigTypeModelConstraints constraints)
        {
            if (constraints != null)
            {
                base.UpdateData((DataValueContainer) constraints);

                this.enableConstraints = constraints.AreConstraintsEnabled();
                this.enableDutyCycle = constraints.IsDutyCycleEnabled();
                this.dutyCycleSeconds = constraints.GetDutyCycleSeconds();
                this.minDutyCycle = constraints.GetMinDutyCycle();
                this.maxDutyCycle = constraints.GetMaxDutyCycle();
                this.optimalDutyCycle = constraints.GetOptimalDutyCycle();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(",enableConstraints=").Append(this.enableConstraints);
            sb.Append(",enableDutyCycle=").Append(this.enableDutyCycle);
            sb.Append(",dutyCycleSeconds=").Append(this.dutyCycleSeconds);
            sb.Append(",minDutyCycle=").Append(this.minDutyCycle);
            sb.Append(",maxDutyCycle=").Append(this.maxDutyCycle);
            sb.Append(",optimalDutyCycle=").Append(this.optimalDutyCycle);

            return sb.ToString();
        }

    }

}
