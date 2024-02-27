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
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Connection
{
    public class MqttClientManagedConnector : IPubSubConnector
    {
        // static consts


        // private member vars

        private string serverHost = "localhost";
        private int serverPort = 1883;
        private string clientID = "UnityDTClient";
        private string productName = ConfigConst.PRODUCT_NAME;
        private bool isEncrypted = false;
        private bool isConnected = false;
        private bool areIncomingMessagesPaused = false;

        private IManagedMqttClient mqttClient = null;

        private ISystemStatusEventListener eventListener = null;

        private ConnectionStateData connStateData = null;

        // constructors

        public MqttClientManagedConnector(
            string serverHost, int serverPort, string clientID,
            ISystemStatusEventListener eventListener) :
            this(serverHost, serverPort, clientID, ConfigConst.PRODUCT_NAME, eventListener)
        {
            // nothing to do - this simply calls the other constructor with defaults
        }

        public MqttClientManagedConnector(
            string serverHost, int serverPort, string clientID,
            string productName, ISystemStatusEventListener eventListener)
        {
            if (serverHost != null)
            {
                this.serverHost = serverHost;
            }

            if (serverPort > 0 && serverPort < 65535)
            {
                this.serverPort = serverPort;
            }

            if (clientID != null && clientID.Length > 0)
            {
                this.clientID = clientID;
            }

            if (productName != null && productName.Length > 0)
            {
                this.productName = productName;
            }

            this.eventListener = eventListener;
            this.connStateData = new ConnectionStateData(this.productName, "UUID", this.serverHost, this.serverPort);

            this.connStateData.SetMessage("Server host: " + this.serverHost + ", Product name: " + this.productName);
            this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
        }

        // public methods

        public void ConnectClient()
        {
            if (this.mqttClient == null)
            {
                this.InitConnector();
            }

            if (this.mqttClient != null && !this.mqttClient.IsConnected)
            {
                this.connStateData.SetMessage("Connecting...");

                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());

                _ = this.HandleConnect();
            }
        }

        public void DisconnectClient()
        {
            if (this.mqttClient != null && this.mqttClient.IsConnected)
            {
                this.connStateData.SetMessage("Disconnecting...");

                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());

                _ = this.HandleDisconnect();
            }
        }

        public bool IsClientConnected()
        {
            return this.isConnected;
        }

        public void PublishMessage(ResourceNameContainer resource)
        {
            if (this.isConnected)
            {
                this.connStateData.SetMessage("Publishing...");

                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());

                _ = this.HandlePublish(resource);
            }
        }

        public void SubscribeToAllTopics()
        {
            this.SubscribeToTopic(null);
        }

        public void SubscribeToTopic(ResourceNameContainer resource)
        {
            if (this.mqttClient != null && this.isConnected)
            {
                this.connStateData.SetMessage("Subscribing...");

                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());

                _ = this.HandleSubscribe(resource);
            }
        }

        public void UnsubscribeFromTopic(ResourceNameContainer resource)
        {
            if (this.mqttClient != null && this.isConnected)
            {
                this.connStateData.SetMessage("Unsubscribing...");

                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
            }
        }

        public bool IsIncomingMessageProcessingPaused()
        {
            return this.areIncomingMessagesPaused;
        }

        public void PauseIncomingMessages()
        {
            this.areIncomingMessagesPaused = true;
        }

        public void UnpauseIncomingMessages()
        {
            this.areIncomingMessagesPaused = false;
        }


        // protected

        protected async Task HandleConnect()
        {
            if (this.mqttClient != null && ! this.mqttClient.IsConnected)
            {
                this.eventListener?.LogDebugMessage("MQTT client connecting...");
                this.connStateData.SetIsClientConnectingFlag(true);
                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());

                var reconnDelaySeconds = 50;

                var clientOptions =
                    new MqttClientOptionsBuilder().
                         WithClientId(this.clientID).WithTcpServer(this.serverHost, this.serverPort).Build();

                var connOptions =
                    new ManagedMqttClientOptionsBuilder().
                        WithAutoReconnectDelay(TimeSpan.FromSeconds(reconnDelaySeconds)).
                        WithClientOptions(clientOptions).Build();

                await this.mqttClient.StartAsync(connOptions);

                this.isConnected = true;
                this.connStateData.SetIsClientConnectedFlag(true);
                this.connStateData.SetStatusCode(ConfigConst.CONN_SUCCESS_STATUS_CODE);
                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
            }
            else
            {
                // TODO: log msg
            }
        }

        protected async Task HandleDisconnect()
        {
            if (this.mqttClient != null && this.mqttClient.IsConnected)
            {
                this.eventListener?.LogDebugMessage("MQTT client disconnecting...");

                this.connStateData.SetMessage("Disconnected");
                this.connStateData.SetIsClientDisconnectedFlag(true);
                this.connStateData.SetStatusCode(ConfigConst.DISCONN_SUCCESS_STATUS_CODE);
                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());

                await this.mqttClient.StopAsync();

                this.isConnected = false;
            }
            else
            {
                // TODO: log msg
            }
        }

        protected async Task HandlePublish(ResourceNameContainer resource)
        {
            MqttApplicationMessageBuilder msgBuilder = new MqttApplicationMessageBuilder();
            msgBuilder.WithTopic(resource.GetFullResourceName());

            IotDataContext dataContext = resource.DataContext;
            string jsonData = null;

            if (resource.IsActuationResource)
            {
                jsonData = DataUtil.ActuatorDataToJson((ActuatorData)dataContext);
            }
            else if (resource.IsConnStateResource)
            {
                jsonData = DataUtil.ConnectionStateDataToJson((ConnectionStateData)dataContext);
            }
            else if (resource.IsMessageResource)
            {
                jsonData = DataUtil.MessageDataToJson((MessageData)dataContext);
            }
            else if (resource.IsSensingResource)
            {
                jsonData = DataUtil.SensorDataToJson((SensorData)dataContext);
            }
            else if (resource.IsSystemResource)
            {
                jsonData = DataUtil.SystemPerformanceDataToJson((SystemPerformanceData)dataContext);
            }
            else
            {
                jsonData = DataUtil.IotDataContextToJson((IotDataContext)dataContext);
            }

            msgBuilder.WithPayload(jsonData);

            await mqttClient.EnqueueAsync(msgBuilder.Build());
        }

        protected async Task HandleSubscribe(ResourceNameContainer resource)
        {
            if (this.isConnected)
            {
                string topicName = null;

                if (resource == null)
                {
                    topicName = this.productName + "/#"; // e.g., PDT/#
                }
                else
                {
                    resource.ProductPrefix = this.productName;
                    topicName = resource.GetFullResourceName();
                }

                this.eventListener?.LogDebugMessage($"Subscribing to topic: {topicName}");

                MqttTopicFilter topicFilter =
                    new MqttTopicFilterBuilder().
                        WithTopic(topicName).Build();

                MqttClientSubscribeOptions subscribeOptions =
                    new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topicFilter).Build();

                ICollection<MqttTopicFilter> subscriptionList = new List<MqttTopicFilter>
                {
                    topicFilter
                };

                await mqttClient.SubscribeAsync(subscriptionList);
            }
            else
            {
                // TODO: log msg
            }
        }

        // private

        private void InitConnector()
        {
            if (this.mqttClient == null)
            {
                this.eventListener?.LogDebugMessage("MQTT client being created.");

                this.mqttClient = new MqttFactory().CreateManagedMqttClient();

                this.InitSubscriptionEvents();
                this.InitConnectionEvents();
            }
            else
            {
                this.eventListener?.LogDebugMessage("MQTT client already created. Ignoring.");
            }
        }

        private void InitConnectionEvents()
        {
            this.mqttClient.ConnectedAsync += e =>
            {
                try
                {
                    this.eventListener?.LogDebugMessage("Client Connected!");

                    this.isConnected = true;

                    _ = this.HandleSubscribe(null);
                }
                catch (Exception ex)
                {
                    // TODO: log msg
                }

                return Task.CompletedTask;
            };
        }

        private void InitSubscriptionEvents()
        {
            // add the event callback for subscriptions
            this.mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                try
                {
                    this.OnMessageReceived(e);
                }
                catch (Exception ex)
                {
                    // TODO: log msg
                }

                return Task.CompletedTask;
            };
        }

        private void OnMessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            if (! this.areIncomingMessagesPaused)
            {
                // TODO: parse incoming msg
                MqttApplicationMessage msg = args.ApplicationMessage;

                string topic = msg.Topic;
                string payload = msg.ConvertPayloadToString();

                this.eventListener?.LogDebugMessage($"Topic: {topic}. Message Received: {payload}");

                this.connStateData.IncreaseMessageInCount();
                this.connStateData.SetMessage(payload);

                var updatedConnState = this.GetConnectionStateCopy();

                this.HandleIncomingData(updatedConnState);
            }
        }
        
        private void HandleIncomingData(ConnectionStateData connStateData)
        {
            string jsonData = connStateData.GetMessage();

            this.eventListener?.LogDebugMessage("Raw JSON:\n" + jsonData);

            IotDataContext dataContext = DataUtil.JsonToIotDataContext(jsonData);

            this.eventListener?.LogWarningMessage("IotDataContext: " + dataContext.ToString());

            try
            {
                int categoryID = dataContext.GetDeviceCategory();

                if (categoryID == ConfigConst.ENV_TYPE_CATEGORY)
                {
                    SensorData data = DataUtil.JsonToSensorData(jsonData);

                    this.eventListener?.OnMessagingSystemDataReceived(data);
                }
                else if (categoryID == ConfigConst.SYSTEM_TYPE_CATEGORY)
                {
                    SystemPerformanceData data = DataUtil.JsonToSystemPerformanceData(jsonData);

                    this.eventListener?.OnMessagingSystemDataReceived(data);
                }
                else
                {
                    this.eventListener?.LogDebugMessage("Can't identify IotDataContext: " + categoryID);
                }
            }
            catch (Exception ex)
            {
                this.eventListener?.LogErrorMessage("Failed to convert payload to IotDataContext: ", ex);
            }

            this.eventListener?.OnMessagingSystemDataReceived(connStateData);
        }

        private ConnectionStateData GetConnectionStateCopy()
        {
            ConnectionStateData updatedConnStateData =
                new ConnectionStateData(
                    this.connStateData.GetName(),
                    this.connStateData.GetDeviceID(),
                    this.connStateData.GetHostName(),
                    this.connStateData.GetHostPort());

            updatedConnStateData.UpdateData(this.connStateData);

            return updatedConnStateData;
        }

        private void OnConnectSuccess(MqttClientConnectedEventArgs args)
        {
            this.isConnected = true;

            this.eventListener?.LogDebugMessage("MQTT client connection complete.");

            this.connStateData.SetMessage("Connected!");
            this.connStateData.SetIsClientConnectedFlag(true);
            this.connStateData.SetStatusCode(ConfigConst.CONN_SUCCESS_STATUS_CODE);

            this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
        }

        private void OnConnectFailure(ConnectingFailedEventArgs args)
        {
            this.isConnected = false;

            this.eventListener?.LogDebugMessage("MQTT client connection failed.");

            EventMessage msg = new EventMessage();
            msg.StatusCode = ConfigConst.CONN_FAILURE_STATUS_CODE;

            this.connStateData.SetMessage("Conn Failed!");
            this.connStateData.SetIsClientDisconnectedFlag(true);
            this.connStateData.SetStatusCode(ConfigConst.CONN_FAILURE_STATUS_CODE);

            this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
        }

        private void OnDisconnectSuccess(MqttClientDisconnectedEventArgs args)
        {
            this.isConnected = false;

            this.eventListener?.LogDebugMessage("MQTT client disconnect complete.");

            this.connStateData.SetMessage("Disconnected!");
            this.connStateData.SetIsClientDisconnectedFlag(true);
            this.connStateData.SetStatusCode(ConfigConst.DISCONN_SUCCESS_STATUS_CODE);

            this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
        }

    }
}
