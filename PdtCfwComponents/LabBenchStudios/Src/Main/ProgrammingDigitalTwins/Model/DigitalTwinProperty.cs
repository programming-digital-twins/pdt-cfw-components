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
        private bool usePrettyPrint = true;

        private string name = ConfigConst.NOT_SET;
        private string displayName = ConfigConst.NOT_SET;
        private string description = ConfigConst.NOT_SET;
        private string detail = ConfigConst.NOT_SET;

        private HashSet<int> commandOptions = null;

        private bool isEnabled = false;
        private bool isWriteable = false;
        private bool isTelemetry = false;
        private bool isCommand = false;

        private DateTime lastTelemetryUpdate = DateTime.MinValue;
        private DateTime lastTwinUpdate = DateTime.MinValue;

        private IotDataContext dataContext = null;
        private ConfigTypeModelConstraints propertyData = null;

        private ModelNameUtil.DtmiPropertyTypeEnum propertyType = ModelNameUtil.DtmiPropertyTypeEnum.Undefined;
        private ModelNameUtil.DtmiPropertyTypeEnum requestType  = ModelNameUtil.DtmiPropertyTypeEnum.Undefined;
        private ModelNameUtil.DtmiPropertyTypeEnum responseType = ModelNameUtil.DtmiPropertyTypeEnum.Undefined;

        /// <summary>
        /// 
        /// </summary>
        public DigitalTwinProperty()
        {
            this.InitProperty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public DigitalTwinProperty(string name) : this()
        {
            this.SetPropertyName(name);

            this.InitProperty();
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void AddCommandOption(int val)
        {
            this.commandOptions.Add(val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void ApplyTelemetry(SensorData data)
        {
            if (data != null)
            {
                this.UpdateDataContext(data);
                this.UpdatePropertyValues(data.GetDataValues());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="typeCategoryID"></param>
        /// <param name="typeID"></param>
        /// <param name="commandVal"></param>
        /// <returns></returns>
        public ActuatorData GenerateCommand(
            string deviceID,
            int typeCategoryID,
            int typeID,
            int commandVal)
        {
            return this.GenerateCommand(
                deviceID, deviceID, null, typeCategoryID, typeID, commandVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="stateData"></param>
        /// <param name="typeCategoryID"></param>
        /// <param name="typeID"></param>
        /// <param name="commandVal"></param>
        /// <returns></returns>
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
                    new ActuatorData(this.name, deviceID, typeCategoryID, typeID);

                data.SetValue(commandVal);
                data.SetLocationID(locationID);
                data.SetStateData(stateData);

                return data;
            }

            Console.WriteLine($"Command value not included in command set: {commandVal}");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HashSet<int> GetCommandOptions()
        {
            return this.commandOptions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            return this.description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDetail()
        {
            return this.detail;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDisplayName()
        {
            return this.displayName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetPropertyName()
        {
            return this.name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ModelNameUtil.DtmiPropertyTypeEnum GetPropertyType()
        {
            return this.propertyType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ModelNameUtil.DtmiPropertyTypeEnum GetRequestType()
        {
            return this.requestType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ModelNameUtil.DtmiPropertyTypeEnum GetResponseType()
        {
            return this.responseType;
        }

        /// <summary>
        /// Similar to GetPropertyValues(), but returns the property data as a
        /// ConfigTypeModelConstraints instance.
        /// </summary>
        /// <returns></returns>
        public ConfigTypeModelConstraints GetPropertyConstraints()
        {
            // cast isn't required - for clarity only
            return (ConfigTypeModelConstraints) this.propertyData;
        }

        /// <summary>
        /// Similar to GetPropertyConstraints(), but returns the property data as a
        /// DataValueContainer instance.
        /// </summary>
        /// <returns></returns>
        public DataValueContainer GetPropertyValues()
        {
            // cast isn't required - for clarity only
            return (DataValueContainer) this.propertyData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsCommand()
        {
            return this.isCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsPropertyEnabled()
        {
            return this.isEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsPropertyWriteable()
        {
            return this.isWriteable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsPropertyTelemetry()
        {
            return this.isTelemetry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isCommand"></param>
        public void SetAsCommand(bool isCommand)
        {
            this.isCommand = isCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isEnabled"></param>
        public void SetAsEnabled(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isWritable"></param>
        public void SetAsWriteable(bool isWritable)
        {
            this.isWriteable = isWritable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isTelemetry"></param>
        public void SetAsTelemetry(bool isTelemetry)
        {
            this.isTelemetry = isTelemetry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="detail"></param>
        public void SetDetail(string detail)
        {
            this.detail = detail;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public void SetDescription(string description)
        {
            this.description = description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayName"></param>
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void SetPropertyName(string name)
        {
            if (! string.IsNullOrEmpty(name))
            {
                this.name = name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void SetPropertyType(ModelNameUtil.DtmiPropertyTypeEnum type)
        {
            this.propertyType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void SetRequestType(ModelNameUtil.DtmiPropertyTypeEnum type)
        {
            this.requestType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void SetResponseType(ModelNameUtil.DtmiPropertyTypeEnum type)
        {
            this.responseType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            String lfString = "\n\t";

            if (! this.usePrettyPrint)
            {
                lfString = ";";
            }

            if (this.usePrettyPrint)
            {
                sb.Append(lfString).Append("----------");
                sb.Append(lfString);
            }

            sb
              .Append("name").Append('=').Append(this.name)
              .Append(lfString).Append("displayName").Append('=').Append(this.displayName)
              .Append(lfString).Append("description").Append('=').Append(this.description)
              .Append(lfString).Append("propType").Append('=').Append(this.propertyType)
              .Append(lfString).Append("requestType").Append('=').Append(this.requestType)
              .Append(lfString).Append("responseType").Append('=').Append(this.responseType)
              .Append(lfString).Append("detail").Append('=').Append(this.detail)
              .Append(lfString).Append("values").Append('=').Append(this.propertyData)
              .Append(lfString).Append("isEnabled").Append('=').Append(this.isEnabled)
              .Append(lfString).Append("isCommand").Append('=').Append(this.isCommand)
              .Append(lfString).Append("isWriteable").Append('=').Append(this.isWriteable)
              .Append(lfString).Append("isTelemetry").Append('=').Append(this.isTelemetry);

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void UpdateDataContext(IotDataContext data)
        {
            if (data != null)
            {
                this.dataContext.SetName(data.GetName());
                this.dataContext.SetDeviceID(data.GetDeviceID());
                this.dataContext.SetTypeCategoryID(data.GetTypeCategoryID());
                this.dataContext.SetTypeID(data.GetTypeID());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraints"></param>
        public void UpdatePropertyConstraints(ConfigTypeModelConstraints constraints)
        {
            if (constraints != null)
            {
                if (constraints.GetPropertyName().Equals(ConfigConst.NOT_SET))
                {
                    constraints.SetPropertyName(this.GetPropertyName());
                }

                // updates constraints but does not change the value
                this.propertyData.UpdateConstraints(constraints);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void UpdatePropertyValues(DataValueContainer values)
        {
            if (values != null)
            {
                // updates the values but does not change the constraints
                this.propertyData.UpdateData(values);
            }
        }

        // private methods

        /// <summary>
        /// 
        /// </summary>
        private void InitProperty()
        {
            this.commandOptions = new HashSet<int>();
            this.dataContext    = new IotDataContext();

            // NOTE: ConfigTypeModelConstraints is derived from DataValueContainer,
            // which includes value data plus other basic constraint min / max values.
            // ConfigTypeModelConstraints adds other constraints which may or may not
            // be needed by this property object.
            this.propertyData   = new ConfigTypeModelConstraints();
        }
    }
}
