using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using System.Collections;
using WebMaster.lib;

namespace WebMaster.ide.ui
{
    public class PropViewManager
    {
        #region events 
        /// <summary>
        /// raise the event for outer properties control to check the current view modeled
        /// WebElement. event data is useless 
        /// </summary>
        public event EventHandler<CommonEventArgs> CheckWebElementEvt;
        protected virtual void OnCheckWebElementEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> checkWebElementEvt = CheckWebElementEvt;
            if (checkWebElementEvt != null) {
                checkWebElementEvt(this, e);
            }
        }
        /// <summary>
        /// raise the event for outer properties control to check the current view modeled
        /// WebElement. event data is useless 
        /// </summary>
        /// <param name="sender"></param>
        public void raiseCheckWebElementEvt(Object sender) {
            if (sroot != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, null);
                OnCheckWebElementEvt(evt);
            }
        }
        #endregion
        /// <summary>
        /// predefined views table 
        /// </summary>
        Hashtable _viewTable = new Hashtable();
        /// <summary>
        /// container of the property views 
        /// </summary>
        private Control _container = null;
        /// <summary>
        /// current active view 
        /// </summary>
        private IWEPropView _activeView = null;
        private ScriptRoot sroot = null;
        /// <summary>
        /// current active property view 
        /// </summary>
        public IWEPropView ActiveView {
            get { return _activeView; }
            //set { _activeView = value; }
        }
        public PropViewManager(Control container) {
            this._container = container;
        }
        public void initViews() { 
            // add Image view, COLOR view, location view. 
            // add attribute view 
            WEProcAttrView attView = new WEProcAttrView();
            attView.Dock = DockStyle.Fill;
            attView.SizeChanged += new EventHandler(attView_SizeChanged);
            attView.CheckWebElementEvt += new EventHandler<CommonEventArgs>(attView_CheckWebElementEvt);
            this._container.Controls.Add(attView);
            //attView.hideView();
            this._viewTable.Add(WEType.ATTRIBUTE, attView);
            // add code view 
            WEPropCodeView codeView = createCodeView();
            codeView.Dock = DockStyle.Fill;
            this._container.Controls.Add(codeView);
            codeView.hideView();
            this._viewTable.Add(WEType.CODE, codeView);
            // add image view 
            WEProcImageView imgView = createImageView();
            imgView.Dock = DockStyle.Fill;
            this._container.Controls.Add(imgView);
            this._viewTable.Add(WEType.IMAGE, imgView);
            
            // set default active view
            this._activeView = attView;
        }

        void attView_CheckWebElementEvt(object sender, CommonEventArgs e) {
            this.raiseCheckWebElementEvt(sender);
        }

        void attView_SizeChanged(object sender, EventArgs e) {
            IWEPropView pv = sender as IWEPropView;
            pv.handleSizeChangedEvt();
        }
        /// <summary>
        /// get the proper WebElement property view by type or null if errors 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IWEPropView getView(WEType type) { 
            foreach(DictionaryEntry de in this._viewTable){
                if (de.Key.Equals(type)) {
                    return (IWEPropView)de.Value;
                }
            }
            //TODO LOG 
            return null;
        }
        /// <summary>
        /// active the view by type 
        /// </summary>
        /// <param name="type">type of the view that will be actived </param>
        public void activeView(WEType type) {
            IWEPropView view = this.getView(type);
            if (view != null && this._activeView!=null && !view.Equals(this._activeView)) {
                this._activeView.hideView();
                
                view.showView();
                this._activeView = view;
            }
        }
        #region create we views
        private WEProcImageView createImageView() {
            WEProcImageView view = new WEProcImageView();
            view.Dock = DockStyle.Fill;
            return view;
        }

        private UserControl createColorView() {
            throw new NotImplementedException();
        }

        private UserControl createLocationView() {
            throw new NotImplementedException();
        }

        private UserControl createAttView() {
            throw new NotImplementedException();
        }

        private WEPropCodeView createCodeView() {
            WEPropCodeView view = new WEPropCodeView();
            view.Dock = DockStyle.Fill;
            return view;
        }
        #endregion
        /// <summary>
        /// clean active UI info, and prop bar
        /// </summary>
        internal void cleanActivePropView() {
            if (this.ActiveView != null) {
                this.ActiveView.resetView();
            }
        }
        public void setScriptRoot(ScriptRoot sroot) {
            this.sroot = sroot;
            // update properties view
            foreach (DictionaryEntry de in this._viewTable) {
                IWEPropView pv = de.Value as IWEPropView;
                pv.setScriptRoot(sroot);
            }
        }
    }
}
