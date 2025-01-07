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

using LabBenchStudios.Pdt.Model;

namespace LabBenchStudios.Pdt.Test.Model
{
    public class ModelParserUtilTest
    {
        private static readonly string DTDL_TEST_MODEL_FILEPATH =
            "../../../../PdtCfwComponents/LabBenchStudios/Models/Dtdl/";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DisplayDtdlItemsFromModel()
        {
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL
            };

            foreach (var modelFile in modelList)
            {
                string model = ModelParserUtil.LoadDtdlFile(DTDL_TEST_MODEL_FILEPATH + modelFile);

                ModelParserUtil.DisplayDtdlItems(model);
            }
        }

        [Test]
        public void LoadAllDtmlFromPath()
        {
            var modelList = ModelParserUtil.LoadDtdlModelsFromPath(DTDL_TEST_MODEL_FILEPATH);

            //Assert.That(modelList, Is.InstanceOf<IReadOnlyDictionary>());
        }

        [Test]
        public void LoadAllDtmlInterfacesFromPath()
        {
            var modelList = ModelParserUtil.LoadAllDtdlInterfaces(DTDL_TEST_MODEL_FILEPATH);

            //Assert.That(modelList, Is.InstanceOf<IReadOnlyDictionary>());
        }

        [Test]
        public void CheckValidityOfIotModelContextDtdl()
        {
            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL),
                Is.True);
        }

        [Test]
        public void CheckValidityOfHeatingSystemContextDtdl()
        {
            // This test requires BASE_IOT_MODEL_CONTEXT_DTDL_MODEL during the load process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelNameUtil.COMPONENT_ENV_SENSORS_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_FLUID_PUMP_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_HEATING_ZONE_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_HUMIDIFIER_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_THERMOSTAT_DTDL_MODEL,
                ModelNameUtil.CONTEXT_HEATING_SYSTEM_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfInteriorRoomContextDtdl()
        {
            // This test requires both BASE_IOT_MODEL_CONTEXT_DTDL_MODEL and
            // COMPONENT_ENV_SENSORS_DTDL_MODEL during the parse process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelNameUtil.COMPONENT_ENV_SENSORS_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_HUMIDIFIER_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_THERMOSTAT_DTDL_MODEL,
                ModelNameUtil.CONTEXT_INTERIOR_ROOM_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfResidentialStructureContextDtdl()
        {
            // This test requires BASE_IOT_MODEL_CONTEXT_DTDL_MODEL during the load process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelNameUtil.CONTEXT_RESIDENTIAL_STRUCTURE_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfHeatingZoneControllerDtdl()
        {
            // This test requires BASE_IOT_MODEL_CONTEXT_DTDL_MODEL during the load process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelNameUtil.COMPONENT_ENV_SENSORS_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_FLUID_PUMP_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_HEATING_ZONE_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_THERMOSTAT_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfHumidifierControllerDtdl()
        {
            // This test requires BASE_IOT_MODEL_CONTEXT_DTDL_MODEL during the load process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelNameUtil.COMPONENT_ENV_SENSORS_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_HUMIDIFIER_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfFluidPumpControllerDtdl()
        {
            // This file extends BASE_IOT_MODEL_CONTEXT_DTDL_MODEL ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelNameUtil.COMPONENT_ENV_SENSORS_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_FLUID_PUMP_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfPowerWindmillControllerDtdl()
        {
            // This file extends BASE_IOT_MODEL_CONTEXT_DTDL_MODEL ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelNameUtil.COMPONENT_ENV_SENSORS_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_WIND_TURBINE_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfThermostatControllerDtdl()
        {
            // This file extends BASE_IOT_MODEL_CONTEXT_DTDL_MODEL ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelNameUtil.COMPONENT_ENV_SENSORS_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_THERMOSTAT_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfEdgeComputeDeviceControllerDtdl()
        {
            // This file extends BASE_IOT_MODEL_CONTEXT_DTDL_MODEL ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelNameUtil.COMPONENT_DEVICE_SYS_PERF_DTDL_MODEL,
                ModelNameUtil.COMPONENT_ENV_SENSORS_DTDL_MODEL,
                ModelNameUtil.CONTROLLER_EDGE_COMPUTE_DEVICE_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfDeviceSystemPerformanceComponentDtdl()
        {
            // This test requires BASE_IOT_MODEL_CONTEXT_DTDL_MODEL during the load process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelNameUtil.COMPONENT_DEVICE_SYS_PERF_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfEnvironmentalSensorsComponentDtdl()
        {
            // This file extends BASE_IOT_MODEL_CONTEXT_DTDL_MODEL ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelNameUtil.BASE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelNameUtil.COMPONENT_ENV_SENSORS_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        private bool RunDtdlValidation(string modelPath, string modelName)
        {
            string jsonData = ModelParserUtil.LoadDtdlFile(modelPath + modelName);

            Console.WriteLine($"DTDL JSON for Model {modelName}:\n{jsonData}");

            bool isValid = ModelParserUtil.IsValidDtdlJsonData(jsonData);

            Console.WriteLine($"{modelName} Is Valid DTDL: " + isValid);

            return isValid;
        }

        private bool RunDtdlValidation(string modelPath, IEnumerable<string> modelList)
        {
            var modelJsonList = new List<string>();

            foreach (var modelName in modelList)
            {
                string jsonData = ModelParserUtil.LoadDtdlFile(modelPath + modelName);
                modelJsonList.Add(jsonData);

                Console.WriteLine($"DTDL JSON for Model {modelName}:\n{jsonData}");
            }

            bool isValid = ModelParserUtil.IsValidDtdlJsonData(modelJsonList);

            Console.WriteLine($"{modelJsonList} Collectively Contains Valid DTDL: " + isValid);

            return isValid;
        }
    }
}