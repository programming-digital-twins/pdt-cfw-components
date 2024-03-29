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
    public class ConnectionStateData : IotDataContext
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string hostName = ConfigConst.DEFAULT_HOST;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int hostPort = ConfigConst.DEFAULT_MQTT_PORT;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string clientID = ConfigConst.NOT_SET;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string resourcePrefix = ConfigConst.NOT_SET;

        [JsonProperty]
        private string message = ConfigConst.NOT_SET;

        [JsonProperty]
        private int statusCode = ConfigConst.DEFAULT_STATUS;

        [JsonProperty]
        private int msgInCount = 0;

        [JsonProperty]
        private int msgOutCount = 0;

        [JsonProperty]
        private bool isInternalMsg = false;

        [JsonProperty]
        private bool isClientConnecting = false;

        [JsonProperty]
        private bool isClientConnected = false;

        [JsonProperty]
        private bool isClientDisconnected = false;

        private bool enableCountingForInternalMsgs = false;

        // necessary for JSON serialization / deserialization
        public ConnectionStateData() : base() { }

        public ConnectionStateData(string hostName, int hostPort) :
            this(ConfigConst.PRODUCT_NAME, ConfigConst.NOT_SET, hostName, hostPort)
        {
        }

        public ConnectionStateData(string name, string deviceID, string hostName, int hostPort) :
            base(name, deviceID, ConfigConst.SYSTEM_TYPE_CATEGORY, ConfigConst.SYSTEM_CONN_STATE_TYPE)
        {
            this.SetHostName(hostName);
            this.SetHostPort(hostPort);
        }

        // public methods

        public string GetClientID() { return this.clientID; }

        public string GetHostName() { return this.hostName; }

        public int GetHostPort() { return this.hostPort; }

        public string GetMessage() { return this.message; }

        public int GetMessageInCount() { return this.msgInCount; }

        public int GetMessageOutCount() { return this.msgOutCount; }

        public string GetResourcePrefix() { return this.resourcePrefix; }

        public bool HasSameConnectionDetails(ConnectionStateData connStateData)
        {
            if (connStateData != null)
            {
                if (this.hostName.Equals(connStateData.GetHostName()) &&
                    this.hostPort == connStateData.GetHostPort() &&
                    this.clientID.Equals(connStateData.GetClientID()) &&
                    this.resourcePrefix.Equals(connStateData.GetResourcePrefix()))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsClientConnecting() { return this.isClientConnecting; }

        public bool IsClientConnected() { return this.isClientConnected; }

        public bool IsClientDisconnected() { return this.isClientDisconnected; }

        public bool IsInternalMessage() { return this.isInternalMsg; }

        public void IncreaseMessageInCount()
        {
            this.IncreaseMessageInCount(1);
            base.UpdateTimeStamp();
        }

        public void IncreaseMessageInCount(int val)
        {
            if (!this.enableCountingForInternalMsgs && this.isInternalMsg) return;

            if (val >= 0) this.msgInCount += val;

            base.UpdateTimeStamp();
        }

        public void IncreaseMessageOutCount()
        {
            this.IncreaseMessageOutCount(1); base.UpdateTimeStamp();
        }

        public void IncreaseMessageOutCount(int val)
        {
            if (!this.enableCountingForInternalMsgs && this.isInternalMsg) return;

            if (val >= 0) this.msgOutCount += val;

            base.UpdateTimeStamp();
        }

        public void SetClientID(string clientID)
        {
            if (!string.IsNullOrEmpty(clientID))
            {
                this.clientID = clientID;

                base.UpdateTimeStamp();
            }
        }

        public void SetHostName(string hostName)
        {
            if (!string.IsNullOrEmpty(hostName))
            {
                this.hostName = hostName;

                base.UpdateTimeStamp();
            }
        }

        public void SetHostPort(int hostPort)
        {
            if (hostPort > 0 && hostPort <= 65535)
            {
                this.hostPort = hostPort;

                base.UpdateTimeStamp();
            }
        }

        public void SetIsClientConnectingFlag(bool enable) { this.isClientConnecting = enable; base.UpdateTimeStamp(); }

        public void SetIsClientConnectedFlag(bool enable) { this.isClientConnected = enable; base.UpdateTimeStamp(); }

        public void SetIsClientDisconnectedFlag(bool enable) { this.isClientDisconnected = enable; base.UpdateTimeStamp(); }

        public void SetIsInternalMessage(bool enable) { this.isInternalMsg = enable; base.UpdateTimeStamp(); }

        public void SetMessage(string message) { if (!string.IsNullOrEmpty(message)) this.message = message; base.UpdateTimeStamp(); }

        public void SetResourcePrefix(string resourcePrefix)
        {
            if (!string.IsNullOrEmpty(resourcePrefix))
            {
                this.resourcePrefix = resourcePrefix;

                base.UpdateTimeStamp();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(',');
            sb.Append(ConfigConst.VALUE_PROP).Append('=').Append(this.hostName).Append(',');
            sb.Append(ConfigConst.HOST_NAME_PROP).Append('=').Append(this.hostPort).Append(',');
            sb.Append(ConfigConst.HOST_PORT_PROP).Append('=').Append(this.hostPort).Append(',');
            sb.Append(ConfigConst.MESSAGE_DATA_PROP).Append('=').Append(this.message).Append(',');
            sb.Append(ConfigConst.MESSAGE_IN_COUNT_PROP).Append('=').Append(this.msgInCount).Append(',');
            sb.Append(ConfigConst.MESSAGE_OUT_COUNT_PROP).Append('=').Append(this.msgOutCount).Append(',');
            sb.Append(ConfigConst.IS_CONNECTING_PROP).Append('=').Append(this.isClientConnecting).Append(',');
            sb.Append(ConfigConst.IS_CONNECTED_PROP).Append('=').Append(this.isClientConnected).Append(',');
            sb.Append(ConfigConst.IS_DISCONNECTED_PROP).Append('=').Append(this.isClientDisconnected);
            sb.Append(ConfigConst.IS_INTERNAL_MSG_PROP).Append('=').Append(this.isInternalMsg);

            return sb.ToString();
        }

        public void UpdateData(ConnectionStateData data)
        {
            if (data != null)
            {
                base.UpdateData(data);

                this.clientID = data.GetClientID();
                this.hostName = data.GetHostName();
                this.hostPort = data.GetHostPort();
                this.message = data.GetMessage();
                this.msgInCount = data.GetMessageInCount();
                this.msgOutCount = data.GetMessageOutCount();
                this.resourcePrefix = data.GetResourcePrefix();
                this.isClientDisconnected = data.IsClientDisconnected();
                this.isClientConnected = data.IsClientConnected();
                this.isClientConnecting = data.IsClientConnecting();
                this.isInternalMsg = data.IsInternalMessage();
            }
        }
    }
}
