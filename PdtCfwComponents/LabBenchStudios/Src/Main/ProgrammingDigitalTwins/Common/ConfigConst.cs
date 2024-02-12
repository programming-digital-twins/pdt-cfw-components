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
        public static readonly char RESOURCE_SEPARATOR = '/';
        public static readonly char RESOURCE_SUBTYPE_SEPARATOR = '-';

        public static readonly string NOT_SET = "Not-Set";
        public static readonly string DEFAULT_HOST = "localhost";

        public static readonly int DEFAULT_COAP_PORT = 5683;
        public static readonly int DEFAULT_COAP_SECURE_PORT = 5684;
        public static readonly int DEFAULT_MQTT_PORT = 1883;
        public static readonly int DEFAULT_MQTT_SECURE_PORT = 8883;
        public static readonly int DEFAULT_RTSP_STREAM_PORT = 8554;
        public static readonly int DEFAULT_INFLUXDB_PORT = 8086;
        public static readonly int DEFAULT_TSDB_PORT = DEFAULT_INFLUXDB_PORT;
        public static readonly int DEFAULT_KEEP_ALIVE = 60;
        public static readonly int DEFAULT_POLL_CYCLES = 60;
        public static readonly int DEFAULT_COMMAND = 0;
        public static readonly int DEFAULT_STATUS = 0;
        public static readonly int DEFAULT_TIMEOUT = 5;
        public static readonly int DEFAULT_TTL = 300;
        public static readonly int DEFAULT_QOS = 0;

        public static readonly float DEFAULT_VAL = 0.0f;
        public static readonly int DEFAULT_MAX_CACHED_ITEMS = 86400; // number of seconds in a day
        public static readonly long DEFAULT_MAX_CACHE_SIZE_IN_MB = 2^29; // 0.5 GB

        // for purposes of this library, float precision is more then sufficient
        public static readonly float DEFAULT_LAT = DEFAULT_VAL;
        public static readonly float DEFAULT_LON = DEFAULT_VAL;
        public static readonly float DEFAULT_ELEVATION = DEFAULT_VAL;

        public static readonly int DEFAULT_ACTION_ID = 0;
        public static readonly int INITIAL_SEQUENCE_NUMBER = 0;

        public static readonly int DEFAULT_STREAM_FPS = 30;
        public static readonly int DEFAULT_MIN_STREAM_FPS = 8;
        public static readonly int DEFAULT_MAX_STREAM_FPS = 60;
        public static readonly int DEFAULT_STREAM_FRAME_WIDTH = 1440;
        public static readonly int DEFAULT_STREAM_FRAME_HEIGHT = 1080;
        public static readonly int DEFAULT_MIN_MOTION_PIXELS_DIFF = 12000;
        public static readonly int DEFAULT_MAX_CACHED_FRAMES = 10;
        public static readonly string DEFAULT_STREAM_PROTOCOL = "rtsp";

        public static readonly string PRODUCT_NAME = "PDT";
        public static readonly string CLOUD = "Cloud";
        public static readonly string GATEWAY = "Gateway";
        public static readonly string CONSTRAINED = "Constrained";
        public static readonly string DEVICE = "Device";
        public static readonly string SERVICE = "Service";

        public static readonly string CONSTRAINED_DEVICE = CONSTRAINED + DEVICE;
        public static readonly string GATEWAY_SERVICE = GATEWAY + SERVICE;
        public static readonly string CLOUD_SERVICE = CLOUD + SERVICE;

        //////////
        // Property Names
        //

        public static readonly string NAME_PROP = "name";
        public static readonly string DEVICE_ID_PROP = "deviceID";
        public static readonly string TYPE_CATEGORY_ID_PROP = "typeCategoryID";
        public static readonly string TYPE_ID_PROP = "typeID";
        public static readonly string TIMESTAMP_PROP = "timeStamp";
        public static readonly string HAS_ERROR_PROP = "hasError";
        public static readonly string STATUS_CODE_PROP = "statusCode";
        public static readonly string LOCATION_ID_PROP = "locationID";
        public static readonly string LATITUDE_PROP = "latitude";
        public static readonly string LONGITUDE_PROP = "longitude";
        public static readonly string ELEVATION_PROP = "elevation";

        public static readonly string COMMAND_PROP = "command";
        public static readonly string STATE_DATA_PROP = "stateData";
        public static readonly string VALUE_PROP = "value";
        public static readonly string IS_RESPONSE_PROP = "isResponse";

        public static readonly string CPU_UTIL_PROP = "cpuUtil";
        public static readonly string DISK_UTIL_PROP = "diskUtil";
        public static readonly string MEM_UTIL_PROP = "memUtil";

        public static readonly string ACTION_ID_PROP = "actionID";
        public static readonly string DATA_URI_PROP = "dataURI";
        public static readonly string MESSAGE_PROP = "message";
        public static readonly string ENCODING_NAME_PROP = "encodingName";
        public static readonly string RAW_DATA_PROP = "rawData";
        public static readonly string SEQUENCE_NUMBER_PROP = "seqNo";
        public static readonly string USE_SEQUENCE_NUMBER_PROP = "useSeqNo";
        public static readonly string SEQUENCE_NUMBER_TOTAL_PROP = "seqNoTotal";

        public static readonly string RANGE_FLOOR_PROP = "rangeFloor";
        public static readonly string RANGE_CEILING_PROP = "rangeCeiling";

        public static readonly string SEND_RESOURCE_NAME_PROP = "sendResourceName";
        public static readonly string RECEIVE_RESOURCE_NAME_PROP = "receiveResourceName";
        public static readonly string IS_PING_PROP = "isPing";

        public static readonly string MESSAGE_DATA_PROP = "msgData";

        public static readonly string HOST_NAME_PROP = "hostName";
        public static readonly string MESSAGE_IN_COUNT_PROP = "msgInCount";
        public static readonly string MESSAGE_OUT_COUNT_PROP = "msgOutCount";
        public static readonly string IS_CONNECTING_PROP = "isConnecting";
        public static readonly string IS_CONNECTED_PROP = "isConnected";
        public static readonly string IS_DISCONNECTED_PROP = "isDisconnected";

        public static readonly string CMD_DATA_PERSISTENCE_NAME = "pdt-cmd-data";
        public static readonly string CONN_DATA_PERSISTENCE_NAME = "pdt-conn-data";
        public static readonly string SENSOR_DATA_PERSISTENCE_NAME = "pdt-sensor-data";
        public static readonly string SYS_DATA_PERSISTENCE_NAME = "pdt-sys-data";

        //////////
        // Resource and Topic Names
        //

        public static readonly string ACTUATOR_CMD = "ActuatorCmd";
        public static readonly string ACTUATOR_RESPONSE = "ActuatorResponse";
        public static readonly string MGMT_STATUS_MSG = "MgmtStatusMsg";
        public static readonly string MGMT_STATUS_CMD = "MgmtStatusCmd";
        public static readonly string MEDIA_MSG = "MediaMsg";
        public static readonly string SENSOR_MSG = "SensorMsg";
        public static readonly string SYSTEM_PERF_MSG = "SystemPerfMsg";

        public static readonly string UPDATE_NOTIFICATIONS_MSG = "UpdateMsg";
        public static readonly string RESOURCE_REGISTRATION_REQUEST = "ResourceRegRequest";

        public static readonly string LED_ACTUATOR_NAME = "LedActuator";
        public static readonly string SYSTEM_PERF_NAME = "SystemPerfMsg";
        public static readonly string CAMERA_SENSOR_NAME = "CameraSensor";

        public static readonly int COMMAND_OFF = DEFAULT_COMMAND;
        public static readonly int COMMAND_ON = 1;
        public static readonly string COMMAND_KEY = "Command";

        public static readonly string ON_KEY = "ON";
        public static readonly string OFF_KEY = "OFF";
        public static readonly string RUNNING_KEY = "Running";

        public static readonly int DEFAULT_TYPE_ID = 0;
        public static readonly int DEFAULT_TYPE_CATEGORY_ID = 0;
        public static readonly int DEFAULT_ACTUATOR_TYPE = DEFAULT_TYPE_ID;
        public static readonly int DEFAULT_SENSOR_TYPE = DEFAULT_TYPE_ID;

        public static readonly int ENV_TYPE_CATEGORY = 1000;
        public static readonly int ENV_DEVICE_TYPE = 1000;
        public static readonly int HVAC_ACTUATOR_TYPE = 1001;
        public static readonly int HUMIDIFIER_ACTUATOR_TYPE = 1002;
        public static readonly int IMPELLER_RPM_ACTUATOR_TYPE = 1110;

        public static readonly int HUMIDITY_SENSOR_TYPE = 1010;
        public static readonly int PRESSURE_SENSOR_TYPE = 1012;
        public static readonly int TEMP_SENSOR_TYPE = 1013;
        public static readonly int FLUID_VISCOSITY_SENSOR_TYPE = 1020;
        public static readonly int FLUID_RATE_SENSOR_TYPE = 1021;
        public static readonly int IMPELLER_RPM_SENSOR_TYPE = 1022;

        public static readonly string HUMIDIFIER_ACTUATOR_NAME = "HumidifierActuator";
        public static readonly string HVAC_ACTUATOR_NAME = "HvacActuator";
        public static readonly string CIRCULATOR_PUMP_ACTUATOR_NAME = "CirculatorPumpActuator";

        public static readonly string HUMIDITY_SENSOR_NAME = "HumiditySensor";
        public static readonly string PRESSURE_SENSOR_NAME = "PressureSensor";
        public static readonly string TEMP_SENSOR_NAME = "TempSensor";

        public static readonly string PRESSURE_NAME = "Pressure";
        public static readonly string VISCOSITY_NAME = "Viscosity";
        public static readonly string FLOW_RATE_NAME = "FlowRate";
        public static readonly string RPM_NAME = "RPM";

        public static readonly int DISPLAY_CATEGORY_TYPE = 2000;
        public static readonly int DISPLAY_DEVICE_TYPE = 2000;
        public static readonly int LED_DISPLAY_ACTUATOR_TYPE = 2001;

        public static readonly string MEDIA_TYPE_NAME = "MediaType";
        public static readonly int MEDIA_TYPE_CATEGORY = 3000;
        public static readonly int DEFAULT_MEDIA_TYPE = 3000;
        public static readonly int MEDIA_DEVICE_TYPE = 3000;
        public static readonly int CAMERA_SENSOR_TYPE = 3101;
        public static readonly int VIDEO_SYSTEM_TYPE = 3201;
        public static readonly int AUDIO_SYSTEM_TYPE = 3301;
        public static readonly int LIGHTING_SYSTEM_TYPE = 3401;

        public static readonly string LUMENS_NAME = "Lumens";
        public static readonly string DECIBELS_NAME = "Decibels";
        public static readonly string COLOR_NAME = "Color";
        public static readonly string PLAYING_NAME = "Playing";

        public static readonly int ENERGY_TYPE_CATEGORY = 4000;
        public static readonly int STORAGE_LEVEL = 4001;
        public static readonly int STORAGE_DRAW = 4002;

        public static readonly string STORAGE_LEVEL_NAME = "StorageLevel";
        public static readonly string STORAGE_DRAW_NAME = "StorageDraw";

        public static readonly int OIL_SYSTEM_TYPE = 4101;
        public static readonly int NATGAS_SYSTEM_TYPE = 4102;
        public static readonly int PROPANE_SYSTEM_TYPE = 4103;
        public static readonly int WIND_SYSTEM_TYPE = 4201;
        public static readonly int SOLAR_SYSTEM_TYPE = 4301;
        public static readonly int HYDRO_SYSTEM_TYPE = 4401;
        public static readonly int GEOTHERMAL_SYSTEM_TYPE = 4501;
        public static readonly int HYDROGEN_SYSTEM_TYPE = 4601;

        public static readonly string WATTAGE_NAME = "Wattage";
        public static readonly string AMPERAGE_NAME = "Amperage";
        public static readonly string VOLTAGE_NAME = "Voltage";

        public static readonly int SYSTEM_TYPE_CATEGORY = 8000;
        public static readonly int SYSTEM_MGMT_TYPE = 8000;
        public static readonly int RESOURCE_MGMT_TYPE = 8001;
        public static readonly int SYSTEM_CONN_STATE_TYPE = 8002;

        public static readonly string RESOURCE_MGMT_NAME = "ResourceMgmt";

        public static readonly int SYSTEM_PERF_TYPE = 9000;
        public static readonly int CPU_UTIL_TYPE = 9001;
        public static readonly int MEM_UTIL_TYPE = 9002;
        public static readonly int DISK_UTIL_TYPE = 9003;

        public static readonly string CPU_UTIL_NAME = "DeviceCpuUtil";
        public static readonly string DISK_UTIL_NAME = "DeviceDiskUtil";
        public static readonly string MEM_UTIL_NAME = "DeviceMemUtil";

        public static readonly int MESSAGE_TYPE_CATEGORY = 10000;
        public static readonly int MESSAGE_TYPE = 10001;

        //////////
        // status codes
        //
        //
        public static readonly int NOMINAL_STATUS_CODE = 0;
        public static readonly int CONN_SUCCESS_STATUS_CODE = 200;
        public static readonly int CONN_IN_PROC_STATUS_CODE = 201;
        public static readonly int PUB_SUCCESS_STATUS_CODE = 210;
        public static readonly int SUB_SUCCESS_STATUS_CODE = 220;
        public static readonly int DISCONN_SUCCESS_STATUS_CODE = 300;
        public static readonly int DISCONN_IN_PROC_STATUS_CODE = 301;
        public static readonly int CONN_FAILURE_STATUS_CODE = 400;
        public static readonly int MSG_SEND_FAILURE_STATUS_CODE = 401;

        public static readonly string NO_STATUS_MSG = "No_Status";

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
        // for CDA to GDA communications
        // e.g., PIOT/ConstrainedDevice/ActuatorCmd
        // e.g., PIOT/ConstrainedDevice/SensorMsg

        public static readonly string CDA_UPDATE_NOTIFICATIONS_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + UPDATE_NOTIFICATIONS_MSG;
        public static readonly string CDA_ACTUATOR_CMD_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + ACTUATOR_CMD;
        public static readonly string CDA_ACTUATOR_RESPONSE_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + ACTUATOR_RESPONSE;
        public static readonly string CDA_MGMT_STATUS_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + MGMT_STATUS_MSG;
        public static readonly string CDA_MGMT_CMD_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + MGMT_STATUS_CMD;
        public static readonly string CDA_MEDIA_DATA_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + MEDIA_MSG;
        public static readonly string CDA_REGISTRATION_REQUEST_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + RESOURCE_REGISTRATION_REQUEST;
        public static readonly string CDA_SENSOR_DATA_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + SENSOR_MSG;
        public static readonly string CDA_SYSTEM_PERF_MSG_RESOURCE = PRODUCT_NAME + RESOURCE_SEPARATOR + CONSTRAINED_DEVICE + RESOURCE_SEPARATOR + SYSTEM_PERF_MSG;

        //////////
        // Configuration Sections, Keys and Defaults
        //
        //
        // NOTE: You may need to update these paths if you change
        // the directory structure for python-components

        public static readonly string LOCAL = "Local";
        public static readonly string MQTT = "Mqtt";
        public static readonly string DATA = "Data";

        public static readonly string DEVICE_ID_KEY = "deviceID";
        public static readonly string DEVICE_LOCATION_ID_KEY = "deviceLocationID";

        public static readonly string CLOUD_GATEWAY_SERVICE = CLOUD + "." + GATEWAY_SERVICE;
        public static readonly string MQTT_GATEWAY_SERVICE = MQTT + "." + GATEWAY_SERVICE;
        public static readonly string DATA_GATEWAY_SERVICE = DATA + "." + GATEWAY_SERVICE;

        public static readonly string CRED_SECTION = "Credentials";

        public static readonly string FROM_ADDRESS_KEY = "fromAddr";
        public static readonly string TO_ADDRESS_KEY = "toAddr";
        public static readonly string TO_MEDIA_ADDRESS_KEY = "toMediaAddr";
        public static readonly string TO_TXT_ADDRESS_KEY = "toTxtAddr";

        public static readonly string HOST_KEY = "host";
        public static readonly string PORT_KEY = "port";
        public static readonly string SECURE_PORT_KEY = "securePort";

        public static readonly string ROOT_CERT_ALIAS = "root";

        public static readonly string KEY_STORE_CLIENT_IDENTITY_KEY = "keyStoreClientIdentity";
        public static readonly string KEY_STORE_SERVER_IDENTITY_KEY = "keyStoreServerIdentity";

        public static readonly string KEY_STORE_FILE_KEY = "keyStoreFile";
        public static readonly string KEY_STORE_AUTH_KEY = "keyStoreAuth";
        public static readonly string TRUST_STORE_FILE_KEY = "trustStoreFile";
        public static readonly string TRUST_STORE_ALIAS_KEY = "trustStoreAlias";
        public static readonly string TRUST_STORE_AUTH_KEY = "trustStoreAuth";
        public static readonly string USER_NAME_TOKEN_KEY = "userToken";
        public static readonly string USER_AUTH_TOKEN_KEY = "authToken";
        public static readonly string API_TOKEN_KEY = "apiToken";
        public static readonly string CLIENT_TOKEN_KEY = "clientToken";

        public static readonly string ORGANIZATION_ID_KEY = "orgID";
        public static readonly string ENV_DATA_BUCKET_ID = "envDataBucketID";
        public static readonly string STATE_DATA_BUCKET_ID = "stateDataBucketID";
        public static readonly string SYS_PERF_DATA_BUCKET_ID = "sysPerfDataBucketID";
        public static readonly string CMD_DATA_BUCKET_ID = "cmdDataBucketID";

        public static readonly string CERT_FILE_KEY = "certFile";
        public static readonly string CRED_FILE_KEY = "credFile";
        public static readonly string ENABLE_AUTH_KEY = "enableAuth";
        public static readonly string ENABLE_CRYPT_KEY = "enableCrypt";
        public static readonly string ENABLE_SIMULATOR_KEY = "enableSimulator";
        public static readonly string ENABLE_EMULATOR_KEY = "enableEmulator";
        public static readonly string ENABLE_SENSE_HAT_KEY = "enableSenseHAT";
        public static readonly string ENABLE_LOGGING_KEY = "enableLogging";
        public static readonly string USE_WEB_ACCESS_KEY = "useWebAccess";
        public static readonly string POLL_CYCLES_KEY = "pollCycleSecs";
        public static readonly string KEEP_ALIVE_KEY = "keepAlive";
        public static readonly string DEFAULT_QOS_KEY = "defaultQos";

        public static readonly string ENABLE_MQTT_CLIENT_KEY = "enableMqttClient";

        public static readonly string ENABLE_SYSTEM_PERF_KEY = "enableSystemPerformance";
        public static readonly string ENABLE_SENSING_KEY = "enableSensing";

        public static readonly string HUMIDITY_SIM_FLOOR_KEY = "humiditySimFloor";
        public static readonly string HUMIDITY_SIM_CEILING_KEY = "humiditySimCeiling";
        public static readonly string PRESSURE_SIM_FLOOR_KEY = "pressureSimFloor";
        public static readonly string PRESSURE_SIM_CEILING_KEY = "pressureSimCeiling";
        public static readonly string TEMP_SIM_FLOOR_KEY = "tempSimFloor";
        public static readonly string TEMP_SIM_CEILING_KEY = "tempSimCeiling";

        public static readonly string HANDLE_TEMP_CHANGE_ON_DEVICE_KEY = "handleTempChangeOnDevice";
        public static readonly string TRIGGER_HVAC_TEMP_FLOOR_KEY = "triggerHvacTempFloor";
        public static readonly string TRIGGER_HVAC_TEMP_CEILING_KEY = "triggerHvacTempCeiling";

        public static readonly string RUN_FOREVER_KEY = "runForever";
        public static readonly string TEST_EMPTY_APP_KEY = "testEmptyApp";

        public static readonly string STREAM_HOST_ADDR_KEY = "streamHostAddr";
        public static readonly string STREAM_HOST_LABEL_KEY = "streamHostLabel";
        public static readonly string STREAM_PORT_KEY = "streamPort";
        public static readonly string STREAM_PROTOCOL_KEY = "streamProtocol";
        public static readonly string STREAM_PATH_KEY = "streamPath";
        public static readonly string STREAM_ENCODING_KEY = "streamEncoding";
        public static readonly string STREAM_FRAME_WIDTH_KEY = "streamFrameWidth";
        public static readonly string STREAM_FRAME_HEIGHT_KEY = "streamFrameHeight";
        public static readonly string STREAM_FPS_KEY = "streamFps";
        public static readonly string IMAGE_FILE_EXT_KEY = "imageFileExt";
        public static readonly string VIDEO_FILE_EXT_KEY = "videoFileExt";
        public static readonly string MIN_MOTION_PIXELS_DIFF_KEY = "minMotionPixelsDiff";

        public static readonly string IMAGE_ENCODING_KEY = "imageEncoding";
        public static readonly string IMAGE_DATA_STORE_PATH = "imageDataStorePath";
        public static readonly string VIDEO_DATA_STORE_PATH = "videoDataStorePath";
        public static readonly string MAX_MOTION_FRAMES_BEFORE_ACTION_KEY = "maxMotionFramesBeforeAction";
        public static readonly string MAX_CACHED_FRAMES_KEY = "maxCachedFrames";
        public static readonly string STORE_INTERIM_FRAMES_KEY = "storeInterimFrames";
        public static readonly string INCLUDE_RAW_IMAGE_DATA_IN_MSG_KEY = "includeRawImageDataInMsg";

    }
}
