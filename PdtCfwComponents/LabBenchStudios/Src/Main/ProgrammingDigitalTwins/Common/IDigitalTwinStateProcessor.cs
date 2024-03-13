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

using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Model;

namespace LabBenchStudios.Pdt.Common
{
    public interface IDigitalTwinStateProcessor
    {
        DigitalTwinModelState AddConnectedModelState(DigitalTwinModelState modelState);

        bool BuildModelData();

        void EnableIncomingTelemetryProcessing(bool enable);

        string GetDeviceID();

        string GetLocationID();

        ResourceNameContainer GetCommandResource();

        ResourceNameContainer GetCommandResource(string resourceType);

        ResourceNameContainer GenerateOutgoingStateUpdate(IotDataContext dataContext);

        ModelNameUtil.DtmiControllerEnum GetModelControllerID();

        bool HandleIncomingTelemetry(IotDataContext dataContext);

        bool UpdateConnectionState(string deviceID, string locationID);

        bool UpdateConnectionState(IDigitalTwinStateProcessor processor);

    }
}
