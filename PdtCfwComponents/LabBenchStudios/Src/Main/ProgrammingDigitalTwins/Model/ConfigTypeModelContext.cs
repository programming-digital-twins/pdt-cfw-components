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

using LabBenchStudios.Pdt.Common;

namespace LabBenchStudios.Pdt.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ConfigTypeModelContext
    {
        [JsonProperty]
        private string typeName = string.Empty;

        [JsonProperty]
        private string displayName = string.Empty;

        [JsonProperty]
        private string description = string.Empty;

        [JsonProperty]
        private string modelName = string.Empty;

        [JsonProperty]
        private string resourceName = string.Empty;

        [JsonProperty]
        private int id = 0;

        [JsonProperty]
        private ConfigTypeModelConstraints modelConstraints = null;

        // necessary for JSON serialization / deserialization

        /// <summary>
        /// 
        /// </summary>
        public ConfigTypeModelContext() : base()
        {
            // create unused model constraints
            this.modelConstraints = new ConfigTypeModelConstraints();
            this.modelConstraints.SetEnableConstraints(false);
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetConfigTypeName()
        {
            return this.typeName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetConfigTypeDisplayName()
        {
            return this.displayName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetConfigTypeDescription()
        {
            return this.description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConfigTypeModelConstraints GetModelConstraints()
        {
            return this.modelConstraints;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetModelName()
        {
            return this.modelName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            return this.id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetResourceName()
        {
            return this.resourceName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool IsTypeCategory()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void SetConfigTypeName(string name)
        {
            this.typeName = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayName"></param>
        public void SetConfigTypeDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public void SetConfigTypeDescription(string description)
        {
            this.description = description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraints"></param>
        public void SetModelConstraints(ConfigTypeModelConstraints constraints)
        {
            if (constraints != null)
            {
                this.modelConstraints = constraints;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelName"></param>
        public void SetModelName(string modelName)
        {
            this.modelName = modelName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void SetId(int id)
        {
            this.id = id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        public void SetResourceName(string resourceName)
        {
            this.resourceName = resourceName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("name=").Append(this.typeName);
            sb.Append(",displayName=").Append(this.displayName);
            sb.Append(",description=").Append(this.description);
            sb.Append(",modelName=").Append(this.modelName);
            sb.Append(",resourceName=").Append(this.resourceName);
            sb.Append(",id=").Append(this.id);
            sb.Append(",modelConstraints=").Append(this.modelConstraints);

            return sb.ToString();
        }

    }

}
