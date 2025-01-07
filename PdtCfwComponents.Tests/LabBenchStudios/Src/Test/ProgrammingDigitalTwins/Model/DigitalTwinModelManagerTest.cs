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
using LabBenchStudios.Pdt.Model;

namespace LabBenchStudios.Pdt.Test.Model
{
    public class DigitalTwinModelManagerTest
    {

        private DigitalTwinModelManager dtModelManager = null;

        [SetUp]
        public void Setup()
        {
            this.dtModelManager = new DigitalTwinModelManager();
            this.dtModelManager.UpdateModelFilePaths(ConfigConst.TEST_DIGITAL_TWIN_MODEL_FILE_PATH);
            //this.sysModelManager.BuildModelData();
        }

        [Test]
        public void DisplayDtmiKeysFromLoadedDtdl()
        {
            string dtmiAggregateString = string.Join("\n", dtModelManager.GetAllDtmiValues());
            Console.WriteLine(dtmiAggregateString);
        }

        [Test]
        public void CreateDigitalTwinHeatingSystemState()
        {
            SensorData iotData =
                new SensorData(
                    "test", "testDevice001",
                    ConfigConst.UTILITY_SYSTEM_TYPE_CATEGORY, ConfigConst.HEATING_SYSTEM_TYPE);

            iotData.SetLocationID("testDevice001_LocationA");

            Console.WriteLine($"IoT Data Context: {iotData.ToString()}");

            DigitalTwinDataSyncKey dataSyncKey = new DigitalTwinDataSyncKey(iotData);

            Console.WriteLine($"DT Data Sync Key: {dataSyncKey.ToString()}");

            DigitalTwinModelState dtState =
                this.dtModelManager.CreateModelState(
                    dataSyncKey,
                    ModelNameUtil.DtmiControllerEnum.HeatingSystem, null);

            Console.WriteLine($"DT Model State: {dtState.ToString()}");
        }

    }
}