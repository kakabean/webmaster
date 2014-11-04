using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMaster.lib.engine
{
    /// <summary>
    /// This class is used to validate the model to make sure there is no
    /// dirty object or dirty references 
    /// </summary>
    internal class ModelValidation
    {
        private List<WebElement> WECollection = new List<WebElement>();
        private List<Process> ProcCollection = new List<Process>();
        private List<Operation> OPCollection = new List<Operation>();
        private List<Parameter> ParamCollection = new List<Parameter>();

        public void validateScriptRoot(ScriptRoot sroot) {
            buildSRootEntityCollection(sroot);
            validateSroot(sroot);
            validateWE();
            validateProcs();
            validateOps();
            cleanSRootEntityCollection();
        }
        /// <summary>
        /// Validate the operation it self and directly contained elements and properties 
        /// </summary>
        private void validateOps() {
            foreach (Operation op in OPCollection) {
                validateOp(op);
            }
        }
        /// <summary>
        /// Validate the operation containded list and properties info. 
        /// </summary>
        /// <param name="op"></param>
        private void validateOp(Operation op) {
            // 1. op.Commands
            foreach (ParamCmd cmd in op.Commands) {
                validateParamCmd(cmd);   
            }            
            // 2. op.Element;
            // 3. op.Input;
            // 4. op.LogStart;
            // 5. op.LogEnd;
            // 6. op.OpConditions;
            // 7. op.Rules;
            // 8. op.WeakRef;
        }

        private void validateParamCmd(ParamCmd cmd) {
            if (cmd != null) {
                // 1. cmd.Src;
                if (cmd.Src is BaseElement) {
                    BaseElement be = cmd.Src as BaseElement;
                    if (!isEntityInModel(be)) { 
                        
                    }
                }
                // 2. cmd.Target;
                // 3. cmd.WeakRef;
            }
        }
        /// <summary>
        /// Only validate the processes itself and directly owned properties, not touch the sub process/operations. 
        /// 
        /// </summary>
        private void validateProcs() {
            foreach (Process proc in ProcCollection) { 
                // 1. proc.ParamPrivate;
                // 2. proc.ParamPublic;
                // 3. proc.Procs;                
                // 4. validate operation info 
                validateOp(proc);
            }
        }
        /// <summary>
        /// 1. check WEA reference
        /// 2. validate refWebElement
        /// 3. validation weakRef
        /// </summary>
        private void validateWE() {
            foreach (WebElement we in WECollection) {
                // 1. check WEA reference
                foreach (WebElementAttribute wea in we.Attributes) {
                    validateWEA(wea);
                }
                // 2. validate refWebElement
                if (!isEntityInModel(we.refWebElement)) {
                    we.refWebElement = null;
                }
                // 3. validation weakRef
                doValidateBEWeakRef(we);
            }
        }
        /// <summary>
        /// validate the weakRef list, if the referenced BaseElement or list's stub BaseElement in 
        /// not a model entity, it will be removed. 
        /// A weakRef only can be BEList/ListRef/BaseElement
        /// </summary>
        /// <param name="be"></param>
        private void doValidateBEWeakRef(BaseElement be) {
            List<object> tmplist = new List<object>();
            foreach (object obj in be.WeakRef) {
                BaseElement entity = null;
                if (obj is ListRef) {
                    ListRef lref = obj as ListRef;
                    entity = lref.Stub;
                } else {
                    entity = ModelManager.Instance.getBEListOwner(obj);
                    if (entity == null && obj is BaseElement) {
                        entity = obj as BaseElement;
                    }

                }

                if (entity != null && !isEntityInModel(entity)) {
                    tmplist.Add(obj);
                }
            }
            // remove dirty references
            foreach (object obj in tmplist) {
                be.WeakRef.Remove(obj);
            }
            tmplist.Clear();
        }
        /// <summary>
        /// validate weakRef and parameters if have 
        /// </summary>
        /// <param name="wea"></param>
        private void validateWEA(WebElementAttribute wea) {
            // 1. validate parameter if have 
            List<object> tlist = new List<object>();
            foreach (object obj in wea.PValues) {
                if (obj is Parameter) {
                    Parameter p = obj as Parameter;
                    if (!ParamCollection.Contains(p)) {
                        tlist.Add(p);
                    }
                }
            }
            if (tlist.Count > 0) { 
                foreach(object o in tlist){
                    wea.PValues.Remove(o);
                }
            }
            // 2. validate weakRef
            tlist.Clear();
            foreach (object obj in wea.WeakRef) {
                BaseElement be = null;
                if (obj is ListRef) {
                    be = (obj as ListRef).Stub;                    
                } else if (obj is BEList<WebElementAttribute>) {
                    BEList<WebElementAttribute> list = obj as BEList<WebElementAttribute>;
                    be = list.Owner;
                }

                if (be!=null && !isEntityInModel(be)) {
                    tlist.Add(be);
                }
            }
            if (tlist.Count > 0) {
                foreach (object o in tlist) {
                    wea.WeakRef.Remove(o);
                }
            }
        }
        
        /// <summary>
        /// Make sure teh sroot object directly referenced object is valid. 
        /// </summary>
        /// <param name="sroot"></param>
        private void validateSroot(ScriptRoot sroot) {
            //TODO maybe check the referrenced scripts. 
        }

        private void cleanSRootEntityCollection() {
            this.WECollection.Clear();
            this.ProcCollection.Clear();
            this.OPCollection.Clear();
            this.ParamCollection.Clear();
        }

        private void buildSRootEntityCollection(ScriptRoot sroot) {
            buildWECollection(WECollection, sroot);
            buildProcCollection(ProcCollection, sroot);
            buildOPCollection(OPCollection, ProcCollection);
            buildParamCollection(ParamCollection, ProcCollection);
        }
        /// <summary>
        /// Add all script WebElements into WECollection 
        /// </summary>
        /// <param name="WECollection"></param>
        /// <param name="sroot"></param>
        private void buildWECollection(List<WebElement> WECollection, ScriptRoot sroot) {
            if (sroot != null && WECollection != null) {
                WECollection.AddRange(sroot.IFrames.ToArray());
                WECollection.AddRange(sroot.InternalRefWEs.ToArray());
                buildWECollection(WECollection, sroot.RawElemsGrp);
                buildWECollection(WECollection, sroot.WERoot);
            }
        }
        /// <summary>
        /// Add all WebElement under the Group into the collection 
        /// </summary>
        /// <param name="WECollection"></param>
        /// <param name="weg"></param>
        private void buildWECollection(List<WebElement> WECollection, WebElementGroup weg) {
            if (weg != null && WECollection!=null) {
                WECollection.AddRange(weg.Elems.ToArray());
                if (weg.SubGroups.Count > 0) {
                    foreach (WebElementGroup sweg in weg.SubGroups) {
                        buildWECollection(WECollection, sweg);
                    }
                }
            }
        }
        /// <summary>
        /// Add all processes into collection. 
        /// </summary>
        /// <param name="ProcCollection"></param>
        /// <param name="sroot"></param>
        private void buildProcCollection(List<Process> ProcCollection, ScriptRoot sroot) {
            if (ProcCollection != null && sroot != null) {
                buildProcCollection(ProcCollection, sroot.ProcRoot);
            }
        }
        /// <summary>
        /// Add proc and all its sub processes into collection. 
        /// </summary>
        /// <param name="ProcCollection"></param>
        /// <param name="proc"></param>
        private void buildProcCollection(List<Process> ProcCollection, Process proc) {
            if (ProcCollection != null && proc != null) {
                ProcCollection.Add(proc);
                if (proc.Procs.Count > 0) {
                    foreach (Process sproc in proc.Procs) {
                        buildProcCollection(ProcCollection, sproc);
                    }
                }
            }
        }

        /// <summary>
        /// Add all script Operations into collection. 
        /// </summary>
        /// <param name="OPCollection"></param>
        /// <param name="proclist">all script process list </param>
        private void buildOPCollection(List<Operation> OPCollection, List<Process> proclist) {
            if (proclist != null && OPCollection != null) {
                foreach (Process proc in proclist) {
                    OPCollection.AddRange(proc.Ops.ToArray());             
                }
            }
        }                
        /// <summary>
        /// Add all Parameters into the collection. 
        /// </summary>
        /// <param name="ParamCollection"></param>
        /// <param name="proclist">all script process list</param>
        private void buildParamCollection(List<Parameter> ParamCollection, List<Process> proclist) {
            if (ParamCollection != null && proclist != null) {
                foreach (Process proc in proclist) {
                    buildParamCollection(ParamCollection, proc.ParamPublic);
                    buildParamCollection(ParamCollection, proc.ParamPrivate);
                }
            }
        }
        /// <summary>
        /// add all Parameters under the paramGroup. 
        /// </summary>
        /// <param name="ParamCollection"></param>
        /// <param name="paramGroup"></param>
        private void buildParamCollection(List<Parameter> ParamCollection, ParamGroup paramGroup) {
            if (ParamCollection != null && paramGroup != null) {
                ParamCollection.AddRange(paramGroup.Params.ToArray());
                if (paramGroup.SubGroups.Count > 0) {
                    foreach (ParamGroup sgrp in paramGroup.SubGroups) {
                        buildParamCollection(ParamCollection, sgrp);                    
                    }
                }
            }
        }
        #region entity whether on model checking
        /// <summary>
        /// True: if the be is an entity on the model tree, false : it is an dirty object and not contained on the model
        /// </summary>
        /// <param name="be"></param>
        /// <returns></returns>
        private bool isEntityInModel(BaseElement be) {
            return true;
        }
        #endregion entity whether on model checking
    }
}
