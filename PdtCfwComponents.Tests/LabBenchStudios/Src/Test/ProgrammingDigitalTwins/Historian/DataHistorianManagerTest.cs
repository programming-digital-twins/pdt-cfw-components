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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Connection;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Historian;
using LabBenchStudios.Pdt.Plexus;
using System;

namespace LabBenchStudios.Pdt.Test.Historian
{
    public class DataHistorianManagerTest
    {
        private IDataHistorian dataHistorian = null;

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Teardown()
        {
        }
        
        /*
        [Test]
        public void CreateDataHistorianPlayer()
        {
            IDataHistorianPlayer player = this.dataHistorian.CreateDataHistorianPlayer();

            player.SetDisplayName("My Historian");

            Console.WriteLine($"Player info. Name: {player.GetCacheName()}. File: {player.GetCacheFileName()}. Path: {player.GetCacheStorageUri()}. Display name: {player.GetDisplayName()}");
        }
        */

        [Test]
        public void CreatePlayerAndStoreSampleData()
        {
            //IDataHistorianPlayer player = this.dataHistorian.CreateDataHistorianPlayer();
            
            IDataHistorianPlayer player = EventProcessor.GetInstance().GetDataHistorianPlayer();

            player.SetDisplayName("My Historian");

            Console.WriteLine($"Player info. Name: {player.GetCacheName()}. File: {player.GetCacheFileName()}. Path: {player.GetCacheStorageUri()}. Display name: {player.GetDisplayName()}");

            FilePersistenceConnector filePersistenceConnector = new FilePersistenceConnector();
            player.SetDataStorer(filePersistenceConnector);

            SensorData sensorData = new SensorData();
            sensorData.SetValue(21.0f);
            player.HandleSensorData(sensorData);

            sensorData = new SensorData();
            sensorData.SetValue(25.0f);
            player.HandleSensorData(sensorData);

            sensorData = new SensorData();
            sensorData.SetValue(27.0f);
            player.HandleSensorData(sensorData);

            sensorData = new SensorData();
            sensorData.SetValue(15.0f);
            player.HandleSensorData(sensorData);

            sensorData = new SensorData();
            sensorData.SetValue(18.0f);
            player.HandleSensorData(sensorData);

            bool success = player.StoreHistorianCache();

            Console.WriteLine($"Stored cache to file: Success = {success}. File = {player.GetCacheFileName()}");
        }
    }
}