using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;

namespace WebMaster.ide.editor.propview
{
    /// <summary>
    /// This class is used to show the input operations sibling operations 
    /// </summary>
    public partial class OpParamDialog : Form
    {
        private Operation input = null;

        private Operation output = null;

        public Operation Output {
            get { return output; }
            set { output = value; }
        }

        public OpParamDialog() {
            InitializeComponent();
        }

        public DialogResult showOpDialog(IWin32Window handler, Operation input){
            this.input = input;
            initOperationList();
            return ShowDialog(handler);
        }

        private void initOperationList() {
            tv_op.Nodes.Clear();
            List<Process> list = new List<Process>();
            Process owner = ModelManager.Instance.getOwnerProc(this.input);
            if (owner != null) {
                list.Add(owner);
            }
            if (this.input is Process) {
                list.Add(this.input as Process);
            }
            tv_op.BeginUpdate();
            foreach (Process pp in list) {
                TreeNode pnode = buildOpItem(pp);
                this.tv_op.Nodes.Add(pnode);
                // build up process
                foreach (Process proc in pp.Procs) {
                    TreeNode cnode = buildOpItem(proc);
                    pnode.Nodes.Add(cnode);
                }
                // build up operations
                foreach (Operation op in pp.Ops) {
                    TreeNode cnode = buildOpItem(op);
                    pnode.Nodes.Add(cnode);
                }    
            }
            
            tv_op.EndUpdate();
            tv_op.ExpandAll();
        }

        private TreeNode buildOpItem(Operation op) {
            TreeNode node = new TreeNode();
            node.Text = op.Name;
            node.Tag = op;
            int index = getImageIndex(op);
            if (index != -1) {
                node.ImageIndex = index;
                node.SelectedImageIndex = index;
            }
            return node;
        }

        private int getImageIndex(Operation op) {
            int index = -1;
            if (op.OpType == OPERATION.START) {
                index = 0;
            } else if (op.OpType == OPERATION.END) {
                index = 1;
            } else if (op.OpType == OPERATION.OPEN_URL_N_T || op.OpType == OPERATION.OPEN_URL_T) {
                index = 2;
            } else if (op.OpType == OPERATION.CLICK) {
                index = 3;
            } else if (op.OpType == OPERATION.INPUT) {
                index = 4;
            } else if (op.OpType == OPERATION.PROCESS) {
                index = 5;
            } else if (op.OpType == OPERATION.NOP) {
                index = 6;
            }
            return index; 
        }

        private void lv_op_MouseDown(object sender, MouseEventArgs e) {
            TreeNode tnode = tv_op.GetNodeAt(e.X, e.Y);
            if (tnode == null || tnode.Parent == null) {
                this.tv_op.SelectedNode = null;
                btn_OK.Enabled = false;
                this.Output = null;
                updateDetails(null);
            } else {
                this.tv_op.SelectedNode = tnode;
                updateDetails(tnode.Tag as Operation);
                btn_OK.Enabled = true;
                this.Output = tnode.Tag as Operation;
            }
        }

        private void updateDetails(Operation op) {
            if (op != null) {
                this.tb_name.Text = op.Name;
                this.tb_des.Text = op.Description;
            } else {
                this.tb_name.Text = string.Empty;
                this.tb_des.Text = string.Empty;
            }
        }

        private void lv_op_MouseDoubleClick(object sender, MouseEventArgs e) {
            lv_op_MouseDown(sender, e);

            this.btn_OK.PerformClick();
        }

    }
}
