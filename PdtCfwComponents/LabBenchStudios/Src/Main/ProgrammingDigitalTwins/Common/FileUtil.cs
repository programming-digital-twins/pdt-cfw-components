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

namespace LabBenchStudios.Pdt.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileUtil
    {

        // private member vars


        // public methods

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
