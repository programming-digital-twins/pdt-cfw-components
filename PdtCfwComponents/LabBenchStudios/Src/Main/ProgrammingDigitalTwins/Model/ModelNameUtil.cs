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

using CsvHelper.Configuration.Attributes;
using LabBenchStudios.Pdt.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace LabBenchStudios.Pdt.Model
{
    public static class ModelNameUtil
    {
        //////////
        // 
        // Types (for compatibility with IotDataContext using consts from ConfigConst).
        //  why? this ensures an appropriate, but VERY simple, mapping of ID's so
        //  any commands sent from within the Twin Model can be effectively parsed
        //  and interpreted by the recipient system (e.g., the Edge Device App - EDA)
        //
        //  in the future, these types are expected to be synchronized - the approach
        //  indicated here is simply to facilitate backwards compatibility
        //
        //  note: a Humidifier will have sensing and actuation capabilities; however,
        //  the actual mapping of received telemetry (sensor data) and 'to be sent'
        //  actuation commands will be handled within IotModelContext, which is the
        //  base class for all model types
        //
        //  the consts indicated in this section simply facilitate the mapping
        //
        public const int HUMIDIFIER_DEVICE_TYPE = ConfigConst.HUMIDIFIER_ACTUATOR_TYPE;
        public const int THERMOSTAT_DEVICE_TYPE = ConfigConst.HVAC_ACTUATOR_TYPE;

        //////////
        //
        // DTDL property names and default versioning
        //

        // static const entries
        public const string DTMI_NAME = "dtmi";
        public const string DTMI_ORG_NAME = "LabBenchStudios";

        public const string MODEL_FILE_NAME_PREFIX = "Lbs_Pdt_";
        public const string MODEL_FILE_NAME_PATTERN = MODEL_FILE_NAME_PREFIX + "*.json";

        public const string DEFAULT_MODEL_FILE_PATH = "../../../../Models/Dtdl/";

        // static readonly entries
        public static readonly int DTMI_CURRENT_VERSION = 1;
        public static readonly string DTMI_PRODUCT_NAME = "PDT";
        public static readonly string DTMI_PREFIX = DTMI_NAME + ":" + DTMI_ORG_NAME + ":" + DTMI_PRODUCT_NAME;

        //////////
        // 
        // DTDL properties
        //

        // this is the base template model file for all Lbs_Pdt_* DTDL files
        public static readonly string BASE_IOT_MODEL_CONTEXT_DTDL_MODEL        = MODEL_FILE_NAME_PREFIX + "Base_IotModelContext.json";

        // these model files represent context for specific structures and systems
        //
        // each of these depends on (extends) the BASE_IOT_MODEL_CONTEXT_DTDL_MODEL
        public static readonly string CONTEXT_HEATING_SYSTEM_DTDL_MODEL        = MODEL_FILE_NAME_PREFIX + "Context_HeatingSystem.json";
        public static readonly string CONTEXT_INTERIOR_ROOM_DTDL_MODEL         = MODEL_FILE_NAME_PREFIX + "Context_InteriorRoom.json";
        public static readonly string CONTEXT_RESIDENTIAL_STRUCTURE_DTDL_MODEL = MODEL_FILE_NAME_PREFIX + "Context_ResidentialStructure.json";

        // these model files represent the controller description for all data generation
        // and command receptive systems and components
        //
        // each of these depends on (extends) the BASE_IOT_MODEL_CONTEXT_DTDL_MODEL
        public static readonly string CONTROLLER_BAROMETER_DTDL_MODEL           = MODEL_FILE_NAME_PREFIX + "Controller_Barometer.json";
        public static readonly string CONTROLLER_EDGE_COMPUTE_DEVICE_DTDL_MODEL = MODEL_FILE_NAME_PREFIX + "Controller_EdgeComputeDevice.json";
        public static readonly string CONTROLLER_FLUID_PUMP_DTDL_MODEL          = MODEL_FILE_NAME_PREFIX + "Controller_FluidPump.json";
        public static readonly string CONTROLLER_HEATING_ZONE_DTDL_MODEL        = MODEL_FILE_NAME_PREFIX + "Controller_HeatingZone.json";
        public static readonly string CONTROLLER_HUMIDIFIER_DTDL_MODEL          = MODEL_FILE_NAME_PREFIX + "Controller_Humidifier.json";
        public static readonly string CONTROLLER_POWER_WINDMILL_DTDL_MODEL      = MODEL_FILE_NAME_PREFIX + "Controller_PowerWindmill.json";
        public static readonly string CONTROLLER_THERMOSTAT_DTDL_MODEL          = MODEL_FILE_NAME_PREFIX + "Controller_Thermostat.json";

        // these model files represent the telemetry description for all data generation systems and components
        //
        // each of these depends on (extends) the BASE_IOT_MODEL_CONTEXT_DTDL_MODEL
        public static readonly string COMPONENT_DEVICE_SYS_PERF_DTDL_MODEL     = MODEL_FILE_NAME_PREFIX + "Component_DeviceSystemPerformance.json";
        public static readonly string COMPONENT_ENV_SENSORS_DTDL_MODEL         = MODEL_FILE_NAME_PREFIX + "Component_EnvironmentalSensors.json";

        //////////
        //
        // DTDL model property names
        //
        public const string IOT_MODEL_CONTEXT_NAME = "iotModelContext";
        public const string HEATING_SYSTEM_NAME = "heatingSystem";
        public const string INTERIOR_ROOM_NAME = "interiorRoom";
        public const string RESIDENTIAL_STRUCTURE_NAME = "residentialStructure";
        public const string EDGE_COMPUTE_DEVICE_NAME = "edgeComputeDevice";
        public const string FLUID_PUMP_NAME = "fluidPump";
        public const string HEATING_ZONE_NAME = "heatingZone";
        public const string BAROMETER_NAME = "barometer";
        public const string HUMIDIFIER_NAME = "humidifier";
        public const string POWER_WINDMILL_NAME = "powerWindmill";
        public const string THERMOSTAT_NAME = "thermostat";

        public const string SYSTEM_PERFORMANCE_DATA_NAME = "systemPerformanceData";
        public const string ENVIRONMENTAL_SENSOR_DATA_NAME = "environmentalSensorData";
        public const string RELATIVE_HUMIDITY_DATA_NAME = "relativeHumidityData";
        public const string BAROMETRIC_PRESSURE_DATA_NAME = "barometricPressureData";
        public const string TEMPERATURE_DATA_NAME = "temperatureData";

        public const string CURRENT_TEMPERATURE_PROP_NAME = "currentTemperature";
        public const string TARGET_TEMPERATURE_PROP_NAME = "targetTemperature";
        public const string MIN_TEMPERATURE_PROP_NAME = "minTemperature";
        public const string MAX_TEMPERATURE_PROP_NAME = "maxTemperature";

        public const string RELATIVE_HUMIDITY_PROP_NAME = "relativeHumidity";
        public const string TARGET_HUMIDITY_PROP_NAME = "targetHumidity";
        public const string MIN_HUMIDITY_PROP_NAME = "minHumidity";
        public const string MAX_HUMIDITY_PROP_NAME = "maxHumidity";

        //////////
        //
        // DTDL model ID's - these are dynamically generated on class load
        //  the objective is to ensure consistency in naming and versioning
        //  while allocating appropriate static readonly identifiers
        //  for use within the library and across other dependents
        //
        //  e.g., the DTA can look up a property within the model
        //  using a DTML parsing library and one of the naming costs
        //
        //  these consts are not expected to change, although the
        //  static readonly values may change from release to release
        //
        // Example:
        //  readonly string identifier: IOT_MODEL_CONTEXT_MODEL_ID
        //  string representation:      "dtmi:LabBenchStudios:PDT:iotModelContext:1"
        //
        public static readonly string IOT_MODEL_CONTEXT_MODEL_ID                    = CreateModelID(DTMI_PREFIX, IOT_MODEL_CONTEXT_NAME, DTMI_CURRENT_VERSION);
        public static readonly string HEATING_SYSTEM_CONTEXT_MODEL_ID               = CreateModelID(DTMI_PREFIX, HEATING_SYSTEM_NAME, DTMI_CURRENT_VERSION);
        public static readonly string INTERIOR_ROOM_STATE_CONTEXT_MODEL_ID          = CreateModelID(DTMI_PREFIX, INTERIOR_ROOM_NAME, DTMI_CURRENT_VERSION);
        public static readonly string RESIDENTIAL_STRUCTURE_CONTEXT_MODEL_ID        = CreateModelID(DTMI_PREFIX, RESIDENTIAL_STRUCTURE_NAME, DTMI_CURRENT_VERSION);
        public static readonly string FLUID_PUMP_CONTROLLER_MODEL_ID                = CreateModelID(DTMI_PREFIX, FLUID_PUMP_NAME, DTMI_CURRENT_VERSION);
        public static readonly string HEATING_ZONE_CONTROLLER_MODEL_ID              = CreateModelID(DTMI_PREFIX, HEATING_ZONE_NAME, DTMI_CURRENT_VERSION);
        public static readonly string BAROMETER_CONTROLLER_MODEL_ID                 = CreateModelID(DTMI_PREFIX, BAROMETER_NAME, DTMI_CURRENT_VERSION);
        public static readonly string HUMIDIFIER_CONTROLLER_MODEL_ID                = CreateModelID(DTMI_PREFIX, HUMIDIFIER_NAME, DTMI_CURRENT_VERSION);
        public static readonly string POWER_WINDMILL_CONTROLLER_MODEL_ID            = CreateModelID(DTMI_PREFIX, POWER_WINDMILL_NAME, DTMI_CURRENT_VERSION);
        public static readonly string THERMOSTAT_CONTROLLER_MODEL_ID                = CreateModelID(DTMI_PREFIX, THERMOSTAT_NAME, DTMI_CURRENT_VERSION);
        public static readonly string DEVICE_SYS_PERF_COMPONENT_MODEL_ID            = CreateModelID(DTMI_PREFIX, SYSTEM_PERFORMANCE_DATA_NAME, DTMI_CURRENT_VERSION);
        public static readonly string ENV_SENSORS_COMPONENT_MODEL_ID                = CreateModelID(DTMI_PREFIX, ENVIRONMENTAL_SENSOR_DATA_NAME, DTMI_CURRENT_VERSION);
        public static readonly string RELATIVE_HUMIDITY_SENSOR_COMPONENT_MODEL_ID   = CreateModelID(DTMI_PREFIX, RELATIVE_HUMIDITY_DATA_NAME, DTMI_CURRENT_VERSION);
        public static readonly string BAROMETRIC_PRESSURE_SENSOR_COMPONENT_MODEL_ID = CreateModelID(DTMI_PREFIX, BAROMETRIC_PRESSURE_DATA_NAME, DTMI_CURRENT_VERSION);
        public static readonly string TEMP_SENSOR_COMPONENT_MODEL_ID                = CreateModelID(DTMI_PREFIX, TEMPERATURE_DATA_NAME, DTMI_CURRENT_VERSION);

        public static readonly string ORG_NAME_PLACEHOLDER     = "ORG_NAME";
        public static readonly string PRODUCT_NAME_PLACEHOLDER = "PRODUCT_NAME";
        public static readonly string MODEL_NAME_PLACEHOLDER   = "MODEL_NAME";

        public enum DtmiControllerEnum
        {
            Barometer,
            EdgeComputingDevice,
            FluidPump,
            HeatingSystem,
            Humidifier,
            Thermostat,
            InteriorRoom,
            PowerWindmill,
            ResidentialStructure,
            Custom
        }

        public static string GetModelFileName(DtmiControllerEnum controllerID)
        {
            string modelFileName = BASE_IOT_MODEL_CONTEXT_DTDL_MODEL;

            switch (controllerID)
            {
                case DtmiControllerEnum.Barometer:
                    modelFileName = CONTROLLER_BAROMETER_DTDL_MODEL; break;

                case DtmiControllerEnum.EdgeComputingDevice:
                    modelFileName = CONTROLLER_EDGE_COMPUTE_DEVICE_DTDL_MODEL; break;

                case DtmiControllerEnum.FluidPump:
                    modelFileName = CONTROLLER_FLUID_PUMP_DTDL_MODEL; break;

                case DtmiControllerEnum.HeatingSystem:
                    modelFileName = CONTEXT_HEATING_SYSTEM_DTDL_MODEL; break;

                case DtmiControllerEnum.Humidifier:
                    modelFileName = CONTROLLER_HUMIDIFIER_DTDL_MODEL; break;

                case DtmiControllerEnum.Thermostat:
                    modelFileName = CONTROLLER_THERMOSTAT_DTDL_MODEL; break;

                case DtmiControllerEnum.InteriorRoom:
                    modelFileName = CONTEXT_INTERIOR_ROOM_DTDL_MODEL; break;

                case DtmiControllerEnum.PowerWindmill:
                    modelFileName = CONTROLLER_POWER_WINDMILL_DTDL_MODEL; break;

                case DtmiControllerEnum.ResidentialStructure:
                    modelFileName = CONTEXT_RESIDENTIAL_STRUCTURE_DTDL_MODEL; break;

                default:
                    break;
            }

            return modelFileName;
        }

        public static string CreateModelDataSyncKey(string deviceID)
        {
            return CreateModelDataSyncKey(deviceID, deviceID, false);
        }

        public static string CreateModelDataSyncKey(string deviceID, string locationID)
        {
            return CreateModelDataSyncKey(deviceID, locationID, false);
        }

        public static string CreateModelDataSyncKey(string deviceID, string locationID, bool generateGuid)
        {
            string guid = null;

            if (generateGuid)
            {
                guid = Guid.NewGuid().ToString();
            }

            return CreateModelDataSyncKey(deviceID, locationID, guid);
        }

        /// <summary>
        /// Format is {deviceID}:{locationID}:{guid}.
        /// If any parameter is passed in as null or empty, it will be replaced with
        /// ConfigConst.PRODUCT_NAME.
        /// Generally, the default invocation of this method will include a valid
        /// deviceID and locationID, which always default to ConfigConst.NOT_SET
        /// within IotDataContext.
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string CreateModelDataSyncKey(string deviceID, string locationID, string guid)
        {
            if (string.IsNullOrEmpty(deviceID))
            {
                deviceID = ConfigConst.PRODUCT_NAME;
            }

            if (string.IsNullOrEmpty(locationID))
            {
                locationID = deviceID;
            }

            if (string.IsNullOrEmpty(guid))
            {
                guid = ConfigConst.PRODUCT_NAME;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(deviceID).Append(':').Append(locationID).Append(':').Append(guid);

            return sb.ToString();
        }

        public static string CreateModelID(DtmiControllerEnum controllerID)
        {
            return CreateModelID(controllerID, ModelNameUtil.DTMI_CURRENT_VERSION);
        }

        public static string CreateModelID(DtmiControllerEnum controllerID, int version)
        {
            string modelName = IOT_MODEL_CONTEXT_NAME;

            switch(controllerID)
            {
                case DtmiControllerEnum.Barometer:
                    modelName = BAROMETER_NAME; break;
                    
                case DtmiControllerEnum.EdgeComputingDevice:
                    modelName = EDGE_COMPUTE_DEVICE_NAME; break;

                case DtmiControllerEnum.FluidPump:
                    modelName = FLUID_PUMP_NAME; break;

                case DtmiControllerEnum.HeatingSystem:
                    modelName = HEATING_SYSTEM_NAME; break;

                case DtmiControllerEnum.Humidifier:
                    modelName = HUMIDIFIER_NAME; break;

                case DtmiControllerEnum.Thermostat:
                    modelName = THERMOSTAT_NAME; break;

                case DtmiControllerEnum.InteriorRoom:
                    modelName = INTERIOR_ROOM_NAME; break;

                case DtmiControllerEnum.PowerWindmill:
                    modelName = POWER_WINDMILL_NAME; break;

                case DtmiControllerEnum.ResidentialStructure:
                    modelName = RESIDENTIAL_STRUCTURE_NAME; break;

                case DtmiControllerEnum.Custom:
                    return $"{DTMI_NAME}:{ORG_NAME_PLACEHOLDER}:{PRODUCT_NAME_PLACEHOLDER}:{MODEL_NAME_PLACEHOLDER};{version}";

                default:
                    break;
            }

            return ModelNameUtil.CreateModelID(ModelNameUtil.DTMI_PREFIX, modelName, version);
        }

        public static string CreateModelID(string modelName, int version)
        {
            return ModelNameUtil.CreateModelID(ModelNameUtil.DTMI_PREFIX, modelName, version);
        }

        public static string CreateModelID(string dtmiPrefix, string modelName, int version)
        {
            if (! string.IsNullOrEmpty(dtmiPrefix) &&
                ! string.IsNullOrEmpty(modelName) &&
                version > 0)
            {
                if (dtmiPrefix.StartsWith(ModelNameUtil.DTMI_NAME))
                {
                    return $"{dtmiPrefix}:{modelName};{version}";
                }
            }

            return ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;
        }

        public static string GetNameFromDtmiURI(string dtmiURI)
        {
            if (dtmiURI != null)
            {
                string[] parts = dtmiURI.Split(new char[] { ':' , ';'});

                if (parts.Length > 1)
                {
                    return parts[parts.Length - 2];
                }
            }

            return ModelNameUtil.IOT_MODEL_CONTEXT_NAME;
        }

        public static string GetModelID(int typeID)
        {
            string modelID = null;

            // set the DTMI (modelID) - only for those that may be deserialized from the EDA
            // all others will keep the default of ModelConst.IOT_MODEL_CONTEXT_MODEL_ID
            //
            // for now, this will suffice as a simple 'mapping' table, and also mitigate
            // any need to update the EDA with knowledge of DTMI naming conventions
            switch (typeID)
            {
                case ConfigConst.FLUID_RATE_SENSOR_TYPE:
                    modelID = ModelNameUtil.FLUID_PUMP_CONTROLLER_MODEL_ID; break;

                case ConfigConst.HUMIDIFIER_ACTUATOR_TYPE:
                    modelID = ModelNameUtil.HUMIDIFIER_CONTROLLER_MODEL_ID; break;

                case ConfigConst.HVAC_ACTUATOR_TYPE:
                    modelID = ModelNameUtil.THERMOSTAT_CONTROLLER_MODEL_ID; break;

                case ConfigConst.SYSTEM_PERF_TYPE:
                    modelID = ModelNameUtil.DEVICE_SYS_PERF_COMPONENT_MODEL_ID; break;

                case ConfigConst.HUMIDITY_SENSOR_TYPE:
                    modelID = ModelNameUtil.ENV_SENSORS_COMPONENT_MODEL_ID; break;

                case ConfigConst.TEMP_SENSOR_TYPE:
                    modelID = ModelNameUtil.ENV_SENSORS_COMPONENT_MODEL_ID; break;

                case ConfigConst.PRESSURE_SENSOR_TYPE:
                    modelID = ModelNameUtil.ENV_SENSORS_COMPONENT_MODEL_ID; break;

                case ConfigConst.WIND_SYSTEM_TYPE:
                    modelID = ModelNameUtil.POWER_WINDMILL_CONTROLLER_MODEL_ID; break;

                default:
                    modelID = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID; break;
            }

            return modelID;
        }
    }
}
