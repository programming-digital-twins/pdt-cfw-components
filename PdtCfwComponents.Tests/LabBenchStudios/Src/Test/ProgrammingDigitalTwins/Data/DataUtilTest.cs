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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using Newtonsoft.Json;

namespace LabBenchStudios.Pdt.Test.Data
{
    public class DataUtilTest
    {
        private float val = 22.4f;

        private string edaSensorDataJson =
            "{\n    \"timeOffsetSeconds\": 0.0,\n    \"timeStamp\": \"2024-02-12T04:49:21.109944+00:00\",\n    \"hasError\": false,\n    \"name\": \"TempSensor\",\n    \"typeID\": 1013,\n    \"statusCode\": 0,\n    \"latitude\": 0.0,\n    \"longitude\": 0.0,\n    \"elevation\": 0.0,\n    \"locationID\": \"edgedevice001\",\n    \"typeCategoryID\": 0,\n    \"deviceID\": \"edgedevice001\",\n    \"value\": 25.077374439186343\n}";

        private string edaSensorDataWithDtmiJson =
            "{\n    \"timeOffsetSeconds\": 0.0,\n    \"timeStamp\": \"2024-02-12T04:49:21.109944+00:00\",\n    \"hasError\": false,\n    \"name\": \"TempSensor\",\n    \"typeID\": 1013,\n    \"statusCode\": 0,\n    \"latitude\": 0.0,\n    \"longitude\": 0.0,\n    \"elevation\": 0.0,\n    \"locationID\": \"edgedevice001\",\n    \"typeCategoryID\": 0,\n    \"deviceID\": \"edgedevice001\",\n    \"modelID\": \"dtmi:com:labbenchstudios:pdt:env_sensor_data;1\",\n    \"value\": 25.077374439186343\n}";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ConvertSimpleDataDataToJsonAndBack()
        {
            SimpleData data = new SimpleData("Temperature", "Temp001", ConfigConst.ENV_TYPE_CATEGORY, ConfigConst.TEMP_SENSOR_TYPE);

            string jsonData = DataUtilTest.SimpleDataToJson(data);

            Console.WriteLine(data.ToString());
            Console.WriteLine(jsonData);

            SimpleData data2 = DataUtilTest.JsonToSimpleData(jsonData);

            Console.WriteLine(data2.ToString());

            //Assert.Equals(data2.GetDeviceCategory(), data.GetDeviceCategory());
        }

        [Test]
        public void ConvertSensorDataToJsonAndBack()
        {
            SensorData data = new SensorData("Temperature", "Temp001", ConfigConst.ENV_TYPE_CATEGORY, ConfigConst.TEMP_SENSOR_TYPE);
            data.SetValue(val);

            string jsonData = DataUtil.SensorDataToJson(data);
            //string jsonData = DataUtil.SensorDataToJsonUpdated(data);

            Console.WriteLine(data.ToString());
            Console.WriteLine(jsonData);

            SensorData data2 = DataUtil.JsonToSensorData(jsonData);
            //SensorData data2 = DataUtil.JsonToSensorDataUpdated(jsonData);

            Console.WriteLine(data2.ToString());

           //Assert.Equals(data2.GetValue(), data.GetValue());
        }

        [Test]
        public void ConvertIotDataContextWithValuesToJsonAndBack()
        {
            IotDataContextWithValues data = new IotDataContextWithValues("WindTurbine", "device001", ConfigConst.UTILITY_SYSTEM_TYPE_CATEGORY, ConfigConst.WIND_TURBINE_SYSTEM_TYPE);

            DataValueContainer rotationalSpeedValues = new DataValueContainer();
            rotationalSpeedValues.SetValue(50.0f);
            rotationalSpeedValues.SetRangeNominalFloor(5.0f);
            rotationalSpeedValues.SetRangeNominalCeiling(100.0f);
            data.AddDataValues("rotationalSpeed", rotationalSpeedValues);

            DataValueContainer powerOutputValues = new DataValueContainer();
            powerOutputValues.SetValue(25000.0f);
            powerOutputValues.SetRangeNominalFloor(5000.0f);
            powerOutputValues.SetRangeNominalCeiling(50000.0f);
            data.AddDataValues("powerOutput", powerOutputValues);

            string jsonData = DataUtil.IotDataContextWithValuesToJson(data);

            Console.WriteLine(data.ToString());
            Console.WriteLine(jsonData);

            IotDataContextWithValues data2 = DataUtil.JsonToIotDataContextWithValues(jsonData);
            //SensorData data2 = DataUtil.JsonToSensorDataUpdated(jsonData);

            Console.WriteLine(data2.ToString());

            //Assert.Equals(data2.GetValue(), data.GetValue());
        }

        [Test]
        public void ConvertSampleSerializedJsonToSensorData()
        {
            string jsonData = "{\r\n    \"timeStamp\": \"2019-01-20T15:38:35.123123\",\r\n    \"hasError\": false,\r\n    \"name\": \"FooBar SensorData\",\r\n    \"typeID\": 1001,\r\n    \"statusCode\": 0,\r\n    \"latitude\": 0.0,\r\n    \"longitude\": 0.0,\r\n    \"elevation\": 0.0,\r\n    \"locationID\": \"constraineddevice001\",\r\n    \"isResponse\": false,\r\n    \"command\": 0,\r\n    \"stateData\": null,\r\n    \"value\": 15.0\r\n}";

            SensorData data = DataUtil.JsonToSensorData(jsonData);

            Console.WriteLine("Original:\n" + jsonData);
            Console.WriteLine("New:\n" + data.ToString());
        }

        [Test]
        public void ConvertEDASerializedJsonToSensorData()
        {
            SensorData data = DataUtil.JsonToSensorData(edaSensorDataJson);

            Console.WriteLine("Original:\n" + edaSensorDataJson);
            Console.WriteLine("New:\n" + data.ToString());
        }

        [Test]
        public void ConvertEDAWithDtmiSerializedJsonToSensorData()
        {
            SensorData data = DataUtil.JsonToSensorData(edaSensorDataWithDtmiJson);

            Console.WriteLine("Original:\n" + edaSensorDataWithDtmiJson);
            Console.WriteLine("New:\n" + data.ToString());
        }

        // private methods

        private static SimpleData JsonToSimpleData(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                SimpleData simpleData = JsonConvert.DeserializeObject<SimpleData>(jsonData);

                return simpleData;
            }

            return null;
        }

        private static string SimpleDataToJson(SimpleData simpleData)
        {
            if (simpleData != null)
            {
                string jsonData = JsonConvert.SerializeObject(simpleData, Formatting.Indented);

                return jsonData;
            }

            return null;
        }

    }
}