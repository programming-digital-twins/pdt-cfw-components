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

namespace LabBenchStudios.Pdt.Model
{
    public class ConfigTypeModelManagerCache
    {
        /// <summary>
        /// This table maps the top level config type model container and its name - it's
        /// the core cache from which all other models are referenced, including the container
        /// 'edge' nodes, contained by the ConfigTypeModelEntry type.
        /// </summary>
        private Dictionary<string, ConfigTypeModelContainer> configTypeContainerTable = null;

        /// <summary>
        /// This table maps the external model ID to the generalized ConfigTypeModelContext, which
        /// allows for simple lookups of a model (e.g., a DTMI reference name) to its associated
        /// type ID. This is the key method by which incoming telemetry can be mapped to the
        /// appropriate DTML model instance and associated state.
        /// </summary>
        private Dictionary<string, ConfigTypeModelContext> configTypeModelMappingTable = null;

        /// <summary>
        /// This table is a simple name to name lookup - using the config type name to
        /// find its associated model name.
        /// </summary>
        private Dictionary<string, string> configTypeNameToModelNameMappingTable = null;

        /// <summary>
        /// This table maps all unique type ID's to their respective container name, also known
        /// as the model ID (e.g., the integer representing windTurbine, thermostat, etc.). This allows
        /// for simple lookups of the parent container name when only the type ID is known.
        /// </summary>
        private Dictionary<int, string> typeIdToContainerNameMappingTable = null;

        private bool useGeneratedModelID = true;

        /// <summary>
        /// 
        /// </summary>
        public ConfigTypeModelManagerCache() : base()
        {
            this.configTypeContainerTable = new Dictionary<string, ConfigTypeModelContainer>();
            this.configTypeModelMappingTable = new Dictionary<string, ConfigTypeModelContext>();
            this.configTypeNameToModelNameMappingTable = new Dictionary<string, string>();
            this.typeIdToContainerNameMappingTable = new Dictionary<int, string>();
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetConfigTypeContainerInfoCount()
        {
            return this.configTypeContainerTable.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public ConfigTypeModelContainer GetConfigTypeContainer(string containerName)
        {
            if (containerName != null && containerName.Length > 0)
            {
                if (this.configTypeContainerTable.ContainsKey(containerName))
                {
                    return this.configTypeContainerTable[containerName];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public ConfigTypeModelEntry GetConfigType(string typeName)
        {
            if (typeName != null && typeName.Length > 0) {
                foreach (string containerName in this.configTypeContainerTable.Keys) {
                    ConfigTypeModelContainer typeContainer = this.configTypeContainerTable[containerName];

                    if (typeContainer != null) {
                        ConfigTypeModelEntry typeEntry = typeContainer.GetConfigType(typeName);

                        if (typeEntry != null) {
                            return typeEntry;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public ConfigTypeModelEntry GetConfigType(string containerName, string typeName)
        {
            if (typeName != null && typeName.Length > 0)
            {
                if (this.configTypeContainerTable.ContainsKey(containerName))
                {
                    ConfigTypeModelContainer typeContainer = this.configTypeContainerTable[containerName];

                    return typeContainer.GetConfigType(typeName);
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public ConfigTypeModelContext GetConfigTypeContextFromModelId(int modelId)
        {
            if (this.typeIdToContainerNameMappingTable.ContainsKey(modelId)) {
                string containerName = this.typeIdToContainerNameMappingTable[modelId];

                return this.GetConfigTypeContainer(containerName);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public ConfigTypeModelContext GetConfigTypeContextFromModelName(string modelName)
        {
            if (! string.IsNullOrEmpty(modelName)) {
                if (this.configTypeModelMappingTable.ContainsKey(modelName)) {
                    return this.configTypeModelMappingTable[modelName];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetLoadedConfigTypeNames()
        {
            List<string> configTypeNames = new List<string>(this.configTypeContainerTable.Keys);

            return configTypeNames;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetLoadedAndMappedModelNames()
        {
            List<string> mappedModelNames = new List<string>(this.configTypeModelMappingTable.Keys);

            return mappedModelNames;
        }

        /// <summary>
        /// Attempts to load all type config JSON model files from the given
        /// path.
        /// 
        /// Once loaded, any existing DataTypeCategoryInfo instances will
        /// be updated with the loaded (or re-loaded) JSON model file data.
        /// 
        /// This ensures that type config models can be updated dynamically
        /// and cached as needed.
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <returns></returns>
        public bool LoadConfigTypeModels(string modelFilePath)
        {
            if (modelFilePath != null)
            {
                if (Directory.Exists(modelFilePath))
                {
                    string[] typeConfigFiles = Directory.GetFiles(modelFilePath, ConfigConst.MODEL_FILE_NAME_SUFFIX);

                    foreach (string typeConfigFile in typeConfigFiles)
                    {
                        Console.WriteLine($"Attempting to load config type model: {typeConfigFile}");

                        try
                        {
                            ConfigTypeModelContainer typeContainer = ConfigTypeModelUtil.JsonFileToDataTypeCategoryInfo(typeConfigFile);

                            string containerTypeName = typeContainer.GetConfigTypeName();

                            // add the container to the internal container cache
                            // NOTE: can just use table[key] = value
                            if (this.configTypeContainerTable.ContainsKey(containerTypeName))
                            {
                                Console.WriteLine($"Container config type name already loaded: {containerTypeName}. Replacing old with new.");
                                this.configTypeContainerTable.Remove(containerTypeName);
                            }

                            this.configTypeContainerTable.Add(containerTypeName, typeContainer);

                            string containerModelName = typeContainer.GetModelName();

                            if (string.IsNullOrEmpty(containerModelName)) {
                                containerModelName = containerTypeName;
                                typeContainer.SetModelName(containerModelName);
                            }

                            // add the container to the internal container cache
                            // NOTE: can just use table[key] = value
                            if (this.configTypeModelMappingTable.ContainsKey(containerModelName))
                            {
                                Console.WriteLine($"Config type model already loaded: {containerModelName}. Replacing old with new.");
                                this.configTypeModelMappingTable.Remove(containerModelName);
                            }

                            // add the container to the model name mapping table
                            this.configTypeModelMappingTable.Add(containerModelName, typeContainer);

                            List<ConfigTypeModelEntry> configTypeEntries = typeContainer.GetConfigTypeList();

                            foreach (ConfigTypeModelEntry entry in configTypeEntries) {
                                // set ref to parent container
                                entry.SetConfigTypeContainerRef(typeContainer);

                                string entryTypeName = entry.GetConfigTypeName();
                                string entryModelName = entry.GetModelName();

                                // set the model ID - check the local flag to determine if:
                                //  - a dynamically generated model ID should be used ModelNameUtil.CreateModelID()
                                //  - that which is already set, and if empty, use the typeName instead
                                if (this.useGeneratedModelID) {
                                    entryModelName = ModelNameUtil.CreateModelID(entryTypeName);
                                    entry.SetModelName(entryModelName);
                                }

                                if (string.IsNullOrEmpty(entryModelName)) {
                                    entryModelName = entryTypeName;
                                    entry.SetModelName(entryModelName);
                                }

                                // update the mapping tables

                                //
                                // add the type to the internal container cache
                                //
                                if (this.configTypeModelMappingTable.ContainsKey(entryModelName))
                                {
                                    Console.WriteLine($"Entry config type name already loaded: {entryModelName}. Replacing old with new.");
                                    this.configTypeModelMappingTable.Remove(entryModelName);
                                }

                                // add or overwrite existing entry
                                this.configTypeModelMappingTable[entryModelName] = entry;
                                //this.configTypeModelMappingTable.Add(entryModelName, entry);

                                //
                                // add the type ID to the internal container cache
                                //
                                if (this.typeIdToContainerNameMappingTable.ContainsKey(entry.GetId()))
                                {
                                    Console.WriteLine($"Entry config type ID name already loaded: {entry.GetId()}. Replacing old with new.");
                                    this.typeIdToContainerNameMappingTable.Remove(entry.GetId());
                                }

                                // add or overwrite existing entry
                                this.typeIdToContainerNameMappingTable[entry.GetId()] = entryModelName;
                                //this.typeIdToContainerNameMappingTable.Add(entry.GetId(), entryModelName);

                                //
                                // add the type name to model name mapping entry
                                //

                                // add or overwrite existing entry
                                this.configTypeNameToModelNameMappingTable[entryTypeName] = entryModelName;
                                //this.configTypeNameToModelNameMappingTable.Add(entryTypeName, entryModelName);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Failed to load type config JSON model from file {modelFilePath}. Message: {e.Message}. Stack: {e.StackTrace}");
                        }
                    }

                    Console.WriteLine($"Loaded {typeConfigFiles.Length} type config JSON model files from path {modelFilePath}.");

                    return true;
                }
                else
                {
                    Console.WriteLine($"Type config JSON model path is invalid / not a path. Ignoring: {modelFilePath}");
                }
            }
            else
            {
                Console.WriteLine($"Type config JSON model path is null. Ignoring: {modelFilePath}");
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string key in configTypeContainerTable.Keys)
            {
                ConfigTypeModelContainer categoryInfo = configTypeContainerTable[key];

                sb.Append(categoryInfo).Append('\n');
            }

            return sb.ToString();
        }


        // protected methods

    }
}
