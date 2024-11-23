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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LabBenchStudios.Pdt.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DataTypeCategoryInfo : DataTypeContext
    {
        [JsonProperty]
        private string version = string.Empty;

        [JsonProperty]
        private int minId = 0;

        [JsonProperty]
        private int maxId = 0;
        
        [JsonProperty]
        private Dictionary<string, DataTypeInfo> typeEntries = new Dictionary<string, DataTypeInfo>();

        // necessary for JSON serialization / deserialization
        public DataTypeCategoryInfo()
        {
        }


        // public methods

        public void AddTypeEntry(DataTypeInfo dataType)
        {
            if (dataType != null)
            {
                this.typeEntries.Add(dataType.GetName(), dataType);
            }
        }

        public int GetDataTypeMinId()
        {
            return this.minId;
        }

        public int GetDataTypeMaxId()
        {
            return this.maxId;
        }

        public int GetConfigTypeInfoCount()
        {
            return this.typeEntries.Count;
        }

        public DataTypeInfo GetConfigType(string typeName)
        {
            if (typeName != null && typeName.Length > 0)
            {
                if (this.typeEntries.ContainsKey(typeName))
                {
                    return this.typeEntries[typeName];
                }
            }

            return null;
        }

        public string GetVersion()
        {
            return this.version;
        }

        public override bool IsTypeCategory()
        {
            return true;
        }

        public void SetDataTypeMinId(int id)
        {
            this.minId = id;
        }

        public void SetDataTypeMaxId(int id)
        {
            this.maxId = id;
        }

        public void SetVersion(string version)
        {
            this.version = version;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(base.ToString());
            sb.Append(",version=").Append(this.version);
            sb.Append(",minId=").Append(this.minId);
            sb.Append(",maxId=").Append(this.maxId);

            foreach (string key in this.typeEntries.Keys)
            {
                    sb.Append('\n').Append(this.typeEntries[key]);
            }

            return sb.ToString();
        }

    }
}
