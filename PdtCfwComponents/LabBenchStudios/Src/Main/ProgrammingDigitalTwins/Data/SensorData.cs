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
using System.Text;

namespace LabBenchStudios.Pdt.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SensorData : IotDataContext
    {
        [JsonProperty]
        private float value = 0.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private DataValueContainer dataValues = new DataValueContainer();

        // necessary for JSON serialization / deserialization
        public SensorData() : base()
        {
        }

        public SensorData(string name, string deviceID, int typeCategoryID, int typeID) :
            base(name, deviceID, typeCategoryID, typeID)
        {
        }

        // public methods

        public float GetValue()
        {
            return this.dataValues.GetValue();
        }

        public DataValueContainer GetDataValues() { return this.dataValues; }

        public void SetValue(float val)
        {
            // ensure backwards compatibility
            this.value = val;

            this.UpdateValues();
            base.UpdateTimeStamp();
        }

        public void SetDataValues(DataValueContainer data)
        {
            if (data != null)
            {
                // ensure backwards compatibility
                this.value = data.GetValue();

                this.dataValues.UpdateData(data);
                base.UpdateTimeStamp();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(',');
            sb.Append(this.dataValues.ToString());

            return sb.ToString();
        }

        public void UpdateData(SensorData data)
        {
            if (data != null)
            {
                base.UpdateData(data);

                this.SetDataValues(data.GetDataValues());
                this.UpdateTimeStamp();
            }
        }

        public void UpdateValues()
        {
            this.dataValues.SetValue(this.value);
        }
    }
}
