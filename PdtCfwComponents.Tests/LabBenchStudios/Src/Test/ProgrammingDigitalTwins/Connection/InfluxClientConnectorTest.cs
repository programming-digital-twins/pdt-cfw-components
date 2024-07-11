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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Connection;
using LabBenchStudios.Pdt.Data;
using static System.Net.Mime.MediaTypeNames;

namespace ProgrammingDigitalTwins.Test.Connection
{
    public class InfluxClientConnectorTest
    {
        private string hostName = "192.168.4.105";
        private int hostPort = 8086;

        private string clientToken = "j0kL4ObhOvw6gLPeL2-euCggE2i3m35ikTcgTyZu26CySlPtFEXv7fptWaQyXza5beCLURc7pWU0Gg13ojiyAw==";
        private string orgID = "5ddf6f703316165d";

        private IPersistenceConnector dbClient = null;

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public void StoreSensorData()
        {
            // TODO: Implement this
        }

        [Test]
        public void LoadSensorData()
        {
            this.dbClient =
                new InfluxClientConnector(
                    this.hostName, this.hostPort, this.clientToken, this.orgID, null);

            this.dbClient.ConnectClient();

            DateTime startDate = DateTime.Now;
            startDate.AddHours(-5);
            DateTime endDate = DateTime.Now;

            ResourceNameContainer resource = new ResourceNameContainer();

            List<SensorData> dataList =
                this.dbClient.LoadSensorData(resource, ConfigConst.TEMP_SENSOR_TYPE, startDate, endDate);

            if (dataList != null)
            {
                foreach (SensorData data in dataList)
                {
                    string jsonData = DataUtil.SensorDataToJson(data);
                }

                if (dataList.Count == 0)
                {
                    Console.WriteLine("No sensor data retrieved.");
                }
            } else
            {
                Console.WriteLine("No SensorData found in DB.");
            }

            this.dbClient.DisconnectClient();
        }
    }
}