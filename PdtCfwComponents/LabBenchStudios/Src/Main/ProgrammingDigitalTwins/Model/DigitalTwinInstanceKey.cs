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

using Newtonsoft.Json;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Model
{
    /// <summary>
    /// This class contains the properties and current known state of
    /// a digital twin model, along with its references and components.
    /// 
    /// Shared properties are derived from IotDataContext, which is also
    /// described in the base DTDML that all models extend.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DigitalTwinInstanceKey
    {
        private string name = ConfigConst.PRODUCT_NAME;
        private string modelKey  = ConfigConst.PRODUCT_NAME;
        private string modelGuid = System.Guid.NewGuid().ToString();
        private string instanceKey = ConfigConst.PRODUCT_NAME;

        /// <summary>
        /// 
        /// </summary>
        public DigitalTwinInstanceKey() : this(null, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="modelKey"></param>
        public DigitalTwinInstanceKey(string name, string modelKey)
        {
            if (! string.IsNullOrEmpty(name))
            {
                this.name = name;
            }

            if (! string.IsNullOrEmpty(modelKey))
            {
                this.modelKey = modelKey;
            }
            else
            {
                this.modelKey = ConfigConst.PRODUCT_NAME;
            }

            this.instanceKey = this.name + "_" + this.modelKey + "_" + this.modelGuid;
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetInstanceKey()
        {
            return this.instanceKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetModelKey()
        {
            return modelKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetModelGuid()
        {
            return this.modelGuid;
        }

        public string GetName()
        {
            return this.name;
        }

        public bool IsEqual(DigitalTwinInstanceKey key)
        {
            if (key != null)
            {
                return (key.ToString().Equals(this.ToString()));
            }

            return false;
        }

        public bool IsSourceEqual(DigitalTwinInstanceKey key)
        {
            if (key != null)
            {
                return (
                    key.GetName().Equals(this.GetName()) &&
                    key.GetModelKey().Equals(this.GetModelKey()));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetInstanceKey();
        }

    }
}
