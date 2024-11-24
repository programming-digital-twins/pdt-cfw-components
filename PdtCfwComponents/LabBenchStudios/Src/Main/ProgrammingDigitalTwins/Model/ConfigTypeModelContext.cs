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

using System;
using System.Deployment.Internal;
using System.Text;

using Newtonsoft.Json;

using LabBenchStudios.Pdt.Common;

namespace LabBenchStudios.Pdt.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ConfigTypeModelContext
    {
        [JsonProperty]
        private string name = string.Empty;

        [JsonProperty]
        private string displayName = string.Empty;

        [JsonProperty]
        private string description = string.Empty;

        [JsonProperty]
        private string modelName = string.Empty;

        [JsonProperty]
        private int id = 0;

        // necessary for JSON serialization / deserialization
        public ConfigTypeModelContext() : base()
        {
            // nothing to do
        }

        // public methods

        public string GetConfigTypeName()
        {
            return this.name;
        }

        public string GetConfigTypeDisplayName()
        {
            return this.displayName;
        }

        public string GetConfigTypeDescription()
        {
            return this.description;
        }

        public string GetModelName()
        {
            return this.modelName;
        }

        public int GetId()
        {
            return this.id;
        }

        public virtual bool IsTypeCategory()
        {
            return false;
        }

        public void SetConfigTypeName(string name)
        {
            this.name = name;
        }

        public void SetConfigTypeDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        public void SetConfigTypeDescription(string description)
        {
            this.description = description;
        }

        public void SetModelName(string modelName)
        {
            this.modelName = modelName;
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(",name=").Append(name);
            sb.Append(",displayName=").Append(displayName);
            sb.Append(",description=").Append(description);
            sb.Append(",modelName=").Append(modelName);
            sb.Append(",id=").Append(id);

            return sb.ToString();
        }

    }
}
