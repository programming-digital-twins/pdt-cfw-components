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

using LabBenchStudios.Pdt.Common;
using System.Collections.Generic;

namespace LabBenchStudios.Pdt.Connection
{
    public interface IPredictionModelConnector : IMessageProcessor
    {

        // public methods

        public void ClearSessionState();

        public string GetCurrentModel();

        public string GetSessionID();

        public List<string> GetRegisteredModels();

        public string GetServerUri();

        public bool IsServerReachable();

        public bool SetModel(string model);

        public bool SendRequest(string request);

        public void SetPredictionModelListener(IPredictionModelListener listener);

        public void SetSystemStatusEventListener(ISystemStatusEventListener listener);

        public void SetQueryResponseListener(IDataContextEventListener listener);

    }
}
