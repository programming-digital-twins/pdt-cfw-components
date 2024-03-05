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

using System.Text;

using Newtonsoft.Json;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Model
{
    /// <summary>
    /// This class contains the properties and current known state of
    /// a digital twin model, along with its references and components.
    /// 
    /// Shared properties are derived from IotDataContext, which is also
    /// described in the base DTDML that all models extend.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DigitalTwinTelemetryKey
    {
        private string name = ConfigConst.PRODUCT_NAME;
        private string deviceID  = ConfigConst.PRODUCT_NAME;
        private string locationID = ConfigConst.PRODUCT_NAME;
        private string guid = ConfigConst.PRODUCT_NAME;
        private string telemetryKey = ConfigConst.NOT_SET;

        /// <summary>
        /// 
        /// </summary>
        public DigitalTwinTelemetryKey()
        {
            this.GenerateKey(null, null, null, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="useGuid"></param>
        public DigitalTwinTelemetryKey(IotDataContext dataContext, bool useGuid)
        {
            if (dataContext != null)
            {
                this.GenerateKey(
                    dataContext.GetName(),
                    dataContext.GetDeviceID(),
                    dataContext.GetLocationID(),
                    useGuid);
            }
            else
            {
                this.GenerateKey(null, null, null, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="useGuid"></param>
        public DigitalTwinTelemetryKey(
            string name, string deviceID, string locationID, bool useGuid)
        {
            this.GenerateKey(name, deviceID, locationID, useGuid);
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetTelemetryKey()
        {
            return this.telemetryKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDeviceID()
        {
            return this.deviceID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetLocationID()
        {
            return this.locationID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetGuid()
        {
            return this.guid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return this.name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public bool IsEqual(string keyName)
        {
            if (! string.IsNullOrEmpty(keyName))
            {
                return (keyName.Equals(this.ToString()));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public bool IsEqual(IotDataContext dataContext)
        {
            if (dataContext != null)
            {
                return (this.IsEqual(ModelNameUtil.GenerateTelemetrySyncKey(dataContext)));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsEqual(DigitalTwinTelemetryKey key)
        {
            if (key != null)
            {
                return (key.ToString().Equals(this.ToString()));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsSourceEqual(DigitalTwinTelemetryKey key)
        {
            if (key != null)
            {
                return (
                    key.GetName().Equals(this.GetName()) &&
                    key.GetDeviceID().Equals(this.GetDeviceID()) &&
                    key.GetLocationID().Equals(this.GetLocationID()));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetTelemetryKey();
        }


        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="useGuid"></param>
        private void GenerateKey(
            string name, string deviceID, string locationID, bool useGuid)
        {
            if (! string.IsNullOrEmpty(name))
            {
                this.name = name;
            }

            if (! string.IsNullOrEmpty(deviceID))
            {
                this.deviceID = deviceID;
            }

            if (! string.IsNullOrEmpty(locationID))
            {
                this.locationID = locationID;
            }

            if (useGuid)
            {
                this.guid = System.Guid.NewGuid().ToString();
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(this.name).Append(':')
                .Append(this.deviceID).Append(':')
                .Append(this.locationID).Append(':')
                .Append(this.guid);

            this.telemetryKey = sb.ToString();
        }

    }
}
