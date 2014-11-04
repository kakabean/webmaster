using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.ide.ui;
using WebMaster.com.script;

namespace WebMaster.ide.editor.mapping
{
    public partial class MSWEAPanel : UserControl,IMappingSrc
    {
        #region variables 
        /// <summary>
        /// Mapping source type, this is the final type of the mapping source 
        /// </summary>
        private ParamType srcType = ParamType.STRING;
        /// <summary>
        /// Mapping source object, To be returned WebElementAttribute for the invoker. 
        /// </summary>
        private WebElementAttribute outputWEA = null;
        /// <summary>
        /// Root of script, it is used to list all WebElements. 
        /// </summary>
        private ScriptRoot sroot = null;
        #endregion variables
        #region events
        /// <summary>
        /// The sender is IMappingSrc panel, the data is mapping WebElementAttribute
        /// </summary>
        public event EventHandler<CommonEventArgs> MappingSrcChangedEvt;
        protected virtual void OnMappingSrcChangedEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> mappingSrcChangedEvt = MappingSrcChangedEvt;
            if (mappingSrcChangedEvt != null) {
                mappingSrcChangedEvt(this, e);
            }
        }
        /// <summary>
        /// The sender is IMappingSrc panel, the data is mapping WebElementAttribute
        /// </summary>
        /// <param name="sender">IMappingSrc panel</param>
        /// <param name="obj">WebElementAttribute</param>
        public void raiseMappingSrcChangedEvt(Object sender, object obj) {
            if (obj != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, obj);
                OnMappingSrcChangedEvt(evt);
            }
        }
        #endregion events
        public MSWEAPanel() {
            InitializeComponent();
            this.rtb_exp.ForeColor = Color.FromName(UIConstants.COLOR_MAPPING_EXP_TEXT);
        }
        #region mandatory methods 
        public string getExpression() {
            if (isValid()) {
                return ModelManager.Instance.getMappingSrcText(outputWEA);
            } else {
                return string.Empty;
            }
        }

        public string getValidMsg() {
            string msg = null;
            if(this.sroot == null){
                msg = UILangUtil.getMsg("mapping.src.wea.err.msg0");
            }else if (outputWEA == null) {
                msg = UILangUtil.getMsg("mapping.src.wea.err.msg1");
            } else {
                if (srcType == ParamType.STRING) {
                    msg = null;
                } else if (srcType == ParamType.NUMBER) {
                    bool mb = ModelManager.Instance.isMaybeNumberValue(outputWEA);
                    if (mb == true) {
                        msg = null;
                    } else {
                        msg = UILangUtil.getMsg("mapping.src.wea.err.msg2");
                    }                    
                } else {
                    msg = UILangUtil.getMsg("mapping.src.const.err.msg3");
                }
            }
            return msg;
        }

        public bool isValid() {
            return getValidMsg() == null; 
        }

        /// <summary>
        /// Mapping src should be deciaml(Number),string(String) or null if errors 
        /// </summary>
        /// <returns></returns>
        public object getMappingSrc() {
            if (isValid()) {
                return outputWEA;
            }
            return null;
        }
        /// <summary>
        /// Must set the cachedWElist before call this method 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcType"></param>
        public void show(object src, ParamType srcType) {            
            // update mapping source type info 
            updateSrcType(srcType);
            // update view with input 
            setInput(src);
            this.Visible = true;
        }

        public void hidden() {
            this.Visible = false;
        }
        #endregion mandatory methods 
        #region common methods
        /// <summary>
        /// set the scripts root WebElementGroup. Make sure user need to guarantee that 
        /// the selected WEA is available after the stub op/proc execution. 
        /// </summary>
        /// <param name="weroot">all WebElement root group</param>
        public void setSRoot(ScriptRoot sroot) {
            this.sroot = sroot;
        }
        private void setInput(object src) {
            // initialize and update outputWEA 
            WebElementAttribute inputWEA = null;
            if (src is WebElementAttribute) {
                inputWEA = src as WebElementAttribute;
            } else {
                inputWEA = null;
            } 
            this.outputWEA = createAndInitOutput(inputWEA);
            // clean the WEA tree
            this.tv_wea.Nodes.Clear();
            // build update the WEA tree 
            if (this.sroot == null) {
                return;
            } else {
                buildWEATree();
            }
            // update validation message 
            this.lb_msg.Text = getValidMsg();
            // update expression msg             
            this.rtb_exp.Text = getExpression();                        
            // update the WEA selected status 
            if (this.outputWEA != null) {
                setWEASelected(outputWEA);
            }
        }
        /// <summary>
        /// Create an WebElementAttribute and initialize it the same as the inputWEA
        /// </summary>
        private WebElementAttribute createAndInitOutput(WebElementAttribute inputWEA) {
            //WebElementAttribute wea = null;
            //if (inputWEA != null) {
            //    wea = inputWEA.Clone();
            //}
            //return wea;
            return inputWEA;
        }
        /// <summary>
        /// Set the WEA binded TreeNode selected if find 
        /// </summary>
        /// <param name="wea"></param>
        private void setWEASelected(WebElementAttribute wea) {
            if (wea != null) {
                TreeNode node = UIUtils.getTreeNodeByTag(this.tv_wea, wea);
                if (node != null) {
                    this.tv_wea.SelectedNode = node;
                }
            }
        }

        private void buildWEATree() {
            if (this.sroot != null) {
                this.tv_wea.BeginUpdate();
                string type = "string";
                if (srcType == ParamType.NUMBER) {
                    type = "number";
                }
                List<TreeNode> nodes = UIUtils.createWEGSubNodes(this.sroot.WERoot, true, type, UIConstants.IMG_WEG, UIConstants.IMG_WEG, UIConstants.IMG_WE, UIConstants.IMG_WE, UIConstants.IMG_WEA, UIConstants.IMG_WEA);
                this.tv_wea.Nodes.AddRange(nodes.ToArray());                
                // remove password WebElement 
                UIUtils.removeWEPassword(tv_wea);

                this.tv_wea.EndUpdate();
            }
        }

        private void updateSrcType(ParamType srcType) {
            if (srcType == ParamType.STRING || srcType == ParamType.NUMBER) {
                this.srcType = srcType;
            }
        }

        #endregion common methods   

        private void tv_wea_MouseDown(object sender, MouseEventArgs e) {
            TreeNode tn = this.tv_wea.GetNodeAt(new Point(e.X, e.Y));
            if (tn != null) {
                // update wea 
                if (tn.Tag is WebElementAttribute) {
                    this.outputWEA = tn.Tag as WebElementAttribute;
                } else {
                    this.outputWEA = null;
                }
                // update validation msg
                lb_msg.Text = getValidMsg();
                // update expression 
                rtb_exp.Text = getExpression();

                this.raiseMappingSrcChangedEvt(this, getMappingSrc());
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e) {
            ControlPaint.DrawBorder(e.Graphics, this.panel2.ClientRectangle, Color.FromName(UIConstants.COLOR_MAPPING_EXP_BORDER), ButtonBorderStyle.Solid);
        }
    }
}
