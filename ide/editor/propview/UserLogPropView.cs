using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.ide.editor.model;
using WebMaster.ide.ui;
using WebMaster.lib.engine;

namespace WebMaster.ide.editor.propview
{
    /// <summary>
    /// this properties view is used to show about Process and OpCondition
    /// </summary>
    public partial class UserLogPropView : UserControl, IPropView
    {
        /// <summary>
        /// acceptable input Process and Operation
        /// </summary>
        private BaseElement input = null;
        /// <summary>
        /// record which log view is currently shown
        /// </summary>
        private Panel activePanel = null;
        private FlowPropViewManager flowPVManager = null;

        public FlowPropViewManager FlowPVManager {
            get { return flowPVManager; }
            set { 
                flowPVManager = value;
                log_op.FlowPVManager = value;
                log_proc.FlowPVManager = value;                
            }
        }
        public UserLogPropView() {
            InitializeComponent();
            initData();
        }

        private void initData() {
            this.op_panel.Visible = true;
            this.op_panel.Dock = DockStyle.Fill;
            this.proc_panel.Visible = false;
            this.proc_panel.Dock = DockStyle.Fill;
            
            this.activePanel = op_panel;
        }
        #region madatory methods 
        public void updatedInput() {
            cleanView();
        }
        public object getInput() {
            return input;
        }       
        /// <summary>
        /// input must be Process and Operation.
        /// </summary>
        /// <param name="input"></param>
        public void setInput(Object input) {
            if (input is Process || input is Operation) {
                // return if input is not changed 
                if (this.input != null && this.input.Equals(input)) {
                    return;
                }
                this.input = input as Operation;
                if (input is Process) {
                    this.op_panel.Visible = false;
                    this.proc_panel.Visible = true;
                    this.activePanel = proc_panel;
                    this.enableView();

                    UserLogComponent logpv = this.getUserLog();
                    logpv.setInput(input as BaseElement, 1);
                    this.btn_end.BackColor = SystemColors.ActiveCaption;
                    this.btn_start.BackColor = SystemColors.Window;
                } else {
                    this.op_panel.Visible = true;
                    this.proc_panel.Visible = false;
                    this.activePanel = op_panel;

                    this.enableView();
                    UserLogComponent logpv = this.getUserLog();
                    logpv.setInput(input as BaseElement, 1);
                }

                showView();
            } else {
                this.input = null;
                this.disableView();
                this.cleanView();
            }
        }
        
        public void cleanView() {
            getUserLog().cleanView();
        }

        private UserLogComponent getUserLog() {
            if (activePanel == op_panel) {
                return log_op;
            } else {
                return log_proc;
            }
        }

        public void showView() {
            this.Visible = true;
        }

        public void hideView() {
            this.Visible = false;
        }

        public void enableView() {
            if (activePanel == proc_panel) {
                btn_start.Enabled = true;
                btn_end.Enabled = true;
            }
            getUserLog().enableView();
        }

        public void disableView() {
            if (activePanel == proc_panel) {
                btn_start.Enabled = false;
                btn_end.Enabled = false;
            }
            getUserLog().disableView();
        }
        #endregion mandatory method 

        private void btn_start_Click(object sender, EventArgs e) {
            this.btn_start.BackColor = SystemColors.ActiveCaption;
            this.btn_end.BackColor = SystemColors.Window;
            UserLogComponent logpv = this.getUserLog();
            logpv.setInput(input as BaseElement, 0);
        }

        private void btn_end_Click(object sender, EventArgs e) {
            this.btn_end.BackColor = SystemColors.ActiveCaption;
            this.btn_start.BackColor = SystemColors.Window;
            UserLogComponent logpv = this.getUserLog();
            logpv.setInput(input as BaseElement, 1);
        }

    }
}
