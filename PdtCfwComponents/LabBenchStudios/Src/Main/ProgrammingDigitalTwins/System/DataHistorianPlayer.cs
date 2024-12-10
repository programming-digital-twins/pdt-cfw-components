using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Connection;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.System
{
    /// <summary>
    /// This class is responsible for managing the playback of a
    /// data historian cache.
    /// </summary>
    public class DataHistorianPlayer : IDataHistorianPlayer
    {
        // static


        // private

        private ISystemStatusEventListener eventListener;

        private DataHistorianState.DataHistorianReplayState replayState =
            DataHistorianState.DataHistorianReplayState.Uninitialized;

        private IDataHistorianCache historianCache = null;

        private Thread playbackThread = null;

        private string playerName = ConfigConst.NOT_SET;

        private bool cacheFillingEnabled = false;
        private bool playbackEnabled = false;
        private bool enablePlaybackLoopOnStart = false;
        private float playbackDelayFactor = 0.0f;
        private int maxJoinMillis = 500;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        public DataHistorianPlayer(string cacheName)
        {
            this.InitHistorianCache(new DataHistorianCache(cacheName));

            this.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        public DataHistorianPlayer(IDataHistorianCache historianCache)
        {
            this.InitHistorianCache(historianCache);

            this.Reset();
        }
        

        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataHistorianState.DataHistorianReplayState GetCacheReplayState()
        {
            return this.replayState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataHistorianState.DataHistorianReplayDirection GetCacheReplayDirection()
        {
            if (this.HasValidCache())
            {
                return this.historianCache.GetCacheReplayDirection();
            } else
            {
                Console.WriteLine("Data historian player instance has no valid backing cache. Ignoring.");
            }

            return DataHistorianState.DataHistorianReplayDirection.Uninitialized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetCacheSize()
        {
            if (this.HasValidCache())
            {
                return this.historianCache.GetCacheSize();
            } else
            {
                Console.WriteLine("Data historian player instance has no valid backing cache. Ignoring.");
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetCacheName()
        {
            if (this.HasValidCache())
            {
                return this.historianCache.GetCacheName();
            } else
            {
                Console.WriteLine("Data historian player instance has no valid backing cache. Ignoring.");
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetPlaybackDelayFactor()
        {
            return this.playbackDelayFactor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataHistorianState.DataHistorianReplayDirection GetReplayDirection()
        {
            if (this.HasValidCache())
            {
                return this.historianCache.GetCacheReplayDirection();
            } else
            {
                Console.WriteLine("Data historian player instance has no valid backing cache. Ignoring.");
            }

            return DataHistorianState.DataHistorianReplayDirection.Uninitialized;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataHistorianState.DataHistorianReplayState GetReplayState()
        {
            return this.replayState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleActuatorData(ActuatorData data)
        {
            if (data != null && this.cacheFillingEnabled)
            {
                DataCacheEntryContainer cacheEntry = new DataCacheEntryContainer();
                cacheEntry.SetActuatorData(data);

                if (! this.HasValidCache())
                {
                    this.InitHistorianCache(null);
                }

                this.historianCache.AddCacheItem(cacheEntry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleConnectionStateData(ConnectionStateData data)
        {
            if (data != null && this.cacheFillingEnabled)
            {
                DataCacheEntryContainer cacheEntry = new DataCacheEntryContainer();
                cacheEntry.SetConnectionStateData(data);

                if (!this.HasValidCache())
                {
                    this.InitHistorianCache(null);
                }

                this.historianCache.AddCacheItem(cacheEntry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleMessageData(MessageData data)
        {
            if (data != null && this.cacheFillingEnabled)
            {
                DataCacheEntryContainer cacheEntry = new DataCacheEntryContainer();
                cacheEntry.SetMessageData(data);

                if (!this.HasValidCache())
                {
                    this.InitHistorianCache(null);
                }

                this.historianCache.AddCacheItem(cacheEntry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSensorData(SensorData data)
        {
            if (data != null && this.cacheFillingEnabled)
            {
                DataCacheEntryContainer cacheEntry = new DataCacheEntryContainer();
                cacheEntry.SetSensorData(data);

                if (!this.HasValidCache())
                {
                    this.InitHistorianCache(null);
                }

                this.historianCache.AddCacheItem(cacheEntry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSystemPerformanceData(SystemPerformanceData data)
        {
            if (data != null && this.cacheFillingEnabled)
            {
                DataCacheEntryContainer cacheEntry = new DataCacheEntryContainer();
                cacheEntry.SetSystemPerformanceData(data);

                if (!this.HasValidCache())
                {
                    this.InitHistorianCache(null);
                }

                this.historianCache.AddCacheItem(cacheEntry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasValidCache()
        {
            return (this.historianCache != null && this.historianCache.HasCachedEntries());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsCacheFillingEnabled()
        {
            return this.cacheFillingEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsPlaybackEnabled()
        {
            return this.playbackEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsPlaying()
        {
            if (this.HasValidCache())
            {
                return (this.replayState == DataHistorianState.DataHistorianReplayState.Play);
            } else
            {
                Console.WriteLine("Data historian player instance has no valid backing cache. Ignoring.");
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Play()
        {
            // store the cache - in case updates were made before this call
            this.StoreHistorianCache();

            // handle the action
            if (this.HasValidCache() && this.playbackEnabled)
            {
                this.historianCache.SetCacheState(DataHistorianState.DataHistorianReplayState.Play);
                this.replayState = this.historianCache.GetCacheReplayState();

                try
                {
                    if (this.playbackThread != null)
                    {
                        // determine if thread is just initialized, or running but in a sleep state
                        if (!this.playbackThread.IsAlive)
                        {
                            this.playbackThread.Start();

                            Console.WriteLine($"Play: Initialized thread started: {this.playerName}.");
                        } else
                        {
                            this.playbackThread.Interrupt();
                        }
                    }

                    return true;
                } catch (ThreadInterruptedException t)
                {
                    Console.WriteLine($"Play: Paused thread re-started: {this.playerName}.");

                    return true;
                } catch (Exception e)
                {
                    Console.WriteLine($"Failed to start thread: {this.playerName}. Exception: {e.Message}");
                }
            } else
            {
                Console.WriteLine("Data historian player instance has no valid backing cache. Ignoring.");
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Pause()
        {
            // store the cache - in case updates were made before this call
            this.StoreHistorianCache();

            // handle the action
            if (this.HasValidCache() && this.playbackEnabled)
            {
                // just need to set the replay state to Pause - the thread's runner will then
                // stop processing messages and will not send any to the event listener
                this.historianCache.SetCacheState(DataHistorianState.DataHistorianReplayState.Pause);
                this.replayState = this.historianCache.GetCacheReplayState();

                try
                {
                    if (this.playbackThread != null)
                    {
                        this.playbackThread.Interrupt();
                    }
                } catch (ThreadInterruptedException t)
                {
                }

                return true;
            } else
            {
                Console.WriteLine("Data historian player instance has no valid backing cache. Ignoring.");
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            return this.Reset(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public bool Reset(bool clearCache)
        {
            this.Stop();

            this.SetCacheFillingEnabledFlag(false);
            this.SetPlaybackEnabledFlag(false);

            if (this.HasValidCache())
            {
                this.historianCache.ResetCache();

                if (clearCache)
                {
                    this.historianCache.ClearCache();
                }

                this.replayState = this.historianCache.GetCacheReplayState();
            } else
            {
                Console.WriteLine("Data historian player instance has no valid backing cache. Ignoring.");
            }

            this.InitializePlayer();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            // store the cache - in case updates were made before this call
            this.StoreHistorianCache();

            // handle the action
            if (this.HasValidCache() && this.playbackEnabled)
            {
                this.historianCache.SetCacheState(DataHistorianState.DataHistorianReplayState.Stop);
                this.replayState = this.historianCache.GetCacheReplayState();

                try
                {
                    if (this.playbackThread != null)
                    {
                        this.playbackThread.Interrupt();
                    }
                } catch (ThreadInterruptedException t)
                {
                }

                return true;
            } else
            {
                Console.WriteLine("Data historian player instance has no valid backing cache. Ignoring.");
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        public void SetCacheFillingEnabledFlag(bool enabled)
        {
            this.cacheFillingEnabled = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public void SetPlaybackEnabledFlag(bool enabled)
        {
            this.playbackEnabled = enabled;

            if (! this.playbackEnabled)
            {
                this.Reset();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delayFactor"></param>
        public void SetPlaybackDelayFactor(float delayFactor)
        {
            if (delayFactor >= 0.0f)
            {
                this.playbackDelayFactor = delayFactor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public void SetEventListener(ISystemStatusEventListener listener)
        {
            this.eventListener = listener;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        public void SetReplayDirection(DataHistorianState.DataHistorianReplayDirection direction)
        {
            if (this.HasValidCache())
            {
                this.historianCache.SetCacheAccessDirection(direction);
            } else
            {
                Console.WriteLine("Data historian player instance has no valid backing cache. Ignoring.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool StoreHistorianCache()
        {
            if (this.HasValidCache())
            {
                Console.WriteLine($"Storing cache for player: {this.playerName}. Items in cache: {this.historianCache.GetCacheSize()}");

                return this.historianCache.StoreDataCache();
            } else
            {
                Console.WriteLine($"No valid cache to store for player: {this.playerName}.");
            }

            return false;
        }


        // private methods

        /// <summary>
        /// 
        /// </summary>
        private void ActivatePlayer()
        {
            while (this.enablePlaybackLoopOnStart)
            {
                try
                {
                    switch (this.replayState)
                    {
                        case DataHistorianState.DataHistorianReplayState.Play:
                            this.HandlePlayState();

                            break;

                        default:
                            // todo: determine if an artificial delay is warranted here
                            break;
                    }
                } catch (Exception e)
                {
                    Console.WriteLine($"Handled exception. Could be a planned thread interruption to change state: {this.playerName}. Exception: {e.Message}");
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void HandlePlayState()
        {
            if (this.HasValidCache())
            {
                // get most recent cache entry and the next cache entry
                DataCacheEntryContainer curCacheEntry = this.historianCache.GetCurrentEntry();
                DataCacheEntryContainer nextCacheEntry = this.historianCache.GetNextEntry();

                // calculate the time delay before processing the next cache entry
                // and include any playback delay factor (if > 0)
                double delayMillis = (long) nextCacheEntry.GetElapsedEpochMillisDelta(curCacheEntry);

                if (this.playbackDelayFactor > 0)
                {
                    delayMillis = delayMillis * this.playbackDelayFactor;
                }

                // granular pause for delayMillis
                Stopwatch stopwatch = Stopwatch.StartNew();

                while (true)
                {
                    if (stopwatch.ElapsedMilliseconds >= delayMillis)
                    {
                        break;
                    }
                }

                // process the cache entry - send any stored data items to the event listener
                if (this.eventListener != null)
                {
                    if (nextCacheEntry.HasActuatorData())
                    {
                        this.eventListener.OnMessagingSystemDataReceived(nextCacheEntry.GetActuatorData());
                    } else if (nextCacheEntry.HasConnectionStateData())
                    {
                        this.eventListener.OnMessagingSystemDataReceived(nextCacheEntry.GetConnectionStateData());
                    } else if (nextCacheEntry.HasSensorData())
                    {
                        this.eventListener.OnMessagingSystemDataReceived(nextCacheEntry.GetSensorData());
                    } else if (nextCacheEntry.HasSystemPerformanceData())
                    {
                        this.eventListener.OnMessagingSystemDataReceived(nextCacheEntry.GetSystemPerformanceData());
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializePlayer()
        {
            if (this.playbackThread != null)
            {
                try
                {
                    this.enablePlaybackLoopOnStart = false;

                    if (this.playbackThread.IsAlive)
                    {
                        this.playbackThread.Join(this.maxJoinMillis);
                    }
                } catch (Exception e)
                {
                    Console.WriteLine($"Exception during playback thread join: {this.playerName}. Delay ms: {this.maxJoinMillis}. Exception: {e.Message}");
                } finally
                {
                    this.playbackThread = null;
                }
            }

            if (this.playbackThread == null)
            {
                Console.WriteLine($"Creating playback thread: {this.playerName}.");

                ThreadStart ts = new ThreadStart(this.ActivatePlayer);

                this.playbackThread = new Thread(ts);
                this.playbackThread.IsBackground = true;
                this.playbackThread.Name = this.playerName;
            }

            this.enablePlaybackLoopOnStart = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="historianCache"></param>
        private void InitHistorianCache(IDataHistorianCache historianCache)
        {
            if (historianCache == null)
            {
                historianCache = new DataHistorianCache();
            }

            this.historianCache = historianCache;
            this.playerName = this.historianCache.GetCacheName() + "_Player";
        }

    }

}
