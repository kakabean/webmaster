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
    public partial class WEProcImageView : UserControl, IWEPropView
    {
        private ScriptRoot sroot = null;

        public WEProcImageView() {
            InitializeComponent();
        }
        #region mandatory methods 
        public void updateView(object elem, bool isNew) {            
        }
        /// <summary>
        /// 
        /// </summary>
        public void resetView() {
            
        }

        public WebElement getWebElement() {
            return null;
        }

        public void showView() {
            this.Visible = true;
        }

        public void hideView() {
            this.Visible = false;
        }

        public void enableView() {            
        }

        public void disableView() {
        }
        public void showErrorMsg()
        {
            //TODO
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
