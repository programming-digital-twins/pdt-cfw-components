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
using System.Threading;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Connection;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Historian;
using LabBenchStudios.Pdt.Plexus;
using LabBenchStudios.Pdt.Util;

namespace LabBenchStudios.Pdt.Test.Historian
{
    public class DataHistorianManagerTest
    {
        private IDataHistorian dataHistorian = null;
        private string sampleCacheName = "PDT_DataHistorianCache_TEST";

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public void RunPlaybackRoutine()
        {
            IDataHistorianPlayer player = EventProcessor.GetInstance().GetDataHistorianPlayer(this.sampleCacheName);

            player.SetEventListener(new DefaultSystemStatusEventListener());
            player.SetDisplayName("My Playback Historian");

            // enable playback
            player.SetPlaybackEnabledFlag(true);

            // set to double time (delay factor of 0.5f - 1.0f is normal time)
            player.SetPlaybackDelayFactor(0.5f);

            this.WritePlayerInfoToConsole("Loaded cache from file", player);
            this.WritePlayerInfoToConsole("Starting playback.", player);

            player.Play();

            try
            {
                // playback for ~30 seconds
                Thread.Sleep(30000);
            } catch (Exception ex)
            {
            }

            this.WritePlayerInfoToConsole("Pausing playback.", player);

            player.Pause();

            try
            {
                // pause for ~30 seconds
                Thread.Sleep(30000);
            } catch (Exception ex)
            {
            }

            this.WritePlayerInfoToConsole("Re-starting playback.", player);

            player.Play();

            try
            {
                // playback for ~30 seconds
                Thread.Sleep(30000);
            } catch (Exception ex)
            {
            }

            this.WritePlayerInfoToConsole("Stopping playback.", player);

            player.Stop();
        }

        [Test]
        public void LoadCachedHistorianData()
        {
            IDataHistorianPlayer player = EventProcessor.GetInstance().GetDataHistorianPlayer(this.sampleCacheName);

            player.SetEventListener(new DefaultSystemStatusEventListener());
            player.SetDisplayName("My Cached Historian");

            this.WritePlayerInfoToConsole("Loaded cache from file", player);
        }

        [Test]
        public void CreatePlayerAndStoreSampleData()
        {
            IDataHistorianPlayer player = EventProcessor.GetInstance().GetDataHistorianPlayer();

            player.SetEventListener(new DefaultSystemStatusEventListener());
            player.SetDisplayName("My New Historian");

            this.WritePlayerInfoToConsole(player);

            FilePersistenceConnector filePersistenceConnector = new FilePersistenceConnector(FileUtil.PersistenceDataTypeEnum.Historian);
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

            this.WritePlayerInfoToConsole("Stored cache to file", player, success);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        private void WritePlayerInfoToConsole(IDataHistorianPlayer player)
        {
            this.WritePlayerInfoToConsole("Data historian player info.", player, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="player"></param>
        private void WritePlayerInfoToConsole(string msg, IDataHistorianPlayer player)
        {
            this.WritePlayerInfoToConsole(msg, player, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="player"></param>
        /// <param name="success"></param>
        private void WritePlayerInfoToConsole(string msg, IDataHistorianPlayer player, bool success)
        {
            Console.WriteLine($"{msg}. Display Name: {player.GetDisplayName()}. Cache Name: {player.GetCacheName()}. Entries: {player.GetCacheSize()}. Memory: {player.GetCacheMemoryUsage()} bytes. File: {player.GetCacheFileName()}. Success = {success}");
        }
    }
}