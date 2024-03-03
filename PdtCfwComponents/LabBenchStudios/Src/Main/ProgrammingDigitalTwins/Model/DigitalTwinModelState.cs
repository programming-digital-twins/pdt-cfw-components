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
using System.Web.ModelBinding;
using System.Linq;

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
        // this is the DTMI for the model (e.g., dtmi:LabBenchStudios.Pdt.{modelName};1)
        private string modelID = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;
        private string modelGUID = System.Guid.NewGuid().ToString();

        private string instanceKey = null;

        private Dictionary<string, DigitalTwinProperty> modelProperties;
        private Dictionary<string, DigitalTwinModelState> attachedComponents;

        private ModelNameUtil.DtmiControllerEnum modelControllerID;

        private string rawModelJson = null;

        private DigitalTwinModelState parentState = null;

        private bool hasParent = false;

        private IDataContextEventListener virtualAssetListener = null;

        public DigitalTwinModelState() :
            base(
                ConfigConst.NOT_SET, ConfigConst.NOT_SET,
                ConfigConst.DEFAULT_TYPE_CATEGORY_ID, ConfigConst.DEFAULT_TYPE_ID)
        {
            InitState();
        }

        public DigitalTwinModelState(
            string name, string deviceID) :
            base(
                name, deviceID,
                ConfigConst.DEFAULT_TYPE_CATEGORY_ID, ConfigConst.DEFAULT_TYPE_ID)
        {
            InitState();
        }

        public DigitalTwinModelState(
            string name, string deviceID, int typeCategoryID, int typeID) :
            base(name, deviceID, typeCategoryID, typeID)
        {
            this.modelID = ModelNameUtil.GetModelID(typeID);

            InitState();
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
        public void AddConnectedModelState(DigitalTwinModelState modelState)
        {
            if (modelState != null)
            {
                string key = modelState.GetInstanceKey();

                if (! this.HasConnectedModelState(key))
                {
                    if (this.attachedComponents == null)
                    {
                        this.attachedComponents = new Dictionary<string, DigitalTwinModelState>();
                    }

                    this.attachedComponents.Add(key, modelState);
                }
            }
        }
        
        public DigitalTwinModelState GetConnectedModelState(string key)
        {
            if (this.HasConnectedModelState(key))
            {
                return this.attachedComponents[key];
            }

            return null;
        }

        public List<string> GetConnectedModelStateKeys()
        {
            if (this.attachedComponents != null && this.attachedComponents.Count > 0)
            {
                return new List<string>(this.attachedComponents.Keys);
            }
            
            return null;
        }

        public bool HasConnectedModelState(string key)
        {
            return (this.attachedComponents != null && this.attachedComponents.ContainsKey(key));
        }

        /// <summary>
        /// Returns the unique instance key for this state object.
        /// This is generated via a combination of properties
        /// that are both generic and unique to this instance,
        /// with the help of a utility method in ModelNameUtil.
        /// </summary>
        /// <returns></returns>
        public string GetInstanceKey()
        {
            return this.instanceKey;
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
            return this.modelControllerID;
        }

        public string GetRawModelJson()
        {
            return this.rawModelJson;
        }

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
        /// This method is invoked when incoming telemetry is received
        /// from the 'remote' system.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public bool HandleIncomingTelemetry(IotDataContext dataContext)
        {
            bool success = false;

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

                    }
                }

                success = true;
            }
            else
            {
                Console.WriteLine($"Received null IotDataContext. Ignoring.");
            }

            return success;
        }

        /// <summary>
        /// This method is invoked when local state is updated and needs to
        /// be sent to the 'remote' system.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public bool HandleOutgoingStateUpdate(IotDataContext dataContext)
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

        /// <summary>
        /// Safe for external entities to call, as the internal generation
        /// of an instance key will yield a consistent result.
        /// 
        /// This must be invoked for an instance key to be generated, however.
        /// </summary>
        public void InitInstanceKey()
        {
            this.InitInstanceKey(false);
        }

        /// <summary>
        /// Safe for external entities to call, as the internal generation
        /// of an instance key will yield a consistent result.
        /// 
        /// This must be invoked for an instance key to be generated, however.
        /// </summary>
        /// <param name="useGuid">If true, the GUID generated when this
        /// class was instanced will be used in the isntancing key.</param>
        public void InitInstanceKey(bool useGuid)
        {
            this.instanceKey =
                useGuid ?
                    ModelNameUtil.CreateModelDataSyncKey(this.GetDeviceID(), this.GetLocationID(), this.GetModelGUID()) :
                    ModelNameUtil.CreateModelDataSyncKey(this.GetDeviceID(), this.GetLocationID());

            Console.WriteLine($"DT model state instance key generated: {this.instanceKey}");
        }

        /// <summary>
        /// This will simply re-parse the stored raw JSON data.
        /// This is typically invoked internally after the JSON
        /// is set via SetRawModelJson(); however, it can be
        /// called to re-generate any internal structures,
        /// which can be useful when the state instance itself
        /// needs to reset.
        /// </summary>
        /// <returns></returns>
        public bool ReloadModelData()
        {
            // TODO: implement this
            return true;
        }

        public void SetConnectedDeviceID(string deviceID)
        {
            // base class will handle validation of the name
            base.SetDeviceID(deviceID);
        }

        public void SetConnectedDeviceLocation(string locationID)
        {
            // base class will handle validation of the name
            base.SetLocationID(locationID);
        }

        public void SetModelControllerID(ModelNameUtil.DtmiControllerEnum controllerID)
        {
            this.modelControllerID = controllerID;
        }

        public void SetParentStateRef(DigitalTwinModelState modelState)
        {
            if (modelState != null)
            {
                this.parentState = modelState;
                this.hasParent = true;
            }
        }

        public void SetRawModelJson(string json)
        {
            this.rawModelJson = json;

            this.ReloadModelData();
        }

        public void SetVirtualAssetListener(IDataContextEventListener listener)
        {
            if (listener != null)
            {
                this.virtualAssetListener = listener;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(ConfigConst.MODEL_ID_PROP).Append('=').Append(this.modelID).Append(',');
            sb.Append("InstanceKey").Append('=').Append(this.instanceKey);

            return sb.ToString();
        }

        // private methods

        private void InitState()
        {
            this.modelProperties = new Dictionary<string, DigitalTwinProperty>();
            this.attachedComponents = new Dictionary<string, DigitalTwinModelState>();

            this.InitInstanceKey(false);
        }

    }
}
