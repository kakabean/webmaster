using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;

namespace WebMaster.ide.ui
{
    public partial class WEPropCodeView : UserControl, IWEPropView
    {
        public WEPropCodeView() {
            InitializeComponent();
        }
        #region variable definition
        // current editable WebElement 
        private WebElement _we =null;
        private ScriptRoot sroot = null;
        private string _tag = "";
        private string _id = "";
        public static string EMPTY_NAME = ""; //"Please input name for the element";
        public static string EMPTY_DESCRIPTION ="";// "Some descriptions for the element";
        #endregion variable definition
        #region event
        #endregion event
        #region mandatory methods 
        /// <summary>
        /// update the property view based on the element. e.g by specific string, by attributes, 
        /// by color, by location or by image, it depends on the WEType.
        /// </summary>
        /// <param name="elem">HtmlElement as a input of the property view</param>
        public void updateView(Object elem, bool isNew) {
            // come from webelement capture 
            if (elem is HtmlElement) {
                this._we = ModelFactory.createWebElement();
                HtmlElement he = (HtmlElement)elem;
                // handle name 
                if (he.Name == null || he.Name.Trim().Length < 1) {
                    this.textBox1.Text = EMPTY_NAME;
                } else {
                    this.textBox1.Name = he.Name;
                }
                // handle description
                this.textBox2.Text = EMPTY_DESCRIPTION;
                // handle feature string
                this.textBox3.Text = getFeatureString(he);
                this._tag = he.TagName;
                this._id = he.Id;
            } else if (elem is WebElement) {
                // come from flow editor 
                this._we = (WebElement)elem;
                this.textBox1.Text = _we.Name;
                this.textBox2.Text = _we.Description;
                this.textBox3.Text = _we.FeatureString;
                this._tag = _we.Tag;
                this._id = _we.ID;
            }else { 
                //TODO LOG
            }
        }        
        /// <summary>
        /// build a feature string for the HtmlElement, that can unique locate 
        /// the web element in current page most time.  ref WEType.Code definition. 
        /// 
        /// Feature string = tag + id + name + gloable tag index + innerText
        /// like: {tag:input},{id:username},{name:username},{gIndex:177},{text:xxx}, each 
        /// part is blocked with {}
        /// frame info is internal referenced. 
        /// </summary>
        /// <param name="he"></param>
        /// <returns></returns>
        private string getFeatureString(HtmlElement he) {
            StringBuilder sb = new StringBuilder();
            sb.Append("{").Append(Constants.HE_TAG).Append(":").Append(he.TagName).Append("}");
            if (he.Id != null && he.Id.Length > 0) {
                sb.Append(",{").Append(Constants.HE_ID).Append(":").Append(he.Id).Append("}");
            }
            if (he.Name != null && he.Name.Trim().Length > 0) {
                sb.Append(",{name:").Append(he.Name).Append("}");
            }
            // get the index of the element 
            HtmlElementCollection elems = he.Document.GetElementsByTagName(he.TagName);
            for (int i = 0; i < elems.Count; i++) {
                if (elems[i].Equals(he)) {
                    sb.Append(",{").Append(Constants.HE_GINDEX).Append(":").Append(i).Append("}");
                    break;
                }
            }
            if (he.InnerText != null && he.InnerText.Length > 0) {
                sb.Append(",{").Append("Text :").Append(he.InnerText).Append("}");
            }
            return sb.ToString();
        }
        /// <summary>
        /// get the WebElement specified by the property view. 
        /// </summary>
        /// <returns>get the WebElement from the property view</returns>
        public WebElement getWebElement() {
            // update values 
            this._we.TYPE = WEType.CODE;
            if (!EMPTY_NAME.Equals(this.textBox1.Text)) {
                this._we.Name = this.textBox1.Text;
            }
            if (!EMPTY_DESCRIPTION.Equals(this.textBox2.Text)) {
                this._we.Description = this.textBox2.Text;
            }
            //this._we.Tag = this._tag;
            //this._we.ID = this._id;
            
            if(this._we.FeatureString == null || !this._we.FeatureString.Equals(this.textBox3.Text)) {
                this._we.FeatureString = this.textBox3.Text;
                WebUtil.convertCodeToAttribute(this._we);
            }
            return this._we;
        }
        
        public void resetView() {
            // clean WebElement 
            this._we = null;
            // clean UI info 
            this.textBox1.Text = Constants.BLANK_TEXT;
            this.textBox2.Text = Constants.BLANK_TEXT;
            this.textBox3.Text = Constants.BLANK_TEXT;
        }
        public void showView() {
            this.Visible = true;
        }
        public void hideView() {
            this.Visible = false;
        }
        public void enableView() {
            if (this.textBox1.Enabled == false) {
                this.textBox1.Enabled = true;
            }
            if (this.textBox2.Enabled == false) {
                this.textBox2.Enabled = true;
            }
            if (this.textBox3.Enabled == false) {
                this.textBox3.Enabled = true;
            }            
        }
        public void disableView() {
            if (this.textBox1.Enabled == true) {
                this.textBox1.Enabled = false;
            }
            if (this.textBox2.Enabled == true) {
                this.textBox2.Enabled = false;
            }
            if (this.textBox3.Enabled == true) {
                this.textBox3.Enabled = false;
            }
        }
        public bool isValid()
        {
            return false;
        }
        public string getInvalidMsg() {
            return string.Empty;
        }
        public void setScriptRoot(ScriptRoot sroot) {
            this.sroot = sroot;
            this.resetView();
        }
        /// <summary>
        /// this method can be invoked when control size changed 
        /// </summary>
        public void handleSizeChangedEvt() {
            //TODO
        }

        public void resetUISize() {
            //TODO 
        }
        #endregion mandatory methods         
    }
}
