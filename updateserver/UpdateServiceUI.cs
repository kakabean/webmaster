using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.IO;
using System.Collections;
using WebMaster.updateserver;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using WebMaster.lib.engine;

namespace WebMaster.updateserver
{
    public partial class UpdateServiceUI : Form
    {
        public UpdateServiceUI() {
            InitializeComponent();
            Logger.listLog = this.lvApps;
        }

        private void button1_Click(object sender, EventArgs e) {
            DialogResult r = this.folderBrowserDialog1.ShowDialog(this);
            if (r == System.Windows.Forms.DialogResult.OK) {
                this.tb_appDir.Text = this.folderBrowserDialog1.SelectedPath;
                EnumerateDirectory();
            }
        }
        /// <summary>
        ///	Enumerate the directory content
        /// </summary>
        private void EnumerateDirectory() {
            lvApps.Items.Clear();            
            DirectoryInfo dInfo = new DirectoryInfo(this.tb_appDir.Text);
            ArrayList fileCollection = new ArrayList();
            foreach (FileInfo fInfo in dInfo.GetFiles()) {	// iterate thru the files
                string version = Constants.FILE_UNKNOWN;
                //if (fInfo.Extension == ".exe" || fInfo.Extension == ".dll") {
                    version = ConfigInfo.Instance.getVersion(fInfo);
                //}

                ListViewItem item = lvApps.Items.Add(fInfo.Name);
                item.SubItems.Add(version);
                item.Tag = fInfo;

                fileCollection.Add(new FileObject(fInfo, version));
            }
            // Get the file information
            ConfigInfo.Instance.FileObjects = fileCollection.ToArray(typeof(FileObject)) as FileObject[];
        }
        /// <summary>
        ///		Starts the server onto a specific port
        /// </summary>
        private void startServer() {
            // Display the log
            logServerInfo("Opening channel..");            
            // start listening
            TcpServerChannel serverChannel = new TcpServerChannel(Constants.UPDATE_SERVER_CHANNEL_NAME, Constants.UPDATE_SERVER_CHANNEL_PORT);
            //TcpServerChannel serverChannel = new TcpServerChannel(7444);
            // now register the channel
            logServerInfo("Opening channel..completed.");
            logServerInfo("Registering channel..");
            ChannelServices.RegisterChannel(serverChannel, false);
            logServerInfo("Registering channel..completed.");
            logServerInfo("Registering WKO Objects..");
            // register/expose the wko objects
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(UpdateService), Constants.UPDATE_OBJ_URI, WellKnownObjectMode.SingleCall);
            logServerInfo("Registering WKO Objects..completed.");
        }

        private void logServerInfo(string msg) {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToShortTimeString()).Append(" : ");
            sb.Append(msg);
            this.rtb_logs.AppendText(sb.ToString());
        }

        private void tsb_start_Click(object sender, EventArgs e) {
            startServer();
            this.tsb_start.Enabled = false;
            this.tsb_stop.Enabled = true;
        }

        private void tsb_stop_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
