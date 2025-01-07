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
using System.Text;

using Newtonsoft.Json;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ConfigTypeModelEntry : ConfigTypeModelContext
    {
        // keep a reference to the containing 'container' for this entry
        private ConfigTypeModelContainer configTypeContainer = null;

        [JsonProperty]
        private string dataContainerType = string.Empty;

        // necessary for JSON serialization / deserialization

        /// <summary>
        /// 
        /// </summary>
        public ConfigTypeModelEntry() : base()
        {
            // nothing to do
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IotDataContext CreateDataContainer()
        {
            if (this.dataContainerType != null && this.dataContainerType != string.Empty)
            {
                try
                {
                    Type t = Type.GetType(this.dataContainerType);
                    IotDataContext dataContext = (IotDataContext)Activator.CreateInstance(t);

                    // TODO: fill in details
                    if (this.configTypeContainer != null)
                    {
                        dataContext.SetTypeCategoryID(this.configTypeContainer.GetId());
                    }

                    dataContext.SetTypeName(this.GetConfigTypeName());
                    dataContext.SetTypeID(this.GetId());

                    return dataContext;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to create data container from data container type name: {this.dataContainerType}");
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDataContainerTypeName()
        {
            return this.dataContainerType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetTypeCategoryId()
        {
            if (this.configTypeContainer != null) {
                return this.configTypeContainer.GetId();
            } else {
                return ConfigConst.DEFAULT_TYPE_CATEGORY_ID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerRef"></param>
        public void SetConfigTypeContainerRef(ConfigTypeModelContainer containerRef)
        {
            this.configTypeContainer = containerRef;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        public void SetDataContainerTypeName(string typeName)
        {
            if (typeName != null) {
                this.dataContainerType = typeName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(",dataContainerType=").Append(this.dataContainerType);

            return sb.ToString();
        }

    }
}
