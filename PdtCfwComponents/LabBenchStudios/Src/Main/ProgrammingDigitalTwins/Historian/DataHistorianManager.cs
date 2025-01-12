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

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Connection;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Plexus;
using LabBenchStudios.Pdt.Util;

namespace LabBenchStudios.Pdt.Historian
{
    /// <summary>
    /// This class is responsible for managing all locally accessible historical
    /// data, and uses an IPersistenceConnector implementation as the underlying
    /// store / load functionality.
    /// 
    /// As this class will manage storing, loading, and ultimately caching of
    /// data, there are some limits that need to be established. The following
    /// assumptions are based on simple simulation runs using the Edge Device
    /// App when generating no more than two messages for each poll cycle.
    /// 
    /// Anticipated storage needs for a single IotDataContext object, when converted
    /// to JSON, is ~500 bytes. A typical simulator will generate a single sensor
    /// payload (about 500 bytes) and a single system performance payload (also about
    /// 500 bytes) every poll cycle. Storage consumption can therefore be estimated
    /// as follows on a per simulator basis (with the given assumptions):
    ///   - poll cycle: 5 seconds
    ///   - sensor data: ~500 bytes
    ///   - perf data: ~500 bytes
    ///   - data / poll cycle: ~1000 bytes
    ///   - poll cycles / min: 12
    ///   - data / min: 12 * 1000 = 12,000 bytes
    ///   - data / hour: 60 * 12,000 bytes = 720,000 bytes
    ///   - data / day: 17,280,000 bytes
    /// 
    /// In summary, a single simulator operating at a 5 second poll rate for an entire
    /// day should generate (and required storage for) approx. 16.8 MB
    /// 
    /// Using this figure for a sample run of a single simulator yields the following:
    ///   - ~0.7 MB / hr per simulator
    ///   - ~16.8 MB / day per simulator
    ///   - ~117.6 MB / week per simulator
    ///   ~ ~510 MB / month per simulator
    ///   - ~6.12 GB / year per simulator
    ///   
    /// Using this figure for a sample run of 5 simulators yields the following:
    ///   - ~3.5 MB / hr for 5 simulators
    ///   - ~84.0 MB / day for 5 simulators
    ///   - ~588.0 MB / week for 5 simulators
    ///   ~ ~2.55 GB / month for 5 simulators
    ///   - ~30.6 GB / year for 5 simulators
    ///   
    /// While storage durations are configurable within the coded properties configuration,
    /// the default functionality for this class (and its underlying in memory cache) will
    /// center on the following rules:
    ///   - 1 data file (.dat) per device (simulator) per load request (1 day's worth of data)
    ///   - Max of 10 unique devices (simulators) in cache at any given time
    ///   - Estimated memory requirements for operation of the internal cache: ~168 MB
    /// </summary>
    public class DataHistorianManager : IDataHistorian
    {
        private string rootPathName = ConfigConst.DEFAULT_FILE_STORAGE_PATH;

        private bool initializeBackingFileStore = true;

        private int maxItemsPerType = ConfigConst.DEFAULT_MAX_CACHED_ITEMS;
        private long maxCacheSize = ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB;

        private long totalHeapMemory = 0L;

        private Dictionary<string, IDataHistorianPlayer> dataHistorianPlayerTable = null;

        private ISystemStatusEventListener eventListener = null;

        private IPersistenceConnector persistenceConnector = null;

        /// <summary>
        /// 
        /// </summary>
        public DataHistorianManager() : this(null)
        {
            // nothing to do - delegates to other constructor
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public DataHistorianManager(ISystemStatusEventListener listener)
        {
            this.SetEventListener(listener);

            this.dataHistorianPlayerTable = new Dictionary<string, IDataHistorianPlayer>();

            this.totalHeapMemory = GC.GetTotalMemory(false);

            this.InitPersistenceLayer();
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDataHistorianPlayer CreateDataHistorianPlayer()
        {
            Console.WriteLine("\n\n=====\nCreating data historian player...\n");

            // create the historian player with a new backing cache
            IDataHistorianPlayer historianPlayer = new DataHistorianPlayer();

            Console.WriteLine($"Created data historian player: {historianPlayer.GetCacheName()}");

            this.InitHistorianPlayer(historianPlayer);

            //IDataHistorianPlayer historianPlayer = this.GetDataHistorianPlayer(cacheName, false);

            Console.WriteLine($"Data historian player created. File URI: {historianPlayer.GetCacheStorageUri()}. Cache: {historianPlayer.GetCacheFileName()}");
            Console.WriteLine("\n=====\n\n");

            return historianPlayer;
        }

        /// <summary>
        /// This is an expensive operation.
        /// </summary>
        /// <returns></returns>
        public double GetTotalMemoryUsage()
        {
            double totalMemoryBytes = 0.0d;

            foreach (var playerEntry in this.dataHistorianPlayerTable)
            {
                IDataHistorianPlayer player = playerEntry.Value;
                totalMemoryBytes += player.GetCacheMemoryUsage();
            }

            return totalMemoryBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetMaxCacheSize()
        {
            return maxCacheSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public bool IsCacheReplaying(string cacheName)
        {
            // TODO: implement this

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public IDataHistorianPlayer GetDataHistorianPlayer(string cacheName)
        {
            return this.GetDataHistorianPlayer(cacheName, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="loadFromPersistence"></param>
        /// <returns></returns>
        public IDataHistorianPlayer GetDataHistorianPlayer(string cacheName, bool loadFromPersistence)
        {
            if (!string.IsNullOrWhiteSpace(cacheName))
            {
                IDataHistorianPlayer historianPlayer = null;

                if (this.dataHistorianPlayerTable.ContainsKey(cacheName))
                {
                    Console.WriteLine($"Retrieving historian cache from internal table: {cacheName}.");

                    historianPlayer = this.dataHistorianPlayerTable[cacheName];
                } else
                {
                    Console.WriteLine($"Historian cache not yet stored internally. Loading / creating: {cacheName}.");

                    if (loadFromPersistence)
                    {
                        Console.WriteLine($"Attempting to load historian cache from persistence layer: {cacheName}.");

                        // create a new (or load an existing) backing cache
                        IDataHistorianCache dataCache = this.LoadDataHistorianCache(cacheName);

                        if (dataCache == null)
                        {
                            Console.WriteLine($"DANGER: No historian cache! Name: {cacheName}");
                        }

                        // create the historian player with the new (or loaded) data cache
                        historianPlayer = new DataHistorianPlayer(dataCache);

                        this.InitHistorianPlayer(historianPlayer);
                    } else
                    {
                        Console.WriteLine($"Attempting to create historian cache: {cacheName}.");

                        // create the historian player with a new backing cache
                        historianPlayer = new DataHistorianPlayer(cacheName);

                        this.InitHistorianPlayer(historianPlayer);
                    }
                }

                return historianPlayer;
            } else
            {
                Console.WriteLine("No cache name specified to get data historian cache. Ignoring request.");
            }

            Console.WriteLine($"Data historian cache not loaded for cache {cacheName}.");

            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetLoadableCacheList()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetLoadedCacheNames()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCacheFilePath()
        {
            if (this.persistenceConnector != null)
            {
                return this.persistenceConnector.GetDataStoreUri();
            } else
            {
                return this.rootPathName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetRootFilePath()
        {
            return this.rootPathName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        public void ResetAndRemoveDataHistorian(string cacheName)
        {
            if (!string.IsNullOrWhiteSpace(cacheName))
            {
                if (this.dataHistorianPlayerTable.ContainsKey(cacheName))
                {
                    Console.WriteLine($"Retrieving historian cache from internal table for cache name {cacheName}.");

                    IDataHistorianPlayer dataHistorianPlayer = this.dataHistorianPlayerTable[cacheName];

                    dataHistorianPlayer.Reset();

                    EventProcessor.GetInstance().UnregisterListener((IDataContextEventListener) dataHistorianPlayer);
                    EventProcessor.GetInstance().UnregisterListener((IUserEventStateListener) dataHistorianPlayer);

                    this.dataHistorianPlayerTable.Remove(cacheName);
                } else
                {
                    Console.WriteLine($"No data cache player with name {cacheName} registered. Ignoring.");
                }
            }
        }


        /// <summary>
		/// Attempts to retrieve the named data instance from the persistence server.
        /// Will return null if there's no data matching the given type with the
		/// given parameters.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<ActuatorData> LoadActuatorData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            if (this.persistenceConnector != null)
            {
                Console.WriteLine($"Loading actuator data. Play: {startDate}. End: {endDate}.");
                return this.persistenceConnector.LoadActuatorData(resource, startDate, endDate);
            } else
            {
                Console.WriteLine($"No persistence connector. Can't load actuator data. Play: {startDate}. End: {endDate}.");
                return null;
            }
        }

        /// <summary>
        /// Attempts to retrieve the named data instance from the persistence server.
        /// Will return null if there's no data matching the given type with the
        /// given parameters.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<ConnectionStateData> LoadConnectionStateData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            if (this.persistenceConnector != null)
            {
                Console.WriteLine($"Loading connection playbackState data. Play: {startDate}. End: {endDate}.");
                return this.persistenceConnector.LoadConnectionStateData(resource, startDate, endDate);
            } else
            {
                Console.WriteLine($"No persistence connector. Can't load connection playbackState data. Play: {startDate}. End: {endDate}.");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public IDataHistorianPlayer LoadDataHistorianPlayer(string pathName, string fileName)
        {
            if (Directory.Exists(pathName))
            {
                if (File.Exists(fileName))
                {

                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to retrieve the named data instance from the persistence server.
        /// Will return null if there's no data matching the given type with the
        /// given parameters.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<SensorData> LoadSensorData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            if (this.persistenceConnector != null)
            {
                Console.WriteLine($"Loading sensor data. Play: {startDate}. End: {endDate}.");
                return this.persistenceConnector.LoadSensorData(resource, startDate, endDate);
            } else
            {
                Console.WriteLine($"No persistence connector. Can't load sensor data. Play: {startDate}. End: {endDate}.");
                return null;
            }
        }

        /// <summary>
        /// Attempts to retrieve the named data instance from the persistence server.
        /// Will return null if there's no data matching the given type with the
        /// given parameters.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<SystemPerformanceData> LoadSystemPerformanceData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            if (this.persistenceConnector != null)
            {
                Console.WriteLine($"Loading system performance data. Play: {startDate}. End: {endDate}.");
                return this.persistenceConnector.LoadSystemPerformanceData(resource, startDate, endDate);
            } else
            {
                Console.WriteLine($"No persistence connector. Can't load system performance data. Play: {startDate}. End: {endDate}.");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public void SetEventListener(ISystemStatusEventListener listener)
        {
            if (listener != null)
            {
                this.eventListener = listener;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootFilePath"></param>
        /// <returns></returns>
        public bool SetRootFilePath(string rootFilePath)
        {
            if (! string.IsNullOrEmpty(rootFilePath))
            {
                if (rootFilePath.Equals(".") || rootFilePath.Equals("..") || rootFilePath.Contains("..."))
                {
                    rootFilePath = ConfigConst.DEFAULT_FILE_STORAGE_PATH;
                    Console.WriteLine($"New file path is using invalid char's. Setting to default: {rootFilePath}");
                }
            }

            if (string.IsNullOrEmpty(rootFilePath) && this.persistenceConnector != null)
            {
                rootFilePath = ConfigConst.DEFAULT_FILE_STORAGE_PATH;
                Console.WriteLine($"New file path is null or empty. Setting to default: {rootFilePath}");
            }

            bool initPersistence = false;

            if (this.rootPathName.Equals(rootFilePath) && this.persistenceConnector == null) {
                initPersistence = true;
            }
            
            if (! this.rootPathName.Equals(rootFilePath))
            {
                initPersistence = true;
            }

            // validate file path
            try
            {
                if (!string.IsNullOrWhiteSpace(rootFilePath))
                {
                    if (Directory.Exists(rootFilePath))
                    {
                        this.rootPathName = rootFilePath;
                    } else
                    {
                        Console.WriteLine($"Path {rootFilePath} doesn't exist. Using default: {this.rootPathName}");
                    }
                } else
                {
                    Console.WriteLine($"No path specified. Using default: {this.rootPathName}");
                }
            } catch (Exception e)
            {
                Console.WriteLine($"Failed to validate path {rootFilePath}. Using default: {this.rootPathName}");
            }

            // check if we need to init (or re-init) persistence layer
            if (initPersistence)
            {
                this.InitPersistenceLayer();
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="persistenceConnector"></param>
        public void SetPersistenceConnector(IPersistenceConnector persistenceConnector)
        {
            if (persistenceConnector != null)
            {
                if (this.persistenceConnector != null)
                {
                    this.persistenceConnector.DisconnectClient();
                }

                this.persistenceConnector = persistenceConnector;
                this.persistenceConnector.ConnectClient();

                // todo: update all persistence conn's ref's withe historian player table
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="direction"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetReplayDirection(string cacheName, MediaPlayerState.PlaybackDirection direction)
        {
            IDataHistorianPlayer cachePlayer = this.GetDataHistorianPlayer(cacheName);

            if (cachePlayer != null)
            {
                cachePlayer.SetPlaybackDirection(direction);
            }
        }


        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="historianPlayer"></param>
        private void InitHistorianPlayer(IDataHistorianPlayer historianPlayer)
        {
            this.InitHistorianPlayer(historianPlayer, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="historianPlayer"></param>
        private void InitHistorianPlayer(IDataHistorianPlayer historianPlayer, bool registerForEvents)
        {
            // by default, enable cache filling
            historianPlayer.SetCacheFillingEnabledFlag(true);

            // set the listener for the player - this will allow notifications from the player
            historianPlayer.SetEventListener(this.eventListener);

            // add the player to the internal table
            if (! this.dataHistorianPlayerTable.ContainsKey(historianPlayer.GetCacheName())) {
                this.dataHistorianPlayerTable.Add(historianPlayer.GetCacheName(), historianPlayer);
            }

            // set the persistence layer
            Console.WriteLine($"Updating historian player with current persistence connector: {historianPlayer.GetCacheName()}");
            historianPlayer.SetDataLoader(this.persistenceConnector);
            historianPlayer.SetDataStorer(this.persistenceConnector);

            // update EventProcessor with any newly registered device ID's
            //EventProcessor.GetInstance().UpdateDeviceIDContent(historianPlayer.GetCacheEntries());

            if (registerForEvents)
            {
                // register the player for incoming events (from EventProcessor)
                EventProcessor.GetInstance().RegisterListener((IDataContextEventListener) historianPlayer);

                // register the player for incoming events (from EventProcessor)
                EventProcessor.GetInstance().RegisterListener((IUserEventStateListener) historianPlayer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void InitPersistenceLayer()
        {
            this.SetPersistenceConnector(new FilePersistenceConnector(this.rootPathName, FileUtil.PersistenceDataTypeEnum.Historian));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        private IDataHistorianCache LoadDataHistorianCache(string cacheName)
        {
            if (this.persistenceConnector == null)
            {
                this.InitPersistenceLayer();
            }

            if (this.persistenceConnector != null)
            {
                // todo: need to redesign the cache load process - it prob should be handled
                // exclusively through the player - so a load request for the cache name
                // should trigger the creation of a new player, which then loads the cache
                DataHistorianCache dataCache = new DataHistorianCache(cacheName);
                dataCache.SetDataLoader(this.persistenceConnector);
                dataCache.SetDataStorer(this.persistenceConnector);

                if (dataCache.LoadDataCache())
                {
                    Console.WriteLine($"Successfully loaded data cache {cacheName} with {dataCache.GetCacheSize()} items.");
                } else
                {
                    Console.WriteLine($"Successully created data cache {cacheName}. Cache is currently empty.");
                }

                return dataCache;
            } else
            {
                Console.WriteLine($"Warning - no persistence connector initialized. Can't load cache {cacheName}.");
            }

            return null;
        }

    }

}
