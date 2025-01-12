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
    public static class ConfigConst
    {
        public const char RESOURCE_SEPARATOR = '/';
        public const char RESOURCE_SUBTYPE_SEPARATOR = '-';

        public const string NOT_SET = "Not-Set";
        public const string UUID_NAME = "UUID";
        public const string DEFAULT_HOST = "localhost";

        public const string DEFAULT_FILE_STORAGE_PATH = "/tmp";
        public const string JSON_EXT = ".json";

        public const string TEST_MODEL_FILE_PATH =
            "../../../../PdtCfwComponents/LabBenchStudios/Models/";
        public const string TEST_DIGITAL_TWIN_MODEL_FILE_PATH = TEST_MODEL_FILE_PATH + "Dtdl/";
        public const string TEST_CONFIG_TYPE_MODEL_FILE_PATH = TEST_MODEL_FILE_PATH + "Types/";

        public const string RELATIVE_MODEL_PATH_PREFIX = "../../../../Models/";

        public const string DEFAULT_CONFIG_TYPE_FILE_PATH = RELATIVE_MODEL_PATH_PREFIX + "Types/";
        public const string DEFAULT_MODEL_FILE_PATH = RELATIVE_MODEL_PATH_PREFIX + "Dtdl/";

        public const string MODEL_FILE_NAME_PREFIX = "Lbs_Pdt_";
        public const string MODEL_FILE_NAME_SUFFIX = "*" + JSON_EXT;

        public const string FILE_DATE_TIME_FORMAT = "ddMMMyyyy";

        public const string CONFIG_TYPE_FILE_NAME_PREFIX = MODEL_FILE_NAME_PREFIX + "TypeConfig";
        public const string CONFIG_TYPE_FILE_NAME_PATTERN = CONFIG_TYPE_FILE_NAME_PREFIX + MODEL_FILE_NAME_SUFFIX;
        public const string MODEL_FILE_NAME_PATTERN = MODEL_FILE_NAME_PREFIX + MODEL_FILE_NAME_SUFFIX;

        public const int DEFAULT_COAP_PORT = 5683;
        public const int DEFAULT_COAP_SECURE_PORT = 5684;
        public const int DEFAULT_MQTT_PORT = 1883;
        public const int DEFAULT_MQTT_SECURE_PORT = 8883;
        public const int DEFAULT_RTSP_STREAM_PORT = 8554;
        public const int DEFAULT_INFLUXDB_PORT = 8086;
        public const int DEFAULT_TSDB_PORT = DEFAULT_INFLUXDB_PORT;
        public const int DEFAULT_KEEP_ALIVE = 60;
        public const int DEFAULT_POLL_CYCLES = 60;
        public const int DEFAULT_COMMAND = 0;
        public const int DEFAULT_STATUS = 0;
        public const int DEFAULT_TIMEOUT = 5;
        public const int DEFAULT_TTL = 300;
        public const int DEFAULT_QOS = 0;

        public const float DEFAULT_VAL = 0.0f;
        public const int DEFAULT_MAX_CACHED_ITEMS = 86400; // number of seconds in a day
        public const long DEFAULT_MAX_CACHE_SIZE_IN_MB = 2 ^ 29; // 0.5 GB

        // for purposes of this library, float precision is more then sufficient
        public const float DEFAULT_LAT = DEFAULT_VAL;
        public const float DEFAULT_LON = DEFAULT_VAL;
        public const float DEFAULT_ELEVATION = DEFAULT_VAL;

        public const int DEFAULT_ACTION_ID = 0;
        public const int INITIAL_SEQUENCE_NUMBER = 0;

        public const int DEFAULT_STREAM_FPS = 30;
        public const int DEFAULT_MIN_STREAM_FPS = 8;
        public const int DEFAULT_MAX_STREAM_FPS = 60;
        public const int DEFAULT_STREAM_FRAME_WIDTH = 1440;
        public const int DEFAULT_STREAM_FRAME_HEIGHT = 1080;
        public const int DEFAULT_MIN_MOTION_PIXELS_DIFF = 12000;
        public const int DEFAULT_MAX_CACHED_FRAMES = 10;
        public const string DEFAULT_STREAM_PROTOCOL = "rtsp";

        public const string PRODUCT_NAME = "PDT";
        public const string CLOUD = "Cloud";
        public const string GATEWAY = "Gateway";
        public const string CONSTRAINED = "Constrained";
        public const string EDGE = "Edge";
        public const string DEVICE = "Device";
        public const string SERVICE = "Service";

        public const string DEFAULT_QUERY_SESSION_ID = PRODUCT_NAME + "_QuerySession";

        public const string PRODUCT_NAME_KEY = "productName";
        public const string DEVICE_NAME_KEY = "deviceName";
        public const string TYPE_NAME_KEY = "typeName";

        public const string DATA_STORE_NAME = "objectStore";
        public const string HISTORIAN_CACHE_NAME = "historianCache";
        public const string PREDICTION_CACHE_NAME = "predictionCache";
        public const string TEXT_CACHE_NAME = "textCache";

        public const string CONSTRAINED_DEVICE = CONSTRAINED + DEVICE;
        public const string EDGE_DEVICE = EDGE + DEVICE;
        public const string GATEWAY_SERVICE = GATEWAY + SERVICE;
        public const string CLOUD_SERVICE = CLOUD + SERVICE;

        //////////
        // Property Names
        //

        public const string NAME_PROP = "name";
        public const string DEVICE_ID_PROP = "deviceID";
        public const string TYPE_CATEGORY_ID_PROP = "typeCategoryID";
        public const string TYPE_ID_PROP = "typeID";
        public const string TYPE_NAME_PROP = "typeName";
        public const string TYPE_CATEGORY_NAME_PROP = "typeCategoryName";
        public const string TIMESTAMP_PROP = "timeStamp";
        public const string HAS_ERROR_PROP = "hasError";
        public const string STATUS_CODE_PROP = "statusCode";
        public const string LOCATION_ID_PROP = "locationID";
        public const string LATITUDE_PROP = "latitude";
        public const string LONGITUDE_PROP = "longitude";
        public const string ELEVATION_PROP = "elevation";

        public const string COMMAND_PROP = "command";
        public const string COMMAND_NAME_PROP = "commandName";
        public const string STATE_DATA_PROP = "stateData";
        public const string IS_RESPONSE_PROP = "isResponse";
        public const string IS_INTERNAL_MSG_PROP = "isInternalMsg";

        public const string MODEL_ID_PROP = "modelID";

        public const string CPU_UTIL_PROP = "cpuUtil";
        public const string DISK_UTIL_PROP = "diskUtil";
        public const string MEM_UTIL_PROP = "memUtil";

        public const string SYS_START_TIME_PROP = "sysStartTime";

        public const string ACTION_ID_PROP = "actionID";
        public const string DATA_URI_PROP = "dataURI";
        public const string MESSAGE_PROP = "message";
        public const string ENCODING_NAME_PROP = "encodingName";
        public const string RAW_DATA_PROP = "rawData";
        public const string SEQUENCE_NUMBER_PROP = "seqNo";
        public const string USE_SEQUENCE_NUMBER_PROP = "useSeqNo";
        public const string SEQUENCE_NUMBER_TOTAL_PROP = "seqNoTotal";

        public const string DATA_VALUES_PROP = "dataValues";
        public const string UNIT_PROP = "unit";
        public const string VALUE_PROP = "value";
        public const string TARGET_VALUE_PROP = "targetValue";
        public const string RANGE_NOMINAL_FLOOR_PROP = "rangeNominalFloor";
        public const string RANGE_NOMINAL_CEILING_PROP = "rangeNominalCeiling";
        public const string RANGE_MAX_FLOOR_PROP = "rangeMaxFloor";
        public const string RANGE_MAX_CEILING_PROP = "rangeMaxCeiling";
        public const string NOMINAL_VALUE_DELTA_PROP = "nominalValueDelta";
        public const string MAX_VALUE_DELTA_PROP = "maxValueDelta";

        public const string SEND_RESOURCE_NAME_PROP = "sendResourceName";
        public const string RECEIVE_RESOURCE_NAME_PROP = "receiveResourceName";
        public const string IS_PING_PROP = "isPing";

        public const string MESSAGE_DATA_PROP = "msgData";

        public const string HOST_NAME_PROP = "hostName";
        public const string HOST_PORT_PROP = "hostPort";
        public const string MESSAGE_IN_COUNT_PROP = "msgInCount";
        public const string MESSAGE_OUT_COUNT_PROP = "msgOutCount";
        public const string IS_CONNECTING_PROP = "isConnecting";
        public const string IS_CONNECTED_PROP = "isConnected";
        public const string IS_DISCONNECTED_PROP = "isDisconnected";

        public const string CMD_DATA_PERSISTENCE_NAME = "pdt-cmd-data";
        public const string CONN_DATA_PERSISTENCE_NAME = "pdt-conn-data";
        public const string SENSOR_DATA_PERSISTENCE_NAME = "pdt-sensor-data";
        public const string SYS_DATA_PERSISTENCE_NAME = "pdt-sys-data";

        //////////
        // Resource and Topic Names
        //

        public const string ACTUATOR_CMD = "ActuatorCmd";
        public const string ACTUATOR_RESPONSE = "ActuatorResponse";
        public const string MGMT_STATUS_MSG = "MgmtStatusMsg";
        public const string MGMT_STATUS_CMD = "MgmtStatusCmd";
        public const string MEDIA_MSG = "MediaMsg";
        public const string SENSOR_MSG = "SensorMsg";
        public const string SYSTEM_PERF_MSG = "SystemPerfMsg";

        public const string UPDATE_NOTIFICATIONS_MSG = "UpdateMsg";
        public const string RESOURCE_REGISTRATION_REQUEST = "ResourceRegRequest";

        public const string LED_ACTUATOR_NAME = "LedActuator";
        public const string SYSTEM_PERF_NAME = "SystemPerformance";
        public const string CAMERA_SENSOR_NAME = "CameraSensor";

        public const int COMMAND_ON = 1;
        public const int COMMAND_OFF = 2;
        public const int COMMAND_MSG_ONLY = 5;

        public const string COMMAND_KEY = "Command";

        public const string ON_KEY = "ON";
        public const string OFF_KEY = "OFF";
        public const string RUNNING_KEY = "Running";

        public const int DEFAULT_TYPE_ID = 0;
        public const int DEFAULT_TYPE_CATEGORY_ID = 0;
        public const int DEFAULT_ACTUATOR_TYPE = DEFAULT_TYPE_ID;
        public const int DEFAULT_SENSOR_TYPE = DEFAULT_TYPE_ID;

        public const int CUSTOM_ACTUATOR_CATEGORY = 400;
        public const int CUSTOM_SENSOR_CATEGORY = 500;

        public const int ENV_TYPE_CATEGORY = 1000;
        public const int ENV_DEVICE_TYPE = ENV_TYPE_CATEGORY;

        public const int HVAC_ACTUATOR_TYPE = 1001;
        public const int HUMIDIFIER_ACTUATOR_TYPE = 1002;
        public const int HUMIDIFIER_TYPE = HUMIDIFIER_ACTUATOR_TYPE;
        public const int BAROMETER_TYPE = 1003;
        public const int THERMOSTAT_TYPE = 1004;

        public const int HUMIDITY_SENSOR_TYPE = 1010;
        public const int PRESSURE_SENSOR_TYPE = 1012;
        public const int TEMP_SENSOR_TYPE = 1013;

        public const int DISPLAY_CATEGORY_TYPE = 2000;
        public const int DISPLAY_DEVICE_TYPE = DISPLAY_CATEGORY_TYPE;
        public const int LED_DISPLAY_ACTUATOR_TYPE = 2001;

        public const int MEDIA_TYPE_CATEGORY = 3000;
        public const int DEFAULT_MEDIA_TYPE = MEDIA_TYPE_CATEGORY;
        public const int MEDIA_DEVICE_TYPE = 3000;
        public const int CAMERA_SENSOR_TYPE = 3101;
        public const int VIDEO_SYSTEM_TYPE = 3201;
        public const int AUDIO_SYSTEM_TYPE = 3301;
        public const int LIGHTING_SYSTEM_TYPE = 3401;


        public const int UTILITY_SYSTEM_TYPE_CATEGORY = 4000;
        public const int UTILITY_SYSTEM_DEVICE_TYPE = UTILITY_SYSTEM_TYPE_CATEGORY;

        public const int HEATING_SYSTEM_TYPE = 4100;
        public const int FLUID_VISCOSITY_SENSOR_TYPE = 4101;
        public const int FLUID_RATE_SENSOR_TYPE = 4102;
        public const int FLUID_PUMP_TYPE = 4103;
        public const int IMPELLER_RPM_SENSOR_TYPE = 4105;
        public const int IMPELLER_RPM_ACTUATOR_TYPE = 4107;

        public const int ELECTRICAL_SYSTEM_TYPE = 4200;
        public const int POWER_OUTPUT_SENSOR_TYPE = 4201;

        public const int WIND_TURBINE_UTILITY_TYPE_CATEGORY = 4300;
        public const int WIND_TURBINE_POWER_OUTPUT_SENSOR_TYPE = 4301;
        public const int WIND_TURBINE_GENERATOR_TEMP_SENSOR_TYPE = 4302;
        public const int WIND_TURBINE_ROTATIONAL_SPEED_SENSOR_TYPE = 4303;
        public const int WIND_TURBINE_AIR_SPEED_SENSOR_TYPE = 4304;

        public const int WIND_TURBINE_BRAKE_SYSTEM_ACTUATOR_TYPE = 4320;

        public const int ENERGY_TYPE_CATEGORY = 5000;
        public const int STORAGE_LEVEL = 5001;
        public const int STORAGE_DRAW = 5002;

        public const int NATURAL_RESOURCE_ENERGY_SYSTEM_TYPE = 5100;
        public const int WIND_SYSTEM_TYPE = 5101;
        public const int WIND_TURBINE_SYSTEM_TYPE = WIND_SYSTEM_TYPE;
        public const int SOLAR_SYSTEM_TYPE = 5102;
        public const int HYDRO_SYSTEM_TYPE = 5103;
        public const int GEOTHERMAL_SYSTEM_TYPE = 5104;
        public const int OTHER_ENERGY_SYSTEM_TYPE = 5200;
        public const int HYDROGEN_SYSTEM_TYPE = 5201;
        public const int FOSSIL_FUEL_ENERGY_SYSTEM_TYPE = 5900;
        public const int OIL_SYSTEM_TYPE = 5901;
        public const int NATGAS_SYSTEM_TYPE = 5902;
        public const int PROPANE_SYSTEM_TYPE = 5903;
        public const int GASOLINE_SYSTEM_TYPE = 5904;

        public const int STRUCTURE_TYPE_CATEGORY = 7200;
        public const int STRUCTURE_TYPE = 7201;
        public const int STRUCTURE_LEVEL_TYPE = 7202;
        public const int STRUCTURE_SPACE_TYPE = 7203;

        public const int SYSTEM_TYPE_CATEGORY = 8000;
        public const int SYSTEM_MGMT_TYPE = 8000;
        public const int RESOURCE_MGMT_TYPE = 8001;
        public const int SYSTEM_CONN_STATE_TYPE = 8002;
        public const int FILE_SYSTEM_TYPE = 8003;
        public const int DATABASE_SYSTEM_TYPE = 8004;

        public const int SYSTEM_PERF_TYPE_CATEGORY = 9000;
        public const int SYSTEM_PERF_TYPE = SYSTEM_PERF_TYPE_CATEGORY;
        public const int CPU_UTIL_TYPE = 9001;
        public const int MEM_UTIL_TYPE = 9002;
        public const int DISK_UTIL_TYPE = 9003;
        public const int OPS_STATE_CATEGORY = 9900;
        public const int OPS_STATE_TYPE = 9901;

        public const int MESSAGE_TYPE_CATEGORY = 10000;
        public const int MESSAGE_TYPE = 10001;

        public const string HUMIDIFIER_NAME = "Humidifier";
        public const string THERMOSTAT_NAME = "Thermostat";
        public const string PRESSURE_NAME = "BarometricPressure";
        public const string FLUID_PUMP_NAME = "FluidPump";
        public const string WIND_TURBINE_NAME = "WindTurbine";

        public const string HUMIDIFIER_ACTUATOR_NAME = "HumidifierActuator";
        public const string HVAC_ACTUATOR_NAME = "HvacActuator";
        public const string CIRCULATOR_PUMP_ACTUATOR_NAME = "CirculatorPumpActuator";

        public const string HUMIDITY_SENSOR_NAME = "Hygrometer";
        //public const string HUMIDITY_SENSOR_NAME = "RelativeHumidity";
        public const string PRESSURE_SENSOR_NAME = "PressureSensor";
        public const string TEMP_SENSOR_NAME = "Temperature";
        //public const string TEMP_SENSOR_NAME = "Temperature";

        public const string VISCOSITY_NAME = "Viscosity";
        public const string FLOW_RATE_NAME = "FlowRate";
        public const string RPM_NAME = "RPM";

        public const string MEDIA_TYPE_NAME = "MediaType";

        public const string LUMENS_NAME = "Lumens";
        public const string DECIBELS_NAME = "Decibels";
        public const string COLOR_NAME = "Color";
        public const string PLAYING_NAME = "Playing";

        public const string STORAGE_LEVEL_NAME = "StorageLevel";
        public const string STORAGE_DRAW_NAME = "StorageDraw";

        public const string WATTAGE_NAME = "Wattage";
        public const string AMPERAGE_NAME = "Amperage";
        public const string VOLTAGE_NAME = "Voltage";

        public const string RESOURCE_MGMT_NAME = "ResourceMgmt";

        public const string CPU_UTIL_NAME = "DeviceCpuUtil";
        public const string DISK_UTIL_NAME = "DeviceDiskUtil";
        public const string MEM_UTIL_NAME = "DeviceMemUtil";

        //////////
        // status codes
        //
        //
        public const int NOMINAL_STATUS_CODE = 0;
        public const int CONN_SUCCESS_STATUS_CODE = 200;
        public const int CONN_IN_PROC_STATUS_CODE = 201;
        public const int PUB_SUCCESS_STATUS_CODE = 210;
        public const int SUB_SUCCESS_STATUS_CODE = 220;
        public const int DISCONN_SUCCESS_STATUS_CODE = 300;
        public const int DISCONN_IN_PROC_STATUS_CODE = 301;
        public const int CONN_FAILURE_STATUS_CODE = 400;
        public const int MSG_SEND_FAILURE_STATUS_CODE = 401;

        public const string NO_STATUS_MSG = "No_Status";

        public static readonly ReadOnlyDictionary<int, string> STATUS_MSG_TABLE =
            new ReadOnlyDictionary<int, string>(
                new Dictionary<int, string>
        {
            { NOMINAL_STATUS_CODE, "Nominal" },
            { CONN_SUCCESS_STATUS_CODE, "Connected" },
            { CONN_IN_PROC_STATUS_CODE, "Connecting..." },
            { PUB_SUCCESS_STATUS_CODE, "Published" },
            { SUB_SUCCESS_STATUS_CODE, "Subscribed" },
            { DISCONN_SUCCESS_STATUS_CODE, "Disconnected" },
            { DISCONN_IN_PROC_STATUS_CODE, "Disconnecting" },
            { CONN_FAILURE_STATUS_CODE, "Conn Failure" },
            { MSG_SEND_FAILURE_STATUS_CODE, "Msg Send Failure" }
        });


        //////////
        // typical topic naming conventions
        //
        //
        // for DTA to EDA topic mapping communications
        // e.g., PDT/EdgeDevice/ActuatorCmd
        // e.g., PDT/EdgeDevice/SensorMsg

        public static readonly string EDA_UPDATE_NOTIFICATIONS_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + UPDATE_NOTIFICATIONS_MSG;
        public static readonly string EDA_ACTUATOR_CMD_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + ACTUATOR_CMD;
        public static readonly string EDA_ACTUATOR_RESPONSE_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + ACTUATOR_RESPONSE;
        public static readonly string EDA_MGMT_STATUS_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + MGMT_STATUS_MSG;
        public static readonly string EDA_MGMT_CMD_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + MGMT_STATUS_CMD;
        public static readonly string EDA_MEDIA_DATA_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + MEDIA_MSG;
        public static readonly string EDA_REGISTRATION_REQUEST_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + RESOURCE_REGISTRATION_REQUEST;
        public static readonly string EDA_SENSOR_DATA_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + SENSOR_MSG;
        public static readonly string EDA_SYSTEM_PERF_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + SYSTEM_PERF_MSG;

        //////////
        // Configuration Sections, Keys and Defaults
        //
        //
        // NOTE: You may need to update these paths if you change
        // the directory structure for python-components

        public const string LOCAL = "Local";
        public const string MQTT = "Mqtt";
        public const string DATA = "Data";

        public const string DEVICE_ID_KEY = "deviceID";
        public const string DEVICE_LOCATION_ID_KEY = "deviceLocationID";

        public const string CLOUD_GATEWAY_SERVICE = CLOUD + "." + GATEWAY_SERVICE;
        public const string MQTT_GATEWAY_SERVICE = MQTT + "." + GATEWAY_SERVICE;
        public const string DATA_GATEWAY_SERVICE = DATA + "." + GATEWAY_SERVICE;

        public const string CRED_SECTION = "Credentials";

        public const string FROM_ADDRESS_KEY = "fromAddr";
        public const string TO_ADDRESS_KEY = "toAddr";
        public const string TO_MEDIA_ADDRESS_KEY = "toMediaAddr";
        public const string TO_TXT_ADDRESS_KEY = "toTxtAddr";

        public const string HOST_KEY = "host";
        public const string PORT_KEY = "port";
        public const string SECURE_PORT_KEY = "securePort";

        public const string ROOT_CERT_ALIAS = "root";

        public const string KEY_STORE_CLIENT_IDENTITY_KEY = "keyStoreClientIdentity";
        public const string KEY_STORE_SERVER_IDENTITY_KEY = "keyStoreServerIdentity";

        public const string KEY_STORE_FILE_KEY = "keyStoreFile";
        public const string KEY_STORE_AUTH_KEY = "keyStoreAuth";
        public const string TRUST_STORE_FILE_KEY = "trustStoreFile";
        public const string TRUST_STORE_ALIAS_KEY = "trustStoreAlias";
        public const string TRUST_STORE_AUTH_KEY = "trustStoreAuth";
        public const string USER_NAME_TOKEN_KEY = "userToken";
        public const string USER_AUTH_TOKEN_KEY = "authToken";
        public const string API_TOKEN_KEY = "apiToken";
        public const string CLIENT_TOKEN_KEY = "clientToken";

        public const string ORGANIZATION_ID_KEY = "orgID";
        public const string ENV_DATA_BUCKET_ID = "envDataBucketID";
        public const string STATE_DATA_BUCKET_ID = "stateDataBucketID";
        public const string SYS_PERF_DATA_BUCKET_ID = "sysPerfDataBucketID";
        public const string CMD_DATA_BUCKET_ID = "cmdDataBucketID";

        public const string CERT_FILE_KEY = "certFile";
        public const string CRED_FILE_KEY = "credFile";
        public const string ENABLE_AUTH_KEY = "enableAuth";
        public const string ENABLE_CRYPT_KEY = "enableCrypt";
        public const string ENABLE_SIMULATOR_KEY = "enableSimulator";
        public const string ENABLE_EMULATOR_KEY = "enableEmulator";
        public const string ENABLE_SENSE_HAT_KEY = "enableSenseHAT";
        public const string ENABLE_LOGGING_KEY = "enableLogging";
        public const string USE_WEB_ACCESS_KEY = "useWebAccess";
        public const string POLL_CYCLES_KEY = "pollCycleSecs";
        public const string KEEP_ALIVE_KEY = "keepAlive";
        public const string DEFAULT_QOS_KEY = "defaultQos";

        public const string ENABLE_MQTT_CLIENT_KEY = "enableMqttClient";

        public const string ENABLE_SYSTEM_PERF_KEY = "enableSystemPerformance";
        public const string ENABLE_SENSING_KEY = "enableSensing";

        public const string HUMIDITY_SIM_FLOOR_KEY = "humiditySimFloor";
        public const string HUMIDITY_SIM_CEILING_KEY = "humiditySimCeiling";
        public const string PRESSURE_SIM_FLOOR_KEY = "pressureSimFloor";
        public const string PRESSURE_SIM_CEILING_KEY = "pressureSimCeiling";
        public const string TEMP_SIM_FLOOR_KEY = "tempSimFloor";
        public const string TEMP_SIM_CEILING_KEY = "tempSimCeiling";

        public const string HANDLE_TEMP_CHANGE_ON_DEVICE_KEY = "handleTempChangeOnDevice";
        public const string TRIGGER_HVAC_TEMP_FLOOR_KEY = "triggerHvacTempFloor";
        public const string TRIGGER_HVAC_TEMP_CEILING_KEY = "triggerHvacTempCeiling";

        public const string RUN_FOREVER_KEY = "runForever";
        public const string TEST_EMPTY_APP_KEY = "testEmptyApp";

        public const string STREAM_HOST_ADDR_KEY = "streamHostAddr";
        public const string STREAM_HOST_LABEL_KEY = "streamHostLabel";
        public const string STREAM_PORT_KEY = "streamPort";
        public const string STREAM_PROTOCOL_KEY = "streamProtocol";
        public const string STREAM_PATH_KEY = "streamPath";
        public const string STREAM_ENCODING_KEY = "streamEncoding";
        public const string STREAM_FRAME_WIDTH_KEY = "streamFrameWidth";
        public const string STREAM_FRAME_HEIGHT_KEY = "streamFrameHeight";
        public const string STREAM_FPS_KEY = "streamFps";
        public const string IMAGE_FILE_EXT_KEY = "imageFileExt";
        public const string VIDEO_FILE_EXT_KEY = "videoFileExt";
        public const string MIN_MOTION_PIXELS_DIFF_KEY = "minMotionPixelsDiff";

        public const string IMAGE_ENCODING_KEY = "imageEncoding";
        public const string IMAGE_DATA_STORE_PATH = "imageDataStorePath";
        public const string VIDEO_DATA_STORE_PATH = "videoDataStorePath";
        public const string MAX_MOTION_FRAMES_BEFORE_ACTION_KEY = "maxMotionFramesBeforeAction";
        public const string MAX_CACHED_FRAMES_KEY = "maxCachedFrames";
        public const string STORE_INTERIM_FRAMES_KEY = "storeInterimFrames";
        public const string INCLUDE_RAW_IMAGE_DATA_IN_MSG_KEY = "includeRawImageDataInMsg";

    }
}
