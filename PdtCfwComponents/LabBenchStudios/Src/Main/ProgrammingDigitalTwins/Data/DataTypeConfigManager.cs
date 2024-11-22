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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LabBenchStudios.Pdt.Data
{
    public class DataTypeConfigManager
    {
        public const string TYPE_CONFIG_FILE_NAME_PREFIX = "Lbs_Pdt_TypeConfig";
        public const string TYPE_CONFIG_FILE_NAME_PATTERN = TYPE_CONFIG_FILE_NAME_PREFIX + "*.json";

        public const string DEFAULT_TYPE_CONFIG_FILE_PATH = "../../../../Models/Types/";

        private HashSet<string> typeConfigFilePaths = new HashSet<string>();


        // necessary for JSON serialization / deserialization
        public DataTypeConfigManager() : this(DEFAULT_TYPE_CONFIG_FILE_PATH)
        {
        }

        public DataTypeConfigManager(string typeConfigFilePath)
        {
            this.typeConfigFilePaths = new HashSet<string>();

            this.UpdateTypeConfigFilePaths(typeConfigFilePath);
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeConfigFilePath"></param>
        /// <returns></returns>
        private bool IsTypeConfigFilePathValid(string typeConfigFilePath)
        {
            if (!string.IsNullOrEmpty(typeConfigFilePath))
            {
                if (!this.typeConfigFilePaths.Contains(typeConfigFilePath))
                {
                    if (Directory.Exists(typeConfigFilePath))
                    {
                        Console.WriteLine($"Updating type config file paths. New file path is good: {typeConfigFilePath}");

                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to update type config file paths. Requested model file path doesn't exist: {typeConfigFilePath}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to update type config file paths. File path already used and stored: {typeConfigFilePath}");
                }

            }
            else
            {
                Console.WriteLine($"Failed to update type config file paths. File path is null or empty: {typeConfigFilePath}");
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeConfigFilePath"></param>
        /// <returns></returns>
        public bool UpdateTypeConfigFilePaths(string typeConfigFilePath)
        {
            return this.UpdateTypeConfigFilePaths(typeConfigFilePath, true);
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
                    if (this.UpdateTypeConfigFilePaths(typeConfigFilePath))
                    {
                        counter++;
                    }
                }
            }

            return (counter > 0 && typeConfigFileCount == counter ? true : false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <param name="reloadModels"></param>
        /// <returns></returns>
        public bool UpdateTypeConfigFilePaths(string modelFilePath, bool reloadModels)
        {
            if (this.IsTypeConfigFilePathValid(modelFilePath))
            {
                this.typeConfigFilePaths.Add(modelFilePath);

                if (reloadModels)
                {
                    if (!this.BuildTypeConfigCache())
                    {
                        Console.WriteLine("Failed to reload type config's. Check log output.");
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
            if (this.typeConfigFilePaths.Count > 0)
            {
                return true;
            }

            return false;
        }

    }
}
