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
using LabBenchStudios.Pdt.Connection;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Historian;
using LabBenchStudios.Pdt.Model;
using LabBenchStudios.Pdt.Util;

namespace LabBenchStudios.Pdt.Prediction
{
    /// <summary>
    /// This class serves as both factory and manager for created objects
    /// related to the stored DTDL models within its internal cache.
    /// It will store both the raw JSON and the DTInterfaceInfo for each
    /// DTMI absolute URI, using the latter as the key for each separate cache.
    /// </summary>
    public class PredictionSystemManager : IDataContextEventListener, IPredictionModelListener
    {
        // flag to check if default file paths should be included
        private bool useDefaultFilePaths = true;

        // useful for passing event messages and debugging
        private ISystemStatusEventListener eventListener = null;

        /// <summary>
        /// Key: session ID
        /// Value: prediction system URI
        /// </summary>
        private Dictionary<string, string> sessionToConnectorMap = null;

        /// <summary>
        /// Key: prediction system URI
        /// Value: IPredictionModelConnector instance (these should be stateless query wrappers)
        /// </summary>
        private Dictionary<string, IPredictionModelConnector> predictionConnectorMap = null;

        /// <summary>
        /// Key: session ID
        /// Value: PredictionSystemQueryCache all queries - ordered - issued to the given URI
        /// </summary>
        private Dictionary<string, PredictionSystemQueryCache> queryCacheMap = null;

        /// <summary>
        /// Key: uri
        /// Value: prediction model names as a List
        /// </summary>
        private Dictionary<string, List<string>> modelCacheMap = null;

        /// <summary>
        /// 
        /// </summary>
        private IPredictionModelListener predictionModelListener = null;

        /// <summary>
        /// Default constructor. Uses the default model file path specified
        /// in ModelNameUtil.
        /// </summary>
        public PredictionSystemManager() : base()
        {
            this.sessionToConnectorMap = new Dictionary<string, string>();
            this.queryCacheMap = new Dictionary<string, PredictionSystemQueryCache>();
            this.predictionConnectorMap = new Dictionary<string, IPredictionModelConnector>();
            this.modelCacheMap = new Dictionary<string, List<string>>();
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="modelName"></param>
        /// <param name="queryMsg"></param>
        public void AddQuery(string sessionID, string modelName, string queryMsg)
        {
            if (!string.IsNullOrEmpty(sessionID) && !string.IsNullOrEmpty(queryMsg))
            {
                PredictionSystemQueryCache queryCache = null;

                if (this.queryCacheMap.ContainsKey(sessionID))
                {
                    queryCache = this.queryCacheMap[sessionID];
                } else
                {
                    queryCache = new PredictionSystemQueryCache(sessionID, modelName);

                    this.queryCacheMap.Add(sessionID, queryCache);
                }

                queryCache.AddQueryMessage(queryMsg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public List<string> GetAllRegisteredModels(string sessionID, string uri)
        {
            IPredictionModelConnector connector = this.GetPredictionConnectorFromSessionID(sessionID, uri);

            if (connector != null)
            {
                return connector.GetRegisteredModels();
            } else
            {
                Console.WriteLine($"No prediction connector and models for session ID: {sessionID}");

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public List<string> GetCachedModelList(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                if (this.modelCacheMap.ContainsKey(uri))
                {
                    return this.modelCacheMap[uri];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool ProvisionQueryEngine(string sessionID, string uri)
        {
            return this.ProvisionQueryEngine(sessionID, null, uri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="modelName"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool ProvisionQueryEngine(string sessionID, string modelName, string uri)
        {
            if (!string.IsNullOrEmpty(sessionID) && !string.IsNullOrEmpty(uri))
            {
                Console.WriteLine($"Provisioning query engine. Params: {sessionID} - {uri}. Model: {modelName}");

                IPredictionModelConnector connector = new AiChatClientConnector(sessionID, uri);
                connector.SetPredictionModelListener(this);
                connector.SetModel(modelName);
                
                if (this.predictionConnectorMap.ContainsKey(uri))
                {
                    Console.WriteLine($"Replacing previous prediction connector with new: {uri}");
                    this.predictionConnectorMap[uri] = connector;
                } else
                {
                    Console.WriteLine($"Adding prediction connector: {uri}");
                    this.predictionConnectorMap.Add(uri, connector);
                }

                if (this.sessionToConnectorMap.ContainsKey(sessionID))
                {
                    Console.WriteLine($"Replacing previous session ID to prediction connector URI with new: {sessionID}");
                    this.sessionToConnectorMap[sessionID] = uri;
                } else
                {
                    Console.WriteLine($"Adding session ID to prediction connector URI: {sessionID}");
                    this.sessionToConnectorMap.Add(sessionID, uri);
                }

                Console.WriteLine($"Creating prediction engine backing cache: {sessionID}.");

                PredictionSystemQueryCache queryCache = new PredictionSystemQueryCache(sessionID, modelName);

                if (this.queryCacheMap.ContainsKey(sessionID))
                {
                    Console.WriteLine($"Query cache already exists for session ID {sessionID}. Saving then clearing.");

                    // TODO: save the cache - make this configurable?

                    this.queryCacheMap[sessionID] = queryCache;
                } else
                {
                    this.queryCacheMap.Add(sessionID, queryCache);
                }
            } else
            {
                Console.WriteLine($"Failed to provision query engine. Params invalid: {sessionID} - {uri}. Model: {modelName}");
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="modelName"></param>
        /// <param name="uri"></param>
        /// <param name="queryMsg"></param>
        /// <returns></returns>
        public bool SubmitQuery(string sessionID, string modelName, string uri, string queryMsg)
        {
            if (!string.IsNullOrEmpty(sessionID))
            {
                IPredictionModelConnector connector = null;

                if (!this.predictionConnectorMap.ContainsKey(uri))
                {
                    this.ProvisionQueryEngine(sessionID, uri);
                }

                connector = this.predictionConnectorMap[uri];

                Console.WriteLine($"Retrieved prediction connector. Sending query: {sessionID} - {connector.GetServerUri()}.");

                if (connector.SendRequest(queryMsg))
                {
                    Console.WriteLine($"Query submission SUCCESS: {sessionID} - {connector.GetServerUri()}");

                    // add query request to cache
                    if (this.queryCacheMap.ContainsKey(sessionID))
                    {
                        PredictionSystemQueryCache queryCache = this.queryCacheMap[sessionID];

                        queryCache.AddQueryMessage(queryMsg);
                    }

                    return true;
                } else
                {
                    Console.WriteLine($"Query submission FAILED: {sessionID} - {connector.GetServerUri()}");
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public PredictionSystemQueryCache GetQueryCache(string sessionID)
        {
            PredictionSystemQueryCache queryCache = null;

            if (!string.IsNullOrEmpty(sessionID))
            {
                if (this.queryCacheMap.ContainsKey(sessionID))
                {
                    queryCache = this.queryCacheMap[sessionID];
                }
            }

            return queryCache;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearAllCachedQueries()
        {
            this.ClearAllCachedQueries(true);

            this.sessionToConnectorMap.Clear();
            this.predictionConnectorMap.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveCache"></param>
        public void ClearAllCachedQueries(bool saveCache)
        {
            // TODO: save the query cache

            this.queryCacheMap.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void ClearCachedQueries(string key)
        {
            this.ClearCachedQueries(key, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="saveCache"></param>
        public void ClearCachedQueries(string sessionID, bool saveCache)
        {
            PredictionSystemQueryCache queryCache = this.GetQueryCache(sessionID);

            if (queryCache != null)
            {
                if (saveCache)
                {
                    this.SaveQueryCache(sessionID, queryCache);
                }

                queryCache.ClearCache();
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
        /// <param name="listener"></param>
        public void SetQueryResponseListener(IDataContextEventListener listener)
        {
            if (listener != null)
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleActuatorData(ActuatorData data)
        {
            // TODO: implement this
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleConnectionStateData(ConnectionStateData data)
        {
            // TODO: implement this
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleMessageData(MessageData data)
        {
            // TODO: implement this
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSensorData(SensorData data)
        {
            // TODO: implement this
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSystemPerformanceData(SystemPerformanceData data)
        {
            // TODO: implement this
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelListContainer"></param>
        public void OnModelListRetrieved(ModelListContainer modelListContainer)
        {
            string uri = modelListContainer.GetUri();
            List<string> modelList = modelListContainer.GetModelList();

            // add or overwrite existing entries - don't care which
            this.modelCacheMap[uri] = modelList;

            this.predictionModelListener?.OnModelListRetrieved(modelListContainer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryResponseContainer"></param>
        public void OnQueryResponseReceived(QueryResponseContainer queryResponseContainer)
        {
            if (queryResponseContainer != null)
            {
                string sessionID = queryResponseContainer.GetSessionID();
                string response = queryResponseContainer.GetResponse();

                if (!string.IsNullOrEmpty(sessionID))
                {
                    if (this.queryCacheMap.ContainsKey(sessionID))
                    {
                        PredictionSystemQueryCache queryCache = this.queryCacheMap[sessionID];

                        queryCache.AddQueryResponse(response);
                    }
                }

                this.predictionModelListener?.OnQueryResponseReceived(queryResponseContainer);
            }
        }

        /// <summary>
        /// Allows redirection of responses to another listener.
        /// </summary>
        /// <param name="listener"></param>
        public void SetPredictionModelListener(IPredictionModelListener listener)
        {
            if (listener != null)
            {
                this.predictionModelListener = listener;
            }
        }

        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        private IPredictionModelConnector GetPredictionConnectorFromSessionID(string sessionID, string uri)
        {
            if (!string.IsNullOrEmpty(sessionID))
            {
                if (!this.sessionToConnectorMap.ContainsKey(sessionID) || !this.predictionConnectorMap.ContainsKey(uri))
                {
                    this.ProvisionQueryEngine(sessionID, uri);
                }

                if (this.predictionConnectorMap.ContainsKey(uri))
                {
                    IPredictionModelConnector connector = this.predictionConnectorMap[uri];

                    return connector;
                } else
                {
                    Console.WriteLine($"No prediction connector available for URI: {uri}. Call 'ProvisionQueryEngine'.");
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        private string CreateQueryCacheFileName(string sessionID)
        {
            // TODO: create file name
            string fileName = sessionID;

            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = Path.GetFullPath(fileName);

                if (File.Exists(fileName))
                {
                    // log a message indicating the existing cache will be overwritten
                    Console.WriteLine($"AI query cache file name exists. Any save will overwrite.");
                }

                if (FileUtil.IsAccessible(fileName))
                {
                    Console.WriteLine($"Cache file name now set: {fileName}");
                }
            }

            return fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="queryCache"></param>
        /// <returns></returns>
        private bool SaveQueryCache(string sessionID, PredictionSystemQueryCache queryCache)
        {
            bool success = false;

            string fileName = this.CreateQueryCacheFileName(sessionID);

            return success;
        }
        
    }

}
