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

using LabBenchStudios.Pdt.Connection;

namespace LabBenchStudios.Pdt.Test.Connection
{
    public class MqttClientConnectorTest
    {
        private string hostName = "localhost";
        private int hostPort = 1883;

        private IPubSubConnector mqttClient = null;

        [SetUp]
        public void Setup()
        {
            this.mqttClient = new MqttClientManagedConnector(this.hostName, this.hostPort, null, null);

            this.mqttClient.ConnectClient();
        }

        [TearDown]
        public void Teardown()
        {
            this.mqttClient.DisconnectClient();
        }

        [Test]
        public void ConnectAndSubscribe()
        {
            Assert.True(mqttClient != null);

            // TODO: Implement this
        }

        [Test]
        public void ConnectAndPublish()
        {
            Assert.True(mqttClient != null);

            // TODO: Implement this
        }
    }
}