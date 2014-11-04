using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.lib;
using WebMaster.lib.ui.browser;
using System.Xml;
using System.Diagnostics;

namespace WebMaster.com
{
    public partial class LoginDialog : Form
    {
        //private string username = null;
        //private string pwd = null;
        /// <summary>
        /// URL link to create a new account 
        /// </summary>
        private string regURL = null;
        /// <summary>
        /// URL link to find the lost password 
        /// </summary>
        private string lostURL = null;
        /// <summary>
        /// Temp way to handle login
        /// </summary>
        private WebBrowserEx browser = null ;
        private string status = "inital";
        private UserProfile user = null;
        private Stopwatch sw = new Stopwatch();
        long reqAvgTime = -1;
        /// <summary>
        /// Dialog login user after do login. 
        /// </summary>
        public UserProfile User {
            get { return user; }
            //set { user = value; }
        }

        public LoginDialog() {
            InitializeComponent();
            initUI();
            timer1.Tick += new EventHandler(timer1_Tick);
        }

        void timer1_Tick(object sender, EventArgs e) {
            // login failed
            if (reqAvgTime > 0 && sw.ElapsedMilliseconds>reqAvgTime*2) {
                this.lb_loading.Visible = false;
                this.timer1.Stop();
                string txt = UILangUtil.getMsg("dlg.login.err.text1");
                labelMsg.Text = txt;
                this.enableView();
                this.sw.Stop();
            }
        }

        private void initUI() {
            user = null;            
            XmlDocument cfg = ConfigUtil.getConfigFile();
            user = ConfigUtil.getUserInfo(cfg);
            if (user != null) {
                string key = ConfigUtil.getEncyptKey(user.Name);
                string name = user.Name;
                string pwd = ConfigUtil.DecryptDES(user.Password, key);
                //Console.WriteLine("Decode : name = "+name+", pwd = "+pwd+", key = "+key);
                this.tb_name.Text = name;
                this.tb_pwd.Text = pwd;
                this.ckb_savepwd.Checked = true;
                this.btn_login.Focus();
            } else {
                tb_name.Focus();
            }
        }

        //#region override 
        //protected override bool ProcessDialogKey(Keys keyData) {
        //    //if (keyData == Keys.Escape) {
        //    //    this.Close();
        //    //    return true;
        //    //} else {
        //        return base.ProcessDialogKey(keyData);
        //    //}
        //}
        //#endregion override

        private void tb_pwd_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                btn_login.PerformClick();
            }
            if (this.labelMsg.Text.Length > 0) {
                this.labelMsg.Text = string.Empty;
            }
        }

        private void tb_name_KeyDown(object sender, KeyEventArgs e) {
            if (this.labelMsg.Text.Length > 0) {
                this.labelMsg.Text = string.Empty;
            }
        }
        private void btn_login_Click(object sender, EventArgs e) {
            timer1.Start();
            sw.Restart();
            reqAvgTime = -1;
            disableView();
            tempDoLogin();
            //////////////////////////////////////////////////////////////////
            //Log.println_login("user name = "+this.tb_name.Text + ", pwd = "+this.tb_pwd.Text);
            //this.user = AccessCtrl.login(this.tb_name.Text, tb_pwd.Text, "");
            //if (user.Response == RESPONSE.LOGIN_SUCCESS) {                
            //    this.Close();
            //    this.DialogResult = System.Windows.Forms.DialogResult.OK;
            //}else{
            //    updateErrorMsg(user.Response);
            //}
        }
        /// <summary>
        /// Disable UI fields 
        /// </summary>
        private void disableView() {
            this.tb_name.Enabled = false;
            this.tb_pwd.Enabled = false;
            this.btn_login.Enabled = false;
            this.ckb_savepwd.Enabled = false;
            this.linkNewAcc.Enabled = false;
            this.linkLostPwd.Enabled = false;
        }
        /// <summary>
        /// enable UI fields 
        /// </summary>
        private void enableView() {
            this.tb_name.Enabled = true;
            this.tb_pwd.Enabled = true;
            this.btn_login.Enabled = true;
            this.ckb_savepwd.Enabled = true;
            this.linkNewAcc.Enabled = true;
            this.linkLostPwd.Enabled = true;
        }
        /// <summary>
        /// If save user info checked, save the user info in local file 
        /// </summary>
        private void handleSaveUserInfoLocal() {
            XmlDocument configXml = ConfigUtil.getConfigFile();
            if (ckb_savepwd.Checked && User!=null && User.Response == RESPONSE.LOGIN_SUCCESS) {                
                if (configXml != null) {
                    bool needSave = false;
                    XmlNode userNode = configXml.SelectSingleNode("/Config/User");
                    if (userNode == null) {
                        XmlElement userElem = configXml.CreateElement("User");
                        configXml.DocumentElement.AppendChild(userElem);
                        userNode = configXml.SelectSingleNode("/Config/User");
                        needSave = true;
                    }
                    string key = ConfigUtil.getEncyptKey(user.Name);
                    string name = user.Name;
                    string pwd = ConfigUtil.EncryptDES(user.Password, key);
                    //Console.WriteLine("encode : name = " + name + ", pwd = " + pwd + ", key = " + key);
                    XmlNode nameNode = configXml.SelectSingleNode("/Config/User/Name");                    
                    if (nameNode == null) {
                        userNode = configXml.SelectSingleNode("/Config/User");
                        XmlElement nameElem = configXml.CreateElement("Name");                        
                        userNode.AppendChild(nameElem);
                        nameNode = userNode.SelectSingleNode("Name");
                        needSave = true;
                    }
                    if (nameNode.InnerText != name) {                        
                        nameNode.InnerText = name;
                        needSave = true;
                    }
                    XmlNode pwdNode = configXml.SelectSingleNode("/Config/User/Pwd");                    
                    if (pwdNode == null) {
                        userNode = configXml.SelectSingleNode("/Config/User");
                        XmlElement pwdElem = configXml.CreateElement("Pwd");                        
                        userNode.AppendChild(pwdElem);
                        pwdNode = userNode.SelectSingleNode("Pwd");
                        needSave = true;
                    }
                    if (pwdNode.InnerText != pwd) {
                        pwdNode.InnerText = pwd;
                        needSave = true;
                    }
                    if (needSave) {
                        string configFullPath = Application.StartupPath + "\\" + Constants.LOCAL_CONFIG_XML;
                        configXml.Save(configFullPath);
                    }
                } else if (ckb_savepwd.Checked == false) {
                    if (configXml != null) {
                        XmlNode userNode = configXml.SelectSingleNode("/Config/User");                        
                        if (userNode != null) {
                            configXml.DocumentElement.RemoveChild(userNode);
                            string configFullPath = Application.StartupPath + "\\" + Constants.LOCAL_CONFIG_XML;
                            configXml.Save(configFullPath);
                        }
                    }
                }
            }            
        }

        private void tempDoLogin() {
            // do login from web page. 
            
            this.browser.Navigate(Constants.WEB_HOME);
            this.lb_loading.Visible = true;
            this.browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            //Console.WriteLine("======================Log in compelte stauts = " + this.browser.ReadyState + ", busy = " + this.browser.IsBusy+", count = "+count);
            if (!browser.IsBusy && this.browser.ReadyState == WebBrowserReadyState.Complete) {        
                //Console.WriteLine("Completed .  count = " + count);
                if (status == "inital") {
                    // input user name and pwd and login
                    if (putNamePwd()) {                        
                        reqAvgTime = sw.ElapsedMilliseconds;
                        //Console.WriteLine("reqAvgTime = " + reqAvgTime);
                        status = "login";
                    }
                } else if (status == "login") {
                    HtmlElement elemName = this.browser.Document.GetElementById("ls_username");                    
                    if (elemName == null) {
                        HtmlElementCollection ec = this.browser.Document.GetElementsByTagName("a");
                        bool find = false;
                        foreach (HtmlElement elemA in ec) {
                            string outer = elemA.OuterHtml;
                            
                            if (outer.Contains("title=访问我的空间") && outer.Contains("href=\"home.php?mod=space&amp;uid=")) {
                                string name = elemA.InnerText;
                                if (this.user == null) {
                                    user = new UserProfile();                                    
                                }
                                user.Name = name;
                                user.Password = this.tb_pwd.Text;
                                user.Response = RESPONSE.LOGIN_SUCCESS;

                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                                handleSaveUserInfoLocal();
                                timer1.Stop();
                                reqAvgTime = -1;
                                sw.Stop();
                                this.Close();
                                find = true;
                                break;
                            }
                        }
                        if (find) {
                            this.browser.DocumentCompleted -= browser_DocumentCompleted;
                            status = "initial";
                            this.lb_loading.Visible = true;
                        } else {
                            this.lb_loading.Visible = false;
                            if (this.user == null) {
                                user = new UserProfile();
                                user.Name = this.tb_name.Text;
                                user.Response = RESPONSE.PWD_ERROR;
                                this.enableView();
                            }
                            updateErrorMsg(user.Response);
                        }
                    }
                }
            }
        }
        
        private bool putNamePwd() {
            bool flag = false;
            HtmlElement elemName = this.browser.Document.GetElementById("ls_username");
            if (elemName != null && elemName.Name == "username") {
                elemName.SetAttribute("value", this.tb_name.Text);
                flag = true;
            } else {
                flag = false;
            }
            HtmlElement elemPwd = this.browser.Document.GetElementById("ls_password");
            if (elemPwd != null && elemPwd.Name == "password") {
                elemPwd.SetAttribute("value", this.tb_pwd.Text);
                flag = true;
            } else {
                flag = false;
            }
            bool login = false;
            HtmlElementCollection ec = this.browser.Document.GetElementsByTagName("button");
            foreach (HtmlElement btn in ec) {
                if (btn.InnerHtml.Contains("<EM>登录</EM>")) {
                    //Console.WriteLine("登陆点击=================================");
                    login = true;
                    btn.InvokeMember("click");
                    break;
                }
            }
            flag = flag && login;
            return flag;
        }

        private void updateErrorMsg(RESPONSE response) {
            string txt = UILangUtil.getMsg("dlg.login.err.text1");
            labelMsg.Text = txt;
        }
        
        private void btn_close_Click(object sender, EventArgs e) {
            Log.println_login("close pressed ");
            //this.Close();
        }

        public void tempSetMethod(WebBrowserEx browser){
            this.browser = browser ;
        }

        private void linkNewAcc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (browser != null) {
                this.browser.Navigate(Constants.WEB_HOME);
            }
            this.btn_close.PerformClick();
        }

        private void linkLostPwd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (browser != null) {
                this.browser.Navigate(Constants.WEB_HOME);
            }
            this.btn_close.PerformClick();
        }


    }
}
