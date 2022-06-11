using CoreAudio.Enumerations;
using CoreAudio.Externals;
using CoreAudio.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreAudio
{
    public class AudioEndpointVolume : AudioBase, IAudioEndpointVolumeCallback, INotifyPropertyChanged
    {
        //#################################################################################################################################
        #region Properties
        public AudioEndpoint Endpoint { get; private set; } 
        internal IAudioEndpointVolume EndpointVolume { get; private set; }
        internal Guid Guid { get; private set; }

        /// <summary>
        /// Gets or sets the master volume of this endpoint, values should be between 0 and 100. Return value of -1 indicates error.
        /// </summary>
        public float Level {
            get
            {
                EndpointVolume.GetMasterVolumeLevelScalar(out float volumeLevel);
                return volumeLevel * 100.0f;
            }

            set
            {
                EndpointVolume.SetMasterVolumeLevelScalar(value / 100.0f, Guid);
            }
        }

        /// <summary>
        /// Gets or sets the mute state of the master volume. 
        /// While the volume can be muted the <see cref="Volume"/> will still return the pre-muted volume value.
        /// </summary>
        public bool IsMuted
        {
            get {
                EndpointVolume.GetMute(out bool isMuted);
                return isMuted;
            }

            set
            {
                EndpointVolume.SetMute(value, Guid);
            }
        }

        /// <summary>
        /// Gets the Channel Count for this device
        /// </summary>
        public uint ChannelCount {
            get
            {
                EndpointVolume.GetChannelCount(out uint count);
                return count;
            }
        }

        public float[] ChannelLevel
        {
            get
            {
                float[] level = new float[ChannelCount];
                for (uint i = 0; i < ChannelCount; i++)
                {
                    EndpointVolume.GetChannelVolumeLevelScalar(i, out float clevel);
                    level[i] = clevel;
                }
                return level;
            }
            set
            {
                if (value.Length != ChannelCount) return;
                for (uint i = 0; i < ChannelCount; i++)
                {
                    EndpointVolume.SetChannelVolumeLevelScalar(i, value[i], Guid);
                }
            }
        }

        #endregion

        //#################################################################################################################################
        #region Public Functions
      
        #endregion

        //#################################################################################################################################
        #region Cleanup Functions
        protected override void DisposeManagedResources()
        {
        }

        protected override void DisposeUnmanagedResources()
        {
            EndpointVolume.UnregisterControlChangeNotify(this);

            if (EndpointVolume != null) Marshal.ReleaseComObject(EndpointVolume);
        }
        #endregion

        //#################################################################################################################################
        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        //#################################################################################################################################
        #region Internal Functions

        internal AudioEndpointVolume(AudioEndpoint endpoint, IMMDevice device)
        {
            Guid = Guid.NewGuid();
            Endpoint = endpoint;
            Guid IID_IAudioEndpointVolume = typeof(IAudioEndpointVolume).GUID;
            device.Activate(IID_IAudioEndpointVolume, 0, IntPtr.Zero, out object o);
            EndpointVolume = (IAudioEndpointVolume)o;

            EndpointVolume.RegisterControlChangeNotify(this);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //#################################################################################################################################
        #region IAudioEndpointVolumeCallback implementation
        public int OnNotify([In] IntPtr notificationData)
        {
            var voldata = CoreAudio.Structures.AUDIO_VOLUME_NOTIFICATION_DATA.FromIntPtr(notificationData);
            if (voldata.EventContext == Guid) return 0;
            
            NotifyPropertyChanged(nameof(Level));
            NotifyPropertyChanged(nameof(IsMuted));
            return 0;
        }
        #endregion

        //#################################################################################################################################


    }
}
