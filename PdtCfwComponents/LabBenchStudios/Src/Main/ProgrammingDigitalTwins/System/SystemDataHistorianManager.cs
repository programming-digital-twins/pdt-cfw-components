using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Connection;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.System
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
    public class SystemDataHistorianManager : IDataHistorian
    {
        private bool initializeBackingFileStore = true;

        private int maxItemsPerType = ConfigConst.DEFAULT_MAX_CACHED_ITEMS;
        private long maxCacheSize = ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB;

        private long totalHeapMemory = 0L;

        private Dictionary<string, IDataHistorianPlayer> dataCachePlayerTable = null;

        private ISystemStatusEventListener eventListener = null;

        private IPersistenceConnector persistenceConnector = null;

        /// <summary>
        /// 
        /// </summary>
        public SystemDataHistorianManager() :
            this(null, null,
                 ConfigConst.DEFAULT_MAX_CACHED_ITEMS,
                 ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB)
        {
            // nothing to do - delegates to other constructor
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public SystemDataHistorianManager(ISystemStatusEventListener listener) :
            this(null, listener,
                 ConfigConst.DEFAULT_MAX_CACHED_ITEMS,
                 ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB)
        {
            // nothing to do - delegates to other constructor
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="listener"></param>
        public SystemDataHistorianManager(string filePath, ISystemStatusEventListener listener) :
            this(filePath, listener,
                 ConfigConst.DEFAULT_MAX_CACHED_ITEMS,
                 ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB)
        {
            // nothing to do - delegates to other constructor
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="listener"></param>
        /// <param name="maxItemsPerType"></param>
        /// <param name="maxCacheSize"></param>
        public SystemDataHistorianManager(
            string filePath, ISystemStatusEventListener listener,
            int maxItemsPerType, long maxCacheSize) : base()
        {
            this.SetEventListener(listener);

            if (maxItemsPerType > 0 && maxItemsPerType <= ConfigConst.DEFAULT_MAX_CACHED_ITEMS)
            {
                this.maxItemsPerType = maxItemsPerType;
            }

            if (maxCacheSize > 0L && maxCacheSize <= ConfigConst.DEFAULT_MAX_CACHE_SIZE_IN_MB)
            {
                this.maxCacheSize = maxCacheSize;
            }

            this.dataCachePlayerTable = new Dictionary<string, IDataHistorianPlayer>();

            this.totalHeapMemory = GC.GetTotalMemory(false);

            if (this.initializeBackingFileStore)
            {
                this.InitFileStorage();
            }
        }

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetTotalMemory()
        {
            return this.totalHeapMemory;
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
        /// <param name="loadIfStored"></param>
        /// <returns></returns>
        public IDataHistorianPlayer GetDataHistorianPlayer(string cacheName, bool loadIfStored)
        {
            if (!string.IsNullOrWhiteSpace(cacheName))
            {
                if (this.dataCachePlayerTable.ContainsKey(cacheName))
                {
                    Console.WriteLine($"Retrieving historian cache from internal table for cache name {cacheName}.");

                    return this.dataCachePlayerTable[cacheName];
                } else
                {
                    if (loadIfStored)
                    {
                        Console.WriteLine($"Attempting to load historian cache for cache name {cacheName}.");

                        // create a new (or load an existing) backing cache
                        IDataHistorianCache dataCache = this.LoadDataHistorianCache(cacheName);

                        // create the historian player with the new (or loaded) data cache
                        IDataHistorianPlayer dataCachePlayer = new DataHistorianPlayer(dataCache);

                        // set the listener for the player - this will allow notifications from the player
                        dataCachePlayer.SetEventListener(this.eventListener);

                        // add the player to the internal table
                        this.dataCachePlayerTable.Add(cacheName, dataCachePlayer);

                        // register the player for incoming events (from EventProcessor)
                        EventProcessor.GetInstance().RegisterListener(dataCachePlayer);

                        return dataCachePlayer;
                    }
                }
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
        /// <param name="cacheName"></param>
        public void ResetAndRemoveDataHistorian(string cacheName)
        {
            if (!string.IsNullOrWhiteSpace(cacheName))
            {
                if (this.dataCachePlayerTable.ContainsKey(cacheName))
                {
                    Console.WriteLine($"Retrieving historian cache from internal table for cache name {cacheName}.");

                    IDataHistorianPlayer dataCachePlayer = this.dataCachePlayerTable[cacheName];

                    dataCachePlayer.Reset();

                    EventProcessor.GetInstance().UnregisterListener(dataCachePlayer);

                    this.dataCachePlayerTable.Remove(cacheName);
                } else
                {
                    Console.WriteLine($"No data cache player with name {cacheName} registered. Ignoring.");
                }
            }
        }

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start timeStamp (null if narrowing is not needed).
		 * @param endDate The end timeStamp (null if narrowing is not needed).
		 * @return List<ActuatorData> The data instance(s) associated with the lookup parameters.
		 */
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

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start timeStamp (null if narrowing is not needed).
		 * @param endDate The end timeStamp (null if narrowing is not needed).
		 * @return List<ConnectionStateData> The data instance(s) associated with the lookup parameters.
		 */
        public List<ConnectionStateData> LoadConnectionStateData(ResourceNameContainer resource, DateTime startDate, DateTime endDate)
        {
            if (this.persistenceConnector != null)
            {
                Console.WriteLine($"Loading connection replayState data. Play: {startDate}. End: {endDate}.");
                return this.persistenceConnector.LoadConnectionStateData(resource, startDate, endDate);
            } else
            {
                Console.WriteLine($"No persistence connector. Can't load connection replayState data. Play: {startDate}. End: {endDate}.");
                return null;
            }
        }

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start timeStamp (null if narrowing is not needed).
		 * @param endDate The end timeStamp (null if narrowing is not needed).
		 * @return List<SensorData> The data instance(s) associated with the lookup parameters.
		 */
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

        /**
		 * Attempts to retrieve the named data instance from the persistence server.
		 * Will return null if there's no data matching the given type with the
		 * given parameters.
		 * 
		 * @param resource The resource container with load meta data / additional search criteria.
		 * @param startDate The start timeStamp (null if narrowing is not needed).
		 * @param endDate The end timeStamp (null if narrowing is not needed).
		 * @return List<SystemPerformanceData> The data instance(s) associated with the lookup parameters.
		 */
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
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="direction"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetReplayDirection(string cacheName, DataHistorianState.DataHistorianReplayDirection direction)
        {
            IDataHistorianPlayer cachePlayer = this.GetDataHistorianPlayer(cacheName);

            if (cachePlayer != null)
            {
                cachePlayer.SetReplayDirection(direction);
            }
        }


        // private methods

        /// <summary>
        /// 
        /// </summary>
        private void InitFileStorage()
        {
            this.persistenceConnector = new FilePersistenceConnector();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        private IDataHistorianCache LoadDataHistorianCache(string cacheName)
        {
            if (this.persistenceConnector != null)
            {
                List<DataCacheEntryContainer> dataCacheContent = this.persistenceConnector.LoadDataCache(cacheName);

                DataHistorianCache dataCache = new DataHistorianCache(cacheName);
                dataCache.AddCacheItems(dataCacheContent, true);

                if (dataCache != null)
                {
                    Console.WriteLine($"Successfully loaded data cache {cacheName} with {dataCache.GetCacheSize()} items.");

                    return dataCache;
                }
            } else
            {
                Console.WriteLine($"Warning - no persistence connector initialized. Can't load cache {cacheName}.");
            }

            return null;
        }

    }

}
