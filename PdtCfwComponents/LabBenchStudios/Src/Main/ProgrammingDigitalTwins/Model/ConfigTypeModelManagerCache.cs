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

        private bool useFullModelIDForMappingKey = false;

        // necessary for JSON serialization / deserialization
        public ConfigTypeModelManagerCache() : base()
        {
            this.configTypeContainerTable = new Dictionary<string, ConfigTypeModelContainer>();
            this.configTypeModelMappingTable = new Dictionary<string, ConfigTypeModelContext>();
        }


        // public methods

        public void AddConfigTypeContainerInfo(ConfigTypeModelContainer categoryInfo)
        {
            if (categoryInfo != null)
            {
                this.configTypeContainerTable.Add(categoryInfo.GetConfigTypeName(), categoryInfo);
            }
        }

        public int GetConfigTypeContainerInfoCount()
        {
            return this.configTypeContainerTable.Count;
        }

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

        public ConfigTypeModelContext GetConfigTypeContextFromModelID(string modelID)
        {
            if (modelID != null && modelID.Length > 0) {
                if (this.configTypeModelMappingTable.ContainsKey(modelID)) {
                    return this.configTypeModelMappingTable[modelID];
                }
            }

            return null;
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
                    string[] typeConfigFiles = Directory.GetFiles(modelFilePath);

                    foreach (string typeConfigFile in typeConfigFiles)
                    {
                        try
                        {
                            ConfigTypeModelContainer typeContainer = ConfigTypeModelUtil.JsonFileToDataTypeCategoryInfo(typeConfigFile);

                            string typeName = typeContainer.GetConfigTypeName();

                            // add the container to the internal container cache
                            this.configTypeContainerTable.Add(typeName, typeContainer);

                            // generate a model ID and add the each container AND entry to the internal mapping cache
                            string modelID = typeName;

                            if (this.useFullModelIDForMappingKey) {
                                modelID = ModelNameUtil.CreateModelID(typeName);
                            }

                            this.configTypeModelMappingTable.Add(modelID, typeContainer);

                            List<ConfigTypeModelEntry> configTypeEntries = typeContainer.GetConfigTypeList();

                            foreach (ConfigTypeModelEntry entry in configTypeEntries) {
                                typeName = entry.GetConfigTypeName();
                                modelID = typeName;

                                if (this.useFullModelIDForMappingKey) {
                                    modelID = ModelNameUtil.CreateModelID(typeName);
                                }

                                this.configTypeModelMappingTable.Add(modelID, entry);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Failed to load type config JSON model from file {modelFilePath}. Exception: {e}");
                        }
                    }

                    Console.WriteLine($"Loaded {typeConfigFiles.Length} type config JSON model files from path {modelFilePath}.");
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
