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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Test.Data
{
    public class DtdlParserUtilTest
    {
        private const string DTDL_MODEL_FILEPATH = "../../../../PdtCfwComponents/LabBenchStudios/Models/Dtdl/";

        private const string CONTEXT_RESIDENTIAL_STRUCTURE_DTDL_FILENAME = "Lbs_Pdt_Context_ResidentialStructure.json";
        private const string CONTEXT_INDOOR_HEATING_SYSTEM_DTDL_FILENAME = "Lbs_Pdt_Context_IndoorHeatingSystem.json";
        private const string CONTEXT_INDOOR_ROOM_STATE_DTDL_FILENAME     = "Lbs_Pdt_Context_IndoorRoomState.json";

        private const string TEMPLATE_IOT_DATA_CONTEXT_DTDL_FILENAME     = "Lbs_Pdt_Template_IotDataContext.json";

        // the following DTDL files extend from TEMPLATE_IOT_DATA_CONTEXT_DTDL_FILENAME
        private const string TELEMETRY_DEVICE_SYS_PERF_DTDL_FILENAME = "Lbs_Pdt_Telemetry_DeviceSystemPerformance.json";
        private const string TELEMETRY_ENV_SENSORS_DTDL_FILENAME     = "Lbs_Pdt_Telemetry_EnvironmentalSensors.json";
        private const string TELEMETRY_FLUID_PUMP_DTDL_FILENAME      = "Lbs_Pdt_Telemetry_FluidPump.json";
        private const string TELEMETRY_POWER_WINDMILL_DTDL_FILENAME  = "Lbs_Pdt_Telemetry_PowerWindmill.json";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckValidityOfResidentialStructureDtdl()
        {
            Assert.That(
                this.RunDtdlValidation(DTDL_MODEL_FILEPATH, CONTEXT_RESIDENTIAL_STRUCTURE_DTDL_FILENAME),
                Is.True);
        }

        [Test]
        public void CheckValidityOfIndoorHeatingSystemDtdl()
        {
            Assert.That(
                this.RunDtdlValidation(DTDL_MODEL_FILEPATH, CONTEXT_INDOOR_HEATING_SYSTEM_DTDL_FILENAME),
                Is.True);
        }

        [Test]
        public void CheckValidityOfIndoorRoomStateDtdl()
        {
            Assert.That(
                this.RunDtdlValidation(DTDL_MODEL_FILEPATH, CONTEXT_INDOOR_ROOM_STATE_DTDL_FILENAME),
                Is.True);
        }

        [Test]
        public void CheckValidityOfIotDataContextDtdl()
        {
            Assert.That(
                this.RunDtdlValidation(DTDL_MODEL_FILEPATH, TEMPLATE_IOT_DATA_CONTEXT_DTDL_FILENAME),
                Is.True);
        }

        [Test]
        public void CheckValidityOfDeviceSystemPerformanceTelemetryDtdl()
        {
            // This file extends TEMPLATE_IOT_DATA_CONTEXT_DTDL_FILENAME ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>();
            modelList.Add(TEMPLATE_IOT_DATA_CONTEXT_DTDL_FILENAME);
            modelList.Add(TELEMETRY_DEVICE_SYS_PERF_DTDL_FILENAME);

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfEnvSensorTelemetryDtdl()
        {
            // This file extends TEMPLATE_IOT_DATA_CONTEXT_DTDL_FILENAME ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>();
            modelList.Add(TEMPLATE_IOT_DATA_CONTEXT_DTDL_FILENAME);
            modelList.Add(TELEMETRY_ENV_SENSORS_DTDL_FILENAME);

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfFluidPumpTelemetryDtdl()
        {
            // This file extends TEMPLATE_IOT_DATA_CONTEXT_DTDL_FILENAME ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>();
            modelList.Add(TEMPLATE_IOT_DATA_CONTEXT_DTDL_FILENAME);
            modelList.Add(TELEMETRY_FLUID_PUMP_DTDL_FILENAME);

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_MODEL_FILEPATH, modelList),
                Is.True);
        }

        [Test]
        public void CheckValidityOfPowerWindmillTelemetryDtdl()
        {
            // This file extends TEMPLATE_IOT_DATA_CONTEXT_DTDL_FILENAME ID
            //
            // NOTE: The order of the list doesn't matter to the DTDL parser - all extended
            // ID's simply need to be part of the IEnumerable passed to the parser
            var modelList = new List<string>();
            modelList.Add(TEMPLATE_IOT_DATA_CONTEXT_DTDL_FILENAME);
            modelList.Add(TELEMETRY_POWER_WINDMILL_DTDL_FILENAME);

            Assert.That(
                this.RunDtdlValidation(
                    DTDL_MODEL_FILEPATH, modelList),
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