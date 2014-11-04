using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.IO;
using System.Xml;

namespace ide
{
    public partial class Form1 : Form
    {
        private DateTime startTime = DateTime.Now;
        private DateTime currTime = DateTime.Now;
        int timerSpan = 0;
        Stopwatch sw = new Stopwatch();
        public Form1() {
            InitializeComponent();
            //this.comboBoxEx2.Readonly = true;
            //timer1.Tick += new EventHandler(timer1_Tick);
        }

        void timer1_Tick(object sender, EventArgs e) {
            timerSpan += timer1.Interval;
            int sec = timerSpan / 1000;
            tb_span.Text = sec.ToString() ;
            TimeSpan span = new TimeSpan(0, 0, sec);
            currTime = startTime.Add(span);
            this.numericUpDown6.Value = currTime.Hour;
            this.numericUpDown5.Value = currTime.Minute;
            this.numericUpDown4.Value = currTime.Second;
        }

        private void button1_Click(object sender, EventArgs e) {
            this.textBox1.Text = null;
            this.listView1.Items[0].Selected = true;
            //this.listView1.Items[0].Focused = true;
            this.listView1.Focus() ;
        }

        private void button2_Click(object sender, EventArgs e) {
            String text = this.textBox1.Text;
            Console.WriteLine("\n================"+text+"\n==================");
            //this.comboBoxEx2.SelectedIndex = 1;
        }

        private void button3_Click(object sender, EventArgs e) {
            this.listView1.Items[2].Selected = true;
            //this.listView1.Items[2].Focused = true;
            comboBoxEx2.SelectedIndex = 2;
        }

        private void textBox3_TextChanged(object sender, EventArgs e) {
            label2.Text = textBox3.Text;
        }

        private void comboBoxEx2_SelectionChangeCommitted(object sender, EventArgs e) {
            Console.WriteLine("committed, index = "+this.comboBoxEx2.SelectedIndex);
        }

        private void comboBoxEx2_SelectedIndexChanged(object sender, EventArgs e) {
            Console.WriteLine("IndexChanged, index = " + this.comboBoxEx2.SelectedIndex);
        }

        private void button4_Click(object sender, EventArgs e) {
            string color = this.textBox3.Text;
            Color c1 = Color.FromName(color);
            int rgb = c1.ToArgb();
            Color c2 = Color.FromArgb(rgb);
            this.rtb.SelectionColor = c1;
            this.rtb.AppendText(""+color);
            this.rtb.AppendText(Environment.NewLine);

            this.rtb1.SelectionColor = c2;
            this.rtb1.AppendText("" + rgb);
            this.rtb1.AppendText(Environment.NewLine);

            //DialogResult dr = this.colorDialog1.ShowDialog();
            //if (dr == System.Windows.Forms.DialogResult.OK) {
            //    //Color color = colorDialog1.Color;
            //    Color color = Color.Black;
            //    this.rtb.SelectionColor = color;
            //    int argb = color.ToArgb();
            //    this.rtb.AppendText(argb + "");
            //    rtb.AppendText(Environment.NewLine);

            ////    Color nc = Color.FromArgb(argb);
            ////    this.rtb1.SelectionColor = nc;
            ////    this.rtb1.AppendText(argb + "");
            ////    rtb1.AppendText(Environment.NewLine);
            //}            
            ////ScriptRoot sroot = ModelFactory.createScriptRoot();
            ////foreach (DictionaryEntry de in sroot.ColorMap) {
            ////    Color color = Color.FromArgb(int.Parse(de.Value.ToString()));
            ////    rtb.SelectionColor = color;
            ////    rtb.AppendText(de.Key + " = " + de.Value);
            ////    rtb.AppendText(Environment.NewLine);
            ////}
        }

        private void panel2_Paint(object sender, PaintEventArgs e) {
            //ControlPaint.DrawBorder(e.Graphics, this.panel2.ClientRectangle,
            //                    SystemColors.ActiveBorder,
            //                    1,
            //                    ButtonBorderStyle.Solid,
            //                    SystemColors.ActiveBorder,
            //                    1,
            //                    ButtonBorderStyle.Solid,
            //                    SystemColors.ActiveBorder,
            //                    1,
            //                    ButtonBorderStyle.Solid,
            //                    SystemColors.ActiveBorder,
            //                    1,
            //                    ButtonBorderStyle.Solid);
            ControlPaint.DrawBorder(e.Graphics, this.panel2.ClientRectangle, SystemColors.ActiveCaption, ButtonBorderStyle.Solid);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void button5_Click(object sender, EventArgs e) {
            WebElement we = new WebElement();
            WebElementAttribute wea = new WebElementAttribute();
            we.Attributes.Add(wea);
            Operation op = new Operation();
            op.Element = we;
            Condition con = new Condition();
            con.Input1 = wea;
            GlobalFunction gf = new GlobalFunction();
            gf.Params.Add(wea);
            WebMaster.lib.engine.Process proc = new WebMaster.lib.engine.Process();
            proc.Element = we;
            proc.Input = wea;

            ModelManager.Instance.removeFromModel(wea);
            
            Console.WriteLine("");
            
        }

        private void button6_Click(object sender, EventArgs e) {
            DateTime t1 = new DateTime(1980, 12, 20, (int)t1h.Value, (int)t1m.Value, (int)t1s.Value);
            DateTime t2 = new DateTime(1980, 12, 20, (int)t2h.Value, (int)t2m.Value, (int)t2s.Value);
            TimeSpan ts = new TimeSpan((int)t2h.Value, (int)t2m.Value, (int)t2s.Value);
            DateTime t3 = t1.Add(ts);
            t3h.Value = t3.Hour;
            t3m.Value = t3.Minute;
            t3s.Value = t3.Second;

        }

        private void button7_Click(object sender, EventArgs e) {
            timerSpan = 0;
            button7.Text = "Start";
            int count = 0;
            int ajustWait = 1000;
            long lastElapsed = 0;
            sw.Start();
            currTime = DateTime.Now;
            long sw_elapsed = 0;
            long tobe_elapsed = 0;
            int wave = 100 ;
            while (count++ < 10000) {
                //this.numericUpDown3.Value = DateTime.Now.Hour;
                //this.numericUpDown2.Value = DateTime.Now.Minute;
                //this.numericUpDown1.Value = DateTime.Now.Second;
                sw_elapsed = sw.ElapsedMilliseconds;
                currTime.AddMilliseconds(sw_elapsed);
                //this.numericUpDown6.Value = currTime.Hour;
                //this.numericUpDown5.Value = currTime.Minute;
                //this.numericUpDown4.Value = currTime.Second;
                Console.WriteLine("Time now = "+DateTime.Now+"\\Time New = "+currTime+", sw.elapse = "+sw_elapsed+", last = "+lastElapsed+", adjust wait = "+ajustWait);
                long dw = sw_elapsed - tobe_elapsed;
                if ( dw > (0-wave) && dw<wave) {
                    ajustWait = (int)(1000 - dw);
                }
                //if (sw_elapsed - lastElapsed > 1000) {
                //    ajustWait = (int)(1000 - (sw_elapsed - lastElapsed -1000));
                //} else {
                //    ajustWait = 1000;
                //}
                //lastElapsed = sw_elapsed;

                tobe_elapsed = sw_elapsed + ajustWait;
                Thread.Sleep(ajustWait);

            }
        }

        private void button8_Click(object sender, EventArgs e) {
            webBrowserEx1.Navigate("http://www.baidu.com");
        }
        /// <summary>
        /// Try to the get reqeust/response header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e) {
            string url = tb_url.Text;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            long d1 = sw.ElapsedMilliseconds;
            HttpWebResponse res; 
            try { 
                res = (HttpWebResponse)req.GetResponse(); 
            } catch (WebException ex) { 
                res = (HttpWebResponse)ex.Response;
                this.tb_res.Text = ex.ToString();
            }
            DateTime st = DateTime.MinValue;
            DateTime.TryParse(res.Headers["Date"], out st);
            if (st != DateTime.MinValue) {
                long delay = (long)sw.ElapsedMilliseconds / 2;
                st.AddMilliseconds(delay);
                this.tb_req.Text = " Delay request create = " + d1 + "\n\r Delay get Response = " + delay + "\r\n server Time = " + st;
            }
            DateTime st1 = new DateTime(st.Ticks);
            
            this.tb_res.Text = res.Headers.ToString();
        }

        private void button10_Click(object sender, EventArgs e) {
            TestXmlParse();
        }
        private int TestXmlParse() {
            string localXmlFilePath = Application.StartupPath + "\\update.xml";
            if (!File.Exists(localXmlFilePath)) {
                return -1;
            }

            XmlDocument localXml = new XmlDocument();
            try {
                localXml.Load(localXmlFilePath);
            } catch (Exception) {
                return 0;
            }
            // get the update folder url.
            string url = localXml.SelectSingleNode("//URL").InnerText + "/update.xml";
            
            return 0 ;
        }

        private void button11_Click(object sender, EventArgs e) {
            string assemblylocation = Assembly.GetExecutingAssembly().CodeBase;
            assemblylocation = assemblylocation.Substring(assemblylocation.LastIndexOf("/") + 1);
            //applicationName = assemblylocation;
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            string localVersion = assemblyName.Version.ToString();
            string compath = Application.StartupPath + @"\com.dll";
            Version ver = Assembly.LoadFrom(compath).GetName().Version;
            string test = ver.ToString();


            //XmlDocument xmlFile = new XmlDocument();
            //string url = "http://www.autowebkit.com/update/updater.xml";
            //try {
            //    xmlFile.Load(url);
            //    string name = xmlFile.Name;
            //} catch (Exception e1) {
            //    string tt = e1.StackTrace.ToString();
            //}
            string f1 = @"D:\ZhangHui\mywork\WebMasterAll\solution\WebMaster\com\update\update_files.xml";
            XmlDocument xml = new XmlDocument();
            xml.Load(f1);
            XmlNode n1 = xml.SelectSingleNode("//Files");
            XmlNodeList nlist = n1.ChildNodes;
            foreach (XmlNode node in nlist) {
                string name = node.Attributes["name"].Value;
                string value = node.Attributes["version"].Value;
            }
            int c = nlist.Count;
        }
    }
}
