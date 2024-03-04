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
using System.Collections;
using System.Collections.Generic;
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
        private string deviceID  = ConfigConst.PRODUCT_NAME;
        private string locationID = ConfigConst.PRODUCT_NAME;
        private string guid = ConfigConst.PRODUCT_NAME;
        private string telemetryKey = ConfigConst.NOT_SET;

        /// <summary>
        /// 
        /// </summary>
        public DigitalTwinTelemetryKey() : this(null, null, true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="useGuid"></param>
        public DigitalTwinTelemetryKey(string deviceID, string locationID, bool useGuid)
        {
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

            sb.Append(this.deviceID).Append(':').Append(this.locationID).Append(':').Append(this.guid);

            this.telemetryKey = sb.ToString();
        }


        // public methods

        public string GetTelemetryKey()
        {
            return this.telemetryKey;
        }

        public string GetDeviceID()
        {
            return this.deviceID;
        }

        public string GetLocationID()
        {
            return this.locationID;
        }

        public string GetGuid()
        {
            return this.guid;
        }

        public override string ToString()
        {
            return this.GetTelemetryKey();
        }

    }
}
