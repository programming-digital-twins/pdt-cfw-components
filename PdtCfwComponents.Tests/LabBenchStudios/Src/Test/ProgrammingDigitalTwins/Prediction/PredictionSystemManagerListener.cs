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
    public class PredictionSystemManagerListener : IPredictionModelListener
    {
        /// <summary>
        /// Default constructor. Uses the default model file path specified
        /// in ModelNameUtil.
        /// </summary>
        public PredictionSystemManagerListener() : base()
        {
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelListContainer"></param>
        public void OnModelListRetrieved(ModelListContainer modelListContainer)
        {
            StringBuilder builder = new StringBuilder();

            if (modelListContainer != null)
            {
                List<string> modelList = modelListContainer.GetModelList();

                if (modelList != null && modelList.Count > 0)
                {
                    builder.Append($"{modelList.Count} models found.");

                    foreach (string model in modelList)
                    {
                        builder.Append('\n').Append(model);
                    }
                } else
                {
                    builder.Append("No models found.");
                }

                Console.WriteLine($"Received model list: {modelListContainer.GetUri()} - {builder.ToString()}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryResponseContainer"></param>
        public void OnQueryResponseReceived(QueryResponseContainer queryResponseContainer)
        {
            if (queryResponseContainer != null)
            {
                StringBuilder builder = new StringBuilder();

                builder.Append($"Query response received: {queryResponseContainer.GetSessionID()} - {queryResponseContainer.GetUri()}.");
                builder.Append($"Response:\n").Append(queryResponseContainer.GetResponse());

                Console.WriteLine(builder.ToString());
            }
        }
        
    }

}
