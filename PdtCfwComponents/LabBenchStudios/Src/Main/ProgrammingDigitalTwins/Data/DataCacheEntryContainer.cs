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
    public class DataCacheEntryContainer
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private DateTime timeStamp = DateTime.UtcNow;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private ActuatorData actuatorData = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private ConnectionStateData connStateData = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private MessageData messageData = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private SensorData sensorData = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private SystemPerformanceData sysPerfData = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private bool enableByteCounting = true;

        /// <summary>
        /// 
        /// </summary>
        public DataCacheEntryContainer() : base()
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
                if (this.HasActuatorData())
                {
                    string jsonData = DataUtil.ActuatorDataToJson(this.actuatorData);

                    byteCount += jsonData.Length;
                }

                if (this.HasConnectionStateData())
                {
                    string jsonData = DataUtil.ConnectionStateDataToJson(this.connStateData);

                    byteCount += jsonData.Length;
                }

                if (this.HasMessageData())
                {
                    string jsonData = DataUtil.MessageDataToJson(this.messageData);

                    byteCount += jsonData.Length;
                }

                if (this.HasSensorData())
                {
                    string jsonData = DataUtil.SensorDataToJson(this.sensorData);

                    byteCount += jsonData.Length;
                }

                if (this.HasSystemPerformanceData())
                {
                    string jsonData = DataUtil.SystemPerformanceDataToJson(this.sysPerfData);

                    byteCount += jsonData.Length;
                }

                byteCount += this.GetTimeStamp().ToString().Length;
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
        public ActuatorData GetActuatorData()
        {
            return this.actuatorData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConnectionStateData GetConnectionStateData()
        {
            return this.connStateData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MessageData GetMessageData()
        {
            return this.messageData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SensorData GetSensorData()
        {
            return this.sensorData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SystemPerformanceData GetSystemPerformanceData()
        {
            return this.sysPerfData;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasActuatorData()
        {
            return this.actuatorData != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasConnectionStateData()
        {
            return this.connStateData != null;
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
        public double GetElapsedEpochMillisDelta(DataCacheEntryContainer cacheEntry)
        {
            if (cacheEntry != null)
            {
                double current = this.GetElapsedEpochMillis();
                double comparator = cacheEntry.GetElapsedEpochMillis();

                return (Math.Abs(current - comparator));
            }

            return 0d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasMessageData()
        {
            return this.messageData != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasSensorData()
        {
            return this.sensorData != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasSystemPerformanceData()
        {
            return this.sysPerfData != null;
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
        public void SetActuatorData(ActuatorData data)
        {
            if (data != null)
            {
                this.actuatorData = data;

                this.SetTimeStamp(this.GenerateUtcTime(data.GetTimeStamp()));

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetConnectionStateData(ConnectionStateData data)
        {
            if (data != null)
            {
                this.connStateData = data;

                this.SetTimeStamp(this.GenerateUtcTime(data.GetTimeStamp()));

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetMessageData(MessageData data)
        {
            if (data != null)
            {
                this.messageData = data;

                this.SetTimeStamp(this.GenerateUtcTime(data.GetTimeStamp()));

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetSensorData(SensorData data)
        {
            if (data != null)
            {
                this.sensorData = data;

                this.SetTimeStamp(this.GenerateUtcTime(data.GetTimeStamp()));

            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetSystemPerformanceData(SystemPerformanceData data)
        {
            if (data != null)
            {
                this.sysPerfData = data;

                this.SetTimeStamp(this.GenerateUtcTime(data.GetTimeStamp()));

            }
        }


        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        private DateTime GenerateUtcTime(string timeStamp)
        {
            return DateTime.Parse(timeStamp).ToUniversalTime();
        }

    }

}
