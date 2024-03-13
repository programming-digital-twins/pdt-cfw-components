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
    public class DigitalTwinModelManager : IDataContextEventListener
    {
        private string modelFilePath = ModelNameUtil.DEFAULT_MODEL_FILE_PATH;

        private bool hasSuccessfulDataLoad = false;

        private DigitalTwinModelManagerCache digitalTwinModelMgrCache = null;

        // this contains all the DT model parsed instances
        // this is indexed by the DTDLParser DTMI absolute URI (string)
        private IReadOnlyDictionary<string, DTInterfaceInfo> digitalTwinInterfaceCache;

        // useful for passing event messages and debugging
        private ISystemStatusEventListener eventListener = null;

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

            this.digitalTwinModelMgrCache = new DigitalTwinModelManagerCache();
        }

        // public methods

        /// <summary>
        /// This will trigger a request to model manager to load
        /// (or reload) this state objects respective model.
        /// </summary>
        /// <returns></returns>
        public bool BuildModelData()
        {
            if (!string.IsNullOrEmpty(this.modelFilePath) && Directory.Exists(modelFilePath))
            {
                bool areInterfacesLoaded = this.LoadAndValidateDtdlModelInterfaceData();
                bool areJsonFilesLoaded = this.LoadAndValidateDtdlModelJsonData();

                // both should either succeed or fail
                return (areInterfacesLoaded && areJsonFilesLoaded);
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

            return this.ConfigureAndStoreModelState(dtModelState, controllerID, stateUpdateListener);
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
            var dtModelState = new DigitalTwinModelState(deviceID, deviceID, locationID);

            return this.ConfigureAndStoreModelState(dtModelState, controllerID, stateUpdateListener);
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
        public bool SetModelFilePath(string modelFilePath)
        {
            return this.SetModelFilePath(modelFilePath, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <param name="reloadModels"></param>
        /// <returns></returns>
        public bool SetModelFilePath(string modelFilePath, bool reloadModels)
        {
            if (!string.IsNullOrEmpty(modelFilePath) && Directory.Exists(modelFilePath))
            {
                Console.WriteLine($"Setting model file path. File path is good: {modelFilePath}");
                this.modelFilePath = modelFilePath;

                if (reloadModels)
                {
                    this.BuildModelData();
                }

                return true;
            }

            Console.WriteLine($"Failed to set model file path. File path is invalid: {modelFilePath}");
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
            dtModelState
                .SetModelControllerID(controllerID)
                .SetModelJson(this.digitalTwinModelMgrCache.GetDigitalTwinModelJson(controllerID))
                .SetVirtualAssetListener(stateUpdateListener);

            dtModelState
                .BuildDataSyncKey()
                .BuildModelSyncKey();

            dtModelState.BuildModelData();

            this.UpdateModelStateProperties(dtModelState);

            return this.digitalTwinModelMgrCache.UpdateDigitalTwinModelStateCache(dtModelState);
        }

        /// <summary>
        /// Unfortunately, this method results in each DTDL model being loaded twice
        /// Future optimizations will probably remove this redundancy.
        /// </summary>
        private bool LoadAndValidateDtdlModelJsonData()
        {
            if (this.digitalTwinModelMgrCache.LoadDigitalTwinJsonModels(this.modelFilePath))
            {
                Console.WriteLine($"Successfully loaded DTDL JSON data from path {this.modelFilePath}");

                return true;
            }
            else
            {
                Console.WriteLine($"Failed to load DTDL JSON data from path {this.modelFilePath}");
            }

            return false;
        }

        /// <summary>
        /// Unfortunately, this method results in each DTDL model being loaded twice
        /// Future optimizations will probably remove this redundancy.
        /// </summary>
        private bool LoadAndValidateDtdlModelInterfaceData()
        {
            // update DTDL object cache
            this.digitalTwinInterfaceCache = ModelParserUtil.LoadAllDtdlInterfaces(this.modelFilePath);

            if (this.digitalTwinInterfaceCache != null)
            {
                Console.WriteLine($"Successfully loaded DTDL model interfaces from path {this.modelFilePath}");
                this.hasSuccessfulDataLoad = true;

                return true;
            }
            else
            {
                Console.WriteLine($"Failed to load DTDL model interfaces from path {this.modelFilePath}");
            }

            return false;
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
                        telemetry.SetAsWriteable(false);

                        Console.WriteLine($"Adding telemetry to state {telemetry}");
                        modelState.AddModelProperty(telemetry);
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
                else
                {
                    Console.WriteLine($"No interface for model ID {modelState.GetModelID()}");
                }
            }
        }

    }
}
