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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

using System;
using System.Collections.Generic;
using System.IO;

using DTDLParser;
using DTDLParser.Models;

namespace LabBenchStudios.Pdt.Model
{
    public class DigitalTwinModelManager : IDigitalTwinStateProcessor
    {
        private string modelFilePath = ModelConst.DEFAULT_MODEL_FILE_PATH;

        // this contains all the DT model JSON instances
        private IReadOnlyDictionary<Dtmi, DTEntityInfo> digitalTwinModelCache;

        // this contains all DT model state instances that are associated
        // with a given dtmi string (which is the lookup key)
        //
        // for the current implementation, this is sufficient granularity;
        // a future implementation may expand into a more complex structure
        private Dictionary<string, List<DigitalTwinModelState>> digitalTwinStateCache;

        public DigitalTwinModelManager(string modelFilePath)
        {
            if (!string.IsNullOrEmpty(modelFilePath) && Directory.Exists(modelFilePath))
            {
                this.modelFilePath = modelFilePath;
            }
            else
            {
                Console.WriteLine($"Failed to set model file path. Path non-existent: {modelFilePath}");
            }

            this.LoadAndValidateDtmlModels();
        }

        // public methods

        public DigitalTwinModelState CreateModelState(IDataContextEventListener stateUpdateListener)
        {
            return new DigitalTwinModelState();
                
        }
        public List<string> GetAllDtmiValues()
        {
            if (digitalTwinModelCache != null && digitalTwinModelCache.Count > 0)
            {
                List<string> dtmiValues = new List<string>(digitalTwinModelCache.Count);

                foreach (DTEntityInfo item in digitalTwinModelCache.Values)
                {
                    dtmiValues.Add(item.Id.AbsoluteUri);

                    switch (item.EntityKind)
                    {
                        case DTEntityKind.Property:
                            DTPropertyInfo pi = item as DTPropertyInfo;
                            Console.WriteLine($"\tProperty: {pi.Name} -- schema {pi.Schema}");
                            break;
                        case DTEntityKind.Relationship:
                            DTRelationshipInfo ri = item as DTRelationshipInfo;
                            Console.WriteLine($"\tRelationship: {ri.Name} -- target {ri.Target}");
                            break;
                        case DTEntityKind.Telemetry:
                            DTTelemetryInfo ti = item as DTTelemetryInfo;
                            Console.WriteLine($"\tTelemetry: {ti.Name} -- schema {ti.Schema}");
                            break;
                        case DTEntityKind.Component:
                            DTComponentInfo ci = item as DTComponentInfo;
                            DTInterfaceInfo component = ci.Schema;
                            Console.WriteLine($"\tComponent: {ci.Id} | {ci.Name} -- schema: {component.ToString()}");
                            break;
                    }
                }

                return dtmiValues;
            }
            else
            {
                Console.WriteLine($"No DTDL model cache loaded from file path {this.modelFilePath}");
            }

            return null;
        }

        public bool HandleIncomingTelemetry(IotDataContext dataContext)
        {
            throw new NotImplementedException();
        }

        public bool HandleOutgoingStateUpdate(IotDataContext dataContext)
        {
            throw new NotImplementedException();
        }

        public void RegisterModelController(DigitalTwinModelState dtController)
        {

        }

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

        private void LoadAndValidateDtmlModels()
        {
            this.digitalTwinModelCache = ModelParserUtil.LoadDtdlModels(this.modelFilePath);
        }

    }
}
