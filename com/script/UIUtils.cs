using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using System.Drawing;
using System.Collections;
using WebMaster.lib;
using WebMaster.lib.gf;

namespace WebMaster.com.script
{
    public class UIUtils
    {
        #region ListView 
        /// <summary>
        /// set the listViewItem selected. 
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="itemIndex"></param>
        public static void selectListViewItem(ListView lv, int itemIndex) {
            if (lv != null && itemIndex >= 0 && itemIndex < lv.Items.Count) {
                ListViewItem lvi = lv.Items[itemIndex];
                lvi.EnsureVisible();
                lvi.Selected = true;
                lv.Focus();                
            }
        }
        /// <summary>
        /// set the listViewItem selected. 
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="item"></param>
        public static void selectListViewItem(ListView lv, ListViewItem item) {
            if (lv != null && item != null) {
                int index = lv.Items.IndexOf(item);
                selectListViewItem(lv, index);
            }
        }
        /// <summary>
        /// Get the first ListViewItem that matched tag or null if not found.
        /// Tag can be null. 
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static ListViewItem getListViewItemByTag(ListView lv, object tag) {
            ListViewItem lvi = null;
            if (lv != null /*&& tag != null*/) {
                foreach (ListViewItem item in lv.Items) {
                    if (item.Tag == tag) {
                        lvi = item;
                        break;
                    }
                }
            }
            return lvi;
        }
        #endregion ListView 
        #region TreeView common
        /// <summary>
        /// Get the first tree node in the tree with tag object or null if errors 
        /// </summary>
        /// <param name="tv"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static TreeNode getTreeNodeByTag(TreeView tv, object tag) {
            TreeNode node = null;
            if (tv != null && tag != null) {
                foreach (TreeNode tn in tv.Nodes) {
                    if (tag.Equals(tn.Tag)) {
                        node = tn;
                        break;
                    } else if (tn.Nodes != null && tn.Nodes.Count > 0) {
                        TreeNode n = getTreeNodeByTag(tn, tag);
                        if (n != null) {
                            node = n;
                            break;
                        }
                    }
                }
            }
            return node;
        }
        /// <summary>
        /// Get the first tree node under the node with tag object or null if errors. 
        /// </summary>
        /// <param name="pnode"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static TreeNode getTreeNodeByTag(TreeNode pnode, object tag) {
            TreeNode node = null;
            if (pnode != null && tag != null) {
                foreach (TreeNode tn in pnode.Nodes) {
                    if (tag.Equals(tn.Tag)) {
                        node = tn;
                        break;
                    } else if (tn.Nodes != null && tn.Nodes.Count > 0) {
                        TreeNode n = getTreeNodeByTag(tn, tag);
                        if (n != null) {
                            node = n;
                            break;
                        }
                    }
                }
            }
            return node;
        }
        /// <summary>
        /// get the children node levels of the node, if there is no children node, return 0 ;
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static int getNodeLevel(TreeNode node) {
            int level = 0;
            if (node.Nodes.Count > 0) {
                level++;
                int clevel = 0;
                foreach (TreeNode cnode in node.Nodes) {
                    int ll = getNodeLevel(cnode);
                    if (ll > clevel) {
                        clevel = ll;
                    }
                }
                level += clevel;
            }
            return level;
        }
        #endregion Tree view common
        #region WebElement
        /// <summary>
        /// Create a TreeNode list for all children(WebElementGroup/WebElement) of the parent weg. or return empty list if no children or errors. 
        /// Type will take effect only if showAttribute is true. If type == "string", only show the maybe string attribute, if type == "number",
        /// only show the maybe nubmer attribute, if type == "all", show all attribute. 
        /// </summary>
        /// <param name="parentWEG">Parent weg</param>
        /// <param name="showAttribute">Whether to create WebElementAttribute node</param>
        /// <param name="type">string, number, all</param>
        /// <param name="wegImgKey"></param>
        /// <param name="wegSelectedImgKey"></param>
        /// <param name="weImgKey"></param>
        /// <param name="weSelectedImgKey"></param>
        /// <param name="weaImgKey"></param>
        /// <param name="weaSelectedImgKey"></param>
        /// <returns></returns>
        public static List<TreeNode> createWEGSubNodes(WebElementGroup parentWEG, bool showAttribute, string type, string wegImgKey, string wegSelectedImgKey, string weImgKey, string weSelectedImgKey, string weaImgKey, string weaSelectedImgKey) {
            List<TreeNode> nodes = new List<TreeNode>();
            if (parentWEG == null) {
                return nodes;
            }
            if (parentWEG.SubGroups != null) {
                foreach (WebElementGroup weg in parentWEG.SubGroups) {
                    TreeNode gnode = createSingleWEGNode(weg,wegImgKey,wegSelectedImgKey);
                    nodes.Add(gnode);
                    // build children nodes
                    List<TreeNode> cnodes = createWEGSubNodes(weg, showAttribute, type, wegImgKey, wegSelectedImgKey, weImgKey, weSelectedImgKey, weaImgKey, weaSelectedImgKey);
                    gnode.Nodes.AddRange(cnodes.ToArray());
                }
            }
            if (parentWEG.Elems != null) {
                foreach (WebElement we in parentWEG.Elems) {
                    TreeNode wenode = createWENode(we, showAttribute, type, weImgKey, weSelectedImgKey, weaImgKey, weaSelectedImgKey);
                    nodes.Add(wenode);
                }
            }

            return nodes;
        }
        /// <summary>
        /// Create a only single TreeNode with weg, not create nodes for its children elements. 
        /// </summary>
        /// <param name="weg"></param>
        /// <param name="wegImgKey"></param>
        /// <param name="wegSelectedImgKey"></param>
        /// <returns></returns>
        public static TreeNode createSingleWEGNode(WebElementGroup weg, string wegImgKey, string wegSelectedImgKey) {            
            TreeNode node = new TreeNode();
            node.Text = weg.Name;
            node.ToolTipText = weg.Description;
            node.Tag = weg;
            node.ImageKey = wegImgKey;
            node.SelectedImageKey = wegSelectedImgKey;

            return node;
        }
        /// <summary>
        /// Create a TreeNode with the WebElement and define whether to create the attribute node. or return null if WE is null. 
        /// Type will take effect only if showAttribute is true. If type == "string", only show the maybe string attribute, if type == "number",
        /// only show the maybe nubmer attribute, if type == "all", show all attribute. 
        /// </summary>
        /// <param name="we"></param>
        /// <param name="showAttribute">whether show the attribute as node </param>
        /// <param name="type"></param>
        /// <param name="weImgKey">defined the WebElement image key</param>
        /// <param name="weSelectedImgKey"></param>
        /// <param name="weaImgKey"></param>
        /// <param name="weaSelectedImgKey"></param>
        /// <returns></returns>
        public static TreeNode createWENode(WebElement we, bool showAttribute, string type, string weImgKey, string weSelectedImgKey, string weaImgKey, string weaSelectedImgKey) {
            if (we == null) {
                return null;
            }
            TreeNode node = new TreeNode();
            node.Text = we.Name;
            node.ToolTipText = we.Description;
            node.Tag = we;
            node.ImageKey = weImgKey;
            node.SelectedImageKey = weSelectedImgKey;

            if (showAttribute) {
                if (we.TYPE == WEType.CODE) {                    
                    foreach (WebElementAttribute wea in we.Attributes) {
                        if (type !="number") {
                            if ("text".Equals(wea.Name)) {
                                TreeNode caNode = new TreeNode();
                                caNode.Text = "text";
                                caNode.ToolTipText = we.Description;
                                caNode.Tag = we.FeatureString;
                                caNode.ImageKey = weaImgKey;
                                caNode.SelectedImageKey = weaSelectedImgKey;
                                node.Nodes.Add(caNode);
                                break;
                            }
                        }
                    }
                } else if (we.TYPE == WEType.ATTRIBUTE) {
                    foreach (WebElementAttribute wea in we.Attributes) {
                        if (type !="number") {
                            TreeNode attNode = createWEATreeNode(wea, weaImgKey, weaSelectedImgKey);
                            node.Nodes.Add(attNode);
                        } else{
                            if (ModelManager.Instance.isMaybeNumberValue(wea)) {
                                TreeNode attNode = createWEATreeNode(wea, weaImgKey, weaSelectedImgKey);
                                node.Nodes.Add(attNode);
                            }
                        }
                    }
                }
            }

            return node;
        }
        /// <summary>
        /// Create a TreeNode with Tag wea or null if wea is null.
        /// </summary>
        /// <param name="wea"></param>
        /// <param name="imgKey"></param>
        /// <param name="selectedImgKey"></param>
        /// <returns></returns>
        public static TreeNode createWEATreeNode(WebElementAttribute wea, string imgKey, string selectedImgKey) {
            if (wea == null) {
                return null;
            }
            TreeNode attNode = new TreeNode();
            attNode.Text = wea.Name;
            attNode.ToolTipText = wea.Description;
            attNode.Tag = wea;
            attNode.ImageKey = imgKey;
            attNode.SelectedImageKey = selectedImgKey;

            return attNode;
        }
        #endregion WebElements 
        #region Parameters 
        /// <summary>
        /// Create a TreeNode list of all children(ParamGroup/Parameters) for the parent group Node, Not include the parent group node. 
        /// It will return an empty list if no children elements or errors.
        /// Type can be "string", "number", "set", "datetime", "all", default type is "all". 
        /// </summary>
        /// <param name="parentGrp"></param>
        /// <param name="type">valid value : "string", "number", "set","datetime", "all"</param>
        /// <param name="grpImgKey"></param>
        /// <param name="grpSelectedImgKey"></param>
        /// <param name="paramImgKey"></param>
        /// <param name="paramSelectedImgKey"></param>
        /// <returns></returns>        
        public static List<TreeNode> createParamGrpSubNodes(ParamGroup parentGrp, string type, string grpImgKey, string grpSelectedImgKey, string paramImgKey, string paramSelectedImgKey) {
            List<TreeNode> nodes = new List<TreeNode>();
            if (parentGrp == null) {
                return nodes;
            }
            if (parentGrp.SubGroups != null) {
                foreach (ParamGroup pgrp in parentGrp.SubGroups) {
                    TreeNode gnode = createSingleParamGrpNode(pgrp, grpImgKey, grpSelectedImgKey);
                    nodes.Add(gnode);
                    // build children nodes
                    List<TreeNode> cnodes = createParamGrpSubNodes(pgrp, type, grpImgKey, grpSelectedImgKey, paramImgKey, paramSelectedImgKey);
                    gnode.Nodes.AddRange(cnodes.ToArray());
                }
            }
            if (parentGrp.Params != null) {
                foreach (Parameter param in parentGrp.Params) {
                    if(type == null){
                        type = "all";
                    }
                    bool flag = false ;
                    if(type.Equals("string")){
                        if(ModelManager.Instance.isMaybeStringValue(param)){
                            flag = true ;
                        }
                    }else if(type.Equals("number")){
                        if(ModelManager.Instance.isMaybeNumberValue(param)){
                            flag = true ;
                        }
                    } else if (type.Equals("datetime")) {
                        if (param.Type == ParamType.DATETIME) {
                            flag = true;
                        }
                    } else if (type.Equals("set")) {
                        if (param.Type == ParamType.SET) {
                            flag = true;
                        }
                    } else {
                        flag = true;
                    }
                    if(flag == true){
                        TreeNode paramnode = createParamNode(param, paramImgKey, paramSelectedImgKey);
                        nodes.Add(paramnode);
                    }
                }
            }

            return nodes;
        }

        public static TreeNode createParamNode(Parameter param, string paramImgKey, string paramSelectedImgKey) {
            TreeNode node = new TreeNode();
            node.Text = param.Name;
            node.ToolTipText = param.Description;
            node.Tag = param;
            node.ImageKey = paramImgKey;
            node.SelectedImageKey = paramSelectedImgKey;

            return node;
        }
        /// <summary>
        /// Only create a single TreeNode for ParamGroup, not build its children nodes. 
        /// </summary>
        /// <param name="pgrp"></param>
        /// <param name="grpImgKey"></param>
        /// <param name="grpSelectedImgKey"></param>
        /// <returns></returns>
        public static TreeNode createSingleParamGrpNode(ParamGroup pgrp, string grpImgKey, string grpSelectedImgKey) {
            TreeNode node = new TreeNode();
            node.Text = pgrp.Name;
            node.ToolTipText = pgrp.Description;
            node.Tag = pgrp;
            node.ImageKey = grpImgKey;
            node.SelectedImageKey = grpSelectedImgKey;

            return node;
        }

        
        #endregion Parameters 
        #region Global Functions 
        /// <summary>
        /// Get a TreeNode list of the GF category text. only the filter matched node will be created. 
        /// </summary>
        /// <param name="filter">filter text</param>
        /// <param name="gfCategoryText"></param>
        /// <returns></returns>
        public static List<TreeNode> createGFCategorySubNodes(string filter, string gfCategoryText) {
            List<TreeNode> nodes = new List<TreeNode>();
            List<GF_CMD> cmds = null;
            if (GFManager.getGF_CategoryNameText(GFManager.GF_CATE_STR) == gfCategoryText) {
                cmds = GFManager.getStrGF_CMDList();
            } else if (GFManager.getGF_CategoryNameText(GFManager.GF_CATE_NUM) == gfCategoryText) {
                cmds = GFManager.getNumGF_CMDList();
            }
            if (cmds != null && cmds.Count > 0) {
                foreach (GF_CMD cmd in cmds) {
                    if (filter == null || filter.Trim().Length == 0 || GFManager.getGF_CMDNameText(cmd).Contains(filter)) {
                        TreeNode node = createGF_CMDNode(cmd);
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }

        private static TreeNode createGF_CMDNode(GF_CMD cmd) {
            TreeNode node = new TreeNode();
            node.Text = GFManager.getGF_CMDNameText(cmd);
            node.ToolTipText = GFManager.getGF_CMDDesText(cmd);
            node.Tag = cmd;
            node.ImageKey = UIConstants.IMG_GF_CMD;
            node.SelectedImageKey = UIConstants.IMG_GF_CMD;

            return node;
        }
        #endregion Global Functions 
        #region combo box ex
        /// <summary>
        /// Double confirm to update the combobox text or empty if errors 
        /// </summary>
        /// <param name="cb"></param>
        public static void updateComboBoxText(WebMaster.com.ComboBoxEx cb) {
            if (cb != null) {
                int sIndex = cb.SelectedIndex;
                if (sIndex == -1) {
                    cb.Text = Constants.BLANK_TEXT;
                } else {
                    cb.Text = cb.Items[sIndex].ToString();
                }
            }
        }    
        #endregion combo box 
        #region common ui
        /// <summary>
        /// Get the topest Control of the ctrl, or return ctrl itself if there is no container of ctrl.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static Control getTopControl(Control ctrl) {
            Control r = ctrl;
            while (r != null && r.Parent != null) {
                r = r.Parent;
                if (r is Form) {
                    break;
                }
            }
            return r;
        }
        #endregion common ui
        #region UserLog
        /// <summary>
        /// output application log info. 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="rtb"></param>
        public static void logMsg(string msg, RichTextBox rtb) {
            // this.richTextBox1.SelectionColor = Color.Black;
            string[] strs = msg.Split('\n');
            Color color = Color.Black;
            foreach (string str in strs) {
                string nstr = filterStartEndR(str);
                color = drawText(nstr, color, rtb);
                rtb.AppendText(Environment.NewLine);
            }
        }
        /// <summary>
        /// Filter the start and end '\r' if have 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string filterStartEndR(string str) {
            if (str.Length == 0) {
                return string.Empty;
            }
            int start = 0;
            if (str[0] == '\r') {
                if (str.Length > 1) {
                    start = 1;
                } else {
                    return string.Empty;
                }
            }
            int end = str.Length - 1;
            if (str[end] == '\r') {
                end = str.Length - 2;
            }
            if (start <= end) {
                int length = end - start + 1;
                return str.Substring(start, length);
            } else {
                return string.Empty;
            }
        }
        /// <summary>
        /// Draw a line of text, return the next candidate text color. 
        /// </summary>
        /// <param name="str"></param>
        private static Color drawText(string str, Color color, RichTextBox rtb) {
            if (str == null || str.Length == 0) {
                return color;
            }
            int start = str.IndexOf(Constants.STR_START_FLAG);
            // find a start flag in the string 
            if (start != -1) {
                // color first part text 
                if (start > 0) {
                    string text1 = str.Substring(0, start);
                    drawText(text1, color,rtb);
                    // update the next string as a new string 
                    str = str.Substring(start);
                    start = 0;
                }
                // get the color 
                int end = str.IndexOf(Constants.STR_END_FLAG, start);
                if (end != -1) {
                    string clr = str.Substring(start + 1, end - start - 1);
                    int argb = -16777216;//Color.Black;
                    try {
                        argb = int.Parse(clr);
                    } catch (Exception) { }
                    color = Color.FromArgb(argb);
                } else {
                    Log.println_eng("T = DEBUG, Log Message format error 1. msg = " + str);
                    end = start;
                }
                // get the colorred text 
                int tend = str.IndexOf(Constants.STR_SEPERATOR, end);
                if (tend != -1) {
                    string text = str.Substring(end + 1, tend - end - 1);
                    rtb.SelectionColor = color;
                    rtb.AppendText(text);
                    // start a new text draw. 
                    if (str.Length > tend + 1) {
                        string str1 = str.Substring(tend + 1);
                        return drawText(str1, Color.Black, rtb);
                    } else {
                        return Color.Black;
                    }
                } else {
                    rtb.SelectionColor = color;
                    rtb.AppendText(str.Substring(end + 1));
                    return color;
                }
            } else {
                int tend = str.IndexOf(Constants.STR_SEPERATOR);
                if (tend != -1) {
                    string text = str.Substring(0, tend);
                    rtb.SelectionColor = color;
                    rtb.AppendText(text);
                    // start a new text draw. 
                    if (str.Length > tend + 1) {
                        string str1 = str.Substring(tend + 1);
                        return drawText(str1, Color.Black, rtb);
                    } else {
                        return Color.Black;
                    }
                } else {
                    rtb.SelectionColor = color;
                    rtb.AppendText(str);
                    return color;
                }
            }
        }     
        #endregion UserLog        
        /// <summary>
        /// Remove the Password WebElement from the treeview if have
        /// </summary>
        /// <param name="treeView"></param>
        public static void removeWEPassword(TreeView treeView) {
            if (treeView != null) {
                List<TreeNode> rlist = new List<TreeNode>();
                foreach (TreeNode node in treeView.Nodes) {
                    if (node.Tag is WebElementGroup && node.Nodes.Count>0) {
                        removeWEPassword(node);
                    }
                    if (node.Tag is WebElement) {
                        WebElement we = node.Tag as WebElement;
                        if (we.isPassword) {
                            rlist.Add(node);                            
                        }
                    }
                }
                foreach (TreeNode tnode in rlist) {
                    tnode.Remove();
                }
                rlist.Clear();
            }
        }

        private static void removeWEPassword(TreeNode pnode) {
            if (pnode != null && pnode.Tag is WebElementGroup) {
                List<TreeNode> rlist = new List<TreeNode>();
                foreach (TreeNode node in pnode.Nodes) {
                    if (node.Tag is WebElementGroup && node.Nodes.Count > 0) {
                        removeWEPassword(node);
                    }
                    if (node.Tag is WebElement) {
                        WebElement we = node.Tag as WebElement;
                        if (we.isPassword) {
                            rlist.Add(node);
                        }
                    }
                }
                foreach (TreeNode tnode in rlist) {
                    tnode.Remove();
                }
                rlist.Clear();
            }
        }
        /// <summary>
        /// Remove sensitive parameter node from tree if have
        /// </summary>
        /// <param name="treeView"></param>
        public static void removeSensitiveParam(TreeView treeView) {
            if (treeView != null) {
                List<TreeNode> rlist = new List<TreeNode>();
                foreach (TreeNode node in treeView.Nodes) {
                    if (node.Tag is ParamGroup && node.Nodes.Count > 0) {
                        removeSensitiveParam(node);
                    }
                    if (node.Tag is Parameter) {
                        Parameter param = node.Tag as Parameter;
                        if (param.Sensitive) {
                            rlist.Add(node);
                        }
                    }
                }
                foreach (TreeNode tnode in rlist) {
                    tnode.Remove();
                }
                rlist.Clear();
            }
        }

        public static void removeSensitiveParam(TreeNode pnode) {
            if (pnode != null && pnode.Tag is ParamGroup) {
                List<TreeNode> rlist = new List<TreeNode>();
                foreach (TreeNode node in pnode.Nodes) {
                    if (node.Tag is ParamGroup && node.Nodes.Count > 0) {
                        removeSensitiveParam(node);
                    }
                    if (node.Tag is Parameter) {
                        Parameter param = node.Tag as Parameter;
                        if (param.Sensitive) {
                            rlist.Add(node);
                        }
                    }
                }
                foreach (TreeNode tnode in rlist) {
                    tnode.Remove();
                }
                rlist.Clear();
            }
        }
    }
}
