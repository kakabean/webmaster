using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.lib.ui;

namespace WebMaster.ide.editor.propview
{
    public partial class ErrorLogPropView : UserControl, IPropView
    {
        // hash table main the error properties view info, the key is error element
        /// the value is error msg. 
        private HashtableEx table = null;

        public ErrorLogPropView() {
            InitializeComponent();
        }
        #region events
        /// <summary>
        /// raise the event for outer properties control that an invalid msg was double clicked
        /// the data is invalid element, e.g WebElement,WebElementGroup,Operation, Process, OpCondition
        /// </summary>
        public event EventHandler<CommonEventArgs> ValidationMsgClickedEvt;
        protected virtual void OnValidationMsgClickedEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> validationMsgClickedEvt = ValidationMsgClickedEvt;
            if (validationMsgClickedEvt != null) {
                validationMsgClickedEvt(this, e);
            }
        }
        /// <summary>
        /// raise the event for outer properties control that an invalid msg was double clicked
        /// the data is invalid element, e.g WebElement,WebElementGroup,Operation, Process, OpCondition
        /// 
        /// </summary>
        /// <param name="sender">error view </param>
        /// <param name="data"></param>
        public void raiseValidationMsgClickedEvt(Object sender, object data) {
            if (data != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, data);
                OnValidationMsgClickedEvt(evt);
            }
        }
        #endregion events 
        #region mandatory method 
        /// <summary>
        /// update error table view based on the new input
        /// </summary>
        public void updatedInput() {
            //1. clean table view 
            cleanView();
            //2. render table view for input 
            buildErrorTable(table);
        }
        /// <summary>
        /// setup input and update error table view 
        /// </summary>
        /// <param name="input"></param>
        public void setInput(object input) {
            // clean existed error model info and view 
            if (table != null) {
                this.listView1.Items.Clear();
                if (table != input) {
                    table.clear();
                }
            }            
            if (input is HashtableEx) {                
                table = input as HashtableEx;
                // build error table 
                buildErrorTable(table);
            }
        }

        public object getInput() {
            return table;
        }

        public void cleanView() {
            this.table.clear();
            this.listView1.Items.Clear();
        }

        public void showView() {
            this.Visible = true;
        }

        public void hideView() {
            this.Visible = false;
        }

        public void enableView() {
            this.listView1.Enabled = true;
        }

        public void disableView() {
            this.listView1.Enabled = false;
        }
        #endregion mandatory method 
        /// <summary>
        /// input must be WebElement, Process, Operation and OpCondition
        /// 1. if msg == string.EMPTY, if input in error view, remove
        /// 2. if msg != string.EMPTY, update error view if input contained.
        ///    else add a new error item.
        /// </summary>
        /// <param name="input">validated element</param>
        /// <param name="msg">validation msg with type info</param>
        internal void updateValidationMsg(BaseElement input, ValidationMsg msg) {
            if (input == null || msg == null || this.table == null) {
                return;
            }
            if (input is WebElement || input is Process || input is Operation || input is OpCondition) {
                bool find = false;
                foreach (ListViewItem lvi in this.listView1.Items) {
                    if (input == lvi.Tag) {
                        // 1. removed existed error info 
                        if (msg.Type == MsgType.VALID) {
                            this.listView1.Items.Remove(lvi);
                        } else { // update existed error info 
                            this.table.Add(input,msg);
                            lvi.SubItems[2].Text = msg.Msg;
                        }
                        find = true;
                        break;
                    }
                }
                if (!find) {
                    int index = getValidationMsgIconIndex(msg);
                    if (index != -1) {
                        table.Add(input, msg);
                        ListViewItem lvi = new ListViewItem(new string[] { "", input.Name, msg.Msg }, index);
                        lvi.Tag = input;
                        this.listView1.Items.Add(lvi);
                    }
                }
            }
        }
        /// <summary>
        /// return icon index align with the msg type, or -1 if errors 
        /// </summary>
        /// <param name="msg">validation msg with type info</param>
        /// <returns></returns>
        internal int getValidationMsgIconIndex(ValidationMsg msg) {
            if (msg == null) {
                return -1;
            }
            if (msg.Type == MsgType.ERROR) {
                return 0;
            } else if (msg.Type == MsgType.WARNING) {
                return 1;
            }
            return -1;
        }
        /// <summary>
        /// build up table properties view based on the table content. 
        /// </summary>
        /// <param name="table"></param>
        private void buildErrorTable(HashtableEx table) {
            listView1.BeginUpdate();
            foreach (object o in table) {
                if (o is ScriptRoot || o is WebElement || o is WebElementGroup || o is Process 
                    || o is Operation || o is OpCondition || o is Parameter || o is OperationRule || o is ParamCmd) { 
                    BaseElement be = o as BaseElement ;
                    ValidationMsg ov = table.Get(o) as ValidationMsg ;
                    string msg = ov == null ? null : ov.Msg ;
                    if(msg!=null){
                        string[] items = new string[]{"",be.Name,msg};
                        int imgIndex = 1 ;
                        if(ov.Type == MsgType.ERROR){
                            imgIndex = 1 ;
                        }else if(ov.Type == MsgType.WARNING){
                            imgIndex = 0 ;
                        }
                        ListViewItem lvi = new ListViewItem(items,imgIndex);
                        lvi.Tag = be;
                        this.listView1.Items.Add(lvi);
                    }
                }
            }
            listView1.EndUpdate();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e) {
            ListViewItem lvi = this.listView1.GetItemAt(e.X,e.Y);
            raiseValidationMsgClickedEvt(this, lvi.Tag);
        }

    }
}
