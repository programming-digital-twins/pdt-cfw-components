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

using System.Collections.Generic;
using System.Text;

namespace LabBenchStudios.Pdt.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IotDataContextWithValues : IotDataContext
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<string, DataValueContainer> dataValuesMap = new Dictionary<string, DataValueContainer>();

        // necessary for JSON serialization / deserialization
        public IotDataContextWithValues() : base()
        {
        }

        public IotDataContextWithValues(string name, string deviceID, int typeCategoryID, int typeID) :
            base(name, deviceID, typeCategoryID, typeID)
        {
        }

        // public methods

        public void AddDataValues(string name, DataValueContainer data)
        {
            if (!string.IsNullOrEmpty(name) && data != null)
            {
                if (this.dataValuesMap.ContainsKey(name))
                {
                    this.dataValuesMap[name] = data;
                }
                else
                {
                    this.dataValuesMap.Add(name, data);
                }
            }
        }

        public float GetValue()
        {
            DataValueContainer dataValueContainer = this.GetDataValues(base.GetName());

            if (dataValueContainer != null)
            {
                return dataValueContainer.GetValue();
            }
            else
            {
                // should never occur
                return float.MinValue;
            }
        }

        public DataValueContainer GetDataValues()
        {
            return this.GetDataValues(base.GetName());
        }

        public DataValueContainer GetDataValues(string name)
        {
            if (! string.IsNullOrEmpty(name))
            {
                if (this.dataValuesMap.ContainsKey(name))
                {
                    return this.dataValuesMap[name];
                }
            }

            // may occur
            return null;
        }

        public Dictionary<string, DataValueContainer> GetAllDataValues()
        {
            return this.dataValuesMap;
        }

        public void SetValue(float val)
        {
            DataValueContainer dataValueContainer = this.GetDataValues();

            if (dataValueContainer == null)
            {
                dataValueContainer = new DataValueContainer();

                this.AddDataValues(base.GetName(), dataValueContainer);
            }

            dataValueContainer.SetValue(val);

            base.UpdateTimeStamp();
        }

        public void SetDataValues(DataValueContainer data)
        {
            if (data != null)
            {
                DataValueContainer dataValueContainer = this.GetDataValues();

                if (dataValueContainer == null)
                {
                    this.AddDataValues(base.GetName(), dataValueContainer);
                }

                dataValueContainer.UpdateData(data);

                base.UpdateTimeStamp();
            }
        }

        public void SetDataValues(string name, DataValueContainer dataValueContainer)
        {
            if (!string.IsNullOrEmpty(name) && dataValueContainer != null)
            {
                DataValueContainer curDataValueContainer = this.GetDataValues(name);

                if (curDataValueContainer != null)
                {
                    curDataValueContainer.UpdateData(dataValueContainer);
                }
                else
                {
                    this.AddDataValues(name, dataValueContainer);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            Dictionary<string, DataValueContainer> dataValueMap = this.GetAllDataValues();

            if (dataValueMap != null && dataValueMap.Count > 0)
            {
                sb.Append(',');

                foreach (string key in dataValueMap.Keys)
                {
                    sb.Append(ConfigConst.NAME_PROP).Append('=').Append(key).Append(',');

                    DataValueContainer dataValueContainer = dataValueMap[key];

                    if (dataValueContainer != null)
                    {
                        sb.Append(dataValueContainer.ToString()).Append(',');
                    }
                }
            }

            return sb.ToString();
        }

        public void UpdateData(IotDataContextWithValues data)
        {
            if (data != null)
            {
                base.UpdateData(data);

                Dictionary<string, DataValueContainer> dataValueMap = data.GetAllDataValues();

                foreach (string key in dataValueMap.Keys)
                {
                    DataValueContainer dataValueContainer = dataValueMap[key];

                    this.SetDataValues(key, dataValueContainer);
                }

                this.UpdateTimeStamp();
            }
        }


        // protected methods

        protected override void OnNameUpdate(string prevName)
        {
            // if there exists a previous DataValueContainer stored by prevName,
            // change the data set key from the old name to the new - else, ignore
            DataValueContainer prevDataValueContainer = this.GetDataValues(prevName);

            if (prevDataValueContainer != null)
            {
                string curName = base.GetName();
                
                DataValueContainer curDataValueContainer = this.GetDataValues(curName);

                if (curDataValueContainer != null)
                {
                    curDataValueContainer.UpdateData(prevDataValueContainer);
                    this.dataValuesMap.Remove(curName);
                }

                this.AddDataValues(curName, prevDataValueContainer);
            }
        }
    }
}
