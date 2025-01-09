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

using Microsoft.Extensions.AI;

using OllamaSharp;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

using System.Threading.Tasks;
using System.Text;

namespace LabBenchStudios.Pdt.Connection
{
    public class AiChatClientConnector : IPredictionModelConnector
    {
        // static consts
        public const int OLLAMA_SERVER_PORT = 11434;
        public const Int32 DEFAULT_TIMEOUT_MILLIS = 30000;
        public const string OLLAMA_DEFAULT_MODEL = "llama3.2:latest";

        // private member vars

        private string protocol = "http";
        private string serverHost = "localhost";
        private int serverPort = OLLAMA_SERVER_PORT;

        private string serverUri = "http://localhost:11434";
        private string model = null;
        private string cacheFileName = null;

        private string sessionID = "OllamaPdtClientSession";
        private string topicPrefix = ConfigConst.PRODUCT_NAME;

        private bool isEncrypted = false;
        private bool ignoreResponses = false;

        private IPredictionModelListener predictionListener = null;
        private ISystemStatusEventListener eventListener = null;
        private IDataContextEventListener responseListener = null;

        private ConnectionStateData connStateData = null;

        private IOllamaApiClient chatClient = null;
        //private IChatClient chatClient = null;


        // constructors

        /// <summary>
        /// 
        /// </summary>
        public AiChatClientConnector() : this(null, null)
        {
            // nothing to do - this simply calls the other constructor with defaults
        }

        /// <summary>
        /// 
        /// </summary>
        public AiChatClientConnector(string sessionID, string uri)
        {
            if (!string.IsNullOrEmpty(sessionID))
            {
                this.sessionID = sessionID;
            }

            if (!string.IsNullOrEmpty(uri))
            {
                this.serverUri = uri;
            }

            this.InitAiChatClient();
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        public void ClearSessionState()
        {
            // TODO: handle this
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSessionID()
        {
            return this.sessionID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCurrentModel()
        {
            return this.model;
        }

        /// <summary>
        /// NOTE: This call will block for up to 30 seconds.
        /// </summary>
        /// <returns></returns>
        public List<string> GetRegisteredModels()
        {
            this.eventListener?.LogDebugMessage($"Getting registered models: {this.serverUri}.");

            //_ = this.HandleGetModels();

            var task = this.HandleGetModels();
            task.Wait(DEFAULT_TIMEOUT_MILLIS);

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetServerUri()
        {
            return this.serverUri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsIncomingMessageProcessingPaused()
        {
            return this.ignoreResponses;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsServerReachable()
        {
            // TODO: handle this

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void PauseIncomingMessages()
        {
            this.ignoreResponses = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SetModel(string model)
        {
            if (!string.IsNullOrEmpty (model))
            {
                try
                {
                    this.chatClient.SelectedModel = model;
                    this.model = model;

                    Console.WriteLine($"Chat model now set: {this.model}");

                    return true;
                } catch (Exception e)
                {
                    Console.WriteLine($"Failed to set chat client model: {this.model}. {e.Message}");
                }
            }

            return false;
        }

        /// <summary>
        /// NOTE: This call will block for up to 30 seconds.
        /// </summary>
        /// <param name="queryMsg"></param>
        public bool SendRequest(string queryMsg)
        {
            if (string.IsNullOrEmpty(this.model))
            {
                this.SetModel(OLLAMA_DEFAULT_MODEL);
            }

            //_ = this.HandlePredictionEngineQuery(queryMsg);

            var task = this.HandlePredictionEngineQuery(queryMsg);
            task.Wait(DEFAULT_TIMEOUT_MILLIS);

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public void SetPredictionModelListener(IPredictionModelListener listener)
        {
            if (listener != null)
            {
                this.predictionListener = listener;
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
                this.eventListener?.OnMessagingSystemStatusUpdate(GetConnectionStateCopy());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public void SetQueryResponseListener(IDataContextEventListener listener)
        {
            if (listener != null)
            {
                this.responseListener = listener;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnpauseIncomingMessages()
        {
            this.ignoreResponses = false;
        }


        // private

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task HandleGetModels()
        {
            if (this.chatClient != null)
            {
                Console.WriteLine($"Retrieving prediction engine's models: {this.serverUri}");

                this.eventListener?.LogDebugMessage("Retrieving AI models...");

                var models = await this.chatClient.ListLocalModelsAsync();

                if (models != null)
                {
                    List<string> modelList = new List<string>();

                    foreach (var model in models)
                    {
                        modelList.Add(model.Name);
                    }

                    string msg = $"Retrieved AI models: {modelList.Count}";

                    Console.WriteLine(msg);
                    
                    this.eventListener?.LogDebugMessage(msg);

                    var modelListContainer = new ModelListContainer(this.serverUri, modelList);
                    this.predictionListener?.OnModelListRetrieved(modelListContainer);
                } else
                {
                    Console.WriteLine($"No models retrieved from prediction engine: {this.serverUri}");
                }
            } else
            {
                // TODO: log msg
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMsg"></param>
        private async Task HandlePredictionEngineQuery(string requestMsg)
        {
            if (this.chatClient != null)
            {
                Console.WriteLine($"Sending query: {requestMsg}");
                Console.WriteLine($"Server info: {this.chatClient.Uri}, {this.chatClient.SelectedModel}");
                Console.WriteLine($"Is running? {this.chatClient.IsRunningAsync()}");

                this.eventListener?.LogDebugMessage($"Sending query to prediction engine: {this.serverUri}");

                StringBuilder builder = new StringBuilder();

                await foreach (var stream in this.chatClient.GenerateAsync(requestMsg))
                {
                    builder.Append(stream.Response);
                }

                string response = builder.ToString();

                Console.WriteLine(requestMsg);
                Console.WriteLine(response);

                this.eventListener?.LogDebugMessage($"Received query response from prediction engine: {this.serverUri}. Len: {response}");
                
                var queryResponseContainer = new QueryResponseContainer(this.sessionID, this.serverUri, response);
                this.predictionListener?.OnQueryResponseReceived(queryResponseContainer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void InitAiChatClient()
        {
            this.connStateData =
                new ConnectionStateData(
                    this.topicPrefix, ConfigConst.PRODUCT_NAME, this.serverHost, this.serverPort);

            this.connStateData.SetMessage($"Session ID: {this.sessionID}. Server URI: {this.serverHost}.");

            this.chatClient = new OllamaApiClient(this.serverUri);

            string msg = $"Initialized chat client: {this.sessionID} - {this.serverUri}.";

            Console.WriteLine(msg);

            this.eventListener?.LogDebugMessage(msg);
        }

    }

}
