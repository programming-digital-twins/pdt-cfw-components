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
using System.Text;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Model
{
    public class DigitalTwinModelJsonEntry
    {
        private string dtmiUri = string.Empty;
        private string dtdlJson = string.Empty;

        public DigitalTwinModelJsonEntry() : base()
        {
            // nothing to do
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtmiUri"></param>
        /// <param name="dtdlJson"></param>
        public DigitalTwinModelJsonEntry(string dtmiUri, string dtdlJson) : base()
        {
            this.SetDtmiUri(dtmiUri);
            this.SetDtdlJson(dtdlJson);
        }

        // public methods

        public string GetDtmiUri()
        {
            return dtmiUri;
        }

        public string GetDtdlJson()
        {
            return dtdlJson;
        }

        public void SetDtmiUri(string dtmiUri)
        {
            this.dtmiUri = dtmiUri;
        }

        public void SetDtdlJson(string dtdlJson)
        {
            this.dtdlJson = dtdlJson;
        }

    }
}
