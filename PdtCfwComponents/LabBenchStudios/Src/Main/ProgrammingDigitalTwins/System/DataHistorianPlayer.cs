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

        private bool activatePlaybackThread = false;
        private float playbackDelayFactor = 0.0f;
        private int maxJoinMillis = 500;

        /// <summary>
        /// 
        /// </summary>
        public DataHistorianPlayer(IDataHistorianCache historianCache)
        {
            this.historianCache = historianCache;

            if (this.HasValidCache()) {
                this.playerName = this.historianCache.GetCacheName() + "_Player";
            }

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
        /// <returns></returns>
        public bool HasValidCache()
        {
            return (this.historianCache != null && this.historianCache.HasCachedEntries());
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
            if (this.HasValidCache())
            {
                this.historianCache.SetCacheState(DataHistorianState.DataHistorianReplayState.Play);
                this.replayState = this.historianCache.GetCacheReplayState();

                try
                {
                    if (this.playbackThread != null)
                    {
                        if (! this.playbackThread.IsAlive)
                        {
                            try
                            {
                                this.playbackThread.Start();
                            } catch (Exception e)
                            {
                                Console.WriteLine($"Player thread ");
                            }
                            this.playbackThread.Interrupt();
                        }
                    }

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
            if (this.HasValidCache())
            {
                this.historianCache.SetCacheState(DataHistorianState.DataHistorianReplayState.Pause);
                this.replayState = this.historianCache.GetCacheReplayState();

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
            this.Stop();

            if (this.HasValidCache())
            {
                this.historianCache.ResetCache();
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
            if (this.HasValidCache())
            {
                this.historianCache.SetCacheState(DataHistorianState.DataHistorianReplayState.Stop);
                this.replayState = this.historianCache.GetCacheReplayState();

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


        // private methods

        /// <summary>
        /// 
        /// </summary>
        private void ActivatePlayer()
        {
            while (this.activatePlaybackThread)
            {
                switch (this.replayState)
                {
                    case DataHistorianState.DataHistorianReplayState.Play:
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

                        break;

                    default:
                        // todo: determine if an artificial delay is warranted here
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializePlayer()
        {
            try
            {
                this.activatePlaybackThread = false;

                if (this.playbackThread != null && this.playbackThread.IsAlive)
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

            ThreadStart ts = new ThreadStart(this.ActivatePlayer);

            this.playbackThread = new Thread(ts);
            this.playbackThread.IsBackground = true;
            this.playbackThread.Name = this.playerName;

            this.activatePlaybackThread = true;
        }

    }

}
