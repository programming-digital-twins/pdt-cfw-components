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
using System.Collections.Generic;
using System.IO;
using System.Text;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Util
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileUtil
    {
        // public static methods

        /// <summary>
        /// 
        /// </summary>
        public enum PersistenceDataTypeEnum
        {
            Historian,
            Prediction,
            IotData,
            Text
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public static string CreateDataHistorianFileName(string cacheName)
        {
            return CreateDataHistorianFileName(ConfigConst.DEFAULT_FILE_STORAGE_PATH, cacheName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathPrefix"></param>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public static string CreateDataHistorianFileName(string pathPrefix, string cacheName)
        {
            string historianCachePath = CreateHistorianCacheFilePath(pathPrefix);
            string historianCacheFile = CreateDataCacheFileName(cacheName, historianCachePath);

            return historianCacheFile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathPrefix"></param>
        /// <returns></returns>
        public static string CreateHistorianCacheFilePath(string pathPrefix)
        {
            return InitializeDataFilePath(pathPrefix, ConfigConst.HISTORIAN_CACHE_NAME);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public static string CreatePredictionFileName(string cacheName)
        {
            return CreatePredictionFileName(ConfigConst.DEFAULT_FILE_STORAGE_PATH, cacheName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathPrefix"></param>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public static string CreatePredictionFileName(string pathPrefix, string cacheName)
        {
            string predictionCachePath = CreatePredictionCacheFilePath(pathPrefix);
            string predictionCacheFile = CreateDataCacheFileName(cacheName, predictionCachePath);

            return predictionCacheFile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathPrefix"></param>
        /// <returns></returns>
        public static string CreatePredictionCacheFilePath(string pathPrefix)
        {
            return InitializeDataFilePath(pathPrefix, ConfigConst.PREDICTION_CACHE_NAME);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public static string CreateTextCacheFileName(string cacheName)
        {
            return CreateTextCacheFileName(ConfigConst.DEFAULT_FILE_STORAGE_PATH, cacheName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathPrefix"></param>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public static string CreateTextCacheFileName(string pathPrefix, string cacheName)
        {
            string textCachePath = CreateTextCacheFilePath(pathPrefix);
            string textCacheFile = CreateDataCacheFileName(cacheName, textCachePath);

            return textCacheFile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathPrefix"></param>
        /// <returns></returns>
        public static string CreateTextCacheFilePath(string pathPrefix)
        {
            return InitializeDataFilePath(pathPrefix, ConfigConst.TEXT_CACHE_NAME);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public static string CreateObjectStoreFileName(string cacheName)
        {
            return CreateObjectStoreFileName(ConfigConst.DEFAULT_FILE_STORAGE_PATH, cacheName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathPrefix"></param>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public static string CreateObjectStoreFileName(string pathPrefix, string cacheName)
        {
            string objectStorePath = CreateObjectStoreFilePath(pathPrefix);
            string objectStoreFile = CreateDataCacheFileName(cacheName, objectStorePath);

            return objectStoreFile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathPrefix"></param>
        /// <returns></returns>
        public static string CreateObjectStoreFilePath(string pathPrefix)
        {
            return InitializeDataFilePath(pathPrefix, ConfigConst.DATA_STORE_NAME);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathPrefix"></param>
        /// <param name="subPath"></param>
        /// <returns></returns>
        public static string InitializeDataFilePath(string pathPrefix, string subPath)
        {
            if (string.IsNullOrEmpty(pathPrefix))
            {
                pathPrefix = ConfigConst.DEFAULT_FILE_STORAGE_PATH;

                Console.WriteLine($"Null or empty data file path prefix. Using default: {pathPrefix}");
            }

            if (!Directory.Exists(pathPrefix))
            {
                pathPrefix = ConfigConst.DEFAULT_FILE_STORAGE_PATH;

                Console.WriteLine($"Data file path doesn't exist. Attempting to use default: {pathPrefix}");
            }

            string dataFilePath = null;

            if (string.IsNullOrWhiteSpace(subPath))
            {
                dataFilePath = pathPrefix;
            } else
            {
                dataFilePath = Path.Combine(pathPrefix, subPath);
            }

            dataFilePath = Path.GetFullPath(dataFilePath);

            // make sure the path exists
            if (!Directory.Exists(dataFilePath))
            {
                // path doesn't exist - try to create it
                try
                {
                    DirectoryInfo dirInfo = Directory.CreateDirectory(dataFilePath);

                    Console.WriteLine($"Data file path created: {dataFilePath}. Info: {dirInfo}");

                    return dataFilePath;
                } catch (Exception e)
                {
                    Console.WriteLine($"Failed to create data file path {dataFilePath}. Error: {e.Message}");
                }
            } else
            {
                // path already exists - try to access it
                try
                {
                    string pathInfo = Directory.GetDirectoryRoot(dataFilePath);

                    Console.WriteLine($"Data file path exists: {dataFilePath}. Info: {pathInfo}");

                    return dataFilePath;
                } catch (Exception e)
                {
                    Console.WriteLine($"Failed to access existing data file path {dataFilePath}. Error: {e.Message}");
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsAccessible(string fileName)
        {
            return IsAccessible(fileName, true);
        }

        /// <summary>
        /// A simple and reasonably quick check if a file might be accessible.
        /// This avoids the obligatory DirectorySecurity checks and such, as
        /// it's non-atomic and can't guarantee access.
        /// 
        /// As such, this is a best guess with performance and simplicity paramount.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="inclWriteable"></param>
        /// <returns></returns>
        public static bool IsAccessible(string fileName, bool inclWriteable)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                FileInfo fileInfo = new FileInfo(fileName);

                if ((fileInfo.Attributes & FileAttributes.Directory) != 0)
                {
                    return false;
                }

                if (inclWriteable)
                {
                    if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
                    {
                        return false;
                    }
                }

                // if we get this far, it's probably a writeable file, although
                // there are no guarantees it's writeable by the calling user
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="pathPrefix"></param>
        /// <returns></returns>
        public static string CreateResourceFileName(ResourceNameContainer resource, string pathPrefix)
        {
            return CreateResourceFileName(resource, pathPrefix, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="pathPrefix"></param>
        /// <param name="useDate"></param>
        /// <returns></returns>
        public static string CreateResourceFileName(ResourceNameContainer resource, string pathPrefix, bool useDate)
        {
            string deviceID = resource.DeviceName;
            string locationID = resource.DeviceLocation;
            string dataType = ConfigConst.NOT_SET;

            if (resource.IsActuationResource)
            {
                dataType = nameof(ActuatorData);
            } else if (resource.IsConnStateResource)
            {
                dataType = nameof(ConnectionStateData);
            } else if (resource.IsMediaResource)
            {
                dataType = nameof(MediaData);
            } else if (resource.IsMessageResource)
            {
                dataType = nameof(MessageData);
            } else if (resource.IsSensingResource)
            {
                dataType = nameof(SensorData);
            } else if (resource.IsSystemResource)
            {
                dataType = nameof(SystemPerformanceData);
            } else
            {
                dataType = nameof(IotDataContext);
            }

            StringBuilder fileNameBuilder = new StringBuilder(dataType);

            if (useDate)
            {
                fileNameBuilder.Append('_');
                fileNameBuilder.Append(DateTime.UtcNow.ToString(ConfigConst.FILE_DATE_TIME_FORMAT));
            }

            string absFileName = null;

            try
            {
                absFileName =
                    Path.GetFullPath(Path.Combine(pathPrefix, locationID, deviceID, fileNameBuilder.ToString()));

                Console.WriteLine($"Created resource absolute path name: {absFileName}");
            } catch (Exception e)
            {
                Console.WriteLine($"Failed to create resource absolute path name: {absFileName}. Exception: {e.Message}");
            }

            return absFileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="pathPrefix"></param>
        /// <returns></returns>
        public static string CreateDataCacheFileName(string cacheName, string pathPrefix)
        {
            return CreateDataCacheFileName(cacheName, pathPrefix, false, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="pathPrefix"></param>
        /// <param name="useDate"></param>
        /// <param name="useJsonExt"></param>
        /// <returns></returns>
        public static string CreateDataCacheFileName(string cacheName, string pathPrefix, bool useDate, bool useJsonExt)
        {
            string fileName = cacheName;

            if (useDate)
            {
                fileName = fileName + "_" + DateTime.UtcNow.ToString(ConfigConst.FILE_DATE_TIME_FORMAT);
            }

            if (useJsonExt)
            {
                fileName = fileName + ConfigConst.JSON_EXT;
            }

            string absFileName = null;

            try
            {
                absFileName = Path.GetFullPath(Path.Combine(pathPrefix, fileName));

                Console.WriteLine($"Created historian cache absolute path name: {absFileName}");
            } catch (Exception e)
            {
                Console.WriteLine($"Failed to create historian cache absolute path name: {absFileName}. Exception: {e.Message}");
            }

            return absFileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetFileListing(string path)
        {
            return GetFileListing(path, null, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="includePath"></param>
        /// <returns></returns>
        public static List<string> GetFileListing(string path, string searchPattern, bool includePath)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    string absPath = Path.GetFullPath(path);

                    if (Directory.Exists(path))
                    {
                        bool useSearchPattern = false;

                        if (!string.IsNullOrEmpty(searchPattern))
                        {
                            if (!searchPattern.Contains("*"))
                            {
                                searchPattern = "*" + searchPattern;
                            }

                            useSearchPattern = true;
                        }

                        try
                        {
                            string[] fileNames = null;

                            if (useSearchPattern)
                            {
                                Console.WriteLine($"Getting file listing for path {path} with extension {searchPattern}.");

                                fileNames = Directory.GetFiles(path, searchPattern);
                            } else
                            {
                                Console.WriteLine($"Getting file listing for path {path}.");

                                fileNames = Directory.GetFiles(path);
                            }

                            List<string> fileList = new List<string>();

                            foreach (string file in fileNames)
                            {
                                if (includePath)
                                {
                                    fileList.Add(file);
                                } else
                                {
                                    fileList.Add(Path.GetFileName(file));
                                }
                            }

                            return fileList;

                        } catch (Exception e)
                        {
                            Console.WriteLine($"Failed to retrieve path listing for {path} with files ending in {searchPattern}. Exception: {e.Message}");
                        }
                    }
                } catch (Exception e)
                {
                    Console.WriteLine($"Failed to process path and get file listing for {path}. Exception: {e.Message}");
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public static string GetStringifiedFileListing(List<string> fileList)
        {
            if (fileList != null && fileList.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                foreach (string file in fileList)
                {
                    builder.Append('\t').Append(file).Append('\n');
                }

                return builder.ToString();
            }

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string TrimFileExt(string fileName)
        {
            if (! string.IsNullOrWhiteSpace(fileName))
            {
                return Path.GetFileNameWithoutExtension(fileName);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadDataFromFile(string fileName)
        {
            try
            {
                string data = File.ReadAllText(fileName);

                int bytesRead = data.Length;

                return data;
            } catch (Exception e)
            {
                Console.WriteLine($"Failed to read data cache from file {fileName}. Error: {e.Message}");
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static int WriteDataToFile(string fileName, string data, bool overwrite)
        {
            int bytesWritten = 0;

            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(data))
            {
                Console.WriteLine($"Storing {data.Length} bytes to data cache at location {fileName}.");

                try
                {
                    if (overwrite)
                    {
                        File.Create(fileName).Close();
                    }

                    File.WriteAllText(fileName, data);

                    bytesWritten = data.Length;

                    if (bytesWritten > 0)
                    {
                        Console.WriteLine($"Successfully stored data cache to location {fileName}. Total bytes: {bytesWritten}.");
                    } else
                    {
                        Console.WriteLine($"No data stored for data cache to location {fileName}.");
                    }
                } catch (Exception e)
                {
                    bytesWritten = -1;

                    Console.WriteLine($"Failed to write data cache to file {fileName}. Error: {e.Message}");
                }

                return bytesWritten;
            } else
            {
                Console.WriteLine($"Failed to write text data to file. Filename {fileName} or text is null or empty.");
            }

            return 0;
        }

    }

}
