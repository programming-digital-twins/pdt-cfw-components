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
using System.Diagnostics;
using System.Threading;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Util;

namespace LabBenchStudios.Pdt.Historian
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

        private MediaPlayerState.PlaybackState playbackState =
            MediaPlayerState.PlaybackState.Uninitialized;

        private IDataHistorianCache historianCache = null;

        private Thread playbackThread = null;

        private string playerName = ConfigConst.NOT_SET;
        private string displayName = ConfigConst.NOT_SET;

        private bool enableThreadedPlayback = false;
        private bool cacheFillingEnabled = false;
        private bool cacheOnlyDataEventsEnabled = false;

        private bool playbackThreadInitialized = false;
        private bool playbackEnabled = false;
        private bool playbackModeActive = false;
        private bool enablePlaybackLoopOnStart = false;
        private bool enableLoopAtEnd = false;
        private bool needsCacheLoad = true;
        private float playbackDelayFactor = 0.0f;
        private int maxJoinMillis = 500;

        /// <summary>
        /// 
        /// </summary>
        public DataHistorianPlayer()
        {
            this.historianCache = new DataHistorianCache();

            this.InitPlayerProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        public DataHistorianPlayer(string cacheName)
        {
            this.historianCache = new DataHistorianCache();
            this.historianCache.SetCacheName(cacheName);

            this.InitPlayerProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        public DataHistorianPlayer(IDataHistorianCache historianCache)
        {
            if (historianCache != null)
            {
                this.historianCache = historianCache;
            } else
            {
                this.historianCache = new DataHistorianCache();
            }

            this.InitPlayerProperties();

            //this.Reset();
        }


        // public methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<DataCacheEntryContainer> GetCacheEntries()
        {
            return this.historianCache.GetCacheEntries();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataCacheEntryContainer GetCacheEntryAtIndex(int index)
        {
            return this.historianCache.GetCacheEntryAtIndex(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MediaPlayerState.PlaybackState GetPlaybackState()
        {
            return this.playbackState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCacheEntryContainer GetLastProcessedEventData()
        {
            return this.historianCache.GetCurrentEntry();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MediaPlayerState.PlaybackDirection GetPlaybackDirection()
        {
            if (this.HasValidCache())
            {
                return this.historianCache.GetPlaybackDirection();
            } else
            {
                Console.WriteLine($"Data historian player {this.playerName} has no valid backing cache. Ignoring.");
            }

            return MediaPlayerState.PlaybackDirection.Uninitialized;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCacheFileName()
        {
            return this.historianCache.GetStorageFileName();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCacheStorageUri()
        {
            return this.historianCache.GetStorageResourceUri();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCacheSize()
        {
            if (this.HasValidCache())
            {
                return this.historianCache.GetCacheSize();
            } else
            {
                Console.WriteLine($"Data historian player {this.playerName} has no valid backing cache. Ignoring.");
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetCacheMemoryUsage()
        {
            if (this.HasValidCache())
            {
                return this.historianCache.GetCacheMemoryUsage();
            } else
            {
                Console.WriteLine($"Data historian player {this.playerName} has no valid backing cache. Ignoring.");
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCacheName()
        {
            return this.historianCache.GetCacheName();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDisplayName()
        {
            return this.displayName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetLastProcessedEventIndex()
        {
            return this.historianCache.GetCurrentIndex();
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
        public MediaPlayerState.PlaybackState GetReplayState()
        {
            return this.playbackState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleActuatorData(ActuatorData data)
        {
            if (!this.playbackModeActive)
            {
                if (data != null && this.cacheFillingEnabled)
                {
                    DataCacheEntryContainer cacheEntry = new DataCacheEntryContainer();
                    cacheEntry.SetActuatorData(data);

                    this.historianCache.AddCacheItem(cacheEntry);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleConnectionStateData(ConnectionStateData data)
        {
            if (!this.playbackModeActive)
            {
                if (!this.cacheOnlyDataEventsEnabled)
                {
                    if (data != null && this.cacheFillingEnabled)
                    {
                        DataCacheEntryContainer cacheEntry = new DataCacheEntryContainer();
                        cacheEntry.SetConnectionStateData(data);

                        this.historianCache.AddCacheItem(cacheEntry);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleMessageData(MessageData data)
        {
            if (!this.playbackModeActive)
            {
                if (!this.cacheOnlyDataEventsEnabled)
                {
                    if (data != null && this.cacheFillingEnabled)
                    {
                        DataCacheEntryContainer cacheEntry = new DataCacheEntryContainer();
                        cacheEntry.SetMessageData(data);

                        this.historianCache.AddCacheItem(cacheEntry);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSensorData(SensorData data)
        {
            if (!this.playbackModeActive)
            {
                if (data != null && this.cacheFillingEnabled)
                {
                    DataCacheEntryContainer cacheEntry = new DataCacheEntryContainer();
                    cacheEntry.SetSensorData(data);

                    this.historianCache.AddCacheItem(cacheEntry);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSystemPerformanceData(SystemPerformanceData data)
        {
            if (!this.playbackModeActive)
            {
                if (data != null && this.cacheFillingEnabled)
                {
                    DataCacheEntryContainer cacheEntry = new DataCacheEntryContainer();
                    cacheEntry.SetSystemPerformanceData(data);

                    this.historianCache.AddCacheItem(cacheEntry);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        public void HandleUserEventState(UserEventState.EventType eventType)
        {
            if (!this.playbackModeActive)
            {
                // nothing to do for now
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
                return (this.playbackState == MediaPlayerState.PlaybackState.Play);
            } else
            {
                Console.WriteLine($"Data historian player {this.playerName} has no valid backing cache. Ignoring.");
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            this.needsCacheLoad = true;

            return this.historianCache.ClearCache();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Play()
        {
            // store the cache - in case updates were made before this call
            if (this.needsCacheLoad)
            {
                this.LoadHistorianCache();
            } else
            {
                this.StoreHistorianCache();
            }

            // handle the action
            if (this.HasValidCache() && this.playbackEnabled)
            {
                this.playbackModeActive = true;
                this.historianCache.SetCacheState(MediaPlayerState.PlaybackState.Play);
                this.playbackState = this.historianCache.GetPlaybackState();

                if (this.enableThreadedPlayback)
                {
                    try
                    {
                        if (!this.playbackThreadInitialized)
                        {
                            this.CreatePlaybackThread();
                        }

                        if (this.playbackThread != null)
                        {
                            // determine if thread is just initialized, or running but in a sleep state
                            if (!this.playbackThread.IsAlive)
                            {
                                this.playbackThread.Start();

                                Console.WriteLine($"Play: Initialized thread started: {this.playerName}.");
                            } else
                            {
                                Console.WriteLine($"Play: Initialized thread already started. Ignoring: {this.playerName}");
                                //this.playbackThread.Interrupt();
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
                }
            } else
            {
                Console.WriteLine($"Data historian player {this.playerName} has no valid backing cache. Ignoring.");
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
                this.historianCache.SetCacheState(MediaPlayerState.PlaybackState.Pause);
                this.playbackState = this.historianCache.GetPlaybackState();

                try
                {
                    if (this.playbackThread != null)
                    {
                        // nothing to do
                    }
                } catch (ThreadInterruptedException t)
                {
                }

                return true;
            } else
            {
                Console.WriteLine($"Data historian player {this.playerName} has no valid backing cache. Ignoring.");
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
                    this.Clear();
                }

                this.playbackState = this.historianCache.GetPlaybackState();
            } else
            {
                Console.WriteLine($"Data historian player {this.playerName} has no valid backing cache. Ignoring.");
            }

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
                this.historianCache.SetCacheState(MediaPlayerState.PlaybackState.Stop);
                this.playbackState = this.historianCache.GetPlaybackState();

                if (this.enableThreadedPlayback)
                {
                    try
                    {
                        if (this.playbackThread != null)
                        {
                            this.playbackThread.Interrupt();
                        }

                        if (this.playbackThreadInitialized)
                        {
                            this.DestroyPlaybackThread();
                        }
                    } catch (ThreadInterruptedException t)
                    {
                    }
                }

                this.playbackModeActive = false;

                return true;
            } else
            {
                Console.WriteLine($"Data historian player {this.playerName} has no valid backing cache. Ignoring.");
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
        /// If true, only data events (e.g., Actuator, Sensor, Performance).
        /// Else, cache all events (incl. Connection and Message).
        /// </summary>
        /// <param name="enabled"></param>
        public void SetCacheOnlyDataEventsFlag(bool enabled)
        {
            this.cacheOnlyDataEventsEnabled = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLoader"></param>
        public void SetDataLoader(IDataLoader dataLoader)
        {
            if (dataLoader != null)
            {
                this.historianCache.SetDataLoader(dataLoader);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataStorer"></param>
        public void SetDataStorer(IDataStorer dataStorer)
        {
            if (dataStorer != null)
            {
                this.historianCache.SetDataStorer(dataStorer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayName"></param>
        public void SetDisplayName(string displayName)
        {
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                this.displayName = displayName;
            } else
            {
                Console.WriteLine($"Invalid display name for {this.playerName}. Ignoring set request.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        public void SetEnableLoopAtEndFlag(bool enabled)
        {
            this.enableLoopAtEnd = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        public void SetEnableThreadedPlaybackFlag(bool enabled)
        {
            this.enableThreadedPlayback = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public void SetPlaybackEnabledFlag(bool enabled)
        {
            this.playbackEnabled = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delayFactor"></param>
        public void SetPlaybackDelayFactor(float delayFactor)
        {
            if (delayFactor > 0.0f)
            {
                this.playbackDelayFactor = delayFactor;
            }
            //this.playbackDelayFactor = (float) NumberUtil.CalculateDelayFactor(delayFactor);
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
        public void SetPlaybackDirection(MediaPlayerState.PlaybackDirection direction)
        {
            if (this.HasValidCache())
            {
                this.historianCache.SetCacheAccessDirection(direction);
            } else
            {
                Console.WriteLine($"Data historian player {this.playerName} has no valid backing cache. Ignoring.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetStartingIndex(int index)
        {
            return this.historianCache.SetStartingIndex(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool LoadHistorianCache()
        {
            if (this.HasValidCache())
            {
                Console.WriteLine($"Loading cache for player: {this.playerName}. Name: {this.historianCache.GetCacheName()}");
                
                bool success = this.historianCache.LoadDataCache();

                if (success)
                {
                    this.needsCacheLoad = false;
                } else
                {
                    Console.WriteLine($"Failed to load cache for player: {this.playerName}.");
                }

                return success;
            } else
            {
                Console.WriteLine($"No valid cache to load for player: {this.playerName}.");
            }

            return false;
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

                bool success = this.historianCache.StoreDataCache();

                return success;
            } else
            {
                Console.WriteLine($"No valid cache to store for player: {this.playerName}.");
            }

            return false;
        }


        /// <summary>
        /// This will return immediately with the milliseconds to wait before the
        /// next time this method should be invoked. It's up to the caller to honor
        /// the trigger rate.
        /// </summary>
        /// <returns>The milliseconds to wait until the next call to this method.</returns>
        public double TriggerNextCachePlaybackEvent()
        {
            return this.HandleNextPlaybackCacheEvent(false);
        }

        // private methods

        /// <summary>
        /// 
        /// </summary>
        private void RunPlayer()
        {
            while (this.enablePlaybackLoopOnStart)
            {
                try
                {
                    switch (this.playbackState)
                    {
                        case MediaPlayerState.PlaybackState.Play:
                            this.HandleNextPlaybackCacheEvent(true);

                            break;

                        default:
                            try
                            {
                                // sleep about 1 second if state is not playing
                                Thread.Sleep(1000);
                            } catch (Exception e)
                            {
                                Console.WriteLine($"Playback thread interrupted. Cache Name: {this.GetCacheName()}");
                            }

                            break;
                    }

                } catch (Exception e)
                {
                    Console.WriteLine($"Handled exception. Could be a planned thread interruption to change state: {this.playerName}. Exception: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Returns milliseconds before next event should be triggered.
        /// </summary>
        /// <param name="blockCall"></param>
        /// <returns></returns>
        private double HandleNextPlaybackCacheEvent(bool blockCall)
        {
            double delayMillis = 0.0d;

            if (this.HasValidCache())
            {
                // get most recent cache entry and the next cache entry
                DataCacheEntryContainer curCacheEntry = this.historianCache.GetCurrentEntry();
                DataCacheEntryContainer nextCacheEntry = this.historianCache.GetNextEntry();

                // if current cache entry is set, calculate the time delay before the
                // next cached entry should be processed; if it's null (not set), then
                // process the next cached entry immediately
                if (curCacheEntry != null && nextCacheEntry != null)
                {
                    // calculate the time delay before processing the next cache entry
                    // and include any playback delay factor (if > 0)
                    delayMillis = (long) nextCacheEntry.GetElapsedEpochMillisDelta(curCacheEntry);
                    delayMillis *= this.playbackDelayFactor;

                    if (blockCall)
                    {
                        // granular pause for delayMillis
                        Stopwatch stopwatch = Stopwatch.StartNew();

                        while (true)
                        {
                            if (stopwatch.ElapsedMilliseconds >= delayMillis)
                            {
                                stopwatch.Stop();
                                break;
                            }
                        }

                        stopwatch = null;
                    }
                }

                if (nextCacheEntry == null)
                {
                    if (this.enableLoopAtEnd)
                    {
                        this.historianCache.ResetCache();
                    } else
                    {
                        this.Stop();
                    }
                } else
                {
                    this.HandlePlaybackCacheListenerNotification(nextCacheEntry);
                }
            }

            return delayMillis;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheEntry"></param>
        private void HandlePlaybackCacheListenerNotification(DataCacheEntryContainer cacheEntry)
        {
            // process the cache entry - send any stored data items to the event listener
            if (this.eventListener != null)
            {
                if (cacheEntry.HasActuatorData())
                {
                    this.eventListener?.OnMessagingSystemDataReceived(cacheEntry.GetActuatorData());
                } else if (cacheEntry.HasConnectionStateData())
                {
                    this.eventListener?.OnMessagingSystemDataReceived(cacheEntry.GetConnectionStateData());
                } else if (cacheEntry.HasSensorData())
                {
                    this.eventListener?.OnMessagingSystemDataReceived(cacheEntry.GetSensorData());
                } else if (cacheEntry.HasSystemPerformanceData())
                {
                    this.eventListener?.OnMessagingSystemDataReceived(cacheEntry.GetSystemPerformanceData());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitPlayerProperties()
        {
            this.playerName = this.historianCache.GetCacheName();
            this.displayName = this.playerName;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreatePlaybackThread()
        {
            this.DestroyPlaybackThread();

            if (this.playbackThread == null)
            {
                Console.WriteLine($"Creating playback thread: {this.playerName}.");

                ThreadStart ts = new ThreadStart(this.RunPlayer);

                this.playbackThread = new Thread(ts);
                this.playbackThread.IsBackground = true;
                this.playbackThread.Name = this.playerName;
                this.enablePlaybackLoopOnStart = true;
                this.playbackThreadInitialized = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DestroyPlaybackThread()
        {
            if (this.playbackThread != null)
            {
                Console.WriteLine($"Destroying playback thread: {this.playerName}");

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
                    this.playbackThreadInitialized = false;
                }
            }
        }

    }

}
