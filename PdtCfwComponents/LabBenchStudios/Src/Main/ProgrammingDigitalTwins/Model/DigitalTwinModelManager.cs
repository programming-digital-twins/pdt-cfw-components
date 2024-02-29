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
        private string modelFilePath = ModelNameUtil.DEFAULT_MODEL_FILE_PATH;

        // this contains all the DT model parsed instances
        // this is indexed by the DTDLParser Dtmi object
        private IReadOnlyDictionary<Dtmi, DTEntityInfo> digitalTwinParsedModelCache;

        // this contains all the DT model raw JSON data
        // this is indexed by the string DTMI (use ModelConst.DtmiControllerEnum)
        private Dictionary<string, string> digitalTwinRawModelCache;

        // this contains all DT model state instances that are associated
        // with a given dtmi string (which is the lookup key)
        //
        // for the current implementation, this is sufficient granularity;
        // a future implementation may expand into a more complex structure
        //
        // this is indexed by the string DTMI (use ModelConst.DtmiControllerEnum)
        private Dictionary<string, List<DigitalTwinModelState>> digitalTwinStateCache;

        private bool hasSuccessfulDataLoad = false;

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

            this.LoadAndValidateDtdlModels();
        }

        // public methods
        
        public DigitalTwinModelState CreateModelState(
            ModelNameUtil.DtmiControllerEnum controllerID,
            IDataContextEventListener stateUpdateListener,
            string rawModelJson)
        {
            var dtModelState = new DigitalTwinModelState();

            dtModelState.SetModelControllerID(controllerID);
            dtModelState.SetRawModelJson(rawModelJson);
            dtModelState.SetVirtualAssetListener(stateUpdateListener);

            return dtModelState;
        }

        public List<string> GetAllDtmiValues()
        {
            if (digitalTwinParsedModelCache != null && digitalTwinParsedModelCache.Count > 0)
            {
                List<string> dtmiValues = new List<string>(digitalTwinParsedModelCache.Count);

                foreach (DTEntityInfo item in digitalTwinParsedModelCache.Values)
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

        public string GetRawModelJson(ModelNameUtil.DtmiControllerEnum dtmiController)
        {
            string dtmiUri = ModelNameUtil.CreateModelID(dtmiController);

            if (this.digitalTwinRawModelCache.ContainsKey(dtmiUri))
            {
                return this.digitalTwinRawModelCache[dtmiUri];
            }
            else
            {
                Console.WriteLine($"No raw DTDL JSON available for DTMI URI {dtmiUri}");
                return null;
            }
        }

        public bool HandleIncomingTelemetry(IotDataContext dataContext)
        {
            throw new NotImplementedException();
        }

        public bool HandleOutgoingStateUpdate(IotDataContext dataContext)
        {
            throw new NotImplementedException();
        }

        public bool HasSuccessfulDataLoad()
        {
            return this.hasSuccessfulDataLoad;
        }

        public void RegisterModelController(DigitalTwinModelState dtController)
        {

        }

        public bool ReloadDtdlModels()
        {
            return this.ReloadDtdlModels(this.modelFilePath);
        }

        public bool ReloadDtdlModels(string modelFilePath)
        {
            if (!string.IsNullOrEmpty(modelFilePath) && Directory.Exists(modelFilePath))
            {
                this.modelFilePath = modelFilePath;

                return this.LoadAndValidateDtdlModels();
            }
            else
            {
                Console.WriteLine($"Ignoring DTDL reload request. File path is invalid: {modelFilePath}");
                return false;
            }
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

        /// <summary>
        /// Unfortunately, this method results in each DTDL model being loaded twice
        /// Future optimizations will probably remove this redundancy.
        /// </summary>
        private bool LoadAndValidateDtdlModels()
        {
            bool success = false;

            this.digitalTwinParsedModelCache = ModelParserUtil.LoadAllDtdlModels(this.modelFilePath);

            if (this.digitalTwinParsedModelCache == null)
            {
                Console.WriteLine($"Failed to load DTDL models from path {this.modelFilePath}");
            }
            else
            {
                success = true;
            }

            this.digitalTwinRawModelCache    = new Dictionary<string, string>();

            var dtmiControllerList =
                (ModelNameUtil.DtmiControllerEnum[]) Enum.GetValues(typeof(ModelNameUtil.DtmiControllerEnum));

            foreach (ModelNameUtil.DtmiControllerEnum dtmiController in dtmiControllerList)
            {
                string dtmiUri  = ModelNameUtil.CreateModelID(dtmiController);
                string fileName = ModelNameUtil.GetModelFileName(dtmiController);

                string dtdlJson = ModelParserUtil.LoadDtdlFile(this.modelFilePath, fileName);

                if (!string.IsNullOrEmpty(dtdlJson))
                {
                    this.digitalTwinRawModelCache.Add(dtmiUri, dtdlJson);
                    success = true;
                }
                else
                {
                    Console.WriteLine($"Failed to load DTDL models from file {fileName}");
                }
            }

            this.hasSuccessfulDataLoad = success;

            return success;
        }

    }
}
