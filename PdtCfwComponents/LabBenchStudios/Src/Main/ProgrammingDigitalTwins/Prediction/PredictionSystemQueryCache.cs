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
using System.Text;
using LabBenchStudios.Pdt.Common;

namespace LabBenchStudios.Pdt.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class PredictionSystemQueryCache
    {
        private string sessionID = ConfigConst.DEFAULT_QUERY_SESSION_ID;

        private string latestQueryMsg = null;
        private string latestResponseMsg = null;

        private string modelName = "llama3.2";

        private List<string> queryMsgList = null;

        private List<string> queryResponseList = null;

        private bool annotateResponseWithModelName = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        public PredictionSystemQueryCache(string sessionID) : this(sessionID, null)
        {
            // nothing else to do
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="modelName"></param>
        public PredictionSystemQueryCache(string sessionID, string modelName) : base()
        {
            if (!string.IsNullOrEmpty(sessionID))
            {
                this.sessionID = sessionID;
            }

            this.SetModelName(modelName);

            this.queryMsgList = new List<string>();
            this.queryResponseList = new List<string>();
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryMsg"></param>
        public void AddQueryMessage(string queryMsg)
        {
            if (!string.IsNullOrWhiteSpace(queryMsg))
            {
                this.queryMsgList.Add(queryMsg);

                this.latestQueryMsg = queryMsg;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryResponse"></param>
        public void AddQueryResponse(string queryResponse)
        {
            if (!string.IsNullOrWhiteSpace(queryResponse))
            {
                StringBuilder builder = new StringBuilder();

                if (this.annotateResponseWithModelName)
                {
                    builder.Append('[').Append(this.modelName).Append("]: ");
                }

                builder.Append(queryResponse);

                this.queryResponseList.Add(builder.ToString());

                this.latestResponseMsg = queryResponse;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearCache()
        {
            this.queryMsgList.Clear();
            this.queryResponseList.Clear();
            this.latestQueryMsg = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllQueryMessages()
        {
            return this.queryMsgList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllQueryResponses()
        {
            return this.queryResponseList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetAggregatedQueryMessages()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string message in this.queryMsgList)
            {
                sb.Append(message).Append('\n');
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetAggregatedResponseMessages()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string message in this.queryResponseList)
            {
                sb.Append(message).Append('\n');
            }

            return sb.ToString();
        }

        /// <summary>
        /// By default, this will 'pop' the latest response message,
        /// clearing the value for the 'latest response'.
        /// 
        /// Note that this does NOT affect the cache, only the
        /// convenience of referencing the latest via a separate
        /// separate string.
        /// </summary>
        /// <returns></returns>
        public string GetLatestResponseMessage()
        {
            return this.GetLatestResponseMessage(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <param name="clearOnRequest"></param>
        /// 
        public string GetLatestResponseMessage(bool clearOnRequest)
        {
            string msg = this.latestResponseMsg;

            if (clearOnRequest)
            {
                this.latestResponseMsg = null;
            }

            return msg;
        }

        /// <summary>
        /// By default, this will 'pop' the latest query message,
        /// clearing the value for the 'latest query'.
        /// 
        /// Note that this does NOT affect the cache, only the
        /// convenience of referencing the latest via a separate
        /// separate string.
        /// </summary>
        /// <returns></returns>
        public string GetLatestQueryMessage()
        {
            return this.GetLatestQueryMessage(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <param name="clearOnRequest"></param>
        public string GetLatestQueryMessage(bool clearOnRequest)
        {
            string msg = this.latestQueryMsg;

            if (clearOnRequest)
            {
                this.latestQueryMsg = null;
            }

            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetModelName()
        {
            return this.modelName;
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
        /// <param name="enable"></param>
        public void SetAnnotateResponseWithModelNameFlag(bool enable)
        {
            this.annotateResponseWithModelName = enable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelName"></param>
        public void SetModelName(string modelName)
        {
            if (!string.IsNullOrEmpty(modelName))
            {
                this.modelName = modelName;
            }
        }

        // protected methods

    }
}
