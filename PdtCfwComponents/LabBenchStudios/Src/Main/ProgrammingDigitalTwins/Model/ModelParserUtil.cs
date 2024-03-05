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
using System.Linq;

using DTDLParser;
using DTDLParser.Models;

namespace LabBenchStudios.Pdt.Model
{

    /**
     * This class is based on DataUtil, and is expected to add future parsing
     * and serilization / deserialization capabilities.
     * 
     * For now, it serves as a basic DTDL parsing and validation class only.
     * 
     */
    public static class ModelParserUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtdlJson"></param>
        public static void DisplayDtdlItems(string dtdlJson)
        {
            ModelParser modelParser = new();

            var objectModel = modelParser.Parse(dtdlJson);

            foreach (var i in objectModel.Values)
            {
                Console.WriteLine(i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsValidDtdlJsonFile(string fileName)
        {
            string jsonData = LoadDtdlFile(fileName);

            if (jsonData != null && jsonData.Length > 0)
            {
                return IsValidDtdlJsonData(jsonData);
            }
            else
            {
                Console.WriteLine($"DTDL file returned empty JSON data: {fileName}");

                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static bool IsValidDtdlJsonData(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    ModelParser modelParser = new();

                    var objectModel = modelParser.Parse(jsonData);

                    return true;
                }
                catch (ResolutionException ex)
                {
                    Console.WriteLine($"DTDL model is referentially incomplete. Exception: {ex}");
                }
                catch (ParsingException ex)
                {
                    Console.WriteLine($"DTDL model cannot be parsed - invalid. Exception: {ex}");
                }
            }

            return false;
        }

        /// <summary>
        /// NOTE: The order of the list doesn't matter to the DTDL parser - all extended
        /// ID's simply need to be part of the IEnumerable passed to the parser
        /// 
        /// </summary>
        /// <param name="jsonDataList"></param>
        /// <returns></returns>
        public static bool IsValidDtdlJsonData(IEnumerable<string> jsonDataList)
        {
            if (jsonDataList != null && jsonDataList.Count() > 0)
            {
                try
                {
                    ModelParser modelParser = new();

                    var objectModel = modelParser.Parse(jsonDataList);

                    return true;
                }
                catch (ResolutionException ex)
                {
                    Console.WriteLine($"DTDL model is referentially incomplete. Exception: {ex}");
                }
                catch (ParsingException ex)
                {
                    Console.WriteLine($"DTDL model cannot be parsed - invalid. Exception: {ex}");
                }
            }

            return false;
        }

        /// <summary>
        /// Loads all DTDL models from the given path into a read only dictionary indexed
        /// by DTMI string, containing DTInterfaceInfo instances. This is likely the most useful
        /// static method in this utility class, as most functionality will be derived from
        /// looking up, and operating on, a DTInterfaceInfo using the DTMI string (NOT the
        /// Dtmi type).
        /// 
        /// Note that this does NOT distinguish between model files, as a single model file
        /// can declare multiple DTEntityInfo's.
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <returns>IReadOnlyDictionary<string, DTInterfaceInfo></returns>
        public static IReadOnlyDictionary<string, DTInterfaceInfo> LoadAllDtdlInterfaces(string modelFilePath)
        {
            IReadOnlyDictionary<Dtmi, DTEntityInfo> modelEntities = LoadAllDtdlModels(modelFilePath);
            Dictionary<string, DTInterfaceInfo> modelInterfaces = null;

            if (modelEntities != null && modelEntities.Count > 0)
            {
                modelInterfaces = new Dictionary<string, DTInterfaceInfo>();

                foreach (Dtmi dtmi in modelEntities.Keys)
                {
                    DTEntityInfo entityInfo = modelEntities[dtmi];

                    switch (entityInfo.EntityKind)
                    {
                        case DTEntityKind.Interface:
                            Console.WriteLine($" --> DTInterfaceInfo DTMI: {dtmi.AbsoluteUri}");
                            modelInterfaces.Add(dtmi.AbsoluteUri, (DTInterfaceInfo) entityInfo);
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error generating DTDL model interfaces from file path {modelFilePath}. None found.");
            }

            return modelInterfaces;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string LoadDtdlFile(string pathName, string fileName)
        {
            if (!string.IsNullOrEmpty(pathName) && !string.IsNullOrEmpty(fileName))
            {
                if (Directory.Exists(pathName))
                {
                    string absFileName = Path.Combine(pathName, fileName);

                    return LoadDtdlFile(absFileName);
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string LoadDtdlFile(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    Console.WriteLine($"Attempting to load DTDL file: {fileName}");

                    using (StreamReader streamReader = new StreamReader(fileName))
                    {
                        string jsonData = streamReader.ReadToEnd();

                        return jsonData;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DTDL file cannot be loaded: {fileName}. Exception: {ex}");
                }
            }
            else
            {
                Console.WriteLine($"DTDL file is invalid (null or empty). Ignoring");
            }

            return null;
        }

        /// <summary>
        /// Loads all DTDL models from the given path into a read only dictionary indexed
        /// by Dtmi, containing DTEntityInfo instances. This is the highest level generic
        /// model representation available.
        /// 
        /// Note that this does NOT distinguish between model files, as a single model file
        /// can declare multiple DTEntityInfo's.
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <returns>IReadOnlyDictionary<Dtmi, DTEntityInfo></returns>
        public static IReadOnlyDictionary<Dtmi, DTEntityInfo> LoadAllDtdlModels(string modelFilePath)
        {
            IReadOnlyDictionary<Dtmi, DTEntityInfo> modelDictionary = null;

            if (!string.IsNullOrEmpty(modelFilePath) && Directory.Exists(modelFilePath))
            {
                var modelJsonList = new List<string>();

                var modelFileList =
                    Directory.GetFiles(modelFilePath, ModelNameUtil.MODEL_FILE_NAME_PATTERN);

                foreach (var modelFileName in modelFileList)
                {
                    string jsonData = ModelParserUtil.LoadDtdlFile(modelFileName);
                    modelJsonList.Add(jsonData);

                    Console.WriteLine($"Loaded DTDL JSON for model file: {modelFileName}");
                }

                ModelParser modelParser = new();

                modelDictionary = modelParser.Parse(modelJsonList);

                if (modelDictionary != null)
                {
                    Console.WriteLine($"Validated and loaded DTDL JSON from path: {modelFilePath}");
                }
                else
                {
                    Console.WriteLine($"Failed to load and validate DTDL JSON from path: {modelFilePath}");
                }
            }
            else
            {
                Console.WriteLine($"Invalid file path {modelFilePath}. Dtdl models not loaded.");
            }

            return modelDictionary;
        }

    }

}
