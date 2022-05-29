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

        public Form1()
        {
            InitializeComponent();
            aem = new AudioEndpointManager();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            aem.EndpointByName("", EndpointDataFlow.Render, EndpointState.Any).ForEach(x => listBox1.Items.Add(x));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            aem.EndpointByName("", EndpointDataFlow.Capture, EndpointState.Any).ForEach(x => listBox2.Items.Add(x));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox3.Items.Clear();

            (listBox1.SelectedItem as AudioEndpoint).Sessions.ForEach(x => listBox3.Items.Add($"{x.ProcessTitle} : {x.ProcessName}"));
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox3.Items.Clear();

            (listBox2.SelectedItem as AudioEndpoint).Sessions.ForEach(x => listBox3.Items.Add($"{x.ProcessTitle} : {x.ProcessName}"));
        }
    }
}
