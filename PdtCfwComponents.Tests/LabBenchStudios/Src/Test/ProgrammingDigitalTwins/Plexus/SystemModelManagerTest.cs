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
using System.Text;
using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Model;
using LabBenchStudios.Pdt.Plexus;
using static NUnit.Framework.Interfaces.TNode;

namespace LabBenchStudios.Pdt.Test.Plexus
{
    public class SystemModelManagerTest
    {

        private SystemModelManager sysModelManager = null;

        [SetUp]
        public void Setup()
        {
            this.sysModelManager = new SystemModelManager();
            this.sysModelManager.GetDigitalTwinModelManager().UpdateModelFilePaths(ConfigConst.TEST_DIGITAL_TWIN_MODEL_FILE_PATH);

            this.sysModelManager.BuildAllModels();
        }

        [Test]
        public void GetAllRegisteredDeviceIDs()
        {
            StringBuilder strBuilder = new StringBuilder();
            
            HashSet<string> nameSet =
                this.sysModelManager.GetAllRegisteredDeviceIDs();

            foreach (string name in nameSet)
            {
                strBuilder.Append("  -> ").Append(name).Append('\n');
            }

            Console.WriteLine(strBuilder.ToString());
        }

        [Test]
        public void GetAllConfigTypeNames()
        {
            StringBuilder strBuilder = new StringBuilder();

            List<string> nameList =
                this.sysModelManager.GetConfigTypeModelManager().GetLoadedConfigTypeNames();

            foreach (string name in nameList)
            {
                strBuilder.Append("  -> ").Append(name).Append('\n');
            }

            Console.WriteLine(strBuilder.ToString());
        }

        [Test]
        public void GetAllModelMappingNames()
        {
            StringBuilder strBuilder = new StringBuilder();

            List<string> nameList =
                this.sysModelManager.GetConfigTypeModelManager().GetLoadedAndMappedModelNames();

            foreach (string name in nameList)
            {
                strBuilder.Append("  -> ").Append(name).Append('\n');
            }

            Console.WriteLine(strBuilder.ToString());
        }


    }
}