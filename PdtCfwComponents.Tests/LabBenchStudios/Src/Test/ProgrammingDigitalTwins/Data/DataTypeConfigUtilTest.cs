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
using System.IO;
using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using Newtonsoft.Json;

namespace LabBenchStudios.Pdt.Test.Data
{
    public class DataTypeConfigUtilTest
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void LoadDataTypeConfigJsonFile()
        {
            string pathName = "../../../../PdtCfwComponents/LabBenchStudios/Models/Types/";
            string fileName = "Lbs_Pdt_TypeConfig_EnvironmentalMeasurement.json";

            try
            {
                using StreamReader reader = new StreamReader(pathName + fileName);

                string jsonData = reader.ReadToEnd();

                DataTypeCategoryInfo categoryInfo = DataTypeConfigUtil.JsonToDataTypeCategoryInfo(jsonData);

                Console.WriteLine(categoryInfo);
            }
            catch (IOException e)
            {
                Console.WriteLine($"Failed to load file: {fileName}. Error: {e}. Stack trace:\n{e.StackTrace}");
            }
        }

        [Test]
        public void StoreDataTypeConfigJsonFile()
        {
            string pathName = "../../../../PdtCfwComponents/LabBenchStudios/Models/Types/";
            string fileName = "Lbs_Pdt_TypeConfig_Test.json";

            try
            {
                DataTypeCategoryInfo categoryInfo = new DataTypeCategoryInfo();
                categoryInfo.SetDataTypeName("foobar");
                categoryInfo.SetDataTypeDisplayName("Foobar.");
                categoryInfo.SetDataTypeDescription("This is the foobar.");
                categoryInfo.SetDtmlIdReference("dtmi:foo.bar;1");
                categoryInfo.SetDataTypeMinId(1000);
                categoryInfo.SetDataTypeMaxId(1999);

                DataTypeInfo typeInfo = new DataTypeInfo();
                typeInfo.SetDataTypeName("temp");
                typeInfo.SetDataTypeDisplayName("Temperature.");
                typeInfo.SetDataTypeDescription("This is the temperature.");
                typeInfo.SetDtmlIdReference("dtmi:temp.erature;1");
                typeInfo.SetId(1001);

                categoryInfo.AddTypeEntry(typeInfo);

                string jsonData = DataTypeConfigUtil.DataTypeCategoryInfoToJson(categoryInfo);

                using StreamWriter writer = new StreamWriter(pathName + fileName);
                writer.Write(jsonData);
                writer.Close();

                Console.WriteLine(jsonData);
            }
            catch (IOException e)
            {
                Console.WriteLine($"Failed to load file: {fileName}. Error: {e}. Stack trace:\n{e.StackTrace}");
            }
        }

    }
}