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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

using Newtonsoft.Json;

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
        private string modelID = ModelConst.IOT_MODEL_CONTEXT_MODEL_ID;

        private Hashtable modelProperties = new Hashtable();

        private Dictionary<string, List<DigitalTwinModelState>> stateRelationshipMap =
            new Dictionary<string, List<DigitalTwinModelState>>();

        private Dictionary<string, DigitalTwinModelState> stateComponentMap =
            new Dictionary<string, DigitalTwinModelState>();

        private ModelConst.DtmiControllerEnum modelControllerID;

        private string rawModelJson = null;

        private IDataContextEventListener virtualAssetListener = null;

        public DigitalTwinModelState() : base()
        {
            this.modelID = ModelConst.GetModelID(this.GetTypeID());
        }

        public DigitalTwinModelState(
            string name, string deviceID, int typeCategoryID, int typeID)
        {
            this.modelID = ModelConst.GetModelID(typeID);
        }

        // public methods

        /// <summary>
        /// Adds a model relationship, indexed within the local map by
        /// the DTMI.
        ///
        /// Since relationship references can be => 0 for a single DTMI,
        /// each call will add a new relationship link.
        /// </summary>
        /// <param name="modelState"></param>
        public void AddModelRelationship(DigitalTwinModelState modelState)
        {
            if (modelState != null)
            {

            }
        }

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
        public void AddModelComponent(DigitalTwinModelState modelState)
        {
            if (modelState != null)
            {

            }
        }

        public string GetModelID()
        {
            return this.modelID;
        }

        public ModelConst.DtmiControllerEnum GetModelControllerID()
        {
            return this.modelControllerID;
        }

        public string GetRawModelJson()
        {
            return this.rawModelJson;
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

        public void SetModelControllerID(ModelConst.DtmiControllerEnum controllerID)
        {
            this.modelControllerID = controllerID;
        }

        public void SetRawModelJson(string json)
        {
            this.rawModelJson = json;
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

            return sb.ToString();
        }

    }
}
