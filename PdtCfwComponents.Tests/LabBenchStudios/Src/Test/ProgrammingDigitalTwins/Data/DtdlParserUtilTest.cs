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

namespace LabBenchStudios.Pdt.Test.Data
{
    public class DtdlParserUtilTest
    {
        private const string DTDL_MODEL_FILEPATH = "../../../../PdtCfwComponents/LabBenchStudios/Models/Dtdl/";

        private const string RESIDENTIAL_STRUCTURE_DTDL_FILENAME = "Lbs_Pdt_SampleDtdl_ResidentialStructure.json";
        private const string INDOOR_HEATING_SYSTEM_DTDL_FILENAME = "Lbs_Pdt_SampleDtdl_IndoorHeatingSystem.json";
        private const string INDOOR_ROOM_STATE_DTDL_FILENAME     = "Lbs_Pdt_SampleDtdl_IndoorRoomState.json";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckValidityOfResidentialStructureDtdl()
        {
            Assert.That(
                this.RunDtdlValidation(DTDL_MODEL_FILEPATH, RESIDENTIAL_STRUCTURE_DTDL_FILENAME), Is.True);
        }

        [Test]
        public void CheckValidityOfIndoorHeatingSystemDtdl()
        {
            Assert.That(
                this.RunDtdlValidation(DTDL_MODEL_FILEPATH, INDOOR_HEATING_SYSTEM_DTDL_FILENAME), Is.True);
        }

        [Test]
        public void CheckValidityOfIndoorRoomStateDtdl()
        {
            Assert.That(
                this.RunDtdlValidation(DTDL_MODEL_FILEPATH, INDOOR_ROOM_STATE_DTDL_FILENAME), Is.True);
        }

        private bool RunDtdlValidation(string modelPath, string modelName)
        {
            string jsonData = DtdlParserUtil.LoadJsonFile(modelPath + modelName);

            Console.WriteLine($"DTDL JSON for Model {modelName}:\n{jsonData}");

            bool isValid = DtdlParserUtil.IsValidDtdlJsonData(jsonData);

            Console.WriteLine($"{modelName} Is Valid DTDL: " + isValid);

            return isValid;
        }
    }
}