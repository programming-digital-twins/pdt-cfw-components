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
using System.Linq;
using System.Runtime.CompilerServices;
using DTDLParser;
using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Model;

namespace LabBenchStudios.Pdt.Unity.Common
{
    /**
     * This class handles the registration of various event listeners and
     * the distribution of incoming events (of various types) to all
     * registered event listeners.
     * 
     * It is designed to be instanced once by the system manager, and then
     * accessed via the Singleton-like 'GetInstance()' methods.
     * 
     * It is NOT designed to be used across scenes (yet).
     * 
     */
    public class EventProcessor : ISystemStatusEventListener
    {
        private static string _GUID = null;
        private static bool _IS_TERMINATED = false;

        private static System.Object _LOCK_OBJ = new System.Object();

        private static EventProcessor _INSTANCE = CreateInstance();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static EventProcessor CreateInstance()
        {
            lock (_LOCK_OBJ)
            {
                if (_IS_TERMINATED)
                {
                    return null;
                }

                if (_INSTANCE == null)
                {
                    _GUID = System.Guid.NewGuid().ToString();

                    Console.WriteLine("EventProcess instance created.");

                    return new EventProcessor();
                }
                else
                {
                    return _INSTANCE;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static EventProcessor GetInstance()
        {
            return _INSTANCE;
        }


        // private member vars

        private IRemoteStateProcessor remoteStateProcessor = null;

        private List<IDataContextEventListener> dataContextEventListenerList = null;
        private List<ISystemStatusEventListener> systemStatusEventListenerList = null;

        private DigitalTwinModelManager digitalTwinModelManager = null;

        //private Dictionary<string, DigitalTwinModelState> digitalTwinStateTable = null;
        private Dictionary<string, ConnectionStateData> connectedStateTable = null;

        private HashSet<string> knownDeviceIDSet = null;
        private HashSet<string> testDeviceIDSet = null;
        private HashSet<string> dataSyncKeySet = null;

        // constructors

        /// <summary>
        /// 
        /// </summary>
        private EventProcessor()
        {
            this.digitalTwinModelManager = new DigitalTwinModelManager();

            this.dataContextEventListenerList = new List<IDataContextEventListener>();
            this.systemStatusEventListenerList = new List<ISystemStatusEventListener>();

            //this.digitalTwinStateTable = new Dictionary<string, DigitalTwinModelState>();
            this.connectedStateTable = new Dictionary<string, ConnectionStateData>();

            // telemetry keys
            this.dataSyncKeySet = new HashSet<string>();

            // use these
            this.knownDeviceIDSet = new HashSet<string>();
            this.knownDeviceIDSet.Add(ConfigConst.PRODUCT_NAME);

            // ignore these
            this.testDeviceIDSet = new HashSet<string>();
            this.testDeviceIDSet.Add("UUID");
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void ClearAllListeners()
        {
            this.dataContextEventListenerList.Clear();
            this.systemStatusEventListenerList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetGuid()
        {
            return _GUID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllKnownDeviceIDs()
        {
            return this.knownDeviceIDSet.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public ConnectionStateData GetConnectionState(string deviceID)
        {
            if (this.connectedStateTable.ContainsKey(deviceID))
            {
                return this.connectedStateTable[deviceID];
            }
            else
            {
                return null;
            }
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
        public bool LoadDigitalTwinModels()
        {
            return this.LoadDigitalTwinModels(ModelNameUtil.DEFAULT_MODEL_FILE_PATH);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <returns></returns>
        public bool LoadDigitalTwinModels(string modelFilePath)
        {
            if (this.digitalTwinModelManager != null)
            {
                if (this.digitalTwinModelManager.SetModelFilePath(modelFilePath))
                {
                    return this.digitalTwinModelManager.BuildModelData();
                }
            }

            Console.WriteLine($"Failed to (re)load Digital Twin models from path {modelFilePath}");

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtModelState"></param>
        public void RegisterDigitalTwin(DigitalTwinModelState dtModelState)
        {
            if (dtModelState != null)
            {
                string modelID = dtModelState.GetModelID();

                //if (!this.digitalTwinStateTable.ContainsKey(modelID))
                //{
                //    this.digitalTwinStateTable.Add(dtModelState.GetModelID(), dtModelState);
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterListener(IDataContextEventListener listener)
        {
            if (listener != null)
            {
                this.dataContextEventListenerList.Add(listener);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterListener(ISystemStatusEventListener listener)
        {
            if (listener != null)
            {
                this.systemStatusEventListenerList.Add(listener);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdProcessor"></param>
        public void SetRemoteCommandProcessor(IRemoteStateProcessor cmdProcessor)
        {
            // can only be set once - it's expected the SystemManager will
            // invoke this once after retrieving the EventProcessor Singleton
            if (this.remoteStateProcessor == null)
            {
                this.remoteStateProcessor = cmdProcessor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void LogDebugMessage(string message)
        {
            if (! string.IsNullOrEmpty(message))
            {
                if (this.systemStatusEventListenerList.Count > 0)
                {
                    foreach (var listener in this.systemStatusEventListenerList)
                    {
                        listener.LogDebugMessage(message);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void LogWarningMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (this.systemStatusEventListenerList.Count > 0)
                {
                    foreach (var listener in this.systemStatusEventListenerList)
                    {
                        listener.LogWarningMessage(message);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void LogErrorMessage(string message, Exception ex)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (this.systemStatusEventListenerList.Count > 0)
                {
                    foreach (var listener in this.systemStatusEventListenerList)
                    {
                        listener.LogErrorMessage(message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Update the digital twin's telemetry from the incoming
        /// IotDataContext - this will use the typeID to lookup
        /// the appropriate DTMI and associated DigitalTwinModelState
        /// and invoke its HandleIncomingTelemetry method
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemDataReceived(ActuatorData data)
        {
            if (data != null)
            {
                this.UpdateInternalState(data);

                //string dtmi = ModelNameUtil.GetModelID(data.GetTypeID());

                //if (this.digitalTwinStateTable.ContainsKey(dtmi))
                //{
                //    DigitalTwinModelState dtModelState = this.digitalTwinStateTable[dtmi];
                //    dtModelState.HandleIncomingTelemetry(data);
                //}

                if (this.dataContextEventListenerList.Count > 0)
                {
                    foreach (var listener in this.dataContextEventListenerList)
                    {
                        listener.HandleActuatorData(data);
                    }
                }
            }
        }

        /// <summary>
        /// Update the digital twin's telemetry from the incoming
        /// IotDataContext - this will use the typeID to lookup
        /// the appropriate DTMI and associated DigitalTwinModelState
        /// and invoke its HandleIncomingTelemetry method
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemDataReceived(ConnectionStateData data)
        {
            if (data != null)
            {
                this.UpdateInternalState(data);

                //string dtmi = ModelNameUtil.GetModelID(data.GetTypeID());

                //if (this.digitalTwinStateTable.ContainsKey(dtmi))
                //{
                //    DigitalTwinModelState dtModelState = this.digitalTwinStateTable[dtmi];
                //    dtModelState.HandleIncomingTelemetry(data);
                //}

                if (this.systemStatusEventListenerList.Count > 0)
                {
                    foreach (var listener in this.systemStatusEventListenerList)
                    {
                        listener.OnMessagingSystemDataReceived(data);
                    }
                }
            }
        }

        /// <summary>
        /// Update the digital twin's telemetry from the incoming
        /// IotDataContext - this will use the typeID to lookup
        /// the appropriate DTMI and associated DigitalTwinModelState
        /// and invoke its HandleIncomingTelemetry method
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemDataReceived(SensorData data)
        {
            if (data != null)
            {
                this.UpdateInternalState(data);

                //string dtmi = ModelNameUtil.GetModelID(data.GetTypeID());

                //if (this.digitalTwinStateTable.ContainsKey(dtmi))
                //{
                //    DigitalTwinModelState dtModelState = this.digitalTwinStateTable[dtmi];
                //    dtModelState.HandleIncomingTelemetry(data);
                //}

                if (this.dataContextEventListenerList.Count > 0)
                {
                    foreach (var listener in this.dataContextEventListenerList)
                    {
                        listener.HandleSensorData(data);
                    }
                }
            }
        }

        /// <summary>
        /// Update the digital twin's telemetry from the incoming
        /// IotDataContext - this will use the typeID to lookup
        /// the appropriate DTMI and associated DigitalTwinModelState
        /// and invoke its HandleIncomingTelemetry method
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemDataReceived(SystemPerformanceData data)
        {
            if (data != null)
            {
                this.UpdateInternalState(data);

                //string dtmi = ModelNameUtil.GetModelID(data.GetTypeID());

                //if (this.digitalTwinStateTable.ContainsKey(dtmi))
                //{
                //    DigitalTwinModelState dtModelState = this.digitalTwinStateTable[dtmi];
                //    dtModelState.HandleIncomingTelemetry(data);
                //}

                if (this.dataContextEventListenerList.Count > 0)
                {
                    foreach (var listener in this.dataContextEventListenerList)
                    {
                        listener.HandleSystemPerformanceData(data);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemDataSent(ConnectionStateData data)
        {
            if (data != null)
            {
                if (this.systemStatusEventListenerList.Count > 0)
                {
                    foreach (var listener in this.systemStatusEventListenerList)
                    {
                        listener.OnMessagingSystemDataSent(data);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemStatusUpdate(ConnectionStateData data)
        {
            if (data != null)
            {
                this.UpdateInternalState(data);

                if (this.systemStatusEventListenerList.Count > 0)
                {
                    foreach (var listener in this.systemStatusEventListenerList)
                    {
                        listener.OnMessagingSystemStatusUpdate(data);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnModelUpdateEvent()
        {
            if (this.systemStatusEventListenerList.Count > 0)
            {
                foreach (var listener in this.systemStatusEventListenerList)
                {
                    listener.OnModelUpdateEvent();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public bool ProcessStateUpdateToPhysicalThing(ResourceNameContainer resource)
        {
            if (this.remoteStateProcessor != null)
            {
                return this.remoteStateProcessor.SendStateUpdateToPhysicalThing(resource);
            }
            else
            {
                Console.WriteLine(
                    $"No composite remote command processor registered. Ignoring request: {resource}");
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void ProcessLiveDataFeedEngageRequest(bool enable)
        {
            if (this.remoteStateProcessor != null)
            {
                this.remoteStateProcessor.EnableLiveDataFeed(enable);
            }
            else
            {
                Console.WriteLine(
                    $"No composite remote command processor registered. Ignoring live data feed engagement request.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void ProcessSimulatedDataFeedEngageRequest(bool enable)
        {
            if (this.remoteStateProcessor != null)
            {
                this.remoteStateProcessor.EnableSimulatedDataFeed(enable);
            }
            else
            {
                Console.WriteLine(
                    $"No composite remote command processor registered. Ignoring simulated data feed engagement request.");
            }
        }


        // private methods

        /// <summary>
        /// Updates the internal caches that contain information such
        /// as the current list of device ID's (connected devices),
        /// the connection status for each, and the telemetry keys
        /// (generated) for mapping incoming data unique ID's to any
        /// registered Digital Twins.
        /// </summary>
        /// <param name="data"></param>
        private void UpdateInternalState(IotDataContext data)
        {
            // update internal caches
            this.UpdateDeviceIDSet(data);
            this.UpdateConnectionStateCache(data);
            this.UpdateDataSyncKeyCache(data);

            // notify DT model manager of the data update
            this.digitalTwinModelManager.HandleIncomingTelemetry(data);
        }

        /// <summary>
        /// This simply adds the contained Device ID within IotDataContext
        /// to an internal set of known valid Device ID's.
        /// </summary>
        /// <param name="data"></param>
        private void UpdateDeviceIDSet(IotDataContext data)
        {
            if (data != null)
            {
                string deviceID = data.GetDeviceID();

                // check bogus list first - we only want to add legit ID's
                if (!this.testDeviceIDSet.Contains(deviceID))
                {
                    // the Set structure will ensure only unique ID's
                    this.knownDeviceIDSet.Add(deviceID);
                }
            }
        }

        /// <summary>
        /// This works well for maintaining a cache of Device ID's that are
        /// clearly sending messages into the EventProcessor; however, if these
        /// devices STOP sending data (or disconnect), there's no way for the
        /// EventProcessor to know about it, and it will sustain the table
        /// representing 'is connected' Device ID's.
        /// </summary>
        /// <param name="data"></param>
        private void UpdateConnectionStateCache(IotDataContext data)
        {
            if (data != null)
            {
                string deviceID = data.GetDeviceID();

                if (this.testDeviceIDSet.Contains(deviceID))
                {
                    return;
                }

                ConnectionStateData connStateData = null;

                if (!this.connectedStateTable.ContainsKey(deviceID))
                {
                    connStateData = new ConnectionStateData();

                    this.connectedStateTable.Add(deviceID, connStateData);
                }

                connStateData = this.connectedStateTable[deviceID];

                if (connStateData != null)
                {
                    if (data is ConnectionStateData)
                    {
                        connStateData.UpdateData((ConnectionStateData) data);
                    }
                    else
                    {
                        connStateData.UpdateData(data);
                        connStateData.SetIsClientConnectedFlag(true);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the internal telemetry key name set with this incoming
        /// message's telemetry key if it doesn't already exist in the cache.
        /// </summary>
        /// <param name="data"></param>
        private void UpdateDataSyncKeyCache(IotDataContext data)
        {
            if (data != null)
            {
                string key = ModelNameUtil.GenerateDataSyncKey(data);

                if (! this.dataSyncKeySet.Contains(key))
                {
                    this.dataSyncKeySet.Add(key);
                }
            }
        }

    }

}
