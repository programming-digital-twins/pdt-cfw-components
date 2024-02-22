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

namespace LabBenchStudios.Pdt.Test.Data
{
    public class DtdlParserUtilTest
    {
        private static readonly string DTDL_TEST_MODEL_FILEPATH =
            "../../../../PdtCfwComponents/LabBenchStudios/Models/Dtdl/";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckValidityOfResidentialStructureDtdl()
        {
            // This test requires TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL during the load process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelConst.CONTEXT_RESIDENTIAL_STRUCTURE_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfHeatingSystemDtdl()
        {
            // This test requires TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL during the load process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelConst.TELEMETRY_ENV_SENSORS_DTDL_MODEL,
                ModelConst.CONTROLLER_HEATING_ZONE_DTDL_MODEL,
                ModelConst.CONTROLLER_HUMIDIFIER_DTDL_MODEL,
                ModelConst.CONTROLLER_THERMOSTAT_DTDL_MODEL,
                ModelConst.CONTROLLER_FLUID_PUMP_DTDL_MODEL,
                ModelConst.CONTEXT_HEATING_SYSTEM_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfHeatingZoneDtdl()
        {
            // This test requires TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL during the load process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelConst.TELEMETRY_ENV_SENSORS_DTDL_MODEL,
                ModelConst.CONTROLLER_HEATING_ZONE_DTDL_MODEL,
                ModelConst.CONTROLLER_THERMOSTAT_DTDL_MODEL,
                ModelConst.CONTROLLER_FLUID_PUMP_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfHumidifierDtdl()
        {
            // This test requires TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL during the load process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelConst.TELEMETRY_ENV_SENSORS_DTDL_MODEL,
                ModelConst.CONTROLLER_HUMIDIFIER_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfThermostatDtdl()
        {
            // This test requires TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL during the load process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelConst.TELEMETRY_ENV_SENSORS_DTDL_MODEL,
                ModelConst.CONTROLLER_THERMOSTAT_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfInteriorRoomStateDtdl()
        {
            // This test requires both TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL and
            // TELEMETRY_ENV_SENSORS_DTDL_MODEL during the parse process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelConst.TELEMETRY_ENV_SENSORS_DTDL_MODEL,
                ModelConst.CONTROLLER_HUMIDIFIER_DTDL_MODEL,
                ModelConst.CONTROLLER_THERMOSTAT_DTDL_MODEL,
                ModelConst.CONTEXT_INTERIOR_ROOM_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfIotModelContextDtdl()
        {
            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL),
                Is.True);
        }

        [Test]
        public void CheckValidityOfDeviceSystemPerformanceTelemetryDtdl()
        {
            // This test requires TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL during the load process
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelConst.TELEMETRY_DEVICE_SYS_PERF_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfEnvSensorTelemetryDtdl()
        {
            // This file extends TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelConst.TELEMETRY_ENV_SENSORS_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfFluidPumpTelemetryDtdl()
        {
            // This file extends TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelConst.TELEMETRY_ENV_SENSORS_DTDL_MODEL,
                ModelConst.CONTROLLER_FLUID_PUMP_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfPowerWindmillTelemetryDtdl()
        {
            // This file extends TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelConst.TELEMETRY_ENV_SENSORS_DTDL_MODEL,
                ModelConst.CONTROLLER_POWER_WINDMILL_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfThermostatControllerDtdl()
        {
            // This file extends TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>
            {
                ModelConst.TEMPLATE_IOT_MODEL_CONTEXT_DTDL_MODEL,
                ModelConst.TELEMETRY_ENV_SENSORS_DTDL_MODEL,
                ModelConst.CONTROLLER_THERMOSTAT_DTDL_MODEL
            };

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_TEST_MODEL_FILEPATH, modelList),
                Is.True);
        }

        private bool RunDtdlValidation(string modelPath, string modelName)
        {
            string jsonData = DtdlParserUtil.LoadJsonFile(modelPath + modelName);

            Console.WriteLine($"DTDL JSON for Model {modelName}:\n{jsonData}");

            bool isValid = DtdlParserUtil.IsValidDtdlJsonData(jsonData);

            Console.WriteLine($"{modelName} Is Valid DTDL: " + isValid);

            return isValid;
        }

        private bool RunDtdlValidation(string modelPath, IEnumerable<string> modelList)
        {
            var modelJsonList = new List<string>();

            foreach (var modelName in modelList)
            {
                string jsonData = DtdlParserUtil.LoadJsonFile(modelPath + modelName);
                modelJsonList.Add(jsonData);

                Console.WriteLine($"DTDL JSON for Model {modelName}:\n{jsonData}");
            }

            bool isValid = DtdlParserUtil.IsValidDtdlJsonData(modelJsonList);

            Console.WriteLine($"{modelJsonList} Collectively Contains Valid DTDL: " + isValid);

            return isValid;
        }
    }
}