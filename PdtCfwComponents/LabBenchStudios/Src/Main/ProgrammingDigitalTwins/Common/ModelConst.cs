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

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LabBenchStudios.Pdt.Common
{
    public static class ModelConst
    {
        //////////
        // 
        // DTDL properties
        //

        // this is the base template model file for all Lbs_Pdt_* DTDL files
        public static readonly string TEMPLATE_IOT_DATA_CONTEXT_DTDL_MODEL     = "Lbs_Pdt_Template_IotDataContext.json";

        // these model files represent context for specific structures and systems
        //
        // each of these depends on (extends) the TEMPLATE_IOT_DATA_CONTEXT_DTDL_MODEL
        public static readonly string CONTEXT_HEATING_SYSTEM_DTDL_MODEL        = "Lbs_Pdt_Context_HeatingSystem.json";
        public static readonly string CONTEXT_INTERIOR_ROOM_STATE_DTDL_MODEL   = "Lbs_Pdt_Context_InteriorRoom.json";
        public static readonly string CONTEXT_RESIDENTIAL_STRUCTURE_DTDL_MODEL = "Lbs_Pdt_Context_ResidentialStructure.json";

        // these model files represent the controller description for all data generation
        // and command receptive systems and components
        //
        // each of these depends on (extends) the TEMPLATE_IOT_DATA_CONTEXT_DTDL_MODEL
        public static readonly string CONTROLLER_HEATING_ZONE_DTDL_MODEL       = "Lbs_Pdt_Controller_HeatingZone.json";
        public static readonly string CONTROLLER_HUMIDIFIER_DTDL_MODEL         = "Lbs_Pdt_Controller_Humidifier.json";
        public static readonly string CONTROLLER_THERMOSTAT_DTDL_MODEL         = "Lbs_Pdt_Controller_Thermostat.json";

        // these model files represent the telemetry description for all data generation systems and components
        //
        // each of these depends on (extends) the TEMPLATE_IOT_DATA_CONTEXT_DTDL_MODEL
        public static readonly string TELEMETRY_DEVICE_SYS_PERF_DTDL_MODEL     = "Lbs_Pdt_Telemetry_DeviceSystemPerformance.json";
        public static readonly string TELEMETRY_ENV_SENSORS_DTDL_MODEL         = "Lbs_Pdt_Telemetry_EnvironmentalSensors.json";
        public static readonly string TELEMETRY_FLUID_PUMP_DTDL_MODEL          = "Lbs_Pdt_Telemetry_FluidPump.json";
        public static readonly string TELEMETRY_POWER_WINDMILL_DTDL_MODEL      = "Lbs_Pdt_Telemetry_PowerWindmill.json";

        //////////
        //
        // DTDL model ID's
        //
        public static readonly string IOT_DATA_CONTEXT_MODEL_ID                     = "dtmi:LabBenchStudios:PDT:iotDataContext;1";
        public static readonly string HEATING_SYSTEM_CONTEXT_MODEL_ID               = "dtmi:LabBenchStudios:PDT:heatingSystem;1";
        public static readonly string INTERIOR_ROOM_STATE_CONTEXT_MODEL_ID          = "dtmi:LabBenchStudios:PDT:interiorRoom;1";
        public static readonly string RESIDENTIAL_STRUCTURE_CONTEXT_MODEL_ID        = "dtmi:LabBenchStudios:PDT:residentialStructure;1";
        public static readonly string HEATING_ZONE_CONTROLLER_MODEL_ID              = "dtmi:LabBenchStudios:PDT:heatingZone;1";
        public static readonly string HUMIDIFIER_CONTROLLER_MODEL_ID                = "dtmi:LabBenchStudios:PDT:humidifier;1";
        public static readonly string THERMOSTAT_CONTROLLER_MODEL_ID                = "dtmi:LabBenchStudios:PDT:thermostat;1";
        public static readonly string DEVICE_SYS_PERF_TELEMETRY_MODEL_ID            = "dtmi:LabBenchStudios:PDT:systemPerformanceData;1";
        public static readonly string ENV_SENSORS_TELEMETRY_MODEL_ID                = "dtmi:LabBenchStudios:PDT:environmentalSensorData;1";
        public static readonly string RELATIVE_HUMIDITY_SENSOR_TELEMETRY_MODEL_ID   = "dtmi:LabBenchStudios:PDT:relativeHumidityData;1";
        public static readonly string BAROMETRIC_PRESSURE_SENSOR_TELEMETRY_MODEL_ID = "dtmi:LabBenchStudios:PDT:barometricPressureData;1";
        public static readonly string TEMP_SENSOR_TELEMETRY_MODEL_ID                = "dtmi:LabBenchStudios:PDT:temperatureData;1";
        public static readonly string FLUID_PUMP_TELEMETRY_MODEL_ID                 = "dtmi:LabBenchStudios:PDT:fluidPumpData;1";
        public static readonly string POWER_WINDMILL_TELEMETRY_MODEL_ID             = "dtmi:LabBenchStudios:PDT:powerWindmillData;1";

    }
}
