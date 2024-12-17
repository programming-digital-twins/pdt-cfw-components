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
using LabBenchStudios.Pdt.Historian;
using LabBenchStudios.Pdt.Model;

namespace LabBenchStudios.Pdt.Plexus
{
    /**
     * This class handles the registration of various event listeners and
     * the distribution of incoming events (of various types) to all
     * registered event listeners.
     * 
     * It is designed to be instanced once by the system manager, and then
     * accessed via the Singleton-like 'GetInstance()' methods.
     * 
     * It is NOT designed to be used across scenes (yet).
     * 
     */
    public class DefaultSystemStatusEventListener : ISystemStatusEventListener
    {

        // private member vars


        // constructors

        /// <summary>
        /// 
        /// </summary>
        public DefaultSystemStatusEventListener()
        {
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void LogDebugMessage(string message)
        {
            Console.WriteLine($"{this.GetType().Name}: Debug message: {message}.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void LogErrorMessage(string message, Exception ex)
        {
            Console.WriteLine($"{this.GetType().Name}: Error message: {message}. Exception: {ex}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void LogWarningMessage(string message)
        {
            Console.WriteLine($"{this.GetType().Name}: Warning message: {message}.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemDataReceived(ActuatorData data)
        {
            Console.WriteLine($"{this.GetType().Name}: [{DateTime.Now.Second}] - [{data.GetTimeStamp()}] Data: {data}.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemDataReceived(ConnectionStateData data)
        {
            Console.WriteLine($"{this.GetType().Name}: [{DateTime.Now.Second}] - [{data.GetTimeStamp()}] Data: {data}.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemDataReceived(SensorData data)
        {
            Console.WriteLine($"{this.GetType().Name}: [{DateTime.Now.Second}] - [{data.GetTimeStamp()}] Data: {data}.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemDataReceived(SystemPerformanceData data)
        {
            Console.WriteLine($"{this.GetType().Name}: [{DateTime.Now.Second}] - [{data.GetTimeStamp()}] Data: {data}.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemDataSent(ConnectionStateData data)
        {
            Console.WriteLine($"{this.GetType().Name}: [{DateTime.Now.Second}] - [{data.GetTimeStamp()}] Data: {data}.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void OnMessagingSystemStatusUpdate(ConnectionStateData data)
        {
            Console.WriteLine($"{this.GetType().Name}: System message received: {data}.");
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnModelUpdateEvent()
        {
            Console.WriteLine($"{this.GetType().Name}: OnModelUpdateEvent() called.");
        }


        // private methods


    }

}
