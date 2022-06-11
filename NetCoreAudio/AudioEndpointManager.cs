using CoreAudio.Enumerations;
using CoreAudio.Externals;
using CoreAudio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreAudio
{
    public class AudioEndpointManager : AudioBase, IMMNotificationClient
    {
        //#################################################################################################################################
        #region Properties
        internal IMMDeviceEnumerator DeviceEnumerator { get; private set; }
        #endregion

        //#################################################################################################################################
        #region Public Functions
        public AudioEndpointManager()
        {
            DeviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            DeviceEnumerator.RegisterEndpointNotificationCallback(this);
        }

        public AudioEndpoint? DefaultEndpoint(EndpointRole role, EndpointDataFlow dataFlow)
        {
            IMMDevice? device = null;
            try
            {
                // get the speakers (1st render + multimedia) device
                DeviceEnumerator.GetDefaultAudioEndpoint((EDataFlow)dataFlow, (ERole)role, out device);
                if (device == null) return null;
                return new AudioEndpoint(device);
            }
            catch (COMException)
            {
                if (device != null) Marshal.ReleaseComObject(device);
                return null;
            }
        }
        public List<AudioEndpoint> EndpointByName(string name = "", EndpointDataFlow dataFlow = EndpointDataFlow.All, EndpointState endpointState = EndpointState.Any)
        {
            IMMDeviceCollection? deviceCollection = null;
            IPropertyStore? deviceProperties = null;
            IMMDevice? device = null;

            List<AudioEndpoint> l = new();
            try
            {
                DeviceEnumerator.EnumAudioEndpoints((EDataFlow)dataFlow, (uint)endpointState, out deviceCollection);
                deviceCollection.GetCount(out uint count);

                for (uint i = 0; i < count; ++i)
                {
                    try
                    {
                        deviceCollection.Item(i, out device);
                        if (device == null) continue;

                        if (name != "")
                        {
                            string deviceName = AudioEndpoint.GetPropertyFromStore(device, CoreAudio.Constants.PropertyKeys.PKEY_Device_DeviceDesc);

                            if (deviceName == null || deviceName.ToLowerInvariant() != name.ToLowerInvariant())
                            {
                                if (device != null) Marshal.ReleaseComObject(device);
                                continue;
                            }
                        }

                        l.Add(new AudioEndpoint(device));
                    }
                    catch (COMException)
                    {
                        if (device != null) Marshal.ReleaseComObject(device);
                    }
                    finally
                    {
                        if (deviceProperties != null) Marshal.ReleaseComObject(deviceProperties);
                    }
                }
            }
            finally
            {
                if (deviceProperties != null) Marshal.ReleaseComObject(deviceProperties);
                if (deviceCollection != null) Marshal.ReleaseComObject(deviceCollection);
            }

            return l;
        }
        public AudioEndpoint? EndpointById(string endpointId)
        {
            IMMDevice? device = null;
            try
            {
                DeviceEnumerator.GetDevice(endpointId, out device);

                return new AudioEndpoint(device);
            }
            catch (COMException)
            {
                if (device != null) Marshal.ReleaseComObject(device);
                return null;
            }
        }

        #endregion

        //#################################################################################################################################
        #region Cleanup Functions
        protected override void DisposeManagedResources()
        {
        }

        protected override void DisposeUnmanagedResources()
        {
            if (DeviceEnumerator != null)
            {
                DeviceEnumerator.UnregisterEndpointNotificationCallback(this);
                Marshal.ReleaseComObject(DeviceEnumerator);
            }
        }


        #endregion

        //#################################################################################################################################
        #region Events
        #endregion

        //#################################################################################################################################
        #region Private Functions
        #endregion

        //#################################################################################################################################

        //#################################################################################################################################
        #region IMMNotificationClient implementation
        public void OnDeviceStateChanged([MarshalAs(UnmanagedType.LPWStr)] string deviceId, [MarshalAs(UnmanagedType.U4)] uint newState)
        {
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

    }
}
