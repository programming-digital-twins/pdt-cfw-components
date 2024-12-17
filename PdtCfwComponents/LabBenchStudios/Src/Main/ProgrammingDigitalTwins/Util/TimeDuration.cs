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

namespace LabBenchStudios.Pdt.Util
{
    /// <summary>
    /// Simple wrapper class around two DateTime objects:
    /// - start time
    /// - end time
    /// 
    /// Internal logic will ensure that start time is before end time
    /// and minimum separation (in minutes) between start and end.
    /// 
    /// All time references will be converted to UTC if not already
    /// of that kind.
    /// </summary>
    public class TimeDuration
    {
        // 1 minute minimum time separation
        public const int MIN_TIME_SEPARATION_MINS = 1;

        // 1 month maximum time separation
        public const int MAX_TIME_SEPARATION_MINS = 60 * 24 * 30;

        // 1 hour default time separation
        public const int DEFAULT_TIME_SEPARATION_MINS = 60;

        private int minTimeSeparationInMinutes = DEFAULT_TIME_SEPARATION_MINS;

        private DateTime startTime;
        private DateTime endTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public TimeDuration(DateTime startTime, DateTime endTime) : this(startTime, endTime, DEFAULT_TIME_SEPARATION_MINS)
        {
            // nothing to do - delegated to other constructor
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="minTimeSeparationInMinutes"></param>
        public TimeDuration(DateTime startTime, DateTime endTime, int minTimeSeparationInMinutes)
        {
            this.InitTimeSeparationMinutes(minTimeSeparationInMinutes);
            this.InitStartAndEndTimes(startTime, endTime);
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetMinimumTimeSeparationInMinutes()
        {
            return this.minTimeSeparationInMinutes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime GetStartTime()
        {
            return this.startTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime GetEndTime()
        {
            return this.endTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetDeltaInSeconds()
        {
            return (this.endTime.Subtract(this.startTime).TotalSeconds);
        }

        // private methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        private void InitStartAndEndTimes(DateTime startTime, DateTime endTime)
        {
            // convert start and end times to UTC (if needed)
            startTime = startTime.ToUniversalTime();
            endTime = endTime.ToUniversalTime();

            // ensure end time is no later than now (UTC)
            if (endTime > DateTime.UtcNow)
            {
                endTime = DateTime.UtcNow;
            }

            // ensure start time is no later than end time (UTC)
            if (startTime > endTime)
            {
                startTime = endTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minTimeSeparationInMinutes"></param>
        private void InitTimeSeparationMinutes(int minTimeSeparationInMinutes)
        {
            if (minTimeSeparationInMinutes < MIN_TIME_SEPARATION_MINS ||
                minTimeSeparationInMinutes > MAX_TIME_SEPARATION_MINS)
            {
                minTimeSeparationInMinutes = DEFAULT_TIME_SEPARATION_MINS;
            }

            this.minTimeSeparationInMinutes = minTimeSeparationInMinutes;
        }

    }

}
