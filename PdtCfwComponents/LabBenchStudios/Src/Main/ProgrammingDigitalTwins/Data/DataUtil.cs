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
using Newtonsoft.Json.Serialization;

///
///
///
namespace LabBenchStudios.Pdt.Data
{
 
    /// <summary>
    /// 
    /// </summary>
    public static class DataUtil
    {
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ActuatorDataToJson(ActuatorData data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ConnectionStateDataToJson(ConnectionStateData data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DataCacheEntryContainerToJson(DataCacheEntryContainer data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DataCacheEntryListToJson(List<DataCacheEntryContainer> data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string IotDataContextToJson(IotDataContext data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string IotDataContextWithValuesToJson(IotDataContextWithValues data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string MessageDataToJson(MessageData data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string RequestResponseDataToJson(RequestResponseData data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SystemPerformanceDataToJson(SystemPerformanceData data)
        {
            string jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return jsonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static ActuatorData JsonToActuatorData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            ActuatorData data = new ActuatorData();
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
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static ConnectionStateData JsonToConnectionStateData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            ConnectionStateData data = new ConnectionStateData();
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
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static DataCacheEntryContainer JsonToDataCacheEntryContainer(string jsonData)

        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            DataCacheEntryContainer data = new DataCacheEntryContainer();
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
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static List<DataCacheEntryContainer> JsonToDataCacheEntryList(string jsonData)

        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            List<DataCacheEntryContainer> data = new List<DataCacheEntryContainer>();
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
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static IotDataContext JsonToIotDataContext(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            IotDataContext data = new IotDataContext();
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
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static IotDataContextWithValues JsonToIotDataContextWithValues(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            IotDataContextWithValues data = new IotDataContextWithValues();
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
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static MessageData JsonToMessageData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            MessageData data = new MessageData();
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
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static RequestResponseData JsonToRequestResponseData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            RequestResponseData data = new RequestResponseData();
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
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static SensorData JsonToSensorData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            SensorData data = new SensorData();
            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            // maintain backwards compatability - previous version of SensorData
            // does not include the internal structure DataValuesContainer - this
            // call ensures it is updated with the de-serialized value(s)
            data.UpdateValues();
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static SystemPerformanceData JsonToSystemPerformanceData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) { return null; }

            jsonData = NormalizeData(jsonData);

            SystemPerformanceData data = new SystemPerformanceData();
            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
                Formatting = Formatting.Indented
            });

            return data;
        }


        // alternative

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SensorDataToJsonUpdated(SensorData data)
        {
            if (data != null)
            {
                string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

                return jsonData;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static SensorData JsonToSensorDataUpdated(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                SensorData sensorData = JsonConvert.DeserializeObject<SensorData>(jsonData);

                return sensorData;
            }

            return null;
        }


        // private

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
