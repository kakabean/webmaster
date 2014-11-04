using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.ide.editor.propview;
using WebMaster.com.script;
using WebMaster.ide;

namespace WebMaster.ide.editor.propview
{
    public partial class WEAPValuesEditDlg : Form
    {
        #region variables 
        private ScriptRoot sroot = null;

        private List<object> output = new List<object>();
        /// <summary>
        /// Output values items list, if it is not empty list, it means the input PValues has been updated. 
        /// </summary>
        public List<object> Output {
            get { return output; }
            set { output = value; }
        }
        /// <summary>
        /// 0: nothing to do 
        /// 1: updated by keyboard.
        /// 2: updated by code 
        /// </summary>
        private int tb_name_updateFlag = 0 ;

        #endregion variables 
        public WEAPValuesEditDlg() {
            InitializeComponent();
        }

        public DialogResult showEditDlg(IWin32Window handler, WebElementAttribute wea, ScriptRoot sroot) {
            if (wea == null || sroot == null) {
                disableView();
            } else {
                this.sroot = sroot;
                enableView();
                // update the output objects list as the wea pvalues 
                updateOutput(wea);
                setInput(wea);
            }
            return this.ShowDialog(handler);
        }

        private void setInput(WebElementAttribute wea) {
            if (wea == null) {
                return;
            }
            // update listview             
            updateListView();
            // show the current outputs objects display text 
            updateSummary();
            this.tb_item.Enabled = false;
        }

        #region common function 
        private void disableView() {
            this.lv_wea.Enabled = false;
            this.btn_OK.Enabled = false;
            this.btn_const.Enabled = false;
            this.btn_del.Enabled = false;
            this.btn_Obj.Enabled = false;
            this.tb_item.Enabled = false;
        }

        private void enableView() {
            this.lv_wea.Enabled = true;            
            this.btn_const.Enabled = true;
            this.btn_del.Enabled = false;
            this.btn_Obj.Enabled = true;
        }

        private void updateOutput(WebElementAttribute wea) {
            if (wea != null) {
                this.Output.Clear();
                this.Output.AddRange(wea.PValues.ToArray());
            }
        }

        private void updateItemText(ListViewItem lvi) {
            if (lvi.Tag is string) {
                this.tb_name_updateFlag = 2;
                this.tb_item.Text = lvi.Tag as string;
                this.tb_item.Enabled = true;
            } else if (lvi.Tag is Parameter) {
                Parameter param = lvi.Tag as Parameter;
                tb_item.Text = param.Name;
                this.tb_item.Enabled = false;
            }
        }

        /// <summary>
        /// Show the current outputs objects display text. 
        /// </summary>
        private void updateSummary() {
            string text = ModelManager.Instance.getWEAPValuesText(Output.ToArray());
            this.tb_value.Text = text;
        }
        /// <summary>
        /// update the pattern values with the output
        /// </summary>
        private void updateListView() {
            this.lv_wea.Items.Clear();
            if (Output == null) {
                return;
            }
            foreach (object obj in Output) {
                if (obj is string || obj is Parameter) {
                    ListViewItem lvi = createLVI(obj);
                    if (lvi != null) {
                        lv_wea.Items.Add(lvi);
                    }
                }
            }
        }

        private ListViewItem createLVI(object obj) {
            string imgkey = null;
            string txt = null;
            if (obj is string) {
                txt = obj.ToString();
                imgkey = "string16.png";
            } else if (obj is Parameter) {
                Parameter param = obj as Parameter;
                txt = param.Name;
                imgkey = "param16.gif";
            }
            if (txt != null) {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = txt;
                lvi.Tag = obj;
                lvi.ImageKey = imgkey;
                return lvi;
            }
            return null;
        }
        /// <summary>
        /// Get the first selected ListViewItem index or -1 if no selectedItem 
        /// </summary>
        /// <param name="lv_wea"></param>
        /// <returns></returns>
        private int getSelectedItemIndex(ListView lv) {
            if (lv != null) {
                if (lv.SelectedItems.Count > 0) {
                    return lv.Items.IndexOf(lv.SelectedItems[0]);
                }
            }
            return -1;
        }
        #endregion common functions 
        #region UI event handlers 
        private void lv_wea_MouseDown(object sender, MouseEventArgs e) {
            ListViewItem lvi = lv_wea.GetItemAt(e.X, e.Y);
            if (lvi != null) {
                updateItemText(lvi);
                if (lvi.Tag is string) {
                    tb_item.Enabled = true;
                } else if (lvi.Tag is Parameter) {
                    tb_item.Enabled = false;
                }
                this.btn_del.Enabled = true;
            } else {
                this.btn_del.Enabled = false;
                this.tb_name_updateFlag = 2;
                this.tb_item.Text = string.Empty;
                this.tb_item.Enabled = false;
            }
        }

        private void btn_Obj_Click(object sender, EventArgs e) {
            ParamListDialog dlg = new ParamListDialog();
            DialogResult dr = dlg.showParamDialog(this, "", this.sroot);
            if (dr == System.Windows.Forms.DialogResult.OK) {
                Parameter np = dlg.Output;
                int index = getSelectedItemIndex(lv_wea);
                if (index == -1) {
                    index = this.Output.Count;
                }
                if (index != -1) {
                    ListViewItem lvi = createLVI(np);
                    if (lvi != null) {
                        // update output
                        this.output.Insert(index, np);
                        // update list view 
                        lv_wea.Items.Insert(index, lvi);                        
                        UIUtils.selectListViewItem(lv_wea, lvi);
                        // update items area 
                        updateItemText(lvi);
                        // update summary
                        updateSummary();
                        // update btn
                        btn_OK.Enabled = true;
                    }
                }
            }
        }
        

        private void btn_const_Click(object sender, EventArgs e) {
            int index = getSelectedItemIndex(lv_wea);
            if (index == -1) {
                index = this.Output.Count;
            }
            if (index != -1) {
                string txt = UILangUtil.getMsg("dlg.wea.pvalue.text1"); // new string
                ListViewItem lvi = createLVI(txt);
                if (lvi != null) {
                    // update output
                    this.output.Insert(index, txt);
                    // update list view 
                    lv_wea.Items.Insert(index, lvi);
                    UIUtils.selectListViewItem(lv_wea, lvi);
                    // update items area 
                    updateItemText(lvi);
                    // update summary
                    updateSummary();
                    // update btn
                    btn_OK.Enabled = true;
                    tb_item.Enabled = true;
                }
            }
        }

        private void btn_del_Click(object sender, EventArgs e) {
            int index = getSelectedItemIndex(lv_wea);
            if (index != -1) {
                ListViewItem lvi = lv_wea.Items[index];
                if (lvi != null) {
                    // update output
                    this.output.Remove(lvi.Tag);
                    // update list view 
                    lv_wea.Items.Remove(lvi);
                    if (lv_wea.Items.Count > 0) {
                        UIUtils.selectListViewItem(lv_wea, 0);
                        // update items area 
                        updateItemText(lvi);
                    }
                    // update summary
                    updateSummary();
                    // update btn
                    if (this.tb_value.Text.Length > 0 && this.Output.Count > 0) {
                        btn_OK.Enabled = true;
                    } else {
                        btn_OK.Enabled = false;
                    }
                }
            }
        }

        private void tb_item_KeyDown(object sender, KeyEventArgs e) {
            this.tb_name_updateFlag = 1;
        }

        private void tb_item_TextChanged(object sender, EventArgs e) {
            if (tb_name_updateFlag != 1) {
                return;
            } else {
                tb_name_updateFlag = 0;
            }
            int index = getSelectedItemIndex(lv_wea);            
            if (index != -1) {
                ListViewItem lvi = lv_wea.Items[index];
                if (lvi != null && lvi.Tag is string) {
                    // update output
                    this.output.Insert(index, tb_item.Text);
                    this.output.Remove(lvi.Tag);
                    // update list view 
                    lvi.Tag = this.Output[index];
                    lvi.Text = lvi.Tag.ToString();
                    // update summary
                    updateSummary();
                    if (tb_value.Text.Length > 0) {
                        // update btn
                        btn_OK.Enabled = true;
                    } else {
                        btn_OK.Enabled = false;
                    }
                }
            }
        }

        #endregion UI event handlers 

    }
}
