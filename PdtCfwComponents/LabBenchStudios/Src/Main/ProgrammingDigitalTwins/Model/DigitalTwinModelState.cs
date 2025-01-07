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
    public class DigitalTwinModelState : IotDataContext, IDigitalTwinStateProcessor
    {
        private string resourcePrefix = ConfigConst.PRODUCT_NAME;

        // this is the DTMI for the model (e.g., dtmi:LabBenchStudios.Pdt.{modelName};1)
        private string modelID = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;
        private string modelGUID = Guid.NewGuid().ToString();

        private DigitalTwinModelSyncKey modelSyncKey = null;
        private DigitalTwinDataSyncKey dataSyncKey = null;

        private string dataSyncKeyStr = null;
        private string prevDataSyncKeyStr = null;

        private string modelSyncKeyStr = null;
        private string prevModelSyncKeyStr = null;

        private Dictionary<string, DigitalTwinProperty> modelProperties;
        private Dictionary<string, DigitalTwinModelState> attachedComponents;

        private ModelNameUtil.DtmiControllerEnum controllerID = ModelNameUtil.DtmiControllerEnum.Custom;

        private string modelJson = null;

        private DigitalTwinModelState parentState = null;

        private bool hasParent = false;
        private bool enableIncomingTelemetryProcessing = true;

        private IDataContextEventListener virtualAssetListener = null;

        /// <summary>
        /// 
        /// </summary>
        public DigitalTwinModelState() :
            base(
                ConfigConst.NOT_SET, ConfigConst.NOT_SET,
                ConfigConst.DEFAULT_TYPE_CATEGORY_ID, ConfigConst.DEFAULT_TYPE_ID)
        {
            this.InitState();
        }

        /// <summary>
        /// 
        /// </summary>
        public DigitalTwinModelState(DigitalTwinDataSyncKey dataSyncKey) :
            base(
                ConfigConst.NOT_SET, ConfigConst.NOT_SET,
                ConfigConst.DEFAULT_TYPE_CATEGORY_ID, ConfigConst.DEFAULT_TYPE_ID)
        {
            if (dataSyncKey != null)
            {
                base.SetName(dataSyncKey.GetName());
                base.SetDeviceID(dataSyncKey.GetDeviceID());
                base.SetLocationID(dataSyncKey.GetLocationID());
            }

            this.InitState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelName"></param>
        public DigitalTwinModelState(string modelName) :
            base(modelName, ConfigConst.NOT_SET, ConfigConst.NOT_SET)
        {
            this.InitState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        public DigitalTwinModelState(
            string modelName, string deviceID, string locationID) :
            base(
                modelName, deviceID,
                ConfigConst.DEFAULT_TYPE_CATEGORY_ID, ConfigConst.DEFAULT_TYPE_ID)
        {
            base.SetLocationID(locationID);

            this.InitState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="typeCategoryID"></param>
        /// <param name="typeID"></param>
        public DigitalTwinModelState(
            string modelName, string deviceID, string locationID, int typeCategoryID, int typeID) :
            base(modelName, deviceID, typeCategoryID, typeID)
        {
            base.SetLocationID(locationID);

            this.InitState();
        }

        // public methods

        /// <summary>
        /// Adds a model component, indexed within the local map by
        /// the DTMI.
        ///
        /// Since relationship references can be => 0 for a single DTMI,
        /// each call will add a new relationship link.
        ///
        /// Components are expected to be leaf nodes
        /// </summary>
        /// <param name="modelState"></param>
        public DigitalTwinModelState AddConnectedModelState(DigitalTwinModelState modelState)
        {
            if (modelState != null)
            {
                string key = modelState.GetModelSyncKeyString();

                if (! this.HasConnectedModelState(key))
                {
                    if (this.attachedComponents == null)
                    {
                        this.attachedComponents = new Dictionary<string, DigitalTwinModelState>();
                    }

                    this.attachedComponents.Add(key, modelState);
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop"></param>
        public string AddModelProperty(DigitalTwinProperty prop)
        {
            if (prop != null)
            {
                this.modelProperties.Add(prop.GetPropertyName(), prop);

                return prop.GetPropertyName();
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DigitalTwinModelState BuildDataSyncKey()
        {
            this.prevDataSyncKeyStr = this.dataSyncKeyStr;

            this.dataSyncKey = null;

            if (this.controllerID == ModelNameUtil.DtmiControllerEnum.Custom) {
                this.dataSyncKey =
                    new DigitalTwinDataSyncKey(
                        this.GetModelControllerName(),
                        base.GetDeviceID(),
                        base.GetLocationID());
            } else {
                this.dataSyncKey =
                    new DigitalTwinDataSyncKey(
                        this.GetModelControllerID().ToString(),
                        base.GetDeviceID(),
                        base.GetLocationID());
            }

            this.dataSyncKeyStr = this.dataSyncKey.ToString();

            if (! string.IsNullOrEmpty(this.prevDataSyncKeyStr))
            {
                if (this.prevDataSyncKeyStr.Equals(this.dataSyncKeyStr))
                {
                    this.prevDataSyncKeyStr = null;
                }
            }

            return this;
        }

        /// <summary>
        /// Safe for external entities to call, as the internal generation
        /// of an instance key will yield a consistent result.
        /// 
        /// This must be invoked for an instance key to be generated, however.
        /// </summary>
        public DigitalTwinModelState BuildModelSyncKey()
        {
            this.prevModelSyncKeyStr = this.modelSyncKeyStr;

            // both calls should generate the same Model ID (DTMI URI)
            this.modelID = string.Empty;

            if (this.controllerID == ModelNameUtil.DtmiControllerEnum.Custom) {
                this.modelID = ModelNameUtil.CreateModelID(this.GetModelControllerName());
            } else {
                this.modelID = ModelNameUtil.CreateModelID(this.controllerID);
            }

            this.modelSyncKey = new DigitalTwinModelSyncKey(this.modelID);
            this.modelSyncKeyStr = this.modelSyncKey.ToString();

            if (! string.IsNullOrEmpty(this.prevModelSyncKeyStr))
            {
                if (this.prevModelSyncKeyStr.Equals(this.modelSyncKeyStr))
                {
                    this.prevModelSyncKeyStr = null;
                }
            }

            return this;
        }

        /// <summary>
        /// This will simply re-parse the stored raw JSON data.
        /// This is typically invoked internally after the JSON
        /// is set via SetModelJson(); however, it can be
        /// called to re-generate any internal structures,
        /// which can be useful when the state instance itself
        /// needs to reset.
        /// </summary>
        /// <returns></returns>
        public bool BuildModelData()
        {
            // TODO: implement this - parse the DTDL and
            // instance all properties / relationships

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void EnableIncomingTelemetryProcessing(bool enable)
        {
            Console.WriteLine($"Setting 'enable incoming telemetry processing' flag: {enable}");

            this.enableIncomingTelemetryProcessing = enable;
        }

        /// <summary>
        /// This method is invoked when local state is updated and needs to
        /// be sent to the 'remote' system.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public ResourceNameContainer GenerateOutgoingStateUpdate(IotDataContext dataContext)
        {
            if (dataContext != null)
            {
                ResourceNameContainer resource =
                    new ResourceNameContainer(
                        this.resourcePrefix,
                        this.GetDeviceID(),
                        dataContext.GetName(),
                        dataContext);

                return resource;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllModelKeys()
        {
            List<string> keys = new List<string>(this.modelProperties.Keys);

            return keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ResourceNameContainer GetCommandResource()
        {
            return this.GetCommandResource(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public ResourceNameContainer GetCommandResource(string resourceType)
        {
            if (! string.IsNullOrEmpty(resourceType))
            {
                resourceType = ConfigConst.ACTUATOR_CMD;
            }

            return new ResourceNameContainer(this.GetResourcePrefix(), this.GetDeviceID(), resourceType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DigitalTwinModelState GetConnectedModelState(string key)
        {
            if (this.HasConnectedModelState(key))
            {
                return this.attachedComponents[key];
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetConnectedModelStateKeys()
        {
            if (this.attachedComponents != null && this.attachedComponents.Count > 0)
            {
                return new List<string>(this.attachedComponents.Keys);
            }
            
            return null;
        }

        /// <summary>
        /// This string is used to uniquely represent this model's assigned
        /// unique incoming telemetry state.
        /// </summary>
        /// <returns></returns>
        public string GetDataSyncKeyString()
        {
            return this.dataSyncKeyStr;
        }

        /// <summary>
        /// This string is used to uniquely represent this model's assigned
        /// unique incoming telemetry state.
        /// 
        /// The instance sync key and telemetry sync key are used to ensure
        /// the source system data (which knows nothing of the digital twin)
        /// can be processed by the appropriate digital twin instance - this
        /// state object (which itself knows nothing of the source system).
        /// </summary>
        /// <returns></returns>
        public DigitalTwinDataSyncKey GetDataSyncKey()
        {
            return this.dataSyncKey;
        }

        /// <summary>
        /// This string is used to uniquely represent this model's assigned
        /// unique telemetry data. It represents the previous sync key used,
        /// in the case where the connection state is updated.
        /// 
        /// It will remain null until the connection state properties on this
        /// model state are updated (via UpdateConnectionState()).
        /// </summary>
        /// <returns></returns>
        public string GetPreviousDataSyncKeyString()
        {
            return this.prevDataSyncKeyStr;
        }

        /// <summary>
        /// This string is used to uniquely represent this model state instance.
        /// It represents the previous sync key used, in the case where the
        /// connection state is updated.
        /// 
        /// It will remain null until the connection state properties on this
        /// model state are updated (via UpdateConnectionState()).
        /// </summary>
        /// <returns></returns>
        public string GetPreviousModelSyncKeyString()
        {
            return this.prevModelSyncKeyStr;
        }

        /// <summary>
        /// This string is used to uniquely represent this model state instance.
        /// It's used to map incoming telemetry (from a unique source) to this
        /// digital twin state container (as a unique instance).
        /// </summary>
        /// <returns></returns>
        public string GetModelSyncKeyString()
        {
            return this.modelSyncKeyStr;
        }

        /// <summary>
        /// This object is used to uniquely represent this model state instance.
        /// It's used to map incoming telemetry (from a unique source) to this
        /// digital twin state container (as a unique instance).
        /// </summary>
        /// <returns></returns>
        public DigitalTwinModelSyncKey GetModelSyncKey()
        {
            return this.modelSyncKey;
        }

        /// <summary>
        /// The full DTMI model ID used by this component. Any other
        /// instances of this state class that are configured with the
        /// same controller ID will have the same DTMI Model ID.
        /// 
        /// To uniquely identify each model state, a GUID is generated
        /// for each instance of this object, and can be retrieved
        /// via the GetModelGUID() method.
        /// </summary>
        /// <returns></returns>
        public string GetModelID()
        {
            return this.modelID;
        }

        /// <summary>
        /// Returns the unique GUID for this model state instance.
        /// </summary>
        /// <returns></returns>
        public string GetModelGUID()
        {
            return this.modelGUID;
        }

        /// <summary>
        /// Returns the Enum controller ID associated with this model state.
        /// This is a simple way to provide a coded mapping 'selector' that
        /// can be easily used by other components to align a specific
        /// DTMI and associated DTDL to a type.
        /// </summary>
        /// <returns></returns>
        public ModelNameUtil.DtmiControllerEnum GetModelControllerID()
        {
            return this.controllerID;
        }

        /// <summary>
        /// Simply delegates to the base class 'GetName()' method.
        /// </summary>
        /// <returns></returns>
        public string GetModelControllerName()
        {
            return base.GetName();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetModelJson()
        {
            return this.modelJson;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetModelPropertyKeys()
        {
            List<string> keys = new List<string>();

            foreach (var item in this.modelProperties)
            {
                if (!item.Value.IsPropertyTelemetry()) keys.Add(item.Key);
            }

            return keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetModelPropertyTelemetryKeys()
        {
            List<string> keys = new List<string>();

            foreach (var item in this.modelProperties)
            {
                if (item.Value.IsPropertyTelemetry()) keys.Add(item.Key);
            }

            return keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DigitalTwinProperty GetModelProperty(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (this.modelProperties.ContainsKey(key))
                {
                    return this.modelProperties[key];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetResourcePrefix()
        {
            return this.resourcePrefix;
        }

        /// <summary>
        /// This method is invoked when incoming telemetry is received
        /// from the 'remote' system.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public bool HandleIncomingTelemetry(IotDataContext dataContext)
        {
            // quick fast exit check
            if (! this.enableIncomingTelemetryProcessing)
            {
                return true;
            }

            // process the incoming data
            if (dataContext != null)
            {
                Console.WriteLine(
                    $"Processing IotDataContext {dataContext.GetDeviceID()} for model {this.modelID}.");

                // TODO: update local state

                // check if there's an attached virtual asset listener
                if (this.virtualAssetListener != null)
                {
                    var actuatorData = dataContext as ActuatorData;

                    if (actuatorData != null)
                    {
                        this.virtualAssetListener.HandleActuatorData(actuatorData);
                        return true;
                    }

                    var sensorData = dataContext as SensorData;

                    if (sensorData != null)
                    {
                        this.ProcessIncomingSensorData(sensorData);
                        this.virtualAssetListener.HandleSensorData(sensorData);
                        return true;
                    }

                    var sysPerfData = dataContext as SystemPerformanceData;

                    if (sysPerfData != null)
                    {
                        this.virtualAssetListener.HandleSystemPerformanceData(sysPerfData);
                        return true;
                    }

                    var connStateData = dataContext as ConnectionStateData;

                    if (connStateData != null)
                    {
                        this.virtualAssetListener.HandleConnectionStateData(connStateData);
                        return true;
                    }

                    var msgData = dataContext as MessageData;

                    if (msgData != null)
                    {
                        this.virtualAssetListener.HandleMessageData(msgData);
                        return true;
                    }
                }
            }
            else
            {
                Console.WriteLine($"Received null IotDataContext. Ignoring.");
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasConnectedModelState(string key)
        {
            return (this.attachedComponents != null && this.attachedComponents.ContainsKey(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        public DigitalTwinModelState SetConnectedDeviceID(string deviceID)
        {
            // base class will handle validation of the name
            base.SetDeviceID(deviceID);

            // update all attached components
            if (this.attachedComponents != null)
            {
                foreach (string key in this.attachedComponents.Keys)
                {
                    this.attachedComponents[key].SetConnectedDeviceID(deviceID);
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationID"></param>
        public DigitalTwinModelState SetConnectedDeviceLocation(string locationID)
        {
            // base class will handle validation of the name
            base.SetLocationID(locationID);

            // update all attached components
            if (this.attachedComponents != null)
            {
                foreach (string key in this.attachedComponents.Keys)
                {
                    this.attachedComponents[key].SetConnectedDeviceID(locationID);
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public DigitalTwinModelState SetDataSyncKey(DigitalTwinDataSyncKey key)
        {
            if (key != null)
            {
                this.dataSyncKey = key;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerID"></param>
        public DigitalTwinModelState SetModelControllerID(ModelNameUtil.DtmiControllerEnum controllerID)
        {
            this.controllerID = controllerID;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        public DigitalTwinModelState SetModelJson(string json)
        {
            this.modelJson = json;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelState"></param>
        public DigitalTwinModelState SetParentStateRef(DigitalTwinModelState modelState)
        {
            if (modelState != null)
            {
                this.parentState = modelState;
                this.hasParent = true;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        public DigitalTwinModelState SetResourcePrefix(string prefix)
        {
            if (! string.IsNullOrEmpty(prefix))
            {
                this.resourcePrefix = prefix;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public DigitalTwinModelState SetVirtualAssetListener(IDataContextEventListener listener)
        {
            if (listener != null)
            {
                this.virtualAssetListener = listener;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <returns></returns>
        public bool UpdateConnectionState(IDigitalTwinStateProcessor processor)
        {
            if (processor != null)
            {
                return this.UpdateConnectionState(processor.GetDeviceID(), processor.GetLocationID());
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <returns></returns>
        public bool UpdateConnectionState(string deviceID, string locationID)
        {
            bool success = false;

            if (! string.IsNullOrEmpty(deviceID))
            {
                base.SetDeviceID(deviceID);
                success = true;
            }

            if (! string.IsNullOrEmpty(locationID))
            {
                base.SetLocationID(locationID);
                success = true;
            }

            this.BuildDataSyncKey();
            this.BuildModelSyncKey();

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(ConfigConst.MODEL_ID_PROP).Append('=').Append(this.modelID).Append(',');
            sb.Append("InstanceKey").Append('=').Append(this.modelSyncKeyStr);

            return sb.ToString();
        }

        // private methods

        /// <summary>
        /// 
        /// </summary>
        private void InitState()
        {
            this.modelProperties = new Dictionary<string, DigitalTwinProperty>();
            this.attachedComponents = new Dictionary<string, DigitalTwinModelState>();

            this.BuildDataSyncKey();
            this.BuildModelSyncKey();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void ProcessIncomingSensorData(SensorData data)
        {
            // assume the incoming data name and the DTDL used as the model
            // for this state object are synchronized name-wise
            //
            // e.g., the name 'relativeHumidity' contained within the DTDL interface
            // must also match with the SensorData 'name' sent from the physical thing
            string key = data.GetName();

            if (this.modelProperties.ContainsKey(key))
            {
                DigitalTwinProperty prop = this.modelProperties[key];
                prop.ApplyTelemetry(data);
            }
        }

    }

}
