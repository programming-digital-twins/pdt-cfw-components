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
using System.Text;

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
    public class DigitalTwinModelSyncKey
    {
        private string name = ConfigConst.PRODUCT_NAME;
        private string modelID = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;

        private string modelSyncKey = ConfigConst.PRODUCT_NAME;
        private string modelSyncGuidKey = ConfigConst.PRODUCT_NAME;

        /// <summary>
        /// 
        /// </summary>
        public DigitalTwinModelSyncKey() :
            this(ConfigConst.PRODUCT_NAME, ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelID"></param>
        public DigitalTwinModelSyncKey(string modelID)
        {
            this.GenerateKey(ConfigConst.PRODUCT_NAME, modelID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="modelID"></param>
        public DigitalTwinModelSyncKey(string name, string modelID)
        {
            this.GenerateKey(name, modelID);
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetModelID()
        {
            return modelID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return this.name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSyncKey()
        {
            return this.modelSyncKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSyncKeyWithGuid()
        {
            return this.modelSyncGuidKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsEqual(DigitalTwinModelSyncKey key)
        {
            if (key != null)
            {
                return (key.ToString().Equals(this.ToString()));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsSourceEqual(DigitalTwinModelSyncKey key)
        {
            if (key != null)
            {
                return (
                    key.GetName().Equals(this.GetName()) &&
                    key.GetModelID().Equals(this.GetModelID()));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetSyncKeyWithGuid();
        }

        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="modelID"></param>
        private void GenerateKey(string name, string modelID)
        {
            this.modelSyncKey = ModelNameUtil.GenerateModelSyncKey(name, modelID, false);
            this.modelSyncGuidKey = ModelNameUtil.GenerateModelSyncKey(name, modelID, true);
        }
    }
}
