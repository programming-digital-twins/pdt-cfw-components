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
using System.Text;
using System.Xml.Schema;
using DTDLParser.Models;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using static LabBenchStudios.Pdt.Model.ModelParserUtil;

namespace LabBenchStudios.Pdt.Model
{
    /// <summary>
    /// This class serves as both factory and manager for created objects
    /// related to the stored DTDL models within its internal cache.
    /// It will store both the raw JSON and the DTInterfaceInfo for each
    /// DTMI absolute URI, using the latter as the key for each separate cache.
    /// </summary>
    public class DigitalTwinModelManager : IDataContextEventListener
    {
        private HashSet<string> modelFilePaths = new HashSet<string>();

        private string resourcePrefix = ConfigConst.PRODUCT_NAME;

        private bool hasSuccessfulDataLoad = false;

        private DigitalTwinModelManagerCache digitalTwinModelMgrCache = null;

        // this contains all the DT model parsed instances
        // this is indexed by the DTDLParser DTMI absolute URI (string)
        private Dictionary<string, DTInterfaceInfo> digitalTwinInterfaceCache = null;

        // for convenience, this maps a DTMI URI to the associated file it
        // was loaded from
        private Dictionary<string, string> digitalTwinModelToFileMap = null;

        // useful for passing event messages and debugging
        private ISystemStatusEventListener eventListener = null;

        /// <summary>
        /// Default constructor. Uses the default model file path specified
        /// in ModelNameUtil.
        /// </summary>
        public DigitalTwinModelManager()
        {
            this.digitalTwinModelMgrCache = new DigitalTwinModelManagerCache();
            this.digitalTwinInterfaceCache = new Dictionary<string, DTInterfaceInfo>();
            this.digitalTwinModelToFileMap = new Dictionary<string, string>();
        }

        // public methods

        /// <summary>
        /// This will trigger a request to model manager to load
        /// (or reload) this state objects respective model.
        /// </summary>
        /// <returns></returns>
        public bool BuildModelData()
        {
            if (this.modelFilePaths.Count > 0)
            {
                // the initial call to LoadAndValidDtdlModelInterfaceData may be
                // destructive to this.modelFilePaths, as it will remove any invalid
                // DTDL files from the set
                //
                // this call will succeed as long as >= 1 DTDL interface is loaded
                bool areInterfacesLoaded = this.LoadAndValidateDtdlModelInterfaceData();

                // both should either succeed or fail
                return (areInterfacesLoaded);
            }
            else
            {
                Console.WriteLine($"Ignoring DTDL reload request. No stored file paths to process.");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="useGuid"></param>
        /// <param name="addToParent"></param>
        /// <param name="controllerID"></param>
        /// <param name="stateUpdateListener"></param>
        /// <returns></returns>
        public DigitalTwinModelState CreateModelState(
            IDigitalTwinStateProcessor processor,
            bool useGuid,
            bool addToParent,
            ModelNameUtil.DtmiControllerEnum controllerID,
            IDataContextEventListener stateUpdateListener)
        {
            if (processor != null)
            {
                DigitalTwinModelState createdProcessor =
                    this.CreateModelState(
                        processor.GetDeviceID(),
                        processor.GetLocationID(),
                        useGuid,
                        controllerID,
                        stateUpdateListener);

                if (addToParent)
                {
                    processor.AddConnectedModelState(createdProcessor);
                }

                return createdProcessor;
            }

            return null;
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
            var dtModelState = new DigitalTwinModelState(dataSyncKey);

            return this.ConfigureAndStoreModelState(dtModelState, controllerID, stateUpdateListener);
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
            var dtModelState = new DigitalTwinModelState(deviceID, deviceID, locationID);

            return this.ConfigureAndStoreModelState(dtModelState, controllerID, stateUpdateListener);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="typeCategoryID"></param>
        /// <param name="typeID"></param>
        /// <param name="useGuid"></param>
        /// <param name="controllerID"></param>
        /// <param name="stateUpdateListener"></param>
        /// <returns></returns>
        public DigitalTwinModelState CreateModelState(
            string deviceID,
            string locationID,
            int typeCategoryID,
            int typeID,
            bool useGuid,
            ModelNameUtil.DtmiControllerEnum controllerID,
            IDataContextEventListener stateUpdateListener)
        {
            var dtModelState = new DigitalTwinModelState(deviceID, deviceID, locationID, typeCategoryID, typeID);

            return this.ConfigureAndStoreModelState(dtModelState, controllerID, stateUpdateListener);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="typeCategoryID"></param>
        /// <param name="typeID"></param>
        /// <param name="useGuid"></param>
        /// <param name="customName"></param>
        /// <param name="stateUpdateListener"></param>
        /// <returns></returns>
        public DigitalTwinModelState CreateModelState(
            string deviceID,
            string locationID,
            int typeCategoryID,
            int typeID,
            bool useGuid,
            string customName,
            IDataContextEventListener stateUpdateListener)
        {
            var dtModelState = new DigitalTwinModelState(deviceID, deviceID, locationID, typeCategoryID, typeID);

            dtModelState.SetName(customName);

            return this.ConfigureAndStoreModelState(dtModelState, ModelNameUtil.DtmiControllerEnum.Custom, stateUpdateListener);
        }

        /// <summary>
        /// Generates a new List<string> of DTMI absolute URI's when called.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllDtmiValues()
        {
            if (digitalTwinInterfaceCache.Count > 0)
            {
                List<string> dtmiValues = new List<string>(digitalTwinInterfaceCache.Keys);

                return dtmiValues;
            }
            else
            {
                Console.WriteLine($"No DTDL model cache loaded from stored file paths.");
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCountOfCachedJsonModels()
        {
            return this.digitalTwinModelMgrCache.GetCountOfCachedJsonModels();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCountOfCachedModelStates()
        {
            return this.digitalTwinModelMgrCache.GetCountOfCachedModelStates();
        }

        /// <summary>
        /// Returns the DTDL JSON for the given controller
        /// </summary>
        /// <param name="dtmiController"></param>
        /// <returns></returns>
        public string GetDigitalTwinModelJson(ModelNameUtil.DtmiControllerEnum dtmiController)
        {
            return this.digitalTwinModelMgrCache.GetDigitalTwinModelJson(dtmiController);
        }

        /// <summary>
        /// Returns the DTDL JSON for the given controller model name - short form.
        /// That is, the abbreviated name for the controller model (e.g., thermostat).
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public string GetDigitalTwinModelJson(string modelName)
        {
            return this.digitalTwinModelMgrCache.GetDigitalTwinModelJson(modelName);
        }

        /// <summary>
        /// Returns the internally stored DT Model State instance using
        /// its model state key.
        /// </summary>
        /// <param name="modelStateKey"></param>
        /// <returns></returns>
        public DigitalTwinModelState GetDigitalTwinModelState(string modelStateKey)
        {
            return this.digitalTwinModelMgrCache.GetDigitalTwinModelState(modelStateKey);
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
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool HandleIncomingTelemetry(IotDataContext data)
        {
            List<DigitalTwinModelState> modelStateList =
                this.digitalTwinModelMgrCache.LookupDigitalTwinModelState(data);

            if (modelStateList != null)
            {
                foreach (DigitalTwinModelState modelState in modelStateList)
                {
                    modelState.HandleIncomingTelemetry(data);
                }
            }

            // TODO: log msg?
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleActuatorData(ActuatorData data)
        {
            if (data != null && data.IsResponse())
            {
                List<DigitalTwinModelState> modelStateList =
                    this.digitalTwinModelMgrCache.LookupDigitalTwinModelState(data);

                if (modelStateList != null)
                {
                    foreach (DigitalTwinModelState modelState in modelStateList)
                    {
                        modelState.HandleIncomingTelemetry(data);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleConnectionStateData(ConnectionStateData data)
        {
            List<DigitalTwinModelState> modelStateList =
                this.digitalTwinModelMgrCache.LookupDigitalTwinModelState(data);

            if (modelStateList != null)
            {
                foreach (DigitalTwinModelState modelState in modelStateList)
                {
                    modelState.HandleIncomingTelemetry(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleMessageData(MessageData data)
        {
            List<DigitalTwinModelState> modelStateList =
                this.digitalTwinModelMgrCache.LookupDigitalTwinModelState(data);

            if (modelStateList != null)
            {
                foreach (DigitalTwinModelState modelState in modelStateList)
                {
                    modelState.HandleIncomingTelemetry(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSensorData(SensorData data)
        {
            List<DigitalTwinModelState> modelStateList =
                this.digitalTwinModelMgrCache.LookupDigitalTwinModelState(data);

            if (modelStateList != null)
            {
                foreach (DigitalTwinModelState modelState in modelStateList)
                {
                    modelState.HandleIncomingTelemetry(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSystemPerformanceData(SystemPerformanceData data)
        {
            List<DigitalTwinModelState> modelStateList =
                this.digitalTwinModelMgrCache.LookupDigitalTwinModelState(data);

            if (modelStateList != null)
            {
                foreach (DigitalTwinModelState modelState in modelStateList)
                {
                    modelState.HandleIncomingTelemetry(data);
                }
            }
        }

        /// <summary>
        /// Checks if we have an internally stored DT Model State instance using
        /// its model state key.
        /// </summary>
        /// <param name="modelStateKey"></param>
        /// <returns></returns>
        public bool HasDigitalTwinModelState(string modelStateKey)
        {
            return this.digitalTwinModelMgrCache.HasDigitalTwinModelState(modelStateKey);
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
        /// <param name="prefix"></param>
        public void SetResourcePrefix(string prefix)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                this.resourcePrefix = prefix;

                this.digitalTwinModelMgrCache.SetResourcePrefix(prefix);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public void SetSystemStatusEventListener(ISystemStatusEventListener listener)
        {
            if (listener != null)
            {
                this.eventListener = listener;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <returns></returns>
        public bool UpdateModelFilePaths(string modelFilePath)
        {
            return this.UpdateModelFilePaths(modelFilePath, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilePathSet"></param>
        /// <returns></returns>
        public bool UpdateModelFilePaths(HashSet<string> modelFilePathSet)
        {
            int counter = 0;
            int modelFileCount = 0;

            if (modelFilePathSet != null && modelFilePathSet.Count > 0)
            {
                modelFileCount = modelFilePathSet.Count;

                foreach (string modelFilePath in modelFilePathSet)
                {
                    if (this.UpdateModelFilePaths(modelFilePath))
                    {
                        counter++;
                    }
                }
            }

            Console.WriteLine($"Update model file paths: counter = {counter}; file count = {modelFileCount}");

            return (counter > 0 && modelFileCount == counter ? true : false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <param name="reloadModels"></param>
        /// <returns></returns>
        public bool UpdateModelFilePaths(string modelFilePath, bool reloadModels)
        {
            if (this.IsModelFilePathValid(modelFilePath))
            {
                if (!this.modelFilePaths.Contains(modelFilePath))
                {
                    this.modelFilePaths.Add(modelFilePath);
                } else
                {
                    Console.WriteLine($"DTDL model path already added: {modelFilePath}. Ignoring.");
                 }

                if (reloadModels)
                {
                    if (!this.BuildModelData())
                    {
                        Console.WriteLine("Failed to reload models. Check DTDL manager log output.");
                    }
                }

                return true;
            } else
            {
                Console.WriteLine($"Model file path is invalid: {modelFilePath}. Ignoring path update request.");
            }

            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtModelState"></param>
        /// <returns></returns>
        public bool UpdateModelState(DigitalTwinModelState dtModelState)
        {
            DigitalTwinModelState updatedState =
                this.digitalTwinModelMgrCache.UpdateDigitalTwinModelStateCache(dtModelState);

            return (updatedState != null);
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
                    // TODO: this call will send a command to the appropriate edge device
                    //  - validate the command - ensure it's legit and authorized
                    //  - map the command to the appropriate destination topic / resource
                    //  - notify the appropriate listener 
                }

                success = true;
            }

            return success;
        }

        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtModelState"></param>
        /// <param name="controllerID"></param>
        /// <param name="stateUpdateListener"></param>
        /// <returns></returns>
        private DigitalTwinModelState ConfigureAndStoreModelState(
            DigitalTwinModelState dtModelState,
            ModelNameUtil.DtmiControllerEnum controllerID,
            IDataContextEventListener stateUpdateListener)
        {
            // DigitalTwinModelState implements the Builder patter for most methods
            //  - set properties first, then build the requisite internal structures
            switch (controllerID)
            {
                case ModelNameUtil.DtmiControllerEnum.Custom:
                    string name = dtModelState.GetName();

                    dtModelState
                        .SetModelControllerID(controllerID)
                        .SetModelJson(this.digitalTwinModelMgrCache.GetDigitalTwinModelJson(name))
                        .SetVirtualAssetListener(stateUpdateListener)
                        .SetResourcePrefix(this.resourcePrefix);
                    break;

                default:
                    dtModelState
                        .SetModelControllerID(controllerID)
                        .SetModelJson(this.digitalTwinModelMgrCache.GetDigitalTwinModelJson(controllerID))
                        .SetVirtualAssetListener(stateUpdateListener)
                        .SetResourcePrefix(this.resourcePrefix);
                    break;
            }

            dtModelState
                .BuildDataSyncKey()
                .BuildModelSyncKey();

            dtModelState.BuildModelData();

            this.UpdateModelStateProperties(dtModelState);

            return this.digitalTwinModelMgrCache.UpdateDigitalTwinModelStateCache(dtModelState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <returns></returns>
        private bool IsModelFilePathValid(string modelFilePath)
        {
            if (!string.IsNullOrEmpty(modelFilePath))
            {
                if (!this.modelFilePaths.Contains(modelFilePath))
                {
                    if (Directory.Exists(modelFilePath))
                    {
                        Console.WriteLine($"Updating model file paths. New file path is good: {modelFilePath}");

                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to update model file paths. Requested model file path doesn't exist: {modelFilePath}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to update model file paths. File path already used and stored: {modelFilePath}");
                }

            }
            else
            {
                Console.WriteLine($"Failed to update model file paths. File path is null or empty: {modelFilePath}");
            }

            return false;
        }

        /// <summary>
        /// Unfortunately, this method results in each DTDL model being loaded twice
        /// Future optimizations will probably remove this redundancy.
        /// </summary>
        private bool LoadAndValidateDtdlModelInterfaceData()
        {
            int records = this.modelFilePaths.Count;
            int counter = 0;

            HashSet<string> invalidFileSet = new HashSet<string>();

            foreach (string dtdlFilePath in this.modelFilePaths)
            {
                try {
                    List<DtdlModelContainer> dtdlModelContainerList =
                        ModelParserUtil.LoadDtdlModelsFromPath(dtdlFilePath);

                    Console.WriteLine($" ### Loaded {dtdlModelContainerList.Count} entries from file {dtdlFilePath}");

                    foreach (DtdlModelContainer modelContainer in dtdlModelContainerList) {
                        Dictionary<string, DTInterfaceInfo> interfaceMap = modelContainer.GetEntityInterfaceMap();
                        Console.WriteLine($"Processing DTMI URI interface map. Count: {interfaceMap.Count}");

                        foreach (string dtmiUri in interfaceMap.Keys) {
                            Console.WriteLine($"Adding DTMI URI to cache: {dtmiUri}");
                            this.digitalTwinInterfaceCache.Add(dtmiUri, interfaceMap[dtmiUri]);

                            this.digitalTwinModelMgrCache.UpdateDigitalTwinJsonModelCache(
                                dtmiUri, modelContainer.GetModelJsonFile(), modelContainer.GetModelJsonData());

                            ++counter;
                        }
                    }

                    Console.WriteLine($"Successfully loaded DTDL model interfaces from path {dtdlFilePath}");
                } catch (Exception e) {
                    invalidFileSet.Add(dtdlFilePath);

                    Console.WriteLine($"Failed to load and process DTDL interfaces from {dtdlFilePath}. Exception: {e}.");
                }
            }

            Console.WriteLine($"Loaded and validated {counter} DTDL interfaces from {records} file(s).");

            if (invalidFileSet.Count > 0) {
                foreach (string invalidFile in invalidFileSet) {
                    Console.WriteLine($"Removing invalid DTDL file from file cache: {invalidFile}.");

                    this.modelFilePaths.Remove(invalidFile);
                }
            }

            this.hasSuccessfulDataLoad = (counter > 0 ? true : false);

            return (this.hasSuccessfulDataLoad);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelState"></param>
        private void UpdateModelStateProperties(DigitalTwinModelState modelState)
        {
            if (modelState != null)
            {
                DTInterfaceInfo interfaceInfo = this.digitalTwinInterfaceCache[modelState.GetModelID()];

                if (interfaceInfo != null)
                {
                    Console.WriteLine($"Found interface for model ID {modelState.GetModelID()}: {interfaceInfo.DisplayName}");

                    this.AddPropertiesToModelState(modelState, interfaceInfo);
                    this.AddCommandsToModelState(modelState, interfaceInfo);
                    this.AddTelemetriesToModelState(modelState, interfaceInfo);

                    StringBuilder sb = new StringBuilder();

                    sb.Append($"\n\n=====\n{modelState.GetModelJson()}\n\n=====\n");
                    sb.Append(
                        $"\nLoaded interface for model:" +
                        $"\n -> Model ID: {modelState.GetModelID()}" +
                        $"\n -> Data Sync Key: {modelState.GetDataSyncKey()}" +
                        $"\n -> Model Sync Key: {modelState.GetModelSyncKeyString()}");

                    Console.WriteLine(sb.ToString());
                    this.eventListener?.LogDebugMessage(sb.ToString());
                }
                else
                {
                    Console.WriteLine($"No interface for model ID {modelState.GetModelID()}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="interfaceInfo"></param>
        private void AddCommandsToModelState(DigitalTwinModelState modelState, DTInterfaceInfo interfaceInfo)
        {
            IReadOnlyDictionary<string, DTCommandInfo> commandEntries = interfaceInfo.Commands;

            foreach (var commandEntry in commandEntries)
            {
                DigitalTwinProperty command = new DigitalTwinProperty(commandEntry.Key);
                DTCommandInfo dtdlCommand = commandEntry.Value;

                command.SetDisplayName(dtdlCommand.Name);
                command.SetDescription(dtdlCommand.Description.ToString());
                command.SetDetail(dtdlCommand.ToString());
                command.SetAsEnabled(true);
                command.SetAsCommand(true);
                command.SetAsWriteable(false);

                command.SetRequestType(ModelParserUtil.GetPropertyType(dtdlCommand.Request.Schema.EntityKind));
                command.SetResponseType(ModelParserUtil.GetPropertyType(dtdlCommand.Response.Schema.EntityKind));
                command.SetPropertyType(command.GetRequestType());

                modelState.AddModelProperty(command);

                // debugging
                StringBuilder sbProp = new StringBuilder();

                sbProp.Append(command.ToString());
                sbProp.Append('\n').Append("Command Entity Kind: ").Append(dtdlCommand.EntityKind);
                sbProp.Append('\n').Append("Command Class ID: ").Append(dtdlCommand.ClassId);
                sbProp.Append('\n').Append("Command Request Entity Kind: ").Append(dtdlCommand.Request.Schema.EntityKind);
                sbProp.Append('\n').Append("Command Request Class ID: ").Append(dtdlCommand.Request.Schema.ClassId);
                sbProp.Append('\n').Append("Command Response Entity Kind: ").Append(dtdlCommand.Response.Schema.EntityKind);
                sbProp.Append('\n').Append("Command Response Class ID: ").Append(dtdlCommand.Response.Schema.ClassId);

                Console.WriteLine($"Adding command to state {sbProp.ToString()}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="interfaceInfo"></param>
        private void AddPropertiesToModelState(DigitalTwinModelState modelState, DTInterfaceInfo interfaceInfo)
        {
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

                prop.SetPropertyType(ModelParserUtil.GetPropertyType(dtdlProp.Schema.EntityKind));

                modelState.AddModelProperty(prop);

                // debugging
                StringBuilder sbProp = new StringBuilder();

                sbProp.Append(prop.ToString());
                sbProp.Append('\n').Append("Property Entity Kind: ").Append(dtdlProp.EntityKind);
                sbProp.Append('\n').Append("Property Class ID: ").Append(dtdlProp.ClassId);
                sbProp.Append('\n').Append("Property Schema Entity Kind: ").Append(dtdlProp.Schema.EntityKind);
                sbProp.Append('\n').Append("Property Schema Class ID: ").Append(dtdlProp.Schema.ClassId);

                Console.WriteLine($"Adding property to state {sbProp.ToString()}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="interfaceInfo"></param>
        private void AddTelemetriesToModelState(DigitalTwinModelState modelState, DTInterfaceInfo interfaceInfo)
        {
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
                telemetry.SetAsWriteable(false);

                telemetry.SetPropertyType(ModelParserUtil.GetPropertyType(dtdlTelemetry.Schema.EntityKind));

                modelState.AddModelProperty(telemetry);

                // debugging
                StringBuilder sbProp = new StringBuilder();

                sbProp.Append(telemetry.ToString());
                sbProp.Append('\n').Append("Telemetry Entity Kind: ").Append(dtdlTelemetry.EntityKind);
                sbProp.Append('\n').Append("Telemetry Class ID: ").Append(dtdlTelemetry.ClassId);
                sbProp.Append('\n').Append("Telemetry Schema Entity Kind: ").Append(dtdlTelemetry.Schema.EntityKind);
                sbProp.Append('\n').Append("Telemetry Schema Class ID: ").Append(dtdlTelemetry.Schema.ClassId);

                Console.WriteLine($"Adding telemetry to state {sbProp.ToString()}");
            }

            StringBuilder sb = new StringBuilder();

            sb.Append($"\n\n=====\n{modelState.GetModelJson()}\n\n=====\n");
            sb.Append(
                $"\nLoaded interface for model:" +
                $"\n -> Model ID: {modelState.GetModelID()}" +
                $"\n -> Data Sync Key: {modelState.GetDataSyncKey()}" +
                $"\n -> Model Sync Key: {modelState.GetModelSyncKeyString()}");

            Console.WriteLine(sb.ToString());
            this.eventListener?.LogDebugMessage(sb.ToString());
        }

    }
}
