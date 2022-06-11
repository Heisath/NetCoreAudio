// -----------------------------------------
// SoundScribe (TM) and related software.
// 
// Copyright (C) 2007-2011 Vannatech
// http://www.vannatech.com
// All rights reserved.
// 
// This source code is subject to the MIT License.
// http://www.opensource.org/licenses/mit-license.php
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------

using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Structures
{
    /// <summary>
    /// Describes a change in the volume level or muting state of an audio endpoint device.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370799.aspx
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct AUDIO_VOLUME_NOTIFICATION_DATA
    {
        /// <summary>
        /// The user event context supplied during the change request.
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public Guid EventContext;

        /// <summary>
        /// Specifies whether the audio stream is currently muted.
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool IsMuted;

        /// <summary>
        /// The current master volume level of the audio stream.
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float MasterVolume;

        /// <summary>
        /// Specifies the number of channels in the audio stream.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 ChannelCount;

        /// <summary>
        /// The volume level of each channel in the stream.
        /// </summary>
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4)]
        public float[] ChannelVolumes;

        /// <summary>
        /// Parses a IntPtr pointing to a AUDIO_VOLUME_NOTIFICATION_DATA structure to an instance
        /// </summary>
        /// <returns></returns>
        public static AUDIO_VOLUME_NOTIFICATION_DATA FromIntPtr(IntPtr data)
        {
            AUDIO_VOLUME_NOTIFICATION_DATA result = new AUDIO_VOLUME_NOTIFICATION_DATA();
            if (data == IntPtr.Zero) return result;

            result.EventContext = Marshal.PtrToStructure<Guid>(data);
            data += Marshal.SizeOf(typeof(Guid));

            result.IsMuted = Marshal.PtrToStructure<bool>(data);
            data += Marshal.SizeOf(typeof(bool));

            result.MasterVolume = Marshal.PtrToStructure<float>(data);
            data += Marshal.SizeOf(typeof(float));

            result.ChannelCount = Marshal.PtrToStructure<UInt32>(data);
            data += Marshal.SizeOf(typeof(UInt32));

            result.ChannelVolumes = new float[result.ChannelCount];
            Marshal.Copy(data, result.ChannelVolumes, 0, (int)result.ChannelCount);

            return result;

        }
    }
}
