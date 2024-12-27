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
using System.Runtime.InteropServices;

namespace LabBenchStudios.Pdt.Util
{
    public class NumberUtil
    {
        public const int DELAY_DEC_ROUNDING = 4;

        public const int KB = 1024;
        public const int MB = KB * KB;
        public const int GB = KB * MB;

        public const string DEFAULT_DEC_PLACES = "N3";
        public const string NO_DEC_PLACES = "N0";

        /// <summary>
        /// Calculates a delay factor based on the positive or negative value
        /// of the passed in delay factor.
        /// 
        /// NOTE: If the value is greater than -1.0 or less than +1.0, 0.0 will
        /// be used as the factoring algorithm only works with values at
        /// -1.0 and less, or +1.0 and greater.
        /// 
        /// If >= +1.0f, the delay factor will be calculated as a fraction of 1.0f
        ///   E.g., '10.0f' will result in a delay factor of 0.1d
        /// If <= -1.0f, the delay factor will be calculated as a multiple of 1.0f
        ///   E.g., '-10.0f' will result in a delay factor of 10.0d
        /// If 0.0f, the delay factor will be 1.0f
        /// </summary>
        /// <param name="delayFactor"></param>
        /// <returns></returns>
        public static double CalculateDelayFactor(float delayFactor)
        {
            double updatedDelayFactor = 1.0d;

            if (delayFactor >= 1.0f)
            {
                // speed up calculated
                // e.g.,
                //   factor of 10: 1.0d / 10.0f = 0.1d x [time] before next event
                updatedDelayFactor /= delayFactor;
            } else if (delayFactor <= -1.0f)
            {
                // slow down calculated
                // e.g.,
                //   factor of -10: 1.0d * 10.0f = 10.0d x [time] before next event
                updatedDelayFactor *= Math.Abs(delayFactor);
            } else
            {
                return updatedDelayFactor;
            }

            return Math.Round(updatedDelayFactor, NumberUtil.DELAY_DEC_ROUNDING);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetFormattedBytes(double val)
        {
            double absVal = Math.Abs(val);
            double derivedVal = absVal;
            string magnitude = " KB";

            if (absVal < MB)
            {
                derivedVal = (double) val / KB;
            } else if (absVal >= MB && absVal < GB)
            {
                derivedVal = (double) val / MB;
                magnitude = " MB";
            } else if (absVal > GB)
            {
                derivedVal = (double) val / GB;
                magnitude = " GB";
            }

            return (derivedVal.ToString(DEFAULT_DEC_PLACES) + magnitude);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetFormattedCounter(long val)
        {
            return (val.ToString(NO_DEC_PLACES));
        }

    }

}
