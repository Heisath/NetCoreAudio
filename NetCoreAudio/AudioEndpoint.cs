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
    public class AudioEndpoint : AudioBase, IMMNotificationClient, INotifyPropertyChanged
    {
        //#################################################################################################################################
        #region Properties
        public string Id { get; private set; }
        public string InterfaceName => GetPropertyFromStore(Device, CoreAudio.Constants.PropertyKeys.PKEY_DeviceInterface_FriendlyName);
        public string Name => GetPropertyFromStore(Device, CoreAudio.Constants.PropertyKeys.PKEY_Device_FriendlyName);
        public string Description => GetPropertyFromStore(Device, CoreAudio.Constants.PropertyKeys.PKEY_Device_DeviceDesc);
     
        public AudioEndpointVolume? VolumeControl { get; private set; }

        internal IMMDevice Device { get; private set; }
        internal IMMDeviceEnumerator DeviceEnumerator { get; private set; }

        public List<AudioSession> Sessions { get => GetAudioSessions(); }

        public EndpointState State
        {
            get
            {
                Device.GetState(out uint deviceState);
                return (EndpointState)deviceState;
            }
        }

        public override string ToString()
        {
            return $"({State}) {Description ?? ""} @ {InterfaceName ?? ""} : {Name ?? ""}";
        }
    
        #endregion

        //#################################################################################################################################
        #region Public Functions
      
        #endregion

        //#################################################################################################################################
        #region Cleanup Functions
        protected override void DisposeManagedResources()
        {
            if (VolumeControl != null) VolumeControl.Dispose();
        }

        protected override void DisposeUnmanagedResources()
        {
            if (Device != null) Marshal.ReleaseComObject(Device);
            if (DeviceEnumerator != null)
            {
                DeviceEnumerator.UnregisterEndpointNotificationCallback(this);
                Marshal.ReleaseComObject(DeviceEnumerator);
            }
        }
        #endregion

        //#################################################################################################################################
        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        //#################################################################################################################################
        #region Internal Functions

        internal AudioEndpoint(string id, IMMDevice dev)
        {
            Id = id;
            Device = dev;

            DeviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            DeviceEnumerator.RegisterEndpointNotificationCallback(this);

            if (State == EndpointState.Active)
            {
                VolumeControl = new AudioEndpointVolume(this, dev);
            }
        }

        private List<AudioSession> GetAudioSessions()
        {
            // Endpoint in states NotPresent or Unplugged cannot have any sessions!
            if ((State & (EndpointState.NotPresent | EndpointState.Unplugged)) != 0)
                return new List<AudioSession>();

            IAudioSessionEnumerator? sessionEnumerator = null;
            IAudioSessionManager2? sessionManager = null;
            try
            {
                // activate the session manager. we need the enumerator
                Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
                Device.Activate(IID_IAudioSessionManager2, 0, IntPtr.Zero, out object o);
                sessionManager = (IAudioSessionManager2)o;

                // enumerate sessions for on this device
                sessionManager.GetSessionEnumerator(out sessionEnumerator);
                sessionEnumerator.GetCount(out int count);

                List<AudioSession> results = new();
                for (int i = 0; i < count; ++i)
                {
                    IAudioSessionControl? ctl = null;
                    try
                    {
                        sessionEnumerator.GetSession(i, out ctl);
                        results.Add(new AudioSession(this, (IAudioSessionControl2)ctl));
                    }
                    catch (Exception)
                    {
                        if (ctl != null) Marshal.ReleaseComObject(ctl);
                    }
                }

                return results;
            }
            finally
            {
                if (sessionEnumerator != null) Marshal.ReleaseComObject(sessionEnumerator);
                if (sessionManager != null) Marshal.ReleaseComObject(sessionManager);
            }
        }

        internal static string GetPropertyFromStore(IMMDevice device, PROPERTYKEY PKey)
        {
            IPropertyStore? deviceProperties = null;
            try
            {
                device.OpenPropertyStore(STGM.STGM_READ, out deviceProperties);
                deviceProperties.GetValue(ref PKey, out PROPVARIANT variant);
                return Marshal.PtrToStringUni(variant.Data.AsStringPtr) ?? "";
            }
            finally
            {
                if (deviceProperties != null) Marshal.ReleaseComObject(deviceProperties);
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //#################################################################################################################################

        #region IMMNotificationClient implementation
        public void OnDeviceStateChanged([MarshalAs(UnmanagedType.LPWStr)] string deviceId, [MarshalAs(UnmanagedType.U4)] uint newState)
        {
            if (deviceId != this.Id) return;

        }

        public void OnDeviceAdded([MarshalAs(UnmanagedType.LPWStr)] string deviceId)
        {
        }

        public void OnDeviceRemoved([MarshalAs(UnmanagedType.LPWStr)] string deviceId)
        {
        }

        public void OnDefaultDeviceChanged([MarshalAs(UnmanagedType.I4)] EDataFlow dataFlow, [MarshalAs(UnmanagedType.I4)] ERole deviceRole, [MarshalAs(UnmanagedType.LPWStr)] string defaultDeviceId)
        {
        }

        public void OnPropertyValueChanged([MarshalAs(UnmanagedType.LPWStr)] string deviceId, PROPERTYKEY propertyKey)
        {
        }
        #endregion

        //#################################################################################################################################

    }
}
