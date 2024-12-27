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
using System.IO;
using System.Text;
using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Util;

namespace LabBenchStudios.Pdt.Test.Data
{
    public class NumberUtilTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetCalculatedDelayFactors()
        {
            Console.WriteLine($"Calculating delay factors...");

            float magnitude = 0.0f;
            double factor = 0.0d;
            int counter = 0;

            // Test 1: Mag = 0.5f, Factor = 1
            magnitude = 0.5f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");

            // Test 2: Mag = -0.5f, Factor = 1
            magnitude = -0.5f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");

            // Test 3: Mag = 0.0f, Factor = 1
            magnitude = 0.0f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");

            // Test 4: Mag = 1.0f, Factor = 1
            magnitude = 1.0f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");

            // Test 5: Mag = -1.0f, Factor = 1
            magnitude = -1.0f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");

            // Test 6: Mag = 1.5f, Factor = 0.667
            magnitude = 1.5f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");

            // Test 7: Mag = -1.5f, Factor = 1.5
            magnitude = -1.5f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");

            // Test 8: Mag = 3.0f, Factor = 0.3333
            magnitude = 3.0f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");

            // Test 9: Mag = 5.0f, Factor = 0.2
            magnitude = 5.0f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");

            // Test 10: Mag = -5.0f, Factor = 5
            magnitude = -5.0f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");

            // Test 11: Mag = 25.0f, Factor = 0.04
            magnitude = 25.0f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");

            // Test 12: Mag = -50.0f, Factor = 50
            magnitude = -50.0f;
            factor = NumberUtil.CalculateDelayFactor(magnitude);
            Console.WriteLine($"  --> Test {++counter}. Magnitude: {magnitude}. Delay factor: {factor}");
        }

    }

}