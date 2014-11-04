using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.lib.engine;
using System.Drawing;

namespace WebMaster.lib.engine
{
    public class ModelFactory
    {
        //public static readonly string EMPTY_NAME = "New Element";
        public static WebElement createWebElement() {
            WebElement we = new WebElement();
            we.Name = LangUtil.getMsg("model.WE.Name");
            we.X = WebElement.INVALID_VALUE;
            we.Y = WebElement.INVALID_VALUE;
            return we;
        }
        public static WebElementGroup createWebElementGroup() {
            WebElementGroup weg = new WebElementGroup();
            weg.Name = LangUtil.getMsg("model.WEGrp.Name");

            return weg;
        }
        public static WebElementAttribute createWebElementAttribute() {
            WebElementAttribute wea = new WebElementAttribute();
            return wea;
        }
        public static BigModel createBigModel() {
            BigModel model = new BigModel();
            ViewRoot vroot = createViewRoot();
            ScriptRoot sroot = createScriptRoot();
            
            // initialize the start node 
            Node snode = createNode();
            snode.RefOp = sroot.ProcRoot.StartOp;
            snode.Location = new Point(30,30) ;
            snode.Size = new Size(30,30);
            vroot.MainDiagram.Nodes.Add(snode);
            vroot.MainDiagram.Proc = sroot.ProcRoot;

            model.VRoot = vroot;
            model.SRoot = sroot;

            return model;
        }
        public static ViewRoot createViewRoot() {
            ViewRoot vroot = new ViewRoot();
            return vroot;
        }
        public static Diagram createDiagram() {
            Diagram diagram = new Diagram();
            diagram.Name = LangUtil.getMsg("model.Diagram.Name");
            return diagram;
        }
        public static ScriptRoot createScriptRoot() {            
            ScriptRoot root = new ScriptRoot();
            root.CreationTime = System.DateTime.Now.ToBinary();
            root.Name = LangUtil.getMsg("model.SRoot.Name");
            root.ModelVersion = "1.0.0.0";

            return root;
        }
        public static Operation createOperation(OPERATION type) {
            Operation op = new Operation();
            op.OpType = type;
            switch (type) { 
                case OPERATION.END:
                    op.Name = LangUtil.getMsg("OP_END");
                    op.WaitTime = "0";
                    break; 
                case OPERATION.INPUT:
                    op.Name = LangUtil.getMsg("OP_INPUT");
                    break;
                case OPERATION.CLICK:
                    op.Name = LangUtil.getMsg("OP_CLICK");
                    if (op.Click == null) {
                        op.Click = createClick();
                    }
                    break;
                case OPERATION.OPEN_URL_N_T:
                    op.Name = LangUtil.getMsg("OP_OPEN_URL_N_T");
                    break;
                case OPERATION.OPEN_URL_T:
                    op.Name = LangUtil.getMsg("OP_OPEN_URL_T");
                    break;
                case OPERATION.REFRESH:
                    op.Name = LangUtil.getMsg("OP_REFRESH");
                    break;
                case OPERATION.START:
                    op.Name = LangUtil.getMsg("OP_START");
                    op.WaitTime = "0";
                    break;
                case OPERATION.NOP:
                    op.Name = LangUtil.getMsg("OP_NOP");
                    break;
            }
            return op;
        }

        public static Click createClick() {
            Click click = new Click();
            return click;
        }

        public static Condition createCondtion() {
            Condition cu = new Condition();
            cu.Name = LangUtil.getMsg("model.Con.Name");
            return cu;
        }
        public static ConditionGroup createConditionGroup() {
            ConditionGroup grp = new ConditionGroup();
            grp.Name = LangUtil.getMsg("model.ConGrp.Name");
            grp.Relation = CONDITION.AND;
            return grp;
        }

        public static Node createNode() {
            Node node = new Node();
            node.Size = new Size(100,60);
            return node;
        }
        
        public static Connection createConnection(Point from, Point to) {
            Connection con = new Connection(from, to);
            return con;
        }

        public static Process createProcess() {
            Process proc = new Process();
            proc.OpType = OPERATION.PROCESS;
            proc.Name = LangUtil.getMsg("OP_PROCESS");

            return proc;
        }

        public static OpCondition createOpCondition() {
            OpCondition opc = new OpCondition();
            opc.Name = LangUtil.getMsg("model.Opc.Name");
            ConditionGroup cgrp = createConditionGroup();
            cgrp.Name = LangUtil.getMsg("model.Opc.ConGrp.Name");
            cgrp.Description = LangUtil.getMsg("model.Opc.ConGrp.Des");
            opc.ConditionGroup = cgrp;
            return opc;
        }

        public static ParamGroup createParamGroup() {
            ParamGroup grp = new ParamGroup();
            grp.Name = LangUtil.getMsg("model.ParamGrp.Name");
            return grp;
        }

        public static Parameter createParameter() {
            Parameter param = new Parameter();
            param.Name = LangUtil.getMsg("model.Param.Name");
            param.Type = ParamType.STRING;

            return param;
        }

        public static OperationRule createOperationRule() {
            OperationRule rule = new OperationRule();
            rule.Name = LangUtil.getMsg("model.Rule.Name");
            return rule;
        }

        public static UserLog createUserLog() {
            UserLog log = new UserLog();
            return log;
        }

        public static IHTMLElementWrap createIHTMLElementWrap() {
            IHTMLElementWrap wrap = new IHTMLElementWrap();
            return wrap;
        }

        public static ParamCmd createParamCmd() {
            ParamCmd cmd = new ParamCmd();            
            return cmd;
        }

        public static Expression createExpression() {
            Expression exp = new Expression();
            return exp;
        }

        public static GlobalFunction createGlobalFunction() {
            GlobalFunction gf = new GlobalFunction();
            return gf;
        }

        public static UserLogItem createUserLogItem(object item, int argb) {
            UserLogItem logItem = new UserLogItem();
            logItem.Item = item;
            logItem.Color = argb;
            return logItem;
        }
    }
}
