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
        public const int KB = 1024;
        public const int MB = KB * KB;
        public const int GB = KB * MB;

        public const string DEFAULT_DEC_PLACES = "N3";
        public const string NO_DEC_PLACES = "N0";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetFormattedBytes(long val)
        {
            long absVal = Math.Abs(val);
            double derivedVal = (double) absVal;
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
