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
    public class MessageData : IotDataContext
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string msgData = ConfigConst.NOT_SET;

        // necessary for JSON serialization / deserialization
        public MessageData() : base() {  }

        public MessageData(string name, string deviceID) :
            base(name, deviceID, ConfigConst.MESSAGE_TYPE_CATEGORY, ConfigConst.MESSAGE_TYPE)
        {

        }

        // public methods

        public string GetMessageData() { return this.msgData; }

        public void SetMessageData(string msgData) { if (! string.IsNullOrEmpty(msgData)) this.msgData = msgData; base.UpdateTimeStamp(); }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(',');
            sb.Append(ConfigConst.MESSAGE_DATA_PROP).Append('=').Append(this.msgData);

            return sb.ToString();
        }

        public void UpdateData(MessageData data)
        {
            if (data != null)
            {
                base.UpdateData(data);

                this.msgData = data.GetMessageData();

                this.UpdateTimeStamp();
            }
        }
    }
}
