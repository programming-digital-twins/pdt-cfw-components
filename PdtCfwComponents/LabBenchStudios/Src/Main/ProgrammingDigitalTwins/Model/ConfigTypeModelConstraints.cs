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

using System.Text;

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
    public class ConfigTypeModelConstraints
    {
        [JsonProperty]
        private bool enableConstraints = false;

        [JsonProperty]
        private bool enableDutyCycle = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float dutyCycleSeconds = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float minDutyCycle = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float maxDutyCycle = 100.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float optimalDutyCycle = 50.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float minReading = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float maxReading = 100.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float optimalReading = 50.0f;


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
        public float GetMinReading()
        {
            return this.minReading;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetMaxReading()
        {
            return this.maxReading;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetOptimalReading()
        {
            return this.optimalReading;
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
        /// <param name="val"></param>
        public void SetMinReading(float val)
        {
            this.minReading = val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetMaxReading(float val)
        {
            this.maxReading = val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetOptimalReading(float val)
        {
            this.optimalReading = val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(",enableConstraints=").Append(this.enableConstraints);
            sb.Append(",enableDutyCycle=").Append(this.enableDutyCycle);
            sb.Append(",dutyCycleSeconds=").Append(this.dutyCycleSeconds);
            sb.Append(",minDutyCycle=").Append(this.minDutyCycle);
            sb.Append(",maxDutyCycle=").Append(this.maxDutyCycle);
            sb.Append(",optimalDutyCycle=").Append(this.optimalDutyCycle);
            sb.Append(",minReading=").Append(this.minReading);
            sb.Append(",maxReading=").Append(this.maxReading);
            sb.Append(",optimalReading=").Append(this.optimalReading);

            return sb.ToString();
        }

    }

}
