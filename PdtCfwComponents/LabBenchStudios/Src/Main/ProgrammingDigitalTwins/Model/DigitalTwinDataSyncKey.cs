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
    public class DigitalTwinDataSyncKey
    {
        private string name = ConfigConst.PRODUCT_NAME;
        private string groupID = ConfigConst.PRODUCT_NAME;
        private string deviceID  = ConfigConst.PRODUCT_NAME;
        private string locationID = ConfigConst.PRODUCT_NAME;

        private string modelID = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;

        private string dataSyncKey = ConfigConst.PRODUCT_NAME;
        private string dataSyncGuidKey = ConfigConst.PRODUCT_NAME;

        /// <summary>
        /// 
        /// </summary>
        public DigitalTwinDataSyncKey()
        {
            this.GenerateKey(null, null, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        public DigitalTwinDataSyncKey(IotDataContext dataContext)
        {
            if (dataContext != null)
            {
                this.GenerateKey(
                    dataContext.GetName(),
                    null,
                    dataContext.GetDeviceID(),
                    dataContext.GetLocationID());
            }
            else
            {
                this.GenerateKey(null, null, null, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        public DigitalTwinDataSyncKey(
            string name, string deviceID, string locationID)
        {
            this.GenerateKey(name, null, deviceID, locationID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="groupID"></param>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        public DigitalTwinDataSyncKey(
            string name, string groupID, string deviceID, string locationID)
        {
            this.GenerateKey(name, groupID, deviceID, locationID);
        }


        // public methods

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
        public string GetGroupID()
        {
            return this.groupID;
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
        public string GetName()
        {
            return this.name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetModelID()
        {
            return this.modelID;
        }

        /// <summary>
        /// Checks if the key name passed in is the same as the
        /// internally stored data sync key string.
        /// If so, returns true.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public bool IsEqual(string keyName)
        {
            return IsEqual(keyName, false);
        }

        /// <summary>
        /// Checks if the key name passed in is the same as the
        /// internally stored data sync key string.
        /// If useGuid is true, the GUID version of the data sync
        /// key is used internally for the compare, otherwise,
        /// the non-GUID version of the data sync is used.
        /// If so, returns true.
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="useGuid"></param>
        /// <returns></returns>
        public bool IsEqual(string keyName, bool useGuid)
        {
            if (! string.IsNullOrEmpty(keyName))
            {
                if (useGuid)
                {
                    return (keyName.Equals(this.dataSyncGuidKey));
                }
                else
                {
                    return (keyName.Equals(this.dataSyncKey));
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the data sync key generated from the passed in IotDataContext
        /// is the same as that stored internally WITHOUT use of a GUID.
        /// If so, returns true.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsEqual(IotDataContext data)
        {
            if (data != null)
            {
                return (this.dataSyncKey.Equals(ModelNameUtil.GenerateDataSyncKey(data)));
            }

            return false;
        }

        /// <summary>
        /// Checks if the string representations of this and 'key' are the same.
        /// If so, returns true.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsEqual(DigitalTwinDataSyncKey key)
        {
            if (key != null)
            {
                return (key.ToString().Equals(this.ToString()));
            }

            return false;
        }

        /// <summary>
        /// Checks if the device ID and location ID are the same.
        /// If so, returns true.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsDataSourceEqual(DigitalTwinDataSyncKey key)
        {
            if (key != null)
            {
                return (
                    key.GetDeviceID().Equals(this.GetDeviceID()) &&
                    key.GetLocationID().Equals(this.GetLocationID()));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelID"></param>
        public void SetModelID(string modelID)
        {
            if (! string.IsNullOrEmpty(modelID))
            {
                this.modelID = modelID;
            }
        }

        /// <summary>
        /// Returns the non-GUID representation of the data sync key
        /// stored internally.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.dataSyncKey;
        }


        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="groupID"></param>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        private void GenerateKey(
            string name, string groupID, string deviceID, string locationID)
        {
            this.dataSyncKey = ModelNameUtil.GenerateDataSyncKey(name, groupID, deviceID, locationID, false);
            this.dataSyncGuidKey = ModelNameUtil.GenerateDataSyncKey(name, groupID, deviceID, locationID, true);
        }

    }
}
