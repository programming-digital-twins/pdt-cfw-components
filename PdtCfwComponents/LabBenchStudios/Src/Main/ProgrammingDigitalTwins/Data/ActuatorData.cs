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
    public class ActuatorData : IotDataContext
    {
        [JsonProperty]
        private float value = 0.0f;

        [JsonProperty]
        private int command = 0;

        [JsonProperty]
        private string commandName = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string stateData = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private DataValueContainer dataValues = new DataValueContainer();

        [JsonProperty]
        private bool isResponse = false;

        // necessary for JSON serialization / deserialization
        public ActuatorData() : base() { }

        public ActuatorData(string name, string deviceID, int typeCategoryID, int typeID) :
            base(name, deviceID, typeCategoryID, typeID)
        {

        }

        // public methods

        public int GetCommand() { return this.command; }

        public string GetCommandName() { return this.commandName; }

        public DataValueContainer GetDataValues() { return this.dataValues; }

        public float GetValue()
        {
            return this.dataValues.GetValue();
        }

        public string GetStateData() { return this.stateData; }

        public bool IsResponse() { return this.isResponse; }

        public void SetCommand(int val) { if (val >= 0) this.command = val; base.UpdateTimeStamp(); }

        public void SetCommandName(string name) { if (name != null) this.commandName = name; base.UpdateTimeStamp(); }

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

        public void SetValue(float val)
        {
            // ensure backwards compatibility
            this.value = val;

            this.UpdateValues();
            base.UpdateTimeStamp();
        }

        public void SetResponse(bool isResponse) { this.isResponse = isResponse; base.UpdateTimeStamp(); }

        public void SetStateData(string data) { if (!string.IsNullOrEmpty(data)) this.stateData = data; base.UpdateTimeStamp(); }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(',');
            sb.Append(ConfigConst.COMMAND_PROP).Append('=').Append(this.command).Append(',');
            sb.Append(ConfigConst.COMMAND_NAME_PROP).Append('=').Append(this.commandName).Append(',');
            sb.Append(ConfigConst.STATE_DATA_PROP).Append('=').Append(this.stateData).Append(',');
            sb.Append(ConfigConst.IS_RESPONSE_PROP).Append('=').Append(this.isResponse).Append(',');
            sb.Append(this.dataValues.ToString());

            return sb.ToString();
        }

        public void UpdateData(ActuatorData data)
        {
            if (data != null)
            {
                base.UpdateData(data);

                this.command = data.GetCommand();
                this.commandName = data.GetCommandName();
                this.stateData = data.GetStateData();
                this.isResponse = data.IsResponse();

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
