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

namespace LabBenchStudios.Pdt.Common
{
    public interface IMediaPlayer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetName();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDisplayName();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetPlaybackDelayFactor();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MediaPlayerState.PlaybackDirection GetPlaybackDirection();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MediaPlayerState.PlaybackState GetPlaybackState();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasValidContent();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsPlaybackEnabled();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsPlaying();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Clear();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Play();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Pause();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Reset();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public bool Reset(bool clearCache);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Stop();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayName"></param>
        public void SetDisplayName(string displayName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public void SetPlaybackEnabledFlag(bool enabled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        public void SetEnableThreadedPlaybackFlag(bool enabled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delayFactor"></param>
        public void SetPlaybackDelayFactor(float delayFactor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public void SetEventListener(ISystemStatusEventListener listener);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        public void SetReplayDirection(MediaPlayerState.PlaybackDirection direction);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double TriggerNextPlaybackEvent();

    }
}
