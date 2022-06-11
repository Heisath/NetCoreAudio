using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetCoreAudio;

namespace NetCoreAudioDemo
{
    public partial class Form1 : Form
    {
        AudioEndpointManager aem;
        AudioEndpoint? currentEndpoint;
        AudioSession? currentSession;

        public Form1()
        {
            InitializeComponent();
            aem = new AudioEndpointManager();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            aem.EndpointByName("", EndpointDataFlow.Render, EndpointState.Any).ForEach(x => listBox1.Items.Add(x));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            aem.EndpointByName("", EndpointDataFlow.Capture, EndpointState.Any).ForEach(x => listBox2.Items.Add(x));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePrograms(listBox1);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePrograms(listBox2);
        }

        private void UpdatePrograms(ListBox source)
        {
            listBox3.Items.Clear();

            if (currentEndpoint != null && currentEndpoint.VolumeControl != null)
            {
                currentEndpoint.VolumeControl.PropertyChanged -= VolumeControl_PropertyChanged;
            }

            currentEndpoint = source.SelectedItem as AudioEndpoint;

            if (currentEndpoint != null && currentEndpoint.VolumeControl != null)
            {
                currentEndpoint.VolumeControl.PropertyChanged += VolumeControl_PropertyChanged;
                trackBar1.Value = (int)Math.Round(currentEndpoint.VolumeControl.Level);
            }
            label1.Text = currentEndpoint?.Name;

            currentEndpoint?.Sessions.ForEach(x => listBox3.Items.Add(x));
        }

        private void VolumeControl_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            trackBar1.Invoke(() =>
            {
                trackBar1.Value = (int)Math.Round((sender as AudioEndpointVolume)?.Level ?? 0);
            });
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentSession != null)
            {
                currentSession.PropertyChanged -= CurrentSession_PropertyChanged;
            }

           currentSession = listBox3.SelectedItem as AudioSession;

            if (currentSession != null)
            {
                currentSession.PropertyChanged += CurrentSession_PropertyChanged;
                trackBar2.Value = (int)Math.Round(currentSession.Volume);
            }
            label2.Text = currentSession?.ProcessTitle;

        }

        private void CurrentSession_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            trackBar2.Invoke(() =>
            {
                trackBar2.Value = (int)Math.Round((sender as AudioSession)?.Volume ?? 0);
            });
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (currentEndpoint != null && currentEndpoint.VolumeControl != null)
            {
                currentEndpoint.VolumeControl.Level = trackBar1.Value;
            }
        }
    }
}
