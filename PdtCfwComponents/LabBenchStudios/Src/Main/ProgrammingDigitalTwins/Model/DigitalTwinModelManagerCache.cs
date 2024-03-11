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

using Newtonsoft.Json;

using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Common;

namespace LabBenchStudios.Pdt.Model
{
    /// <summary>
    /// This class contains the properties and current known state of
    /// a digital twin model, along with its references and components.
    /// 
    /// Shared properties are derived from IotDataContext, which is also
    /// described in the base DTDML that all models extend.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DigitalTwinModelManagerCache
    {
        // this contains all the DT model raw JSON data
        // this is indexed by the string DTMI
        // (which is the stringified ModelConst.DtmiControllerEnum)
        private Dictionary<string, string> digitalTwinDtdlJsonCache;

        // this contains all DT model state instances that are associated
        // with a given dtmi string (which is the lookup key)
        //
        // for the current implementation, this is sufficient granularity;
        // a future implementation may expand into a more complex structure
        //
        // this is indexed by a key unique to the twin state's instancing
        // rules with help from ModelNameUtil.
        private Dictionary<string, DigitalTwinModelState> digitalTwinStateCache;

        // this maps the incoming telemetry data sync ID to one or more
        // associated digital twin states
        // this is used for quick lookups of associated models to incoming
        // telemetry so the data can be easily distributed
        private Dictionary<string, List<DigitalTwinModelState>> digitalTwinStateLookupMap;

        // this maps the data sync key (generated for each incoming telemetry message)
        // to one or more associated digital twin model state ID's
        private Dictionary<string, HashSet<string>> modelToDataSyncKeyMap;


        // constructors

        /// <summary>
        /// 
        /// </summary>
        public DigitalTwinModelManagerCache() : base()
        {
            this.digitalTwinDtdlJsonCache = new Dictionary<string, string>();
            this.digitalTwinStateCache = new Dictionary<string, DigitalTwinModelState>();
            this.digitalTwinStateLookupMap = new Dictionary<string, List<DigitalTwinModelState>>();
            this.modelToDataSyncKeyMap = new Dictionary<string, HashSet<string>>();
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSyncKey"></param>
        /// <param name="modelSyncKey"></param>
        public void AssignDataSyncKeyToModelSyncKey(
            DigitalTwinDataSyncKey dataSyncKey,
            string modelSyncKey)
        {
            if (dataSyncKey != null)
            {
                this.AssignDataSyncKeyToModelSyncKey(dataSyncKey.ToString(), modelSyncKey);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSyncKey"></param>
        /// <param name="modelSyncKey"></param>
        public void AssignDataSyncKeyToModelSyncKey(
            string dataSyncKey,
            string modelSyncKey)
        {
            if (!string.IsNullOrEmpty(dataSyncKey) && !string.IsNullOrEmpty(modelSyncKey))
            {
                if (this.modelToDataSyncKeyMap.ContainsKey(dataSyncKey))
                {
                    HashSet<string> modelSyncKeySet = this.modelToDataSyncKeyMap[dataSyncKey];

                    if (!modelSyncKeySet.Contains(modelSyncKey))
                    {
                        modelSyncKeySet.Add(modelSyncKey);
                    }
                }
                else
                {
                    HashSet<string> modelSyncKeySet = new HashSet<string>();
                    modelSyncKeySet.Add(modelSyncKey);

                    this.modelToDataSyncKeyMap.Add(dataSyncKey, modelSyncKeySet);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCountOfCachedJsonModels()
        {
            return this.digitalTwinDtdlJsonCache.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCountOfCachedModelStates()
        {
            return this.digitalTwinStateCache.Count;
        }

        /// <summary>
        /// Returns the DTDL JSON for the given controller
        /// </summary>
        /// <param name="dtmiController"></param>
        /// <returns></returns>
        public string GetDigitalTwinModelJson(ModelNameUtil.DtmiControllerEnum dtmiController)
        {
            string dtmiUri = ModelNameUtil.CreateModelID(dtmiController);

            if (this.digitalTwinDtdlJsonCache.ContainsKey(dtmiUri))
            {
                return this.digitalTwinDtdlJsonCache[dtmiUri];
            }
            else
            {
                Console.WriteLine($"No raw DTDL JSON available for DTMI URI {dtmiUri}");
                return null;
            }
        }

        /// <summary>
        /// Returns the internally stored DT Model State instance using
        /// its model state key.
        /// </summary>
        /// <param name="modelStateKey"></param>
        /// <returns></returns>
        public DigitalTwinModelState GetDigitalTwinModelState(string modelStateKey)
        {
            if (this.HasDigitalTwinModelState(modelStateKey))
            {
                return this.digitalTwinStateCache[modelStateKey];
            }

            Console.WriteLine($"No DT model state available for key {modelStateKey}");
            return null;
        }

        public List<string> GetStoredDataSyncKeys()
        {
            return new List<string>(this.digitalTwinStateLookupMap.Keys);
        }

        /// <summary>
        /// Checks if we have an internally stored DT Model State instance using
        /// its model state key.
        /// </summary>
        /// <param name="modelStateKey"></param>
        /// <returns></returns>
        public bool HasDigitalTwinModelState(string modelStateKey)
        {
            if (!string.IsNullOrEmpty(modelStateKey))
            {
                return this.digitalTwinStateCache.ContainsKey(modelStateKey);
            }

            return false;
        }
        
        /// <summary>
        /// Attempts to load all digital twin JSON model files from the given
        /// path, using the known DTMI controller types (stored within the
        /// ModelNameUtil.DtmiControllerEnum enumerator) to guide the file
        /// loading process (only those models with a matching controller ID
        /// to filename will be loaded).
        /// 
        /// Once loaded, any existing DigitalTwinModelState instances will
        /// be updated with the loaded (or re-loaded) JSON model file data
        /// using the state's associated controller ID.
        /// 
        /// This ensures that models can be updated dynamically, cached,
        /// and re-assigned to existing model states, which use the contents
        /// of the JSON to generate their associative properties and parse
        /// incoming telemetry data.
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <returns></returns>
        public bool LoadDigitalTwinJsonModels(string modelFilePath)
        {
            // update DTDL JSON cache
            var dtmiControllerList =
                (ModelNameUtil.DtmiControllerEnum[])Enum.GetValues(typeof(ModelNameUtil.DtmiControllerEnum));

            foreach (ModelNameUtil.DtmiControllerEnum dtmiController in dtmiControllerList)
            {
                string dtmiUri = ModelNameUtil.CreateModelID(dtmiController);
                string fileName = ModelNameUtil.GetModelFileName(dtmiController);

                string dtdlJson = ModelParserUtil.LoadDtdlFile(modelFilePath, fileName);

                Console.WriteLine($"  -> DTMI URI: {dtmiUri}. Loaded file: {fileName}");

                if (! string.IsNullOrEmpty(dtdlJson))
                {
                    if (!this.digitalTwinDtdlJsonCache.ContainsKey(dtmiUri))
                    {
                        this.digitalTwinDtdlJsonCache.Add(dtmiUri, dtdlJson);
                    }
                    else
                    {
                        this.digitalTwinDtdlJsonCache[dtmiUri] = dtdlJson;
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to load DTDL models from file {fileName}");
                }
            }

            // update model state cache
            foreach (string key in this.digitalTwinStateCache.Keys)
            {
                DigitalTwinModelState modelState = this.digitalTwinStateCache[key];

                if (modelState != null)
                {
                    Console.WriteLine($"Retrieved DT Model State {key} with ID {modelState.GetModelID()}");

                    string rawJson = this.GetDigitalTwinModelJson(modelState.GetModelControllerID());

                    modelState.SetModelJson(rawJson);
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public List<DigitalTwinModelState> LookupDigitalTwinModelState(IotDataContext dataContext)
        {
            if (dataContext != null)
            {
                string dataSyncKey = ModelNameUtil.GenerateDataSyncKey(dataContext).ToString();

                return this.LookupDigitalTwinModelState(dataSyncKey);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSyncKey"></param>
        /// <returns></returns>
        public List<DigitalTwinModelState> LookupDigitalTwinModelState(string dataSyncKey)
        {
            if (!string.IsNullOrEmpty(dataSyncKey))
            {
                if (this.digitalTwinStateLookupMap.ContainsKey(dataSyncKey))
                {
                    return this.digitalTwinStateLookupMap[dataSyncKey];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtModelState"></param>
        /// <returns></returns>
        public DigitalTwinModelState UpdateDigitalTwinModelStateCache(DigitalTwinModelState dtModelState)
        {
            if (dtModelState != null)
            {
                Console.WriteLine($"Adding (or updating) DigitalTwinModelState to internal cache: {dtModelState.GetModelSyncKeyString()}");

                string prevModelSyncKey = dtModelState.GetPreviousModelSyncKeyString();

                // NOTE: The following if clause may now be moot with
                // recent updates to model state ID

                if (!string.IsNullOrEmpty(prevModelSyncKey))
                {
                    // model state is updated - make the switch
                    if (this.HasDigitalTwinModelState(prevModelSyncKey))
                    {
                        if (prevModelSyncKey.Equals(dtModelState.GetModelSyncKeyString()))
                        {
                            Console.WriteLine(
                                $"Nothing to do. DigitalTwinModelState mapping doesn't need update: {prevModelSyncKey}.");

                            return dtModelState;
                        }

                        // check if we need to transfer the model state - we can verify this by checking its GUID
                        if (this.HasDigitalTwinModelState(prevModelSyncKey))
                        {
                            this.digitalTwinStateCache.Remove(prevModelSyncKey);
                        }
                    }
                }

                return this.StoreModelState(dtModelState);
            }

            Console.WriteLine("DigitalTwinModelState ref is null. Ignoring.");

            return null;
        }


        // private methods

        /// <summary>
        /// This method should only be called when the DigitalTwinModelState should
        /// be stored; as a precaution, all stored model states will be
        /// scanned for a duplicate GUID - this can be expensive if there are
        /// a LOT of model states already stored, but will prevent dup's
        /// and strange update behaviors.
        /// </summary>
        /// <param name="dtModelState"></param>
        /// <returns></returns>
        private DigitalTwinModelState StoreModelState(DigitalTwinModelState dtModelState)
        {
            string modelSyncKey = dtModelState.GetModelSyncKeyString();

            // remove any existing ref's using the model state's GUID first
            this.FindAndRemoveState(dtModelState);

            // if this model's sync key isn't yet stored, create a new model map
            // this allows multiple twin instances to receive updates from the
            // same physical thing
            if (!this.HasDigitalTwinModelState(modelSyncKey))
            {
                Console.WriteLine(
                    $"Added DigitalTwinModelState to cache with key {modelSyncKey}.");

                this.digitalTwinStateCache.Add(modelSyncKey, dtModelState);
            }
            else
            {
                Console.WriteLine(
                    $"Replacing previous DigitalTwinModelState in cache with key {modelSyncKey}.");

                this.digitalTwinStateCache[modelSyncKey] = dtModelState;
            }

            DigitalTwinDataSyncKey dataSyncKey = dtModelState.GetDataSyncKey();

            this.AssignDataSyncKeyToModelSyncKey(dataSyncKey, modelSyncKey);
            this.AddModelStateToDataSyncKey(dataSyncKey, dtModelState);

            return dtModelState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSyncKey"></param>
        /// <param name="modelState"></param>
        private void AddModelStateToDataSyncKey(
            DigitalTwinDataSyncKey dataSyncKey, DigitalTwinModelState modelState)
        {
            string key = dataSyncKey.ToString();

            List<DigitalTwinModelState> modelStateList;

            if (!this.digitalTwinStateLookupMap.ContainsKey(key))
            {
                modelStateList = new List<DigitalTwinModelState>
                {
                    modelState
                };

                this.digitalTwinStateLookupMap.Add(key, modelStateList);
            }
            else
            {
                modelStateList = this.digitalTwinStateLookupMap[key];

                if (!modelStateList.Contains(modelState))
                {
                    modelStateList.Add(modelState);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtModelState"></param>
        private void FindAndRemoveState(DigitalTwinModelState dtModelState)
        {
            foreach (var entry in this.digitalTwinStateLookupMap)
            {
                if (entry.Value.Contains(dtModelState))
                {
                    entry.Value.Remove(dtModelState);
                }
            }
        }

    }

}
