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

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using LabBenchStudios.Pdt.Common;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace LabBenchStudios.Pdt.Data
{
    public static class DataTypeConfigUtil
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

        public static string DataTypeCategoryInfoToJson(DataTypeCategoryInfo data)
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
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static DataTypeCategoryInfo JsonToDataTypeCategoryInfo(string jsonData)
        {
            jsonData = NormalizeData(jsonData);
            
            DataTypeCategoryInfo data = new DataTypeCategoryInfo();

            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

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
