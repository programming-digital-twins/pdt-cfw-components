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

using Newtonsoft.Json;

using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Common;

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
    public class DigitalTwinModelStateMap
    {
        private string modelSyncKey = ConfigConst.NOT_SET;
        private string dataSyncKey = ConfigConst.NOT_SET;

        private string modelID = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;
        private ModelNameUtil.DtmiControllerEnum controllerID;

        private Dictionary<string, DigitalTwinModelState> modelStateMap = null;

        /// <summary>
        /// 
        /// </summary>
        public DigitalTwinModelStateMap(string modelSyncKey, string dataSyncKey) : base()
        {
            if (!string.IsNullOrEmpty(modelSyncKey)) { this.modelSyncKey = modelSyncKey; }
            if (!string.IsNullOrEmpty(dataSyncKey)) { this.dataSyncKey = dataSyncKey; }

            this.modelStateMap = new Dictionary<string, DigitalTwinModelState>();
        }

        // public methods

        /// <summary>
        /// Adds a model component, indexed within the local map by
        /// its unique GUID.
        ///
        /// </summary>
        /// <param name="modelState"></param>
        public bool AddModelStateToCache(DigitalTwinModelState modelState)
        {
            if (modelState != null)
            {
                if (!this.modelStateMap.ContainsKey(modelState.GetModelGUID()))
                {
                    Console.WriteLine($"Adding model state to map: {modelState.GetModelGUID()}");
                    this.modelStateMap.Add(modelState.GetModelGUID(), modelState);

                    return true;
                }
                else
                {
                    Console.WriteLine($"Model state already contained within map. Ignoring: {modelState.GetModelGUID()}");
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDataSyncKey() { return dataSyncKey; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetModelSyncKey() { return modelSyncKey; }

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
        /// 
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <returns></returns>
        public DigitalTwinModelState GetStoredModelState(string modelGuid)
        {
            if (this.HasStoredModelState(modelGuid))
            {
                return this.modelStateMap[modelGuid];
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <returns></returns>
        public bool HasStoredModelState(string modelGuid)
        {
            if (!string.IsNullOrEmpty(modelGuid))
            {
                return (this.modelStateMap.ContainsKey(modelGuid));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasStoredModelStates()
        {
            return (this.modelStateMap.Count > 0);
        }

        /// <summary>
        /// Removes a model component, indexed within the local map by
        /// its unique GUID.
        ///
        /// </summary>
        /// <param name="modelState"></param>
        public void RemoveModelStateFromCache(DigitalTwinModelState modelState)
        {
            if (modelState != null)
            {
                if (this.modelStateMap.ContainsKey(modelState.GetModelGUID()))
                {
                    Console.WriteLine($"Removing model state from map: {modelState.GetModelGUID()}");
                    this.modelStateMap.Remove(modelState.GetModelGUID());
                }
                else
                {
                    Console.WriteLine($"Model state not contained within map. Ignoring: {modelState.GetModelGUID()}");
                }
            }
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
                foreach (var kvp in this.modelStateMap)
                {
                    success = kvp.Value.HandleIncomingTelemetry(dataContext);
                }
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
                foreach (var kvp in this.modelStateMap)
                {
                    success = kvp.Value.HandleOutgoingStateUpdate(dataContext);
                }
            }

            return success;
        }

        // overloaded setters - these distribute the update to any other state
        // instances within the list

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        public void SetConnectedDeviceID(string deviceID)
        {
            if (! string.IsNullOrEmpty(deviceID))
            {
                foreach (var kvp in this.modelStateMap)
                {
                    kvp.Value.SetConnectedDeviceID(deviceID);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationID"></param>
        public void SetConnectedDeviceLocation(string locationID)
        {
            if (!string.IsNullOrEmpty(locationID))
            {
                foreach (var kvp in this.modelStateMap)
                {
                    kvp.Value.SetConnectedDeviceLocation(locationID);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerID"></param>
        public void SetModelControllerID(ModelNameUtil.DtmiControllerEnum controllerID)
        {
            this.controllerID = controllerID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelID"></param>
        public void SetModelID(string modelID)
        {
            if (!string.IsNullOrEmpty(modelID))
            {
                this.modelID = modelID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        public void SetRawModelJson(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                foreach (var kvp in this.modelStateMap)
                {
                    kvp.Value.SetRawModelJson(json);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void SetDataSyncKey(DigitalTwinDataSyncKey key)
        {
            if (key != null)
            {
                foreach (var kvp in this.modelStateMap)
                {
                    kvp.Value.SetDataSyncKey(key);
                }
            }
        }

    }

}
