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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Model
{
    /// <summary>
    /// This class contains the properties and current known state of
    /// a digital twin model, along with its references and components.
    /// 
    /// Shared properties are derived from IotDataContext, which is also
    /// described in the base DTDML that all models extend.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DigitalTwinProperty
    {
        private string propertyName = ConfigConst.NOT_SET;
        private string displayName = ConfigConst.NOT_SET;
        private string description = ConfigConst.NOT_SET;
        private string unitLabel = ConfigConst.NOT_SET;

        private HashSet<int> commandOptions = null;

        private bool isEnabled = false;
        private bool isWriteable = false;
        private bool isTelemetry = false;

        private DateTime lastTelemetryUpdate = DateTime.MinValue;
        private DateTime lastTwinUpdate = DateTime.MinValue;
        
        private DataValueContainer propertyValues = new DataValueContainer();

        public DigitalTwinProperty()
        {
            this.commandOptions = new HashSet<int>();
        }

        public DigitalTwinProperty(string name) : this()
        {
            this.SetPropertyName(name);
        }


        // public methods

        public void AddCommandOption(int val)
        {
            this.commandOptions.Add(val);
        }

        public void ApplyTelemetry(SensorData data)
        {
            if (data != null)
            {

            }
        }

        public ActuatorData GenerateCommand(
            string deviceID,
            int typeCategoryID,
            int typeID,
            int commandVal)
        {
            return this.GenerateCommand(
                deviceID, deviceID, null, typeCategoryID, typeID, commandVal);
        }

        public ActuatorData GenerateCommand(
            string deviceID,
            string locationID,
            string stateData,
            int typeCategoryID,
            int typeID,
            int commandVal)
        {
            if (this.commandOptions.Contains(commandVal))
            {
                ActuatorData data =
                    new ActuatorData(this.propertyName, deviceID, typeCategoryID, typeID);

                data.SetValue(commandVal);
                data.SetLocationID(locationID);
                data.SetStateData(stateData);

                return data;
            }

            Console.WriteLine($"Command value not included in command set: {commandVal}");
            return null;
        }

        public HashSet<int> GetCommandOptions()
        {
            return commandOptions;
        }

        public string GetPropertyName()
        {
            return propertyName;
        }

        public DataValueContainer GetPropertyValues()
        {
            return propertyValues;
        }

        public bool IsPropertyEnabled()
        {
            return this.isEnabled;
        }

        public bool IsPropertyWriteable()
        {
            return this.isWriteable;
        }

        public bool IsPropertyTelemetry()
        {
            return this.isTelemetry;
        }

        public void SetAsEnabled(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }

        public void SetAsWriteable(bool isWritable)
        {
            this.isWriteable = isWritable;
        }

        public void SetAsTelemetry(bool isTelemetry)
        {
            this.isTelemetry = isTelemetry;
        }

        public void SetPropertyName(string name)
        {
            if (! string.IsNullOrEmpty(name))
            {
                propertyName = name;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());


            return sb.ToString();
        }

        public void UpdatePropertyValues(DataValueContainer values)
        {
            this.propertyValues.UpdateData(values);
        }
    }
}
