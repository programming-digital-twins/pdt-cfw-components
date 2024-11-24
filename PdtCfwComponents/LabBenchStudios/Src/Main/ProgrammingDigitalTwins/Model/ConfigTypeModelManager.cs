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
    public class ConfigTypeModelManager
    {

        private HashSet<string> configTypeFilePaths = new HashSet<string>();

        private ConfigTypeModelManagerCache configTypeMgrCache = null;

        // necessary for JSON serialization / deserialization
        public ConfigTypeModelManager() : this(ConfigConst.DEFAULT_CONFIG_TYPE_FILE_PATH)
        {
            // nothing to do
        }

        public ConfigTypeModelManager(string typeConfigFilePath)
        {
            this.configTypeFilePaths = new HashSet<string>();
            this.configTypeMgrCache = new ConfigTypeModelManagerCache();

            UpdateTypeConfigFilePaths(typeConfigFilePath);
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeConfigFilePath"></param>
        /// <returns></returns>
        public bool UpdateTypeConfigFilePaths(string typeConfigFilePath)
        {
            return UpdateTypeConfigFilePaths(typeConfigFilePath, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeConfigFilePathList"></param>
        /// <returns></returns>
        public bool UpdateTypeConfigFilePaths(List<string> typeConfigFilePathList)
        {
            int counter = 0;
            int typeConfigFileCount = 0;

            if (typeConfigFilePathList != null && typeConfigFilePathList.Count > 0)
            {
                typeConfigFileCount = typeConfigFilePathList.Count;

                foreach (string typeConfigFilePath in typeConfigFilePathList)
                {
                    if (UpdateTypeConfigFilePaths(typeConfigFilePath))
                    {
                        counter++;
                    }
                }
            }

            return counter > 0 && typeConfigFileCount == counter ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <param name="reloadModels"></param>
        /// <returns></returns>
        public bool UpdateTypeConfigFilePaths(string modelFilePath, bool reloadModels)
        {
            if (IsTypeConfigFilePathValid(modelFilePath))
            {
                this.configTypeFilePaths.Add(modelFilePath);

                if (reloadModels)
                {
                    if (!BuildTypeConfigCache())
                    {
                        Console.WriteLine("Failed to (re)load type config's. No config type model files provisioned. Check log output.");
                    }
                }

                return true;
            }

            return false;
        }


        // protected methods


        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool BuildTypeConfigCache()
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
        /// <param name="typeConfigFilePath"></param>
        /// <returns></returns>
        private bool IsTypeConfigFilePathValid(string typeConfigFilePath)
        {
            if (!string.IsNullOrEmpty(typeConfigFilePath)) {
                if (!this.configTypeFilePaths.Contains(typeConfigFilePath)) {
                    if (Directory.Exists(typeConfigFilePath)) {
                        Console.WriteLine($"Updating type config file paths. New file path is good: {typeConfigFilePath}");

                        return true;
                    } else {
                        Console.WriteLine($"Failed to update type config file paths. Requested model file path doesn't exist: {typeConfigFilePath}");
                    }
                } else {
                    Console.WriteLine($"Failed to update type config file paths. File path already used and stored: {typeConfigFilePath}");
                }

            } else {
                Console.WriteLine($"Failed to update type config file paths. File path is null or empty: {typeConfigFilePath}");
            }

            return false;
        }

    }
}
