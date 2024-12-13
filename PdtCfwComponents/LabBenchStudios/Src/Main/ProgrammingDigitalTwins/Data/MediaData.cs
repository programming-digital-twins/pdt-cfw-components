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
    public class MediaData : IotDataContext
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string payloadUri = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private byte[] payload = new byte[] { };

        // necessary for JSON serialization / deserialization
        public MediaData() : base() { }

        public MediaData(string name, string deviceID) :
            base(name, deviceID, ConfigConst.MEDIA_TYPE_CATEGORY, ConfigConst.DEFAULT_MEDIA_TYPE)
        {

        }

        // public methods

        public byte[] GetPayload() { return this.payload; }

        public string GetPayloadUri() { return this.payloadUri; }

        public void SetPayload(byte[] payload)
        {
            if (payload != null)
            {
                this.payload = payload;
                base.UpdateTimeStamp();
            }
        }

        public void SetPayloadUri(string payloadUri)
        {
            if (!string.IsNullOrWhiteSpace(payloadUri))
            {
                this.payloadUri = payloadUri;
                base.UpdateTimeStamp();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(',');
            sb.Append("PayloadURI").Append('=').Append(this.payloadUri);
            sb.Append("PayloadLen").Append('=').Append(this.payload.Length);

            return sb.ToString();
        }

        public void UpdateData(MediaData data)
        {
            if (data != null)
            {
                base.UpdateData(data);

                this.payload = data.GetPayload();

                this.UpdateTimeStamp();
            }
        }
    }
}
