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
using System.Diagnostics;
using System.IO;
using System.Text;

using LabBenchStudios.Pdt.Common;

namespace LabBenchStudios.Pdt.Model
{
    public class ConfigTypeModelManager
    {
        private HashSet<string> configTypeFilePaths = new HashSet<string>();

        private ConfigTypeModelManagerCache configTypeMgrCache = null;
        
        /// <summary>
        /// 
        /// </summary>
        public ConfigTypeModelManager() : this(ConfigConst.TEST_CONFIG_TYPE_MODEL_FILE_PATH)
        {
            // nothing to do
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configTypeFilePath"></param>
        public ConfigTypeModelManager(string configTypeFilePath)
        {
            this.configTypeFilePaths = new HashSet<string>();
            this.configTypeMgrCache = new ConfigTypeModelManagerCache();

            UpdateConfigTypeFilePaths(configTypeFilePath);
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public ConfigTypeModelEntry GetConfigEntryByTypeName(string typeName)
        {
            ConfigTypeModelEntry modelEntry = this.configTypeMgrCache.GetConfigType(typeName);

            if (modelEntry != null) {
                Console.WriteLine($"Found config type entry using name: {typeName}");
            } else {
                Console.WriteLine($"No config type entry found in cache using name: {typeName}");
            }

            return modelEntry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public ConfigTypeModelEntry GetConfigEntryByModelId(int modelId)
        {
            ConfigTypeModelContext modelContext = this.configTypeMgrCache.GetConfigTypeContextFromModelId(modelId);

            if (modelContext != null) {
                if (!modelContext.IsTypeCategory()) {
                    return (ConfigTypeModelEntry)modelContext;
                } else {
                    Console.WriteLine($"Model ID maps to a ConfigTypeModelContainer, not a ConfigTypeModelEntry. Ignoring: {modelId}");

                    return null;
                }
            } else {
                Console.WriteLine($"Model ID is not cached as a ConfigTypeModelContainer or ConfigTypeModelEntry. Ignoring: {modelId}");

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public ConfigTypeModelEntry GetConfigEntryByModelName(string modelName)
        {
            ConfigTypeModelContext modelContext = this.configTypeMgrCache.GetConfigTypeContextFromModelName(modelName);

            if (modelContext != null) {
                if (!modelContext.IsTypeCategory()) {
                    return (ConfigTypeModelEntry)modelContext;
                } else {
                    Console.WriteLine($"Model name maps to a ConfigTypeModelContainer, not a ConfigTypeModelEntry. Ignoring: {modelName}");

                    return null;
                }
            } else {
                Console.WriteLine($"Model name is not cached as a ConfigTypeModelContainer or ConfigTypeModelEntry. Ignoring: {modelName}");

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public ConfigTypeModelContainer GetConfigCategoryByModelName(string modelName)
        {
            ConfigTypeModelContext modelContext = this.configTypeMgrCache.GetConfigTypeContextFromModelName(modelName);

            if (modelContext != null)
            {
                if (modelContext.IsTypeCategory())
                {
                    return (ConfigTypeModelContainer) modelContext;
                } else
                {
                    Console.WriteLine($"Model name maps to a ConfigTypeModelEntry, not a ConfigTypeModelContainer. Ignoring: {modelName}");

                    return null;
                }
            } else
            {
                Console.WriteLine($"Model name is not cached as a ConfigTypeModelEntry or ConfigTypeModelContainer. Ignoring: {modelName}");

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetLoadedConfigTypeNames()
        {
            return this.configTypeMgrCache.GetLoadedConfigTypeNames();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetLoadedAndMappedModelNames()
        {
            return this.configTypeMgrCache.GetLoadedAndMappedModelNames();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configTypeFilePathSet"></param>
        /// <returns></returns>
        public bool UpdateConfigTypeFilePaths(HashSet<string> configTypeFilePathSet)
        {
            int counter = 0;
            int configTypeFileCount = 0;

            if (configTypeFilePathSet != null && configTypeFilePathSet.Count > 0)
            {
                configTypeFileCount = configTypeFilePathSet.Count;

                foreach (string configTypeFilePath in configTypeFilePathSet)
                {
                    if (UpdateConfigTypeFilePaths(configTypeFilePath))
                    {
                        counter++;
                    }
                }
            }

            return counter > 0 && configTypeFileCount == counter ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configTypeFilePath"></param>
        /// <returns></returns>
        public bool UpdateConfigTypeFilePaths(string configTypeFilePath)
        {
            return UpdateConfigTypeFilePaths(configTypeFilePath, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <param name="reloadModels"></param>
        /// <returns></returns>
        public bool UpdateConfigTypeFilePaths(string modelFilePath, bool reloadModels)
        {
            if (this.IsConfigTypeFilePathConfigured(modelFilePath))
            {
                Console.WriteLine($"Model config type path already configured. Re-testing access: {modelFilePath}");
            }

            if (this.IsConfigTypeFilePathValid(modelFilePath))
            {
                if (!this.configTypeFilePaths.Contains(modelFilePath))
                {
                    this.configTypeFilePaths.Add(modelFilePath);
                } else
                {
                    Console.WriteLine($"Model config type path already added: {modelFilePath}. Ignoring.");
                }

                if (reloadModels)
                {
                    if (!BuildConfigTypeCache())
                    {
                        Console.WriteLine("Failed to (re)load config type's. No config type model files provisioned. Check log output.");
                    }
                }

                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <returns></returns>
        public bool IsConfigTypeFilePathConfigured(string modelFilePath)
        {
            return this.configTypeFilePaths.Contains(modelFilePath);
        }

        // protected methods


        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool BuildConfigTypeCache()
        {
            if (this.configTypeFilePaths.Count > 0)
            {
                foreach (string configTypeFilePath in this.configTypeFilePaths) {
                    if (this.configTypeMgrCache.LoadConfigTypeModels(configTypeFilePath)) {
                        Console.WriteLine($"Successfully loaded config type model from path: {configTypeFilePath}");
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configTypeFilePath"></param>
        /// <returns></returns>
        private bool IsConfigTypeFilePathValid(string configTypeFilePath)
        {
            try
            {
                if (!this.configTypeFilePaths.Contains(configTypeFilePath))
                {
                    if (Directory.Exists(configTypeFilePath))
                    {
                        Console.WriteLine($"Updating config type file paths. New file path is good: {configTypeFilePath}");

                        return true;
                    } else
                    {
                        Console.WriteLine($"Failed to update config type file paths. Requested model file path doesn't exist: {configTypeFilePath}. Stack: {System.Environment.StackTrace}");
                    }
                } else
                {
                    Console.WriteLine($"Failed to update config type file paths. File path already used and stored: {configTypeFilePath}");
                }
            } catch (Exception e) {
                Console.WriteLine($"Failed to update config type file paths. File path is null or empty: {configTypeFilePath}. Error: {e.Message}. Stack: {System.Environment.StackTrace}");
            }

            return false;
        }

    }
}
