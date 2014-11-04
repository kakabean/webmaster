using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;

namespace WebMaster.com.script
{
    public partial class GlobalParamConfigView : UserControl
    {
        #region variables 
        /// <summary>
        /// Global parameter container 
        /// </summary>
        private ParamGroup paramRoot = null;
        // UI editor for parameters list 
        private TextBox ceTextBox = null;
        private Button ceButton = null;
        private Control[] lv_paramCE1 = null;
        private Control[] lv_paramCE2 = null;

        #endregion variables 
        #region events
        /// <summary>
        /// Sender is the view, data is ParamRoot
        /// </summary>
        public event EventHandler<CommonEventArgs> ConfigChanged;

        /// <summary>
        /// Raises the ConfigChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnConfigChanged(CommonEventArgs e) {
            if (ConfigChanged != null) {
                ConfigChanged(this, e);
            }
        }
        #endregion events 

        public GlobalParamConfigView() {
            InitializeComponent();
            initData();
        }

        private void initData() {
            // button cell editor
            this.ceButton = new Button();
            this.ceButton.Location = new System.Drawing.Point(32, 80);
            this.ceButton.Name = "ceButton";
            this.ceButton.Text = "...";
            this.ceButton.Size = new System.Drawing.Size(80, 21);
            this.ceButton.Visible = false;
            this.ceButton.Click += new EventHandler(ceButton_Click);

            // text box cell editor 
            this.ceTextBox = new TextBox();
            this.ceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ceTextBox.Location = new System.Drawing.Point(32, 104);
            this.ceTextBox.Multiline = true;
            this.ceTextBox.Name = "ceTextBox";
            this.ceTextBox.Size = new System.Drawing.Size(80, 16);
            this.ceTextBox.Visible = false;

            this.grpParamCfg.Controls.Add(ceTextBox);
            this.grpParamCfg.Controls.Add(ceButton);
            lv_paramCE1 = new Control[] { null, ceTextBox };
            lv_paramCE2 = new Control[] { null, ceButton };       
        }
        
        void ceButton_Click(object sender, EventArgs e) {
            if(lv_params.SelectedItems.Count == 0){
                return;
            }
            ListViewItem lvi = lv_params.SelectedItems[0];
            if (lvi.Tag is Parameter) {
                Parameter param = lvi.Tag as Parameter;
                if (param.Type == ParamType.SET) {
                    ParamSetEditDialog dlg = new ParamSetEditDialog();
                    DialogResult dr = dlg.showSetInputDialog(UIUtils.getTopControl(this), param, true);
                    if (dr == DialogResult.OK) {
                        List<object> items = dlg.SetItems;
                        if (param.DesignSet == null) {
                            param.DesignSet = new List<object>();
                        }
                        param.DesignSet.Clear();
                        param.DesignSet.AddRange(items);
                        OnConfigChanged(new CommonEventArgs(this, paramRoot));
                    }
                //} else if (param.Type == ParamType.STRING || param.Type == ParamType.NUMBER) {
                //    ParamConstValueEditDialog dlg = new ParamConstValueEditDialog();
                //    DialogResult dr = dlg.showValueDialog(UIUtils.getTopControl(this), param, param.Type);
                //    if (dr == DialogResult.OK) {
                //        object value = dlg.Value;
                //        param.DesignValue = value;
                //    }
                }

                this.lv_params.EndEditing(true);
            }            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proot">Script global parameter root</param>
        public void setInput(ParamGroup proot) {
            this.paramRoot = proot;
            updateParamListView();            
        }

        private void updateParamListView() {
            if (paramRoot == null) {
                cleanView();
                return;
            }
            lv_params.Items.Clear();
            lv_params.BeginUpdate();
            foreach (ParamGroup grp in paramRoot.SubGroups) {
                ListViewGroup lvg = new ListViewGroup();
                lvg.Header = grp.Name;
                lvg.Tag = grp;
                lv_params.Groups.Add(lvg);
                foreach (Parameter p in grp.Params) {
                    ListViewItem lvi = createListViewItem(p);                    
                    lvi.Group = lvg;
                    lv_params.Items.Add(lvi);
                }                
            }
            foreach (Parameter param in paramRoot.Params) {
                ListViewItem lvi = createListViewItem(param);
                lv_params.Items.Add(lvi);
            }
            lv_params.EndUpdate();
        }

        private void cleanView() {
            this.lv_params.Items.Clear();
        }

        private ListViewItem createListViewItem(Parameter param) {
            string name = param.Name;
            if (param.Sensitive) {
                name = "(*)" + name;
            }
            string value = param.DesignValue != null ? param.DesignValue.ToString() : string.Empty;
            string[] tts = new string[] { name, value };
            ListViewItem lvi = new ListViewItem(tts,-1);
            lvi.Tag = param;
            lvi.ToolTipText = param.Description;
            return lvi;
        }

        private void lv_params_Resize(object sender, EventArgs e) {
            int minW = 380;
            int minKey = 180;
            int wk = minKey;
            int wv = 200;
            int w = lv_params.ClientSize.Width-5;
            if (w > minW) {
                wk = (int)(w * 0.3);
                wv = (int)(w * 0.7);
            }
            colKey.Width = wk;
            colValue.Width = wv;
        }

        private void lv_params_SubItemClicked(object sender, SubItemEventArgs e) {
            if (isSetEditor(e.Item)) {
                Rectangle r = lv_params.GetSubItemBounds(e.Item, 1);
                Size size = new Size(24, -1);
                this.lv_params.setAdjustCellCtrl(1, size);
                List<int> list = new List<int>();
                list.Add(1);
                this.lv_params.setCellEditors(lv_paramCE2, list);
            } else {
                this.lv_params.setAdjustCellCtrl(-1, new Size(-1, -1));
                this.lv_params.setCellEditors(lv_paramCE1);
            }
            lv_params.StartEditing(e.SubItem, e.Item);            
        }

        private bool isSetEditor(ListViewItem item) {
            if (item != null && item.Tag is Parameter) {
                Parameter param = item.Tag as Parameter;
                if (param.Type == ParamType.SET) {
                    return true;
                }
            }
            return false;
        }

        private void lv_params_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e) {
            if (e.Item.Tag is Parameter) {
                Parameter param = e.Item.Tag as Parameter;
                // validate the value                 
                if (param.Type == ParamType.NUMBER) {
                    decimal value = ModelManager.Instance.getDecimal(this.ceTextBox.Text);
                    decimal v1 = ModelManager.Instance.getDecimal(param.DesignValue);
                    if (value != decimal.MinValue && value!=v1) {
                        param.DesignValue = value;
                        OnConfigChanged(new CommonEventArgs(this, paramRoot));
                    }
                } else if (param.Type == ParamType.SET) {
                    // parameter will be updated when the EditDialog closed. 
                } else if (param.Type == ParamType.STRING) {
                    if (param.DesignValue!=null || this.ceTextBox.Text != param.DesignValue.ToString()) {
                        param.DesignValue = this.ceTextBox.Text;
                        OnConfigChanged(new CommonEventArgs(this, paramRoot));
                    }
                }
                string txt = string.Empty;
                if (param.DesignValue != null) {
                    txt = param.DesignValue.ToString();
                }
                e.DisplayText = txt;
            }           
        }
    }
}
