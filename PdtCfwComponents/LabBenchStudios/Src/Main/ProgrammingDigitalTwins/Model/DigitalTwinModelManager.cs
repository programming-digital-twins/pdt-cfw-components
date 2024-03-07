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
using System.IO;

using DTDLParser;
using DTDLParser.Models;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Model
{
    /// <summary>
    /// This class serves as both factory and manager for created objects
    /// related to the stored DTDL models within its internal cache.
    /// It will store both the raw JSON and the DTInterfaceInfo for each
    /// DTMI absolute URI, using the latter as the key for each separate cache.
    /// </summary>
    public class DigitalTwinModelManager : IDigitalTwinStateProcessor, IDataContextEventListener
    {
        private string modelFilePath = ModelNameUtil.DEFAULT_MODEL_FILE_PATH;

        // this contains all the DT model parsed instances
        // this is indexed by the DTDLParser DTMI absolute URI (string)
        private IReadOnlyDictionary<string, DTInterfaceInfo> digitalTwinInterfaceCache;

        // this contains all the DT model raw JSON data
        // this is indexed by the string DTMI (use ModelConst.DtmiControllerEnum)
        private Dictionary<string, string> digitalTwinDtdlJsonCache;

        // this contains all DT model state instances that are associated
        // with a given dtmi string (which is the lookup key)
        //
        // for the current implementation, this is sufficient granularity;
        // a future implementation may expand into a more complex structure
        //
        // this is indexed by a key unique to the twin state's instancing
        // rules with help from ModelNameUtil.
        private Dictionary<string, DigitalTwinModelState> digitalTwinStateCache;

        // this maps the incoming telemetry
        // this is indexed by the string instance key
        private Dictionary<string, string> modelToTelemetryKeyMap;


        private bool hasSuccessfulDataLoad = false;

        /// <summary>
        /// Default constructor. Uses the default model file path specified
        /// in ModelNameUtil.
        /// </summary>
        public DigitalTwinModelManager() : this(ModelNameUtil.DEFAULT_MODEL_FILE_PATH)
        {
            // nothing to do
        }

        /// <summary>
        /// Constructor that accepts a model file path. If validated, it
        /// will be used as the path from which to load models; else the
        /// default model file path from ModelNameUtil will be used.
        /// </summary>
        /// <param name="modelFilePath"></param>
        public DigitalTwinModelManager(string modelFilePath)
        {
            this.SetModelFilePath(modelFilePath);

            this.digitalTwinDtdlJsonCache = new Dictionary<string, string>();
            this.digitalTwinStateCache    = new Dictionary<string, DigitalTwinModelState>();
            this.modelToTelemetryKeyMap   = new Dictionary<string, string>();
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSyncKey"></param>
        /// <param name="instanceKey"></param>
        public void AssignTelemetryKeyToModel(
            DigitalTwinDataSyncKey dataSyncKey,
            string instanceKey)
        {
            if (dataSyncKey != null)
            {
                this.AssignTelemetryKeyToModel(dataSyncKey.ToString(), instanceKey);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSyncKey"></param>
        /// <param name="modelSyncKey"></param>
        public void AssignTelemetryKeyToModel(
            string dataSyncKey,
            string modelSyncKey)
        {
            if (! string.IsNullOrEmpty(dataSyncKey) && ! string.IsNullOrEmpty(modelSyncKey))
            {
                if (this.modelToTelemetryKeyMap.ContainsKey(dataSyncKey))
                {
                    this.modelToTelemetryKeyMap[dataSyncKey] = modelSyncKey;
                }
                else
                {
                    this.modelToTelemetryKeyMap.Add(dataSyncKey, modelSyncKey);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSyncKey"></param>
        /// <param name="controllerID"></param>
        /// <param name="stateUpdateListener"></param>
        /// <returns></returns>
        public DigitalTwinModelState CreateModelState(
            DigitalTwinDataSyncKey dataSyncKey,
            ModelNameUtil.DtmiControllerEnum controllerID,
            IDataContextEventListener stateUpdateListener)
        {
            return this.CreateModelState(
                dataSyncKey, false, controllerID, stateUpdateListener);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSyncKey"></param>
        /// <param name="useGuid"></param>
        /// <param name="controllerID"></param>
        /// <param name="stateUpdateListener"></param>
        /// <returns></returns>
        public DigitalTwinModelState CreateModelState(
            DigitalTwinDataSyncKey dataSyncKey,
            bool useGuid,
            ModelNameUtil.DtmiControllerEnum controllerID,
            IDataContextEventListener stateUpdateListener)
        {
            var dtModelState = new DigitalTwinModelState(dataSyncKey);

            dtModelState
                .SetModelControllerID(controllerID)
                .SetRawModelJson(this.GetRawModelJson(controllerID))
                .SetVirtualAssetListener(stateUpdateListener);

            dtModelState.BuildModelData();
            dtModelState.BuildModelSyncKey();

            this.UpdateModelStateProperties(dtModelState);

            string modelSyncKey = dtModelState.GetModelSyncKeyString();

            if (this.HasDigitalTwinModelState(modelSyncKey))
            {
                Console.WriteLine($"Created DigitalTwinModelState has duplicate key {modelSyncKey}. Discarding new and returning original.");

                dtModelState = this.digitalTwinStateCache[modelSyncKey];
            }
            else
            {
                Console.WriteLine($"Created / cached DigitalTwinModelState with instance key {modelSyncKey}");

                this.digitalTwinStateCache.Add(modelSyncKey, dtModelState);
            }

            this.AssignTelemetryKeyToModel(dataSyncKey, modelSyncKey);

            return dtModelState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="controllerID"></param>
        /// <param name="stateUpdateListener"></param>
        /// <returns></returns>
        public DigitalTwinModelState CreateModelState(
            string deviceID,
            string locationID,
            ModelNameUtil.DtmiControllerEnum controllerID,
            IDataContextEventListener stateUpdateListener)
        {
            return this.CreateModelState(
                deviceID, locationID, false, controllerID, stateUpdateListener);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="useGuid"></param>
        /// <param name="controllerID"></param>
        /// <param name="stateUpdateListener"></param>
        /// <returns></returns>
        public DigitalTwinModelState CreateModelState(
            string deviceID,
            string locationID,
            bool useGuid,
            ModelNameUtil.DtmiControllerEnum controllerID,
            IDataContextEventListener stateUpdateListener)
        {
            var dtModelState = new DigitalTwinModelState(deviceID, locationID);

            dtModelState
                .SetConnectedDeviceID(deviceID)
                .SetConnectedDeviceLocation(locationID)
                .SetModelControllerID(controllerID)
                .SetRawModelJson(this.GetRawModelJson(controllerID))
                .SetVirtualAssetListener(stateUpdateListener);
            
            dtModelState.BuildModelData();
            dtModelState.BuildModelSyncKey();

            this.UpdateModelStateProperties(dtModelState);

            DigitalTwinDataSyncKey dataSyncKey =
                new DigitalTwinDataSyncKey(controllerID.ToString(), deviceID, locationID);

            dtModelState.SetDataSyncKey(dataSyncKey);

            string modelSyncKey = dtModelState.GetModelSyncKeyString();

            if (this.HasDigitalTwinModelState(modelSyncKey))
            {
                Console.WriteLine($"Created DigitalTwinModelState has duplicate key {modelSyncKey}. Discarding new and returning original.");

                // TODO: replace the original or retrieve and return the original?
                dtModelState = this.digitalTwinStateCache[modelSyncKey];
            }
            else
            {
                Console.WriteLine(
                    "Created / cached DigitalTwinModelState." +
                    $"\n\tModel Sync Key: {modelSyncKey}" +
                    $"\n\tModel ID: {dtModelState.GetModelID()}");

                this.digitalTwinStateCache.Add(modelSyncKey, dtModelState);
            }

            this.AssignTelemetryKeyToModel(dataSyncKey, modelSyncKey);

            return dtModelState;
        }

        /// <summary>
        /// Returns the internally stored DT Model State instance using
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public DigitalTwinModelState GetDigitalTwinModelState(string instanceKey)
        {
            if (this.HasDigitalTwinModelState(instanceKey))
            {
                return this.digitalTwinStateCache[instanceKey];
            }

            Console.WriteLine($"No DT model state available for key {instanceKey}");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public bool HasDigitalTwinModelState(string instanceKey)
        {
            if (!string.IsNullOrEmpty(instanceKey))
            {
                return this.digitalTwinStateCache.ContainsKey(instanceKey);
            }

            return false;
        }

        /// <summary>
        /// Generates a new List<string> of DTMI absolute URI's when called.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllDtmiValues()
        {
            if (digitalTwinInterfaceCache != null && digitalTwinInterfaceCache.Count > 0)
            {
                List<string> dtmiValues = new List<string>(digitalTwinInterfaceCache.Keys);

                return dtmiValues;
            }
            else
            {
                Console.WriteLine($"No DTDL model cache loaded from file path {this.modelFilePath}");
            }

            return null;
        }

        /// <summary>
        /// Returns the DTDL JSON for the given controller
        /// </summary>
        /// <param name="dtmiController"></param>
        /// <returns></returns>
        public string GetRawModelJson(ModelNameUtil.DtmiControllerEnum dtmiController)
        {
            string dtmiUri = ModelNameUtil.CreateModelID(dtmiController);

            if (this.digitalTwinDtdlJsonCache.ContainsKey(dtmiUri))
            {
                return this.digitalTwinDtdlJsonCache[dtmiUri];
            }
            else
            {
                Console.WriteLine($"No raw DTDL JSON available for DTMI URI {dtmiUri}");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool HandleIncomingTelemetry(IotDataContext dataContext)
        {
            DigitalTwinModelState modelState = LookupDigitalTwinModelState(dataContext);

            if (modelState != null)
            {
                return modelState.HandleIncomingTelemetry(dataContext);
            }

            // TODO: log msg?
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool HandleOutgoingStateUpdate(IotDataContext dataContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void HandleActuatorData(ActuatorData data)
        {
            if (data != null && data.IsResponse())
            {
                DigitalTwinModelState modelState = LookupDigitalTwinModelState(data);

                if (modelState != null)
                {
                    modelState.HandleIncomingTelemetry(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void HandleConnectionStateData(ConnectionStateData data)
        {
            DigitalTwinModelState modelState = LookupDigitalTwinModelState(data);

            if (modelState != null)
            {
                modelState.HandleIncomingTelemetry(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void HandleMessageData(MessageData data)
        {
            DigitalTwinModelState modelState = LookupDigitalTwinModelState(data);

            if (modelState != null)
            {
                modelState.HandleIncomingTelemetry(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void HandleSensorData(SensorData data)
        {
            DigitalTwinModelState modelState = LookupDigitalTwinModelState(data);

            if (modelState != null)
            {
                modelState.HandleIncomingTelemetry(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void HandleSystemPerformanceData(SystemPerformanceData data)
        {
            DigitalTwinModelState modelState = LookupDigitalTwinModelState(data);

            if (modelState != null)
            {
                modelState.HandleIncomingTelemetry(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasSuccessfulDataLoad()
        {
            return this.hasSuccessfulDataLoad;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtController"></param>
        public void RegisterModelController(DigitalTwinModelState dtController)
        {

        }

        /// <summary>
        /// This will trigger a request to model manager to load
        /// (or reload) this state objects respective model.
        /// </summary>
        /// <returns></returns>
        public bool BuildModelData()
        {
            if (!string.IsNullOrEmpty(this.modelFilePath) && Directory.Exists(modelFilePath))
            {
                return this.LoadAndValidateDtdlModels();
            }
            else
            {
                Console.WriteLine($"Ignoring DTDL reload request. File path is invalid: {this.modelFilePath}");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <returns></returns>
        public bool SetModelFilePath(string modelFilePath)
        {
            if (!string.IsNullOrEmpty(modelFilePath) && Directory.Exists(modelFilePath))
            {
                Console.WriteLine($"Setting model file path. File path is good: {modelFilePath}");
                this.modelFilePath = modelFilePath;
                return true;
            }

            Console.WriteLine($"Failed to set model file path. File path is invalid: {modelFilePath}");
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
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

        // private methods

        /// <summary>
        /// Unfortunately, this method results in each DTDL model being loaded twice
        /// Future optimizations will probably remove this redundancy.
        /// </summary>
        private bool LoadAndValidateDtdlModels()
        {
            bool success = false;

            // update DTDL object cache
            this.digitalTwinInterfaceCache = ModelParserUtil.LoadAllDtdlInterfaces(this.modelFilePath);

            if (this.digitalTwinInterfaceCache == null)
            {
                Console.WriteLine($"Failed to load DTDL models from path {this.modelFilePath}");

                return false;
            }

            // update DTDL JSON cache
            var dtmiControllerList =
                (ModelNameUtil.DtmiControllerEnum[]) Enum.GetValues(typeof(ModelNameUtil.DtmiControllerEnum));

            foreach (ModelNameUtil.DtmiControllerEnum dtmiController in dtmiControllerList)
            {
                string dtmiUri  = ModelNameUtil.CreateModelID(dtmiController);
                string fileName = ModelNameUtil.GetModelFileName(dtmiController);

                string dtdlJson = ModelParserUtil.LoadDtdlFile(this.modelFilePath, fileName);

                Console.WriteLine($"  -> DTMI URI: {dtmiUri}. Loaded file: {fileName}");

                if (!string.IsNullOrEmpty(dtdlJson))
                {
                    if (! this.digitalTwinDtdlJsonCache.ContainsKey(dtmiUri))
                    {
                        this.digitalTwinDtdlJsonCache.Add(dtmiUri, dtdlJson);
                    }
                    else
                    {
                        this.digitalTwinDtdlJsonCache[dtmiUri] = dtdlJson;
                    }

                    success = true;
                }
                else
                {
                    Console.WriteLine($"Failed to load DTDL models from file {fileName}");
                }
            }

            // update model state cache
            foreach (string key in this.digitalTwinStateCache.Keys)
            {
                DigitalTwinModelState dtState = this.digitalTwinStateCache[key];

                if (dtState != null)
                {
                    Console.WriteLine($"Retrieved DT Model State {key} with ID {dtState.GetModelID()}");

                    string rawJson = this.GetRawModelJson(dtState.GetModelControllerID());

                    dtState.SetRawModelJson(rawJson);
                }
            }

            this.hasSuccessfulDataLoad = success;

            return success;
        }

        private DigitalTwinModelState LookupDigitalTwinModelState(IotDataContext dataContext)
        {
            if (dataContext != null)
            {
                string dataSyncKey = ModelNameUtil.GenerateDataSyncKey(dataContext).ToString();

                return this.LookupDigitalTwinModelState(dataSyncKey);
            }

            return null;
        }

        private DigitalTwinModelState LookupDigitalTwinModelState(string telemetryKey)
        {
            if (! string.IsNullOrEmpty(telemetryKey))
            {
                if (this.modelToTelemetryKeyMap.ContainsKey(telemetryKey))
                {
                    string instanceKey = this.modelToTelemetryKeyMap[telemetryKey];

                    if (this.digitalTwinStateCache.ContainsKey(instanceKey))
                    {
                        return this.digitalTwinStateCache[instanceKey];
                    }
                }
            }

            return null;
        }

        private void UpdateModelStateProperties(DigitalTwinModelState modelState)
        {
            if (modelState != null)
            {
                DTInterfaceInfo interfaceInfo = this.digitalTwinInterfaceCache[modelState.GetModelID()];

                if (interfaceInfo != null)
                {
                    Console.WriteLine($"Found interface for model ID {modelState.GetModelID()}: {interfaceInfo.DisplayName}");

                    // TODO: update the properties list with Property and Telemetry DTDL data!!

                    IReadOnlyDictionary<string, DTPropertyInfo> propEntries = interfaceInfo.Properties;

                    foreach (var propEntry in propEntries)
                    {
                        DigitalTwinProperty prop = new DigitalTwinProperty(propEntry.Key);
                        DTPropertyInfo dtdlProp = propEntry.Value;

                        prop.SetDisplayName(dtdlProp.Name);
                        prop.SetDescription(dtdlProp.Schema.Comment);
                        prop.SetDetail(dtdlProp.ToString());
                        prop.SetAsEnabled(true);
                        prop.SetAsTelemetry(false);
                        prop.SetAsWriteable(dtdlProp.Writable);

                        Console.WriteLine($"Adding property to state {prop}");
                        modelState.AddModelProperty(prop);
                    }

                    IReadOnlyDictionary<string, DTTelemetryInfo> telemetryEntries = interfaceInfo.Telemetries;

                    foreach (var telemetryEntry in telemetryEntries)
                    {
                        DigitalTwinProperty telemetry = new DigitalTwinProperty(telemetryEntry.Key);
                        DTTelemetryInfo dtdlTelemetry = telemetryEntry.Value;

                        telemetry.SetDisplayName(dtdlTelemetry.Name);
                        telemetry.SetDescription(dtdlTelemetry.Schema.Comment);
                        telemetry.SetDetail(dtdlTelemetry.ToString());
                        telemetry.SetAsEnabled(true);
                        telemetry.SetAsTelemetry(true);

                        Console.WriteLine($"Adding telemetry to state {telemetry}");
                        modelState.AddModelProperty(telemetry);
                    }

                    Console.WriteLine($"\n\n=====\n{modelState.GetRawModelJson()}\n\n=====\n");
                    Console.WriteLine(
                        $"\nLoaded interface for model:" +
                        $"\n -> Model ID: {modelState.GetModelID()}" +
                        $"\n -> Data Sync Key: {modelState.GetDataSyncKey()}" +
                        $"\n -> Model Sync Key: {modelState.GetModelSyncKeyString()}");

                }
                else
                {
                    Console.WriteLine($"No interface for model ID {modelState.GetModelID()}");
                }
            }
        }

    }
}
