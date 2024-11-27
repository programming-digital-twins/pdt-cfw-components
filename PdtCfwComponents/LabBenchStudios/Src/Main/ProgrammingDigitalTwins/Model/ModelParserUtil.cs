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
        public const bool LOAD_PATH_FILES_SIMULTANEOUSLY = true;

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

        public static ModelNameUtil.DtmiPropertyTypeEnum GetPropertyType(DTEntityKind entityKind)
        {
            switch (entityKind)
            {
                case DTEntityKind.Boolean:
                    return ModelNameUtil.DtmiPropertyTypeEnum.Toggle;

                case DTEntityKind.Date:
                    return ModelNameUtil.DtmiPropertyTypeEnum.Schedule;

                case DTEntityKind.DateTime:
                    return ModelNameUtil.DtmiPropertyTypeEnum.Schedule;

                case DTEntityKind.Time:
                    return ModelNameUtil.DtmiPropertyTypeEnum.Schedule;

                case DTEntityKind.Duration:
                    return ModelNameUtil.DtmiPropertyTypeEnum.Schedule;

                case DTEntityKind.Double:
                    return ModelNameUtil.DtmiPropertyTypeEnum.Value;

                case DTEntityKind.Float:
                    return ModelNameUtil.DtmiPropertyTypeEnum.Value;

                case DTEntityKind.Integer:
                    return ModelNameUtil.DtmiPropertyTypeEnum.Count;

                case DTEntityKind.Long:
                    return ModelNameUtil.DtmiPropertyTypeEnum.Count;

                case DTEntityKind.String:
                    return ModelNameUtil.DtmiPropertyTypeEnum.Message;

                default:
                    return ModelNameUtil.DtmiPropertyTypeEnum.Undefined;

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
        /// <returns>List<DtdlModelContainer></returns>
        public static List<DtdlModelContainer> LoadAllDtdlInterfaces(string modelFilePath)
        {
            List<DtdlModelContainer> modelEntities = LoadDtdlModelsFromPath(modelFilePath);
            //Dictionary<string, DTInterfaceInfo> modelInterfaces = null;

            if (modelEntities != null && modelEntities.Count > 0)
            {
                //modelInterfaces = new Dictionary<string, DTInterfaceInfo>();

                /**
                int counter = 0;

                foreach (DtdlModelContainer modelContainer in modelEntities) {
                    IReadOnlyDictionary<Dtmi, DTEntityInfo> modelEntity = modelContainer.GetEntityTable();

                    foreach (Dtmi dtmi in modelEntity.Keys) {
                        DTEntityInfo entityInfo = modelEntity[dtmi];

                        switch (entityInfo.EntityKind) {
                            case DTEntityKind.Interface:
                                modelContainer.AddInterfaceRef(dtmi.AbsoluteUri, (DTInterfaceInfo)entityInfo);
                                //modelInterfaces.Add(dtmi.AbsoluteUri, (DTInterfaceInfo)entityInfo);
                                Console.WriteLine($" --> DTInterfaceInfo DTMI: {dtmi.AbsoluteUri}. Count: {++counter}");
                                break;
                        }
                    }
                }
                */
            }
            else
            {
                Console.WriteLine($"Error generating DTDL model interfaces from file path {modelFilePath}. None found.");
            }

            //return modelInterfaces;
            return modelEntities;
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
                    Console.WriteLine($"Loading DTDL file: {fileName}");

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
        /// can declare multiple DTEntityInfo's. This method does, however, look for a wide
        /// range of files that simply contain the pattern '*.json' in the name.
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <returns>List<DtdlModelContainer></returns>
        public static List<DtdlModelContainer> LoadDtdlModelsFromPath(string modelFilePath)
        {
            return LoadDtdlModelEntities(modelFilePath, "*.json");
        }


        // private methods

        /// <summary>
        /// Loads all DTDL models from the given path into a read only dictionary indexed
        /// by Dtmi, containing DTEntityInfo instances. This is the highest level generic
        /// model representation available.
        /// 
        /// Note that this does NOT distinguish between model files, as a single model file
        /// can declare multiple DTEntityInfo's.
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <param name="filePattern"></param>
        /// <returns>DtdlModelContainer</returns>
        private static List<DtdlModelContainer> LoadDtdlModelEntities(string modelFilePath, string filePattern)
        {
            List<DtdlModelContainer> modelContainerList = new List<DtdlModelContainer>();

            // path has already been validated by caller but do so again to be sure
            if (!string.IsNullOrEmpty(modelFilePath) && Directory.Exists(modelFilePath)) {
                var modelFileList = Directory.GetFiles(modelFilePath, filePattern);

                ModelParser modelParser = new();

                // NOTE: The DTDL parser will fail if one DTDL depends upon another but
                // is not loaded simultaneously. Checking the LOAD_PATH_FILES_SIMULTANEOUSLY
                // flag allows the internal DTDL cache within the ModelParser to be loaded
                // to avoid any errors thrown during single file parsing.
                //
                // While this incurs double parsing of each directory containing DTDL files,
                // it allows for a simple mapping between DTMI URI's and their associated file
                // names, which is useful for simple lookup procedures.

                Console.WriteLine($"Loading all files from path: {modelFilePath}");

                var modelJsonList = new List<string>();

                foreach (var modelFileName in modelFileList) {
                    string jsonData = ModelParserUtil.LoadDtdlFile(modelFileName);
                    modelJsonList.Add(jsonData);

                    Console.WriteLine($"  -> Loaded DTDL JSON for model file: {modelFileName}");
                }

                IReadOnlyDictionary<Dtmi, DTEntityInfo> modelDictionary = modelParser.Parse(modelJsonList);

                foreach (Dtmi dtmiEntry in modelDictionary.Keys) {
                    DTEntityInfo entityInfo = modelDictionary[dtmiEntry];
                    DtdlModelContainer modelContainer = new DtdlModelContainer(modelDictionary, modelFilePath, entityInfo.ToString());
                    modelContainerList.Add(modelContainer);

                    switch (entityInfo.EntityKind) {
                        case DTEntityKind.Interface:
                            string dtmiUri = dtmiEntry.AbsoluteUri;

                            modelContainer.AddInterfaceRef(dtmiUri, (DTInterfaceInfo)entityInfo);

                            // ugh... seems there's no way to get the source JSON from the ModelParser
                            // via any public all - ideally via the DTMI URI
                            //
                            // may create a wrapper class for this purpose...

                            foreach (string jsonData in modelJsonList) {
                                if (jsonData.Contains(dtmiUri)) {
                                    modelContainer.SetModelJsonData(jsonData);
                                    break;
                                }
                            }

                            Console.WriteLine($" --> DTInterfaceInfo DTMI: {dtmiUri}.");
                            //Console.WriteLine($"   > JSON: {modelContainer.GetModelJsonData()}");
                            break;
                    }
                }

                Console.WriteLine($"Successfully parsed DTDL JSON for model entities.");
            }

            return modelContainerList;
        }

        /// <summary>
        /// 
        /// </summary>
        public class DtdlModelContainer
        {
            private IReadOnlyDictionary<Dtmi, DTEntityInfo> entityDictionary = null;

            private Dictionary<string, DTInterfaceInfo> entityInterfaceMap = null;

            private string modelJsonFile = null;

            private string modelJsonData = null;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="entityDictionary"></param>
            /// <param name="modelJsonFile"></param>
            /// <param name="modelJsonData"></param>
            public DtdlModelContainer(
                IReadOnlyDictionary<Dtmi, DTEntityInfo> entityDictionary,
                string modelJsonFile,
                string modelJsonData)
            {
                this.entityDictionary = entityDictionary;
                this.modelJsonFile = modelJsonFile;
                this.modelJsonData = modelJsonData;

                this.entityInterfaceMap = new Dictionary<string, DTInterfaceInfo>();
            }

            // public methods

            public void AddInterfaceRef(string dtmiUri, DTInterfaceInfo dtmiInterfaceRef)
            {
                this.entityInterfaceMap.Add(dtmiUri, dtmiInterfaceRef);
            }
            
            public HashSet<string> GetDtmiUriSet()
            {
                HashSet<string> dtmiUriSet = new HashSet<string>();

                foreach (string key in this.entityInterfaceMap.Keys) {
                    dtmiUriSet.Add(key);
                }

                return dtmiUriSet;
            }

            public DTInterfaceInfo GetDtdlInterface(string dtmiUri)
            {
                if (! string.IsNullOrEmpty(dtmiUri)) {
                    if (this.entityInterfaceMap.ContainsKey(dtmiUri)) {
                        return this.entityInterfaceMap[dtmiUri];
                    }
                }

                return null;
            }

            public Dictionary<string, DTInterfaceInfo> GetEntityInterfaceMap()
            {
                return this.entityInterfaceMap;
            }

            public IReadOnlyDictionary<Dtmi, DTEntityInfo> GetEntityTable()
            {
                return this.entityDictionary;
            }

            public string GetModelJsonFile()
            {
                return this.modelJsonFile;
            }

            public string GetModelJsonData()
            {
                return this.modelJsonData;
            }

            public void SetModelJsonData(string jsonData)
            {
                this.modelJsonData = jsonData;
            }

        }

    }

}
