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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Historian;
using LabBenchStudios.Pdt.Model;
using LabBenchStudios.Pdt.Prediction;

namespace LabBenchStudios.Pdt.Plexus
{
    /// <summary>
    /// This class serves as both factory and manager for created objects
    /// related to the stored DTDL models within its internal cache.
    /// It will store both the raw JSON and the DTInterfaceInfo for each
    /// DTMI absolute URI, using the latter as the key for each separate cache.
    /// </summary>
    public class SystemModelManager : IDataContextEventListener
    {
        // flag to check if default file paths should be included
        private bool useDefaultFilePaths = true;

        // useful for passing event messages and debugging
        private ISystemStatusEventListener eventListener = null;

        // the DTDL model manager
        DigitalTwinModelManager digitalTwinModelManager = null;

        // the type config mapping model manager
        ConfigTypeModelManager configTypeModelManager = null;

        // the data historian manager
        IDataHistorian dataHistorianManager = null;

        // the prediction system manager
        PredictionSystemManager predictionSystemManager = null;

        HashSet<string> coreDigitalTwinModelPathSet = null;
        HashSet<string> customDigitalTwinModelPathSet = null;
        HashSet<string> digitalTwinModelPathSet = null;
        HashSet<string> configTypeModelPathSet = null;
        HashSet<string> deviceIDSet = null;

        /// <summary>
        /// Default constructor. Uses the default model file path specified
        /// in ModelNameUtil.
        /// </summary>
        public SystemModelManager() : base()
        {
            this.digitalTwinModelManager = new DigitalTwinModelManager();
            this.configTypeModelManager = new ConfigTypeModelManager();
            this.dataHistorianManager = new DataHistorianManager();
            this.predictionSystemManager = new PredictionSystemManager();

            this.digitalTwinModelPathSet = new HashSet<string>();
            this.configTypeModelPathSet = new HashSet<string>();
            this.deviceIDSet = new HashSet<string>();
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void AddConfigTypeModelSearchPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (Directory.Exists(path))
                {
                    if (!this.configTypeModelPathSet.Contains(path))
                    {
                        this.configTypeModelPathSet.Add(path);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void AddDigitalTwinModelSearchPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (Directory.Exists(path))
                {
                    if (!this.digitalTwinModelPathSet.Contains(path))
                    {
                        this.digitalTwinModelPathSet.Add(path);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool BuildAllModels()
        {
            bool success = false;

            if (this.digitalTwinModelPathSet.Count > 0)
            {
                success = this.digitalTwinModelManager.UpdateModelFilePaths(this.digitalTwinModelPathSet);

                if (success)
                {
                    Console.WriteLine($"Successfully (re)built all digital twin models from existing file paths. DTMI URI's: {this.digitalTwinModelManager.GetAllDtmiValues()}");
                } else
                {
                    Console.WriteLine($"Failed to (re)build all digital twin models from existing file paths. Cached DTMI URI's: {this.digitalTwinModelManager.GetAllDtmiValues()}");

                    Exception e = new Exception();
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            } else
            {
                Console.WriteLine($"No paths added to digital twin model search. Ignoring. {new Exception().StackTrace}");
            }

            if (this.configTypeModelPathSet.Count > 0) {
                success = this.configTypeModelManager.UpdateConfigTypeFilePaths(this.configTypeModelPathSet);

                if (success)
                {
                    Console.WriteLine($"Successfully (re)built all digital twin models from existing file paths. DTMI URI's: {this.configTypeModelManager.GetLoadedAndMappedModelNames()}");
                } else
                {
                    Console.WriteLine($"Failed to (re)build all digital twin models from existing file paths. Cached DTMI URI's: {this.configTypeModelManager.GetLoadedAndMappedModelNames()}");

                    Exception e = new Exception();
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            } else
            {
                Console.WriteLine($"No paths added to config type model search. Ignoring. {new Exception().StackTrace}");
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="typeCategoryName"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public IotDataContext GenerateCustomDataContext(
            string deviceID, string locationID, string typeCategoryName, string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                Console.WriteLine("Can't generate custom data context. Model name is null / empty. Ignoring.");

                return null;
            }

            IotDataContext dataContext = ModelNameUtil.GenerateCustomDataContext(deviceID, locationID, typeCategoryName, modelName);

            ConfigTypeModelEntry modelEntry = configTypeModelManager.GetConfigEntryByTypeName(modelName);

            if (modelEntry != null)
            {
                dataContext.SetTypeCategoryID(modelEntry.GetTypeCategoryId());
                dataContext.SetTypeID(modelEntry.GetId());
            }

            return dataContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerID"></param>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public IotDataContext GenerateDataContext(
            ModelNameUtil.DtmiControllerEnum controllerID, string deviceID, string locationID, string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                typeName = controllerID.ToString();
            }

            IotDataContext dataContext = ModelNameUtil.GenerateDataContext(controllerID, deviceID, locationID, typeName);

            ConfigTypeModelEntry modelEntry = configTypeModelManager.GetConfigEntryByTypeName(typeName);

            if (modelEntry != null)
            {
                dataContext.SetTypeCategoryID(modelEntry.GetTypeCategoryId());
                dataContext.SetTypeID(modelEntry.GetId());
            }

            return dataContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConfigTypeModelManager GetConfigTypeModelManager()
        {
            return this.configTypeModelManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HashSet<string> GetAllRegisteredDeviceIDs()
        {
            return this.deviceIDSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDataHistorian GetDataHistorianManager()
        {
            return this.dataHistorianManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DigitalTwinModelManager GetDigitalTwinModelManager()
        {
            return this.digitalTwinModelManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PredictionSystemManager GetPredictionSystemManager()
        {
            return this.predictionSystemManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool HandleIncomingTelemetry(IotDataContext data)
        {
            this.RegisterDeviceID(data);

            return this.digitalTwinModelManager.HandleIncomingTelemetry(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleActuatorData(ActuatorData data)
        {
            this.RegisterDeviceID(data);

            this.digitalTwinModelManager.HandleActuatorData(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleConnectionStateData(ConnectionStateData data)
        {
            this.RegisterDeviceID(data);

            this.digitalTwinModelManager.HandleConnectionStateData(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleMessageData(MessageData data)
        {
            this.RegisterDeviceID(data);

            this.digitalTwinModelManager.HandleMessageData(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSensorData(SensorData data)
        {
            this.RegisterDeviceID(data);

            this.digitalTwinModelManager.HandleSensorData(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSystemPerformanceData(SystemPerformanceData data)
        {
            this.RegisterDeviceID(data);

            this.digitalTwinModelManager.HandleSystemPerformanceData(data);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadAndUpdateAllModels()
        {
            // STEPS:
            //  1) load config type mapping models
            //  2) load digital twin models
            //  3) sync the model maps
            this.configTypeModelManager.UpdateConfigTypeFilePaths(configTypeModelPathSet);
            this.digitalTwinModelManager.UpdateModelFilePaths(digitalTwinModelPathSet);
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

                this.digitalTwinModelManager.SetSystemStatusEventListener(listener);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public bool UpdateRemoteSystemState(IotDataContext dataContext)
        {
            this.RegisterDeviceID(dataContext);

            return this.digitalTwinModelManager.UpdateRemoteSystemState(dataContext);
        }

        // private methods

        /// <summary>
        /// Stores the data context's device ID in the local device ID set
        /// if not already stored.
        /// </summary>
        /// <param name="dataContext"></param>
        private void RegisterDeviceID(IotDataContext dataContext)
        {
            string deviceID = dataContext.GetDeviceID();

            if (!this.deviceIDSet.Contains(deviceID))
            {
                this.deviceIDSet.Add(deviceID);
            }
        }

    }

}
