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
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using LabBenchStudios.Pdt.Common;
using System.IO;
using System.Security.Cryptography;

namespace LabBenchStudios.Pdt.Model
{
    public static class ConfigTypeModelUtil
    {
        //////////
        // 
        // Data type configuration information is used to ensure an accurate
        // mapping between edge-generated data sets (e.g., temp sensor data,
        // humidity sensor data, on / off actuation commands, etc.) and their
        // DTDL model counterparts.
        //
        // The mapping is handled using a JSON-LD like type config representation
        // that covers the type category (e.g., environmental data name / ID pair)
        // and the type itself (e.g., temperature sensor name / ID pair), along with
        // other data points that reference the appropriate DTDL model ID.
        //
        // The consts declared in this class and the static methods are used to
        // facilitate this mapping process and contain the configuration data within
        // a cache of objects that can be easily accessed programmatically to
        // appropriately map incoming and outgoing data to the requisite model.
        //

        // static const entries

        /// <summary>
        /// 
        /// </summary>
        static DefaultContractResolver camelCaseResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = true
            }
        };


        // public static methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DataTypeCategoryInfoToJson(ConfigTypeModelContainer data)
        {
            if (data != null)
            {
                string jsonData =
                    JsonConvert.SerializeObject(data, Formatting.Indented);

                return jsonData;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string DataTypeCategoryInfoToJsonFile(ConfigTypeModelContainer data, string filePath)
        {
            string jsonData = DataTypeCategoryInfoToJson(data);

            if (jsonData != null && filePath != null)
            {
                if (File.Exists(filePath))
                {
                    using (StreamWriter writer = new(filePath))
                    {
                        writer.Write(jsonData);
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to write JSON data to filesystem. File path non-existent: {filePath}");
                }
            } else
            {
                Console.WriteLine("JSON data and / or file path are null / empty. Ignoring.");
            }

            return jsonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static ConfigTypeModelContainer JsonToDataTypeCategoryInfo(string jsonData)
        {
            jsonData = NormalizeData(jsonData);

            ConfigTypeModelContainer data = new ConfigTypeModelContainer();

            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ConfigTypeModelContainer JsonFileToDataTypeCategoryInfo(string filePath)
        {
            ConfigTypeModelContainer data = null;

            try
            {
                if (Directory.Exists(filePath))
                {
                    // 'file' is a directory
                    Console.WriteLine($"{filePath} is a directory. Ignoring.");

                    return null;
                } else if (File.Exists(filePath))
                {
                    if (File.Exists(filePath))
                    {
                        string jsonData = null;

                        using (StreamReader reader = new(filePath))
                        {
                            jsonData = reader.ReadToEnd();
                        }

                        data = JsonToDataTypeCategoryInfo(jsonData);
                    } else
                    {
                        Console.WriteLine($"Failed to write JSON data to filesystem. File path non-existent: {filePath}");
                    }
                } else
                {
                    Console.WriteLine($"Path is not a valid file: {filePath}. Ignoring.");
                }
            } catch (Exception e)
            {
                Console.WriteLine($"Failed to convert JSON to type ConfigTypeModelContainer: {filePath}. Exception: {e.StackTrace}");
            }

            return data;
        }


        // private static methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        private static string NormalizeData(string jsonData)
        {
            jsonData = jsonData.Replace("'", "\"").Replace("True", "true").Replace("False", "false");

            return jsonData;
        }
    }
}
