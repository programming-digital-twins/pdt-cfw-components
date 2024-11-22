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
using System.Collections.Generic;
using System.Text;

namespace LabBenchStudios.Pdt.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DataTypeConfigCache
    {
        [JsonProperty]
        private Dictionary<string, DataTypeCategoryInfo> configCategoryTable = new Dictionary<string, DataTypeCategoryInfo>();


        // necessary for JSON serialization / deserialization
        public DataTypeConfigCache()
        {
        }


        // public methods

        public void AddDataTypeCategoryInfo(DataTypeCategoryInfo categoryInfo)
        {
            if (categoryInfo != null)
            {
                this.configCategoryTable.Add(categoryInfo.GetDataTypeName(), categoryInfo);
            }
        }

        public int GetConfigTypeCategoryInfoCount()
        {
            return this.configCategoryTable.Count;
        }

        public DataTypeCategoryInfo GetConfigTypeCategory(string categoryName)
        {
            if (categoryName != null && categoryName.Length > 0)
            {
                if (this.configCategoryTable.ContainsKey(categoryName))
                {
                    return this.configCategoryTable[categoryName];
                }
            }

            return null;
        }

        public DataTypeInfo GetConfigType(string categoryName, string typeName)
        {
            DataTypeCategoryInfo configCategory = this.GetConfigTypeCategory(categoryName);

            if (configCategory != null &&
                (typeName != null && typeName.Length > 0))
            {
                if (this.configCategoryTable.ContainsKey(categoryName))
                {
                    DataTypeCategoryInfo configTypeCategory = this.configCategoryTable[categoryName];

                    return configTypeCategory.GetConfigType(typeName);
                }
            }

            return null;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string key in this.configCategoryTable.Keys)
            {
                DataTypeCategoryInfo categoryInfo = this.configCategoryTable[key];

                sb.Append(categoryInfo).Append('\n');
            }

            return sb.ToString();
        }


        // protected methods

    }
}
