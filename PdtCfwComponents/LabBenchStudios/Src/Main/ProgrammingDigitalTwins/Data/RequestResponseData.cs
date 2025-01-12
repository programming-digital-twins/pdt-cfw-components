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

using Newtonsoft.Json;

///
///
///
///
namespace LabBenchStudios.Pdt.Data
{
    /// <summary>
    /// All time represented in UTC.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class RequestResponseData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private DateTime timeStamp = DateTime.UtcNow;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string sessionID = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string targetUri = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string modelName = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string requestMsg = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string responseMsg = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private bool enableByteCounting = true;

        /// <summary>
        /// 
        /// </summary>
        public RequestResponseData() : base()
        {
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetApproxByteCount()
        {
            int byteCount = 0;

            if (this.enableByteCounting)
            {
                byteCount += this.timeStamp.ToString().Length;
                byteCount += (this.HasSessionID() ? this.sessionID.Length : 0);
                byteCount += (this.HasModelName() ? this.modelName.Length : 0);
                byteCount += (this.HasTargetUri() ? this.targetUri.Length : 0);
                byteCount += (this.HasRequestMsg() ? this.requestMsg.Length : 0);
                byteCount += (this.HasResponseMsg() ? this.responseMsg.Length : 0);
            }

            return byteCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime GetTimeStamp()
        {
            return this.timeStamp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSessionID()
        {
            return this.sessionID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetModelName()
        {
            return this.modelName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetTargetUri()
        {
            return this.targetUri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetRequestMsg()
        {
            return this.requestMsg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetResponseMsg()
        {
            return this.responseMsg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasModelName()
        {
            return !string.IsNullOrEmpty(this.modelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasTargetUri()
        {
            return !string.IsNullOrEmpty(this.targetUri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasSessionID()
        {
            return !string.IsNullOrEmpty(this.sessionID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasRequestMsg()
        {
            return !string.IsNullOrEmpty(this.requestMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasResponseMsg()
        {
            return !string.IsNullOrEmpty(this.responseMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetElapsedEpochMillis()
        {
            return TimeSpan.FromTicks(this.timeStamp.Ticks).TotalMilliseconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetElapsedEpochMillisDelta(RequestResponseData data)
        {
            if (data != null)
            {
                double current = this.GetElapsedEpochMillis();
                double comparator = data.GetElapsedEpochMillis();

                return (Math.Abs(current - comparator));
            }

            return 0d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStamp"></param>
        public void SetTimeStamp(DateTime timeStamp)
        {
            this.timeStamp = timeStamp.ToUniversalTime();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetSessionID(string data)
        {
            if (data != null)
            {
                this.sessionID = data;

                this.SetTimeStamp(DateTime.UtcNow);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetModelName(string data)
        {
            if (data != null)
            {
                this.modelName = data;

                this.SetTimeStamp(DateTime.UtcNow);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetTargetUri(string data)
        {
            if (data != null)
            {
                this.targetUri = data;

                this.SetTimeStamp(DateTime.UtcNow);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetRequestMsg(string data)
        {
            if (data != null)
            {
                this.requestMsg = data;

                this.SetTimeStamp(DateTime.UtcNow);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetResponseMsg(string data)
        {
            if (data != null)
            {
                this.responseMsg = data;

                this.SetTimeStamp(DateTime.UtcNow);

            }
        }

    }

}
