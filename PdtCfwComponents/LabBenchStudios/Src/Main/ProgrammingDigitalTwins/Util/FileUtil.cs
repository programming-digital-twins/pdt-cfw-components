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
        /// <param name="resource"></param>
        /// <param name="pathPrefix"></param>
        /// <returns></returns>
        public static string CreateAbsFileName(ResourceNameContainer resource, string pathPrefix)
        {
            return CreateAbsFileName(resource, pathPrefix, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="pathPrefix"></param>
        /// <param name="useDate"></param>
        /// <returns></returns>
        public static string CreateAbsFileName(ResourceNameContainer resource, string pathPrefix, bool useDate)
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

            string absPath =
                Path.GetFullPath(Path.Combine(pathPrefix, locationID, deviceID, fileNameBuilder.ToString()));

            return absPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="pathPrefix"></param>
        /// <returns></returns>
        public static string CreateAbsFileName(string cacheName, string pathPrefix)
        {
            return CreateAbsFileName(cacheName, pathPrefix, false, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="pathPrefix"></param>
        /// <param name="useDate"></param>
        /// <returns></returns>
        public static string CreateAbsFileName(string cacheName, string pathPrefix, bool useDate)
        {
            return CreateAbsFileName(cacheName, pathPrefix, useDate, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="pathPrefix"></param>
        /// <param name="useDate"></param>
        /// <param name="useJsonExt"></param>
        /// <returns></returns>
        public static string CreateAbsFileName(string cacheName, string pathPrefix, bool useDate, bool useJsonExt)
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

            string absFileName = Path.GetFullPath(Path.Combine(pathPrefix, cacheName, fileName));

            return absFileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetFileListing(string path)
        {
            return GetFileListing(path, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string[] GetFileListing(string path, string ext)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                if (Directory.Exists(path))
                {
                    bool useSearchPattern = false;

                    if (!string.IsNullOrEmpty(ext))
                    {
                        if (!ext.Contains("*"))
                        {
                            ext = "*" + ext;
                        }

                        useSearchPattern = true;
                    }

                    try
                    {
                        string[] fileNames = null;

                        if (useSearchPattern)
                        {
                            fileNames = Directory.GetFiles(path, ext);
                        } else
                        {
                            fileNames = Directory.GetFiles(path);
                        }

                        return fileNames;

                    } catch (Exception e)
                    {
                        Console.WriteLine($"Failed to retrieve path listing for {path} with files ending in {ext}. Exception: {e.Message}");
                    }
                }
            }

            return null;
        }

    }

}
