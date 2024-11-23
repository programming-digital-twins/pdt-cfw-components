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
    public class DataTypeContext
    {
        [JsonProperty]
        private string typeName = string.Empty;

        [JsonProperty]
        private string displayName = string.Empty;

        [JsonProperty]
        private string description = string.Empty;

        [JsonProperty]
        private string dtmlIdRef = string.Empty;

        [JsonProperty]
        private string resourceName = string.Empty;

        [JsonProperty]
        private int id = 0;

        // necessary for JSON serialization / deserialization
        public DataTypeContext()
        {
        }

        // public methods

        public string GetTypeName()
        {
            return this.typeName;
        }

        public string GetDisplayName()
        {
            return this.displayName;
        }

        public string GetDescription()
        {
            return this.description;
        }

        public string GetDtmlIdReference()
        {
            return this.dtmlIdRef;
        }

        public int GetId()
        {
            return this.id;
        }

        public string GetResourceName()
        {
            return this.resourceName;
        }

        public virtual bool IsTypeCategory()
        {
            return false;
        }

        public void SetTypeName(string name)
        {
            this.typeName = name;
        }

        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        public void SetDescription(string description)
        {
            this.description = description;
        }

        public void SetDtmlIdReference(string idRef)
        {
            this.dtmlIdRef = idRef;
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public void SetResourceName(string resourceName)
        {
            this.resourceName = resourceName;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(",typeName=").Append(this.typeName);
            sb.Append(",displayName=").Append(this.displayName);
            sb.Append(",description=").Append(this.description);
            sb.Append(",dtmlIdRef=").Append(this.dtmlIdRef);
            sb.Append(",resourceName=").Append(this.resourceName);
            sb.Append(",id=").Append(this.id);

            return sb.ToString();
        }

    }
}
