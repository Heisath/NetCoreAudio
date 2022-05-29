using CoreAudio.Enumerations;
using CoreAudio.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreAudio
{
    public class AudioSession : AudioBase, INotifyPropertyChanged, IAudioSessionEvents
    {
        //#################################################################################################################################
        #region Properties
        public AudioEndpoint Endpoint { get; private set; }
        internal IAudioSessionControl2 Control { get; private set; }
        internal Guid Guid { get; private set; }

        public uint ProcessId { 
            get
            {
                Control.GetProcessId(out uint cpid);
                return cpid;
            } 
        }
        public string ProcessTitle
        {
            get
            {
                Control.GetDisplayName(out string appname);

                if (appname == null || appname == "")
                {
                    try
                    {
                        var process = Process.GetProcessById((int)ProcessId);
                        if (process == null) return null;

                        appname = process.MainWindowTitle;

                        if (appname == "")
                        {
                            appname = process.MainModule.FileVersionInfo.FileDescription;
                        }
                        if (appname == "")
                        {
                            appname = process.MainModule.FileVersionInfo.FileName;
                        }
                    }
                    catch (Exception) { }
                }

                if (appname != null && appname.Contains(@"@%SystemRoot%\System32\AudioSrv.Dll"))
                {
                    appname = "System";
                }

                return appname;
            }
        }
        public string ProcessName
        {
            get
            {
                string appname = null;
                try
                {
                    var process = Process.GetProcessById((int)ProcessId);
                    if (process == null) return null;

                    appname = process.MainModule.FileVersionInfo.FileName;
                }
                catch (Exception) { }

                if (appname == null) Control.GetDisplayName(out appname);

                if (appname != null && appname.Contains(@"@%SystemRoot%\System32\AudioSrv.Dll"))
                {
                    appname = "System";
                }

                return appname;
            }
        }

        public float Volume
        {
            get
            {
                (Control as ISimpleAudioVolume).GetMasterVolume(out float level);
                return level * 100.0F;
            }
            set
            {
                (Control as ISimpleAudioVolume).SetMasterVolume(value, Guid);
            }
        }

        public float VolumeLog
        {
            get
            {
                (Control as ISimpleAudioVolume).GetMasterVolume(out float level);

                double SliderValue = Math.Log((level * 500.0 - A) / B) / C;
                return (float)SliderValue * 100.0F;
            }
            set
            {
                double DisplayValue = (A + B * Math.Exp(C * value / 100.0F)) / 500.0;
                (Control as ISimpleAudioVolume).SetMasterVolume(DisplayValue, Guid);
            }
        }

        public bool Mute
        {
            get
            {
                (Control as ISimpleAudioVolume).GetMute(out bool mute);
                return mute;
            }
            set
            {
                (Control as ISimpleAudioVolume).SetMute(value, Guid);
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
            if (Control != null)
            {
                Control.UnregisterAudioSessionNotification(this);
                Marshal.ReleaseComObject(Control);
            }
        }
        #endregion

        //#################################################################################################################################
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Privates 
        readonly double A = -100.0 / 3;
        readonly double B = 100.0 / 3;
        readonly double C = Math.Log(16.0);
        #endregion

        //#################################################################################################################################
        #region Internal Functions
        internal AudioSession(AudioEndpoint ep, IAudioSessionControl2 control)
        {
            Endpoint = ep;
            Control = control;
            Guid = Guid.NewGuid();
            Control.RegisterAudioSessionNotification(this);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //#################################################################################################################################
        #region IAudioSessionEvents implementation
        public int OnDisplayNameChanged([In, MarshalAs(UnmanagedType.LPWStr)] string displayName, [In] ref Guid eventContext)
        {
            NotifyPropertyChanged(nameof(ProcessName));
            NotifyPropertyChanged(nameof(ProcessTitle));
            return 0;
        }

        public int OnIconPathChanged([In, MarshalAs(UnmanagedType.LPWStr)] string iconPath, [In] ref Guid eventContext)
        {
            return 0;
        }

        public int OnSimpleVolumeChanged([In, MarshalAs(UnmanagedType.R4)] float volume, [In, MarshalAs(UnmanagedType.Bool)] bool isMuted, [In] ref Guid eventContext)
        {
            if (eventContext == Guid) return 0; // only notify on changes not done by us

            NotifyPropertyChanged(nameof(Volume));
            NotifyPropertyChanged(nameof(Mute));
            return 0;
        }

        public int OnChannelVolumeChanged([In, MarshalAs(UnmanagedType.U4)] uint channelCount, [In, MarshalAs(UnmanagedType.SysInt)] IntPtr newVolumes, [In, MarshalAs(UnmanagedType.U4)] uint channelIndex, [In] ref Guid eventContext)
        {
            return 0;
        }

        public int OnGroupingParamChanged([In] ref Guid groupingId, [In] ref Guid eventContext)
        {
            return 0;
        }

        public int OnStateChanged([In] AudioSessionState state)
        {
            // TODO: Handle state changes
            return 0;
        }

        public int OnSessionDisconnected([In] AudioSessionDisconnectReason disconnectReason)
        {
            // TODO: Handle session disconnect
            return 0;
        }

        #endregion

        //#################################################################################################################################
    }
}
