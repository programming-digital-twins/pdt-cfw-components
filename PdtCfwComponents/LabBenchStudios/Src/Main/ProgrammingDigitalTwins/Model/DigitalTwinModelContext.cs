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
using LabBenchStudios.Pdt.Model;

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;

namespace LabBenchStudios.Pdt.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DigitalTwinModelContext : IotDataContext
    {
        // this is the DTMI for the model (e.g., dtmi:LabBenchStudios.Pdt.{modelName};1)
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string modelID = ModelConst.IOT_MODEL_CONTEXT_MODEL_ID;

        [JsonProperty]
        private bool isCommandSet = false;

        [JsonProperty]
        private bool hasError = false;

        private Hashtable modelProperties = new Hashtable();

        private IDataContextEventListener twinUpdateListener = null;
        private IRemoteCommandProcessor remoteCommandProcessor = null;

        // necessary for JSON serialization / deserialization
        public DigitalTwinModelContext() : base()
        {
            this.UpdateTimeStamp();
        }

        public DigitalTwinModelContext(string name, string deviceID, int typeCategoryID, int typeID) :
            base(name, deviceID, typeCategoryID, typeID)
        {
            // set the DTMI (modelID) - only for those that may be deserialized from the EDA
            // all others will keep the default of ModelConst.IOT_MODEL_CONTEXT_MODEL_ID
            //
            // for now, this will suffice as a simple 'mapping' table, and also mitigate
            // any need to update the EDA with knowledge of DTMI naming conventions
            switch (typeID)
            {
                case ConfigConst.FLUID_RATE_SENSOR_TYPE:
                    this.modelID = ModelConst.FLUID_PUMP_CONTROLLER_MODEL_ID; break;

                case ConfigConst.HUMIDIFIER_ACTUATOR_TYPE:
                    this.modelID = ModelConst.HUMIDIFIER_CONTROLLER_MODEL_ID; break;

                case ConfigConst.HVAC_ACTUATOR_TYPE:
                    this.modelID = ModelConst.THERMOSTAT_CONTROLLER_MODEL_ID; break;

                case ConfigConst.SYSTEM_PERF_TYPE:
                    this.modelID = ModelConst.DEVICE_SYS_PERF_COMPONENT_MODEL_ID; break;

                case ConfigConst.HUMIDITY_SENSOR_TYPE:
                    this.modelID = ModelConst.ENV_SENSORS_COMPONENT_MODEL_ID; break;

                case ConfigConst.TEMP_SENSOR_TYPE:
                    this.modelID = ModelConst.ENV_SENSORS_COMPONENT_MODEL_ID; break;

                case ConfigConst.PRESSURE_SENSOR_TYPE:
                    this.modelID = ModelConst.ENV_SENSORS_COMPONENT_MODEL_ID; break;

                case ConfigConst.WIND_SYSTEM_TYPE:
                    this.modelID = ModelConst.POWER_WINDMILL_CONTROLLER_MODEL_ID; break;
            }

            this.UpdateTimeStamp();
        }

        // public methods

        public string GetModelID()
        {
            return this.modelID;
        }

        public bool GetModelBoolean(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (this.modelProperties.ContainsKey(key))
                {
                    string valStr = (string)this.modelProperties[key];
                    bool isValue = bool.TryParse(valStr, out bool isTrue);

                    if (isValue)
                    {
                        return isTrue;
                    }
                }
            }

            return false;
        }

        public float GetModelValue(string key)
        {
            if(! string.IsNullOrEmpty(key))
            {
                if (this.modelProperties.ContainsKey(key))
                {
                    string valStr = (string)this.modelProperties[key];
                    bool isValue = float.TryParse(valStr, out float val);

                    if (isValue)
                    {
                        return val;
                    }
                }
            }

            return 0.0f;
        }

        public string GetModelProperty(string key)
        {
            if (! string.IsNullOrEmpty(key))
            {
                if (this.modelProperties.ContainsKey(key))
                {
                    return (string) this.modelProperties[key];
                }
            }

            return null;
        }

        public void SetDataContextEventListener(IDataContextEventListener dataListener)
        {
            if (dataListener != null)
            {
                this.twinUpdateListener = dataListener;
            }
        }

        public void SetRemoteCommandProcessor(IRemoteCommandProcessor cmdProcessor)
        {
            if (cmdProcessor != null)
            {
                this.remoteCommandProcessor = cmdProcessor;
            }
        }

        public bool UpdateRemoteSystemState(IotDataContext dataContext)
        {
            bool success = false;

            if (dataContext != null)
            {
                switch (dataContext.GetTypeID())
                {

                }

                success = true;
            }

            return success;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(ConfigConst.MODEL_ID_PROP).Append('=').Append(this.modelID).Append(',');

            return sb.ToString();
        }

    }
}
