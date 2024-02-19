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
using System.Linq;
using DTDLParser;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LabBenchStudios.Pdt.Data
{

    /**
     * This class is based on DataUtil, and will - eventually - be updated to support
     * serializing / deserializing the core data container objects to / from DTDL.
     * 
     */
    public static class DtdlParserUtil
    {
        static DefaultContractResolver camelCaseResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = true
            }
        };

        public static string LoadJsonFile(string fileName)
        {
            if (fileName != null && fileName.Length > 0)
            {
                try
                {
                    Console.WriteLine($"Attempting to load JSON file: {fileName}");

                    using (StreamReader streamReader = new StreamReader(fileName))
                    {
                        string jsonData = streamReader.ReadToEnd();

                        return jsonData;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"JSON file cannot be loaded: {fileName}. Exception: {ex}");
                }
            }
            else
            {
                Console.WriteLine($"JSON file is invalid (null or empty). Ignoring");
            }

            return null;
        }

        public static bool IsValidDtdlJsonFile(string fileName)
        {
            string jsonData = LoadJsonFile(fileName);

            if (jsonData != null && jsonData.Length > 0)
            {
                return IsValidDtdlJsonData(jsonData);
            }
            else
            {
                Console.WriteLine($"DTDL file returned empty JSON data: {fileName}");

                return false;
            }
        }

        public static bool IsValidDtdlJsonData(string jsonData)
        {
            if (jsonData != null && jsonData.Length > 0)
            {
                try
                {
                    ModelParser modelParser = new();

                    var objectModel = modelParser.Parse(jsonData);

                    return true;
                }
                catch (ResolutionException ex)
                {
                    Console.WriteLine($"DTDL model is referentially incomplete. Exception: {ex}");
                }
                catch (ParsingException ex)
                {
                    Console.WriteLine($"DTDL model cannot be parsed - invalid. Exception: {ex}");
                }
            }

            return false;
        }

        // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
        // ID's simply need to be part of the IEnumerable passed to the parser
        public static bool IsValidDtdlJsonData(IEnumerable<string> jsonDataList)
        {
            if (jsonDataList != null && jsonDataList.Count() > 0)
            {
                try
                {
                    ModelParser modelParser = new();
                    
                    var objectModel = modelParser.Parse(jsonDataList);

                    return true;
                }
                catch (ResolutionException ex)
                {
                    Console.WriteLine($"DTDL model is referentially incomplete. Exception: {ex}");
                }
                catch (ParsingException ex)
                {
                    Console.WriteLine($"DTDL model cannot be parsed - invalid. Exception: {ex}");
                }
            }

            return false;
        }

        public static string ActuatorDataToJson(ActuatorData data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        public static string ConnectionStateDataToJson(ConnectionStateData data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        public static string IotDataContextToJson(IotDataContext data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        public static string MessageDataToJson(MessageData data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        public static string SensorDataToJsonUpdated(SensorData data)
        {
            if (data != null)
            {
                string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
                
                return jsonData;
            }

            return null;
        }

        public static SimpleData JsonToSimpleData(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                SimpleData simpleData = JsonConvert.DeserializeObject<SimpleData>(jsonData);

                return simpleData;
            }

            return null;
        }

        public static string SimpleDataToJson(SimpleData simpleData)
        {
            if (simpleData != null)
            {
                string jsonData = JsonConvert.SerializeObject(simpleData, Formatting.Indented);

                return jsonData;
            }

            return null;
        }

        public static SensorData JsonToSensorDataUpdated(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                SensorData sensorData = JsonConvert.DeserializeObject<SensorData>(jsonData);

                return sensorData;
            }

            return null;
        }

        public static string SensorDataToJson(SensorData data)
        {
            Console.WriteLine(data);

            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        public static string SystemPerformanceDataToJson(SystemPerformanceData data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        public static ActuatorData JsonToActuatorData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            //ActuatorData data = JsonConvert.DeserializeObject<ActuatorData>(jsonData);
            ActuatorData data = new ActuatorData();
            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return data;
        }

        public static ConnectionStateData JsonToConnectionStateData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            //ConnectionStateData data = JsonConvert.DeserializeObject<ConnectionStateData>(jsonData);
            ConnectionStateData data = new ConnectionStateData();
            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return data;
        }

        public static IotDataContext JsonToIotDataContext(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            //IotDataContext data = JsonConvert.DeserializeObject<IotDataContext>(jsonData);
            IotDataContext data = new IotDataContext();
            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return data;
        }

        public static MessageData JsonToMessageData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            //MessageData data = JsonConvert.DeserializeObject<MessageData>(jsonData);
            MessageData data = new MessageData();
            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return data;
        }

        public static SensorData JsonToSensorData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            //SensorData data = JsonConvert.DeserializeObject<SensorData>(jsonData);
            SensorData data = new SensorData();
            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return data;
        }

        public static SystemPerformanceData JsonToSystemPerformanceData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            //SystemPerformanceData data = JsonConvert.DeserializeObject<SystemPerformanceData>(jsonData);
            SystemPerformanceData data = new SystemPerformanceData();
            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return data;
        }

        private static string NormalizeData(string jsonData)
        {
            jsonData = jsonData.Replace("'", "\"").Replace("True", "true").Replace("False", "false");

            return jsonData;
        }
    }

}
