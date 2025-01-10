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
using System.Text;
using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Model;

namespace LabBenchStudios.Pdt.Test.Model
{
    public class ConfigTypeModelManagerTest
    {
        private ConfigTypeModelManager ctModelManager = null;

        [SetUp]
        public void Setup()
        {
            this.ctModelManager = new ConfigTypeModelManager();
        }

        [Test]
        public void GetConfigTypeNameAndProperty()
        {
            string name = "windTurbine";
            string dtmi = "dtmi:LabBenchStudios:PDT:windTurbine;1";

            ConfigTypeModelEntry modelEntry = this.ctModelManager.GetConfigEntryByModelName(dtmi);

            if (modelEntry != null)
            {
                StringBuilder builder = new StringBuilder("Config Type Entry:");

                builder.AppendLine(name);
                builder.Append(modelEntry);
            } else
            {
                ConfigTypeModelContainer modelContainer = this.ctModelManager.GetConfigCategoryByModelName(dtmi);
                StringBuilder builder = new StringBuilder("Config Type Category:");

                builder.AppendLine(name);
                builder.Append(modelEntry);
            }
        }

        [Test]
        public void GetAllConfigTypeNames()
        {
        }

        [Test]
        public void GetAllConfigTypeNamesAndProperties()
        {
        }

    }
}