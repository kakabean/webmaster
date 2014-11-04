using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.lib.gf;

namespace WebMaster.lib.engine
{
    #region model entry definition
    /// <summary>
    /// It is the base class for all others element, each element has Collection list, it’s type is BEList<BaseElement>
    /// if the element is in a list, the Collection will be set to the BEList. A BEList has an owner field, which is 
    /// the container of the list. 
    /// </summary>
    [Serializable]
    public class BaseElement
    {
        private int _uid = -1;

        private string _description = null;

        private string _name = null;
        
        /// <summary>
        /// BaseElement name info, in each element hirachy leve, the name
        /// value should be unique
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private List<object> weakRef = new List<object>();
        /// <summary>
        /// NOTES: this is a common system List. 
        /// It is used to maintain the objects that referrenced this object. 
        /// It is used help to handle the element remove operation, to make sure
        /// all references are cleaned when move.
        /// The list Item will be 
        /// 1. ListRef/BEList, it means that this element was referrenced in that list. 
        /// 2. Another BaseElement object, this means that the one of its property referenced this element.
        /// </summary>
        public List<object> WeakRef {
            get { return weakRef; }
            //set { weakRef = value; }
        }

        // COMMENTS: I think a uid is usless to identify a element, name and hirachy structure is
        // enough, each level, all element's name must be unique
        /// <summary>
        /// a uid can unique identify an element, temporary it is not used now
        /// </summary>
        public int UID
        {
            get { return _uid; }
            private set { _uid = value; }
        }
        private List<string> _extFields = new List<string>();
        /// <summary>
        /// Extension fields for later potential usage or extensions
        /// </summary>
        public List<string> ExtFields {
            get { return _extFields; }
            //set { _extFields = value; }
        }
        public BaseElement()
        {
            //UID = System.Guid.NewGuid().ToString();
            //UID = this.GetHashCode();
        }        
    }
    [Serializable]
    public class WebElementAttribute : BaseElement
    {
        private string _key = null;
        /// <summary>
        /// Html elment attribute words. e.g style, onclick, title
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value/*.ToLower()*/;
            Name = _key;
            }
        }
        public WebElementAttribute() {
            this._pvalues.Stub = this;
        }
        /// <summary>
        /// VALUE used for the PATTERN to find the real element, The element only can be String or Parameter. 
        /// </summary>
        private ListRef _pvalues = new ListRef();
        /// <summary>
        /// Target PATTERN VALUE of the Key, it is only used to retrieve the 
        /// real pattern value at runtime in case of there are some parameters info
        /// combined with the pattern value. 
        /// 
        /// The element only can be String or Parameter. 
        /// 
        /// It is used in design time, in runtime or check the WebElement, it will be transfer to 
        /// RealPValue to use.
        /// </summary>
        public ListRef PValues
        {
            get { return _pvalues; }
            set { _pvalues = value; }
        }        
        /// <summary>
        /// Whether the Pattern value has parameters 
        /// </summary>
        public bool hasParameter {
            get {
                foreach (object obj in _pvalues) {
                    if (obj is Parameter) {
                        return true;
                    }
                }
                return false;
            }
        }

        [NonSerialized]
        private string _rpvalue = null;
        /// <summary>
        /// This is the real target PATTERN VALUE of the Key, it is only used to retrieve the 
        /// real Element.
        /// </summary>
        public string RealPValue {
            get { return _rpvalue; }
            set { _rpvalue = value; }
        }

        [NonSerialized]
        private String _rvalue = null;
        /// <summary>
        /// attribute runtime real value, it maybe updated based on the 
        /// runtime status, in a time, the real value respect the current
        /// attribute value. 
        /// </summary>
        public string RValue {
            get { return _rvalue; }
            set { _rvalue = value; }
        }
        /// <summary>
        /// which method will be used to verify the target VALUE
        /// </summary>
        private CONDITION _pattern = CONDITION.STR_FULLMATCH;
        /// <summary>
        /// which method will be used to verify the target value based on the applied string 
        /// pattern
        /// </summary>
        public CONDITION PATTERN
        {
            get { return _pattern; }
            set { _pattern = value; }
        }
        private BEList<WebElementAttribute> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<WebElementAttribute> Collection {
            get { return _collection; }
            set { _collection = value; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<WebElementAttribute>) {
                _collection = (BEList<WebElementAttribute>)belist;
            }
        }
        
        /// <summary>
        /// Clone the WebElementAttribute itself attirubtes, not inlcude the outer reference. 
        /// </summary>
        /// <returns></returns>
        public WebElementAttribute Clone() {
            WebElementAttribute wea = ModelFactory.createWebElementAttribute();
            wea.Description = this.Description;
            wea.Key = this.Key;
            wea.Name = this.Name;
            wea.ExtFields.AddRange(this.ExtFields);
            wea.PATTERN = this.PATTERN;
            wea.PValues.AddRange(this.PValues.ToArray());
            wea.RealPValue = this.RealPValue;
            wea.RValue = this.RValue;
            return wea;
        }
    }
    /// <summary>
    /// WebElement's group, it is a managed unit for WebElements' organization 
    /// </summary>
    [Serializable]
    public class WebElementGroup : BaseElement
    {
        private BEList<WebElement> _elems = new BEList<WebElement>();
        /// <summary>
        /// WebElements directly under the WebElementGroup
        /// </summary>
        public BEList<WebElement> Elems
        {
            get { return _elems; }
            //set { _elems = value; }
        }
        private BEList<WebElementGroup> _groups = new BEList<WebElementGroup>();
        /// <summary>
        /// sub WebElementGroup collection
        /// </summary>
        public BEList<WebElementGroup> SubGroups
        {
            get { return _groups; }
            //set { _groups = value; }
        }
        public WebElementGroup() {
            _elems.Owner = this;
            _groups.Owner = this;
        }
		private BEList<WebElementGroup> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<WebElementGroup> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<WebElementGroup>) {
                _collection = (BEList<WebElementGroup>)belist;
            }
        }
    }
    /// <summary>
    /// WebElement is one of Two object to maitain data in script, anothre is Parameter. 
    /// It is used to record/model the real target target WebElement, e.g traditional HTML Element or other e.g defined with location, color, or img. 
    /// so that the Engine to find the runtime real element to do other operations based on the definition.
    /// </summary>
    [Serializable]
    public class WebElement : BaseElement
    {
        //private string _id = null;
        /// <summary>
        /// it will take effect if the WEType is CODE or ATTRIBUTE, 
        /// the ID is HTMLElement id 
        /// </summary>
        public string ID
        {
            get {
                return getWEARealValue(Constants.HE_ID);
            }
            //set { _id = value; }
        }
        
        //private string _tag = null;
        /// <summary>
        /// tag is use the lower case 
        /// </summary>
        public string Tag
        {
            get {
                string tag = getWEARealValue(Constants.HE_TAG);
                return tag.ToLower();
            }
            //set { _tag = value.ToLower(); }
        }
        /// <summary>
        /// return this WebElement's WEA RealPattern value if have, or use the design Pattern value instead, 
        /// or return string.Empty if errors. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string getWEARealValue(string key) {
            WebElementAttribute wea = ModelManager.Instance.getWebElementAttributeByKeyName(this, key);
            if (wea == null) {
                return string.Empty;
            }
            if (wea.RealPValue != null) {
                return wea.RealPValue;
            } else {
                return ModelManager.Instance.tryGetWEARealPatternValue(wea, null);
            }
        }
        /// <summary>
        /// attributes list 
        /// </summary>
        private BEList<WebElementAttribute> _attrs = new BEList<WebElementAttribute>();
        /// <summary>
        /// WebElement attributes list, it is used if the WEType = CODE or ATTRIBUTE
        /// </summary>
        public BEList<WebElementAttribute> Attributes
        {
            get { return _attrs; }
            private set { _attrs = value; }
        }
        public WebElement() {
            this._attrs.Owner = this;
        }
        private string _featureKey = null;
        /// <summary>
        /// used for WEType == CODE, it is a feature string of the web element. 
        /// 
        /// a specific combined string to identify the WebElement. 
        /// Feature string = tag + id + name + gloable tag index + innerText
        /// like: {tag:input},{id:username},{name:username},{gIndex:177},{text:xxx}, each 
        /// part is blocked with {}
        /// </summary>
        public string FeatureString {
            get { return _featureKey; }
            set { _featureKey = value; }
        }
        private WebElement _refWE = null;
        /// <summary>
        /// A container element of the current webElement, if can be the (in)direct container of it.
        /// A refWebElement must be a HtmlElement modeled WebElement. 
        /// 
        /// the refWebElement should be a unique web element in its parent scope(e.g browser, or another container WebElement),
        /// If there are more than one element find matching refWebElement, just the first one is valid to use. 
        /// 
        /// 1. If WEType is CODE and ATTRIBUTE, the refWE can be iframe, or outer conatiner 
        /// 2. for the the LOCATION, COLOR and Image type, the refWebElement should be flash HtmlElement. ususally is object tag.
        /// 
        /// It used to locate the element, because there maybe some same elements by attributes, 
        /// so use the container to differenciate them. 
        /// </summary>
        public WebElement refWebElement
        {
            get { return _refWE; }
            set {
                if (TYPE == WEType.CODE || TYPE == WEType.ATTRIBUTE) {
                    ModelManager.Instance.updateWeakRef_AssginValue(_refWE, value, this);
                    _refWE = value;                    
                }
            }
        }
        private Bitmap image = null;
        /// <summary>
        /// take effect if the WEType=IMAGE
        /// </summary>
        public Bitmap Image {
            get { return image; }
            set { image = value; }
        }
        private float similarity = 0.9f;
        /// <summary>
        /// take effect if the WEType=IMAGE
        /// identify how similarity the user region marked with the 
        /// target image.
        /// </summary>
        public float Similarity {
            get { return similarity; }
            set { similarity = value; }
        }
        private Object cursor = null;
        /// <summary>
        /// save the cursor's info when it above the WebElement
        /// It is used only for the flash condition, means that if the WEType == LOCATION,COLOR,IMAGE
        /// it will take effect. 
        /// </summary>
        public Object Cursor {
            get { return cursor; }
            set { cursor = value;}            
        }
        private Color color;
        /// <summary>
        /// take effect if the WEType==COLOR
        /// check if the (X,Y) location is the color
        /// when isRegionColor is true, if the color
        /// contains in the rectangle (x,y,width,height), it means the WebElement found
        /// </summary>
        public Color Color {
            get { return color; }
            set { color = value; }
        }
        #region size
        public static int INVALID_VALUE = int.MinValue;
                
        private int _x = INVALID_VALUE;
        /// <summary>
        /// Take effect only TYPE = LOCATOIN, COLOR, IMAGE
        /// 
        /// Location info. e.g if type = WEType.COLOR, the x.y point is the color point
        /// in the browser area. if refWebElement's is not null, X coordinate relative with the refWebElement's top-left 
        /// x bigger than 0 means the element is on the right side of the refOne's rightest border, while
        /// x less than 0 means the element is on the left side of the refOne,
        /// </summary>
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }
        private int _y = INVALID_VALUE;
        /// <summary>
        /// ref X 
        /// </summary>
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }
        private int width = INVALID_VALUE;
        /// <summary>
        /// Width and height used to define a region. if will be reasonable just if 
        /// they both are not INVALIDE_VALUE and the cursor are set.
        /// 
        /// The scenario is that when the user mouse enter the rectangle (x,y,width,height), if usr cursor equal Cursor
        /// the WebElement will be find, so that operation can be applied on the WebElement.
        /// </summary>
        public int Width {
            get { return width; }
            set { width = value; }
        }
        private int height = INVALID_VALUE;
        /// <summary>
        /// ref Width comments
        /// </summary>
        public int Height {
            get { return height; }
            set { height = value; }
        }
        #endregion size
        private WEType _type = WEType.ATTRIBUTE;
        /// <summary>
        /// Here we support four way to locate the WebElement
        /// 0. by code 
        /// 1. by attributes
        /// 2. by location, absolute to the browser window or relative another WebElement
        /// 3. by a region of image
        /// 4. by color on some points 
        /// </summary>
        public WEType TYPE
        {
            get { return _type; }
            set { _type = value; }
        }
        [NonSerialized]
        private bool isRealElement = false;
        /// <summary>
        /// if the TYPE = CODE or ATTIBUTE, it means that whether the WebElement is updated by the real HtmlElement value 
        /// if the type is others, it means that whether the WebElement existed in browser at runtime 
        /// </summary>
        public bool IsRealElement {
            get { return isRealElement; }
            set { isRealElement = value; }
        }
        private bool isPwd = false;
        /// <summary>
        /// Whether the WebElement is a password WebElement, A password WE only can be used as a click Operation Element.
        /// And can be used anymore. 
        /// </summary>
        public bool isPassword {
            get { return isPwd; }
            set { isPwd = value; }
        }
        /// <summary>
        /// Whether the WebElement is marked with parameter, if it is marked with parameter it only can be identified 
        /// at executing time, in design time, it can not be checked existed or not. 
        /// </summary>
        public bool hasParameter {
            get {
                foreach (WebElementAttribute wea in this.Attributes) {
                    if (wea.hasParameter) {
                        return true;
                    }
                }
                return false;
            }
        }
        private BEList<WebElement> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<WebElement> Collection {
            get { return _collection; }
        }
		 /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<WebElement>) {
                _collection = (BEList<WebElement>)belist;
            }
        }
        /// <summary>
        /// Clone all the internal properties and refWebElement, not include the Collection. 
        /// </summary>
        /// <returns></returns>
        public WebElement Clone() {
            WebElement we = ModelFactory.createWebElement();
            foreach (WebElementAttribute wea in this.Attributes) {
                WebElementAttribute nwea = wea.Clone();
                we.Attributes.AddUnique(nwea);
            }
            we.Color = this.Color;
            we.Cursor = this.Cursor;
            we.Description = this.Description;
            we.ExtFields.AddRange(this.ExtFields);
            we.FeatureString = this.FeatureString;
            we.Height = this.Height;
            //we.ID = this.ID;
            we.Image = this.Image;
            we.IsRealElement = this.IsRealElement;
            we.Name = this.Name;
            we.refWebElement = this.refWebElement;
            we.Similarity = this.Similarity;
            //we.Tag = this.Tag;
            we.TYPE = this.TYPE;
            we.Width = this.Width;
            we.X = this.X;
            we.Y = this.Y;

            return we;
        }
        /// <summary>
        /// return true if the we is modeled the same element with current we or false if not. 
        /// </summary>
        /// <param name="we"></param>
        /// <returns></returns>
        public bool isEqual(WebElement we) {
            if (we == null) {
                return false;
            }
            // check type == ATTRIBUTE 
            if (we.TYPE == WEType.ATTRIBUTE || we.TYPE == WEType.CODE) {
                return ModelManager.Instance.isSameHTMLElementByATT(this, we);
            }
            // TODO ... 
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n--------------------------------------------------------------\n");
            sb.Append("[WebElement]:");
            sb.Append("\n  name: " + Name);
            sb.Append("\n  description: " + Description);
            sb.Append("\n  TYPE: " + TYPE);
            sb.Append("\n  tag : " + Tag);
            sb.Append("\n  id  : " + ID);            
            string cname = null;
            if (refWebElement != null)
            {
                cname = refWebElement.Name;
            }
            sb.Append("\n  container name:" + cname);
            sb.Append("\n  Attriubtes: ");
            foreach (BaseElement be in Attributes){
                WebElementAttribute wea = be as WebElementAttribute;
                sb.Append("\n    pattern:" + "key: " + wea.Key + ", value: " + ModelManager.Instance.getWEAText4Design(wea));
            }
            sb.Append("--------------------------------------------------------------\n");
            if (refWebElement != null) {
                sb.Append("######### ref WebElement, name = ").Append(cname).Append(" , start #################\n");
                sb.Append(refWebElement.ToString());
                sb.Append("######### ref WebElement, name = ").Append(cname).Append(" , end #################\n"); 
            }

            return sb.ToString();
        }
    }
    /// <summary>
    /// basic operation for WebElement
    /// When an operation begin to run, follow the steps
    /// 1. check and apply the rules, the default mandatory rule is that if the 
    ///    WebElement doesn't existed, stop operatoin execute
    /// 2. perform the operation
    /// 3. Wait time 
    /// 4. log operation
    /// 5. check the operation conditions one by one, if find a true condition, route the control to the operation
    ///    if none op condition matched, the op will be stopped here
    /// </summary>
    [Serializable]
    public class Operation : BaseElement
    {
        /// <summary>
        /// input can be Parameter, string. 
        /// </summary>
        private object _input = null;
        // If the operation type is click, this will take effect. 
        private Click _click = null;
        /// <summary>
        /// Condition root defined for the Operations 
        /// </summary>
        private BEList<OpCondition> _opCons = new BEList<OpCondition>();
        /// <summary>
        /// An operation can has more than one OpConditions, each OpCondition will route the logic to 
        /// different code
        /// It will check the OpConditions one by one, if a value is true, route the control to relative op
        /// if none op conditions matched, it will go the next operation
        /// </summary>
        public BEList<OpCondition> OpConditions {
            get { return _opCons; }
            set { _opCons = value; }
        }
        /// <summary>
        /// operated Web Element 
        /// </summary>
        private WebElement _element = null;
        /// <summary>
        /// Operated Web Element 
        /// </summary>
        public WebElement Element
        {
            get { return _element; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(_element, value, this);
                _element = value;            
            }
        }
        /// <summary>
        /// Input must be Parameter or string. 
        /// </summary>
        public object Input
        {
            get { return _input; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(_input, value, this);
                _input = value;            
            }
        }
        private string exeTime = null;
        /// <summary>
        /// Operation execute time, if the current time is bigger than exeute time, operation 
        /// will executed, else it will wait until time reached if there is no time control rule.
        /// format is :
        /// HH:mm:ss, HH: 0-23, mm:0-59, ss :0-59
        /// </summary>
        public string ExeuteTime { 
            get { return exeTime; }
            set {
                if (ModelManager.Instance.isValidTime(value)) {
                    exeTime = value;
                }
            }
        }
        /// <summary>
        /// If the operation type is CLICK, this will take effect. 
        /// </summary>
        public Click Click {
            get { return _click; }
            set { _click = value; }
        }
        /// <summary>
        /// time is milli-seconds, default value is 100 mill-seconds
        /// </summary>
        private string _waitTime = "100";
        /// <summary>
        /// This property is used to describe how many seconds to wait after current operation.
        /// it is definded in milli-seconds, default value is 60 seconds 
        /// accepted value is 
        ///  a. number string ,e.g 20, 80
        ///  b. a range, format [number start] - [number end]. e.g 10 - 20, 12-50
        /// </summary>
        public string WaitTime
        {
            get { return _waitTime; }
            set { _waitTime = value; }
        }
        /// <summary>
        /// operation type, e.g click, or input 
        /// </summary>
        private OPERATION _type = OPERATION.NOP;
        /// <summary>
        /// operation type, e.g click, or input, defined in OPERATION class  
        /// </summary>
        public OPERATION OpType
        {
            get { return _type; }
            set { _type = value; }
        }
        private UserLog logStart = null;
        /// <summary>
        /// It is used to guide how to log the user log info with details before the operation start.
        /// For a LogStart, it is taked effect only on the Operation/Process itself, not spread to the children operation/process.
        /// 
        /// An Operation/Process must define a user log if needed, no default user log provided. 
        /// </summary>
        public UserLog LogStart {
            get { return logStart; }
            set {
                UserLog dirtyLog = logStart;
                ModelManager.Instance.updateWeakRef_AssginValue(logStart, value, this);
                logStart = value;
                ModelManager.Instance.removeFromModel(dirtyLog, this);
            }
        }
        private UserLog logEnd = null;
        /// <summary>
        /// It is used to guide how to log the user log info with details when operation finished.
        /// For a LogData, it is taked effect only on the Operation/Process itself, not spread to the children operation/process.
        /// 
        /// An Operation/Process must define a user log if needed, no default user log provided. 
        /// </summary>
        public UserLog LogEnd {
            get { return logEnd; }
            set {
                UserLog dirtyLog = logEnd;
                ModelManager.Instance.updateWeakRef_AssginValue(logEnd, value, this);
                logEnd = value;
                ModelManager.Instance.removeFromModel(dirtyLog,this);                
            }
        }
        private BEList<OperationRule> _rules = new BEList<OperationRule>();
        /// <summary>
        /// applied rules list, item is OperationRule
        /// When an operation begin to run, follow the steps
        /// 1. check and apply rules
        /// 2. perform the operation
        /// 3. Wait time 
        /// 4. log operation
        /// 5. check exception rule whether need to do action. 
        /// 6. check the operation conditions one by one, if find a true condition, route the control to the operation
        ///    if none op condition matched, the op will be stopped here
        /// </summary>
        public BEList<OperationRule> Rules
        {
            get { return _rules; }
            //set {
            //    _rules = value;
            //    if (_rules != null) {
            //        _rules.Owner = this;
            //    }
            //}
        }

        private BEList<ParamCmd> updateParams = new BEList<ParamCmd>();
        /// <summary>
        /// Element is ParamCmd. 
        /// This is used to updated some parameters value if needed after the Operation/Process finished.
        /// 
        /// The target parameter is operation accessable parameter(Include process parameter and global parameter). 
        /// source can be Constants, WebElementAttriute, Parameter, Expression and GlobalFunction or others, please ref
        /// ParamCmd. 
        /// </summary>
        public BEList<ParamCmd> Commands {
            get { return updateParams; }
            //set { 
            //    updateParams = value;
            //    if (updateParams != null) {
            //        updateParams.Owner = this;
            //    }
            //}
        }
        public Operation() {
            this._opCons.Owner = this;
            this._rules.Owner = this;
            this.updateParams.Owner = this;
        }
        /// <summary>
        /// get the wait time values, if it is a fixed value, it will return a array with the time value like [100]
        /// if it is a range value, it will return a int array with 2 values like[n1,n2], the first one is start value
        /// and the second one is the end value. if there are errors for 2 values, it will return [50,100]
        /// if it is a invalid value it will return 1 as defect. 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public int[] getWaitTimes() {
            if (this.WaitTime == null || this.WaitTime.Trim().Length < 1) {
                throw new InvalidCastException("Operation "+this.Name+" wait time syntext error");
            }
            if (WaitTime.Contains("..")) {
                int index = WaitTime.IndexOf("..");
                int n1 = 50;
                int n2 = 100;
                try {
                    string s1 = WaitTime.Substring(0, index);
                    n1 = Convert.ToInt32(s1.Trim());
                    string s2 = WaitTime.Substring(index + 2, WaitTime.Length - index - 2);
                    n2 = Convert.ToInt32(s2.Trim());
                } catch (Exception e) {
                    throw new InvalidCastException("Operation " + this.Name + " wait time syntext error, e = "+e.Message);
                }
                if (n1 > n2) {
                    int t = n1;
                    n1 = n2;
                    n2 = t;
                }
                return new int[] { n1, n2 };
            } else {
                int n1 = 100;
                try {
                    n1 = Convert.ToInt32(WaitTime.Trim());
                } catch (Exception e) {
                    throw new InvalidCastException("Operation " + this.Name + " wait time syntext error. "+e.Message);
                }
                return new int[]{n1};
            }
        }
        /// <summary>
        /// get the wait time value, if the value is fixed, return the value itself
        /// if the value is a range, it will random return a value in the range. 
        /// default value is 1000*60 milli-seconds if some errors 
        /// </summary>
        /// <returns></returns>
        public int getWaitTime() {
            int[] ts = null;
            try {
                ts = getWaitTimes();
            } catch (InvalidCastException) {
                return 1000 * 60;
            }
            if (ts == null) {
                return 1000 * 60;
            }else if (ts.Length == 1) {
                return ts[0];
            } else if (ts.Length == 2) {
                Random rand = new Random(this.GetHashCode());
                return rand.Next(ts[0], ts[1]);
            }

            return 1000*60;
        }
		private BEList<Operation> _collection = null;
        /// <summary>
        /// container BEList of the current element. Warning that if use the Collection of a 
        /// unknow type op or process, make sure that check the process's Collection first, else it 
        /// will always return Operation's Collection as null some times
        /// </summary>
        public BEList<Operation> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<Operation>) {
                _collection = (BEList<Operation>)belist;
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n--------------------------------------------------------------\n");
            sb.Append("[Operation]:");
            sb.Append("\n  name: " + Name);
            sb.Append("\n  description: " + Description);
            sb.Append("\n  TYPE: " + OpType);
            string ename = Element != null ? Element.Name : null;
            sb.Append("\n  Element : " + ename);
            sb.Append("\n  Value : " + Input);
            sb.Append("\n  Wait time : " + WaitTime);
            sb.Append("--------------------------------------------------------------\n");

            return sb.ToString();
        }
    }
    /// <summary>
    /// class for loop operation 
    /// </summary>    
    [Serializable]
    public class Process : Operation
    {
        /// <summary>
        /// first Operation Element of the process
        /// </summary>
        private Operation _startNode = null;
        /// <summary>
        /// first Operation Element of the process
        /// </summary>
        public Operation StartOp
        {
            get { return _startNode; }
        }
        private ParamGroup paramPublic = null;
        /// <summary>
        /// Root of public parameter container, public parameters can be seen be out invoke process
        /// it is used for outer process to transfer values to invoked process. 
        /// </summary>
        public ParamGroup ParamPublic {
            get { return paramPublic; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(paramPublic, value, this);
                paramPublic = value;            
            }
        }
        private ParamGroup paramPrivate = null;
        /// <summary>
        /// Root of private parameter container
        /// </summary>
        public ParamGroup ParamPrivate {
            get { return paramPrivate; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(paramPrivate, value, this);
                paramPrivate = value;            
            }
        }
        private BEList<Operation> _ops = new BEList<Operation>();
        /// <summary>
        /// all operations defined in the process.
        /// </summary>
        public BEList<Operation> Ops {
            get { return _ops;}
        }
        private BEList<Process> _procs = new BEList<Process>();
        /// <summary>
        /// all Processes defined in the process 
        /// </summary>
        public BEList<Process> Procs {
            get { return _procs; }
        }

        public Process() {            
            this._ops.Owner = this;
            this._procs.Owner = this;

            this.ParamPublic = ModelFactory.createParamGroup();
            this.ParamPublic.Name = LangUtil.getMsg("model.Proc.PubParam.Name");
            this.ParamPublic.Description = LangUtil.getMsg("model.Proc.PubParam.Des"); 

            this.ParamPrivate = ModelFactory.createParamGroup();
            this.ParamPrivate.Name = LangUtil.getMsg("model.Proc.PriParam.Name");
            this.ParamPrivate.Description = LangUtil.getMsg("model.Proc.PriParam.Des");

            Operation sop = ModelFactory.createOperation(OPERATION.START);
            _startNode = sop;
            Ops.AddUnique(StartOp);
        }
		private BEList<Process> _collection = null;
        /// <summary>
        /// container BEList of the current element. Warning that if use the Collection of a 
        /// unknow type op or process, make sure that check the process's Collection first, else it 
        /// will always return Operation's Collection as null some times
        /// </summary>
        public new BEList<Process> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public new void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<Process>) {
                _collection = (BEList<Process>)belist;
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n--------------------------------------------------------------\n");
            sb.Append("[Process]:");
            sb.Append("\n  name: " + Name);
            sb.Append("\n  description: " + Description);
            sb.Append("\n  TYPE: " + OpType.ToString());
            string ename = Element != null ? Element.Name : null;
            sb.Append("\n  Element : " + ename);
            string sop = StartOp != null ? StartOp.Name : null;
            sb.Append("\n  Start Operation  : " + sop);
            sb.Append("\n  Wait time : " + WaitTime);
            sb.Append("--------------------------------------------------------------\n");

            return sb.ToString();
        }
    }
    /// <summary>
    /// a condition of Operation
    /// </summary>
    [Serializable]
    public class OpCondition : BaseElement
    {
        private ConditionGroup conGrp = null;
        /// <summary>
        /// root of all the conditions/conditionGroups
        /// </summary>
        public ConditionGroup ConditionGroup {
            get { return conGrp; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(conGrp, value, this);
                conGrp = value;            
            }
        }
        private Operation op = null;
        /// <summary>
        /// if the condition group is true, Op will be executed
        /// </summary>
        public Operation Op {
            get { return op; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(op, value, this);
                op = value;            
            }
        }
        private BEList<ParamCmd> mappings = new BEList<ParamCmd>();
        /// <summary>
        /// Only take effect when proc -> proc
        /// update the target process public parameters
        /// 
        /// make sure that the target parameter is the target process's public parameter. 
        /// </summary>
        public BEList<ParamCmd> Mappings {
            get { return mappings; }
            //set {
            //    mappings = value;
            //    if (mappings != null) {
            //        mappings.Owner = this;
            //    }
            //}
        }
		private BEList<OpCondition> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<OpCondition> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<OpCondition>) {
                _collection = (BEList<OpCondition>)belist;
            }
        }

        public OpCondition() {
            this.mappings.Owner = this;
        }
    }
    /// <summary>
    /// the basic condition unit
    /// </summary>
    [Serializable]
    public class Condition : BaseElement
    {
        private Object _input1 = null;
        /// <summary>
        /// First condition input, it is an object. Candidate object can be 
        /// String : for directly keyboard input 
        /// WebElement : use a WebElement as the compare one input, candidate pattern can be Existed or Non-Existed 
        /// WebElement Attribute : the input is a WebElementAttribute object, use the Attribute string value as the input
        ///              if there are more than one WebElement found, it will check all until find one that the WebElementAttribute
        ///              match the condition true if have. 
        /// Parameter : use the parameter value as input, the input type is parameter type, can be String, Number, 
        ///            File and Set. 
        ///            
        /// Input can be a parameter, WebElement, WebElement attributes or constant string
        /// </summary>
        public Object Input1
        {
            get { return _input1; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(_input1, value, this);
                _input1 = value;            
            }
        }
        private Object _input2 = null;

        public Object Input2
        {
            get { return _input2; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(_input2, value, this);
                _input2 = value;            
            }
        }
        /// <summary>
        ///  condition patten VALUE 
        /// </summary>
        private CONDITION _pattern = CONDITION.EMPTY;        
        /// <summary>
        /// This field is used to define how to handle the condition inputs, with the field, it will calculate
        /// the condition value in the runtime. 
        /// 
        /// currently we support a set of pattern defined in CONDITION enum, they are with 4 classes
        /// 1. string, e.g. fullmatch, contain, startWith, endWidth
        /// 2. number, e.g. bigger, less, equal
        /// 3. Object, e.g. existed
        /// 4. Set,    e.g. include, exclude. 
        /// </summary>
        public CONDITION COMPARE
        {
            get { return _pattern; }
            set { _pattern = value; }
        }
        private Boolean isNot = false;
        /// <summary>
        /// whether to do the NOT operation for the condition 
        /// </summary>
        public Boolean IsNot {
            get { return isNot; }
            set { isNot = value; }
        }
        [NonSerialized]
        private bool isChecked = false;
        /// <summary>
        /// whether the Condition value has checked. If true, it will just use the _result value directly
        /// </summary>
        public bool IsChecked {
            get { return isChecked; }
            //set { isChecked = value; }
        }

        [NonSerialized]
        private bool _result = false;
        /// <summary>
        /// condition RESULT 
        /// </summary>
        public bool Result {
            get {
                if (false == IsChecked) {
                    int r = ModelManager.Instance.checkResult(COMPARE, Input1, Input2);
                    if (r == -1) {
                        isChecked = false;
                    } else {
                        _result = r == 1 ? true : false;
                        _result = IsNot ? !_result : _result;
                        isChecked = true;
                    }                    
                }
                return _result;
            }
        }
		private BEList<BaseElement> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<BaseElement> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<BaseElement>) {
                _collection = (BEList<BaseElement>)belist;
            }
        }        
        /// <summary>
        /// Reset the condition as non-initialized status, IsChecked = false & Result = false. 
        /// </summary>
        public void reset() {
            this.isChecked = false;
            this._result = false;
        }
    }
    [Serializable]
    public class ConditionGroup : BaseElement
    {
        /// <summary>
        /// the elements is Condition or ConditionGroup
        /// </summary>
        private BEList<BaseElement> _conditions = new BEList<BaseElement>();
        /// <summary>
        /// Condtion or CondtionGroup list, it is used for some times the Condition and 
        /// ConditionGroup are composed 
        /// </summary>
        public BEList<BaseElement> Conditions
        {
            get { return _conditions; }
            private set { _conditions = value; }
        }
        private Boolean isNot = false;
        /// <summary>
        /// whether to do the NOT operation for the condition 
        /// </summary>
        public Boolean IsNot {
            get { return isNot; }
            set { isNot = value; }
        }
        public ConditionGroup() {
            this._conditions.Owner = this;
        }
        private CONDITION _relation = CONDITION.EMPTY;
        /// <summary>
        /// How to handle the directly contained children condition/CondtionGroup's relation
        /// CONDITION.AND, CONDITION.OR, 
        /// e.g Relation is AND, if the children is con1,conGrp1,con2, it means that the operation
        /// is  AND between the three condition/group.
        /// </summary>
        public CONDITION Relation {
            get { return _relation; }
            set { _relation = value; }
        }
        [NonSerialized]
        private bool _result = false;
        /// <summary>
        /// condition gropu RESULT 
        /// </summary>
        public bool Result
        {
            get {
                if (false == IsChecked) {
                    // isChecked flag is updated in the checkResult() method 
                    _result = ModelManager.Instance.checkResult(this);
                    _result = IsNot ? !_result : _result;                    
                }
                return _result;
            }
            private set { _result = value; }
        }
        [NonSerialized]
        private bool isChecked = false;
        /// <summary>
        /// whether the Condition value has checked. If true, it will just use the _result value directly
        /// </summary>
        public bool IsChecked {
            get { return isChecked; }
            set { isChecked = value; }
        }
        private BEList<BaseElement> _collection = new BEList<BaseElement>();
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<BaseElement> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<BaseElement>) {
                _collection = (BEList<BaseElement>)belist;
            }
        }   
        /// <summary>
        /// Reset the conditionGroup as non-initialized status, IsChecked = false & Result = false. 
        /// </summary>
        public void reset() {
            this.isChecked = false;
            this._result = false;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n--------------------------------------------------------------\n");
            sb.Append("[ConditionGroup]:");
            sb.Append("\n  name: " + Name);
            sb.Append("\n  description: " + Description);
            sb.Append("\n  TOP level children: ");
            foreach (BaseElement be in Conditions)
            {
                if (be is ConditionGroup)
                {
                    sb.Append("  [ConditionGroup]:");
                    sb.Append("\n    name: " + be.Name);
                    sb.Append("\n    description: " + be.Description);
                }
                else if (be is Condition)
                {
                    Condition con = be as Condition;
                    sb.Append("  [Condition]:");
                    sb.Append("\n    name: " + con.Name);
                    sb.Append("\n    description: " + con.Description);                    
                    string atn1 = con.Input1.ToString() != null ? con.Input1.ToString() : null;
                    sb.Append("\n    attribute1: " + atn1);
                    string atn2 = con.Input2.ToString() != null ? con.Input2.ToString() : null;
                    sb.Append("\n    attribute2: " + atn2);
                    sb.Append("\n    pattern: " + con.COMPARE.ToString());
                }
            }
            sb.Append("--------------------------------------------------------------\n");

            return sb.ToString();
        }
    }
    /// <summary>
    /// parameter group 
    /// </summary>
    [Serializable]
    public class ParamGroup : BaseElement {
        private BEList<Parameter> _params = new BEList<Parameter>();
        /// <summary>
        /// parameters under current group
        /// </summary>
        public BEList<Parameter> Params {
            get { return _params; }
        }

        private BEList<ParamGroup> _subGroups = new BEList<ParamGroup>();
        /// <summary>
        /// sub parameter groups of the current element 
        /// </summary>
        public BEList<ParamGroup> SubGroups {
            get { return _subGroups; }
        }

        public ParamGroup() {
            this._params.Owner = this;
            this._subGroups.Owner = this;
        }
		private BEList<ParamGroup> _collection = null;
        /// <summary>
        /// container BEList of the current element 
        /// </summary>
        public BEList<ParamGroup> Collection {
            get { return _collection; }            
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<ParamGroup>) {
                _collection = (BEList<ParamGroup>)belist;
            }
        }
        /// <summary>
        /// deep copy the ParamGroup, make sure there is a totally new copy
        /// that has the same relative value with the original paramGroup. 
        /// </summary>
        /// <returns></returns>
        public ParamGroup Clone() {            
            ParamGroup grp = ModelFactory.createParamGroup();
            grp.Name = grp.Name;
            grp.Description = grp.Description;
            ExtFields.AddRange(this.ExtFields);
            // update Parameters 
            foreach (Parameter p in this.Params) {
                Parameter np = p.Clone();
                grp.Params.AddUnique(np);
            }
            // update the subGroups 
            foreach (ParamGroup gp in this.SubGroups) {
                ParamGroup ngp = gp.Clone();
                grp.SubGroups.AddUnique(ngp);
            }
            return grp;
        }
    }
    /// <summary>
    /// A parameter is used for Process, and it is used to maintain and share data in different scope and lifecycle. 
    /// Parameter is one of Two object to maitain data in script, anothre is WebElement. 
    /// . A root process parameter is valid in all script and during whole script root active time, 
    ///   It is used as the global parameter, all the element can refer the global element 
    /// . A process parameter is only valid for the direct contained element, include operation, sub process, 
    ///   condition and web element referenced.
    /// 
    ///   when enter processing, all process level parameters will be initilized and cleaned when 
    ///   process finished. 
    /// 
    /// A parameter is an instance object that is used to share data info between different objects
    /// 
    /// Parameter usage scope : 
    /// 1. Parameter is an instance object
    ///    Operation/Process Condition input, OperationInput, Rule parameter, Expression/GlobalFunction parameter.
    /// 2. Parameter value, Must a concrete constant String or Number
    /// </summary>
    [Serializable]
    public class Parameter : BaseElement
    {        
        private ParamType _type = ParamType.STRING;
        /// <summary>
        /// type of the parameter 
        /// </summary>
        public ParamType Type
        {
            get { return _type; }
            set {
                _type = value;
                if (_type == ParamType.SET) {                    
                    if (this._rset == null) {
                        this._rset = new List<object>();
                    }
                    if (this._dset == null) {
                        this._dset = new List<object>();
                    }
                }
            }
        }        
        [NonSerialized]
        object _rvalue = null;
        /// <summary>
        /// Runtime real value, it will not be persistant to model, it will be calculated at runtime. 
        /// In runtime, you should use this one to get / update value. 
        /// 
        /// The value is a constant String or Number at runtime. 
        /// In the runtime, when used, the value is a consant string or decimal or string.Empty if errors. 
        /// 
        /// Type == string or Number, the value is string or number. 
        /// Type == Date, the value is string. 
        /// Type == Set, the value is an element in the set based on It will take effect 
        ///         if the type is numbe or string, if it is a set it will show the set items values          
        ///         If set is empty or there are some errors with Set, it will return string.Empty. 
        /// </summary>
        public object RealValue {
            get {
                object rv = null;
                if (Type == ParamType.STRING || Type == ParamType.NUMBER || Type == ParamType.DATETIME) {                    
                    rv = _rvalue;
                } else if (Type == ParamType.SET) {
                    if (RealSet == null || RealSet.Count <= 0) {
                        return string.Empty;
                    }
                    // initial the iterator index if not initialized
                    bool init = false;
                    if (ConsumeSet == true && nextSetItem == -1) {
                        init = true;
                        nextSetItem = 0;
                    }
                    if (SetCtrl == SET_ACCESS.LOOP) {
                        if (nextSetItem < RealSet.Count && nextSetItem >= 0) {
                            if (ConsumeSet) {
                                ConsumeSet = false;
                                if (nextSetItem >= RealSet.Count) {
                                    nextSetItem = 0;
                                } else {
                                    if (init == false) {
                                        nextSetItem++;
                                    }
                                }
                            }
                            if (nextSetItem < 0 || nextSetItem> RealSet.Count-1) {
                                nextSetItem = 0;
                            }
                            rv = RealSet[nextSetItem];                            
                        }
                    } else if (SetCtrl == SET_ACCESS.RANDOM) {
                        if (ConsumeSet) {
                            ConsumeSet = false;
                            nextSetItem = ModelManager.Instance.randomNumber(0, RealSet.Count);
                        }
                        if (nextSetItem <0 || nextSetItem>RealSet.Count-1) {
                            nextSetItem = 0;
                        }
                        rv = RealSet[nextSetItem];
                    } else if (SetCtrl == SET_ACCESS.RANDOM_NO_DUPLICATE) {
                        // initialize left set 
                        if (leftSet == null) {
                            leftSet = new List<object>();
                            leftSet.AddRange(RealSet);
                        } else if (leftSet.Count == 0) {
                            // if it finished a set of items, it will rebuild it again. 
                            leftSet.AddRange(RealSet);
                        }
                        
                        if (ConsumeSet) {
                            ConsumeSet = false;
                            int tindex = -1;
                            tindex = ModelManager.Instance.randomNumber(0, leftSet.Count);
                            if (tindex < 0 || tindex > leftSet.Count - 1) {
                                tindex = 0;
                            }
                            object obj = leftSet[tindex];
                            nextSetItem = this.RealSet.IndexOf(obj);
                            if (nextSetItem < 0 || nextSetItem > RealSet.Count - 1) {
                                nextSetItem = 0;
                            }
                            leftSet.Remove(tindex);
                        }
                        rv = RealSet[nextSetItem];
                    }
                    if (rv != null) {
                        if (SetType == ParamType.NUMBER) {
                            decimal number = ModelManager.Instance.getDecimal(rv);
                            if (number != decimal.MinValue) {
                                rv = number;
                            }
                        } else if (SetType == ParamType.STRING) {
                            rv = rv.ToString();
                        }
                    }
                }

                return rv == null ? string.Empty : rv;
                //return rv;
            }
            set { // The value only can be changed for string and number type
                if (Type == ParamType.STRING || Type == ParamType.NUMBER) {
                    _rvalue = value;
                } else if (Type == ParamType.DATETIME) { 
                    if(value!=null && ModelManager.Instance.isValidTime(value.ToString())){
                        _rvalue = value.ToString() ;
                    }
                }
            }
        }

        object _dvalue = null;
        /// <summary>
        /// Design time value, it will be saved in the model, in design time, you should use this one to 
        /// get/update value. 
        /// 
        /// Value allowed : Constant string/number/SET
        /// In design time, It will return the value if it is constant string/number/SET, for a set type, it will always return the 
        /// first element as design value. 
        /// </summary>
        public object DesignValue
        {
            get {                
                object rv = null;
                if (Type == ParamType.STRING || Type == ParamType.NUMBER || Type == ParamType.DATETIME) {                    
                    rv = _dvalue;
                } else if (Type == ParamType.SET) {
                    if (this._dset.Count > 0) {
                        rv = this._dset[0];
                    }
                }
                return rv == null ? string.Empty : rv;                                
            }
            set {
                // The value only can be changed for string and number type
                if (Type == ParamType.STRING || Type == ParamType.NUMBER) {
                    _dvalue = value;
                } else if (value != null && ModelManager.Instance.isValidTime(value.ToString())) {
                    _dvalue = value.ToString();
                }
            }
        }
        private bool sensitive = false;
        /// <summary>
        /// Whether the this parameter is a sensitive data, A sensitive data is that if the data binding to a 
        /// password input WebElement or the data is defined as sensitive. 
        /// 
        /// A sensitive parameter only can be as Operation input with password type, or can only be ParamCmd target parameter. 
        /// A sensitive parameter can only bind to one WebElement in a script. 
        /// </summary>
        public bool Sensitive {
            get { return sensitive; }
            set { sensitive = value; }
        }

        #region used for set value control      
        [NonSerialized]
        private bool consumeSet = false;
        /// <summary>
        /// This is only take effect if the Type == Set. It is used to control the Set Parameter's RealValue's behaviour. 
        /// 
        /// true: it is a consumer of the set, the RealValue will be changed based on the access rule
        /// false: it is not a consumre of the set, just return the current RealValue if need.
        /// </summary>
        public bool ConsumeSet {
            get { return consumeSet; }
            set { consumeSet = value; }
        }

        [NonSerialized]
        private int nextSetItem = -1;
        [NonSerialized]
        private List<object> leftSet = null;
        #endregion used for set value control 
        private ParamType setType = ParamType.STRING;
        /// <summary>
        /// it will take effet if the Type is SET, it is used to record the set children type 
        /// all the set element should be the same type, currently the set only can be string or number or DateTime string
        /// </summary>
        public ParamType SetType {
            get { return setType; }
            set { setType = value; }
        }
        private SET_ACCESS setCtrl = SET_ACCESS.LOOP;
        /// <summary>
        /// Take effect if the Type is set
        /// How to get the set value when parameter was accessed
        /// </summary>
        public SET_ACCESS SetCtrl {
            get { return setCtrl; }
            set { setCtrl = value; }
        }        
        [NonSerialized]
        private List<object> _rset = null;
        /// <summary>
        /// It is used in runtime, in runtime, you should use this one to get/update set value.
        /// 
        /// All allowed object is : constant String, Number. set items can be duplicated. 
        /// It will take effect if Type is SET, please make sure to do null check before use the set, 
        /// if set is null, please new a list. 
        /// All set children have the same type, string or number. 
        /// When use the set, please take care the the element type. 
        /// </summary>
        public List<object> RealSet {
            get { return _rset; }
            //set { _rset = value; }
        }        
        private List<object> _dset = null;
        /// <summary>
        /// It is used in design time, all the values are saved in model, in design time, you should 
        /// use this one to get/update set value. 
        /// 
        /// All allowed object is : constant String, Number. set items can be duplicated. 
        /// It will take effect if Type is SET, please make sure to do null check before use the set, 
        /// if set is null, please new a list. 
        /// All set children have the same type, string or number. 
        /// When use the set, please take care the the element type. 
        /// </summary>
        public List<object> DesignSet {
            get { return _dset; }
            set { _dset = value; }
        }         
		private BEList<Parameter> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<Parameter> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<Parameter>) {
                _collection = (BEList<Parameter>)belist;
            }
        }
        public Parameter() {
            if (this.Type == ParamType.SET) {
                if (this._rset == null) {
                    this._rset = new List<object>();
                }
                if (this._dset == null) {
                    this._dset = new List<object>();
                }
            }
        }
        /// <summary>
        /// Reset the parameters internal status variables, e.g the flags to control SET. 
        /// clean RealSet, set RealValue=null. 
        /// </summary>
        public void reset() {
            if (Type == ParamType.SET && _rset == null) {
                this._rset = new List<object>();
            }
            this.consumeSet = false;
            if (this.leftSet != null) {
                this.leftSet.Clear();
            }
            this.nextSetItem = -1;
            if (this._rset != null) {
                this._rset.Clear();
            }
            this.RealValue = null;
        }
        /// <summary>
        /// Deep clone a new Parameter that has the same structure and value with the original one itself, not include 
        /// the container info. Just the Parameter self attributes. 
        /// </summary>
        /// <returns></returns>
        public Parameter Clone() {
            Parameter param = ModelFactory.createParameter();
            param.Name = this.Name;
            param.Description = this.Description;
            ExtFields.AddRange(this.ExtFields);
            param._type = this._type;
            param._dvalue = this._dvalue;
            param._rvalue = this._rvalue;
            if (this.Type == ParamType.SET) {
                param.SetType = this.SetType;
                param.SetCtrl = this.SetCtrl;
                if (this.DesignSet != null) {
                    param.DesignSet = new List<object>();
                    foreach (object o in this.DesignSet) {
                        param.DesignSet.Add(o);
                    }
                }
                if (this.RealSet.Count > 0) {
                    param.RealSet.Clear();
                    foreach (object o in this.RealSet) {
                        param.RealSet.Add(o);
                    }
                }
            }
            return param;
        }
        public override string ToString() {
            if (Type == ParamType.STRING || Type == ParamType.NUMBER) {
                if (_rvalue != null) {
                    return _rvalue.ToString();
                } else if (_dvalue != null) {
                    return _dvalue.ToString();
                } else {
                    return string.Empty;
                }
            } else if (Type == ParamType.SET) {
                List<object> set = this.DesignSet;
                if (this.RealSet!=null && this.RealSet.Count>0) {
                    set = RealSet;
                }
                if (set != null) {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    foreach (object o in set) {
                        sb.Append("\"").Append(o.ToString()).Append("\"").Append(",");
                    }
                    sb.Replace(",", "", sb.Length - 1, 1);
                    sb.Append("}");
                    return sb.ToString();
                } else {
                    return "{}";
                }
            }
            return base.ToString();

        }
    }
    /// <summary>
    /// A rule is used to handle the exceptions when stub operation finished. e.g if the input is null, wait until
    /// it is available. 
    /// </summary>
    [Serializable]
    public class OperationRule : BaseElement
    {
        private RuleTrigger trigger = RuleTrigger.INVALID;

        public RuleTrigger Trigger {
            get { return trigger; }
            set { trigger = value; }
        }
        private RuleAction action = RuleAction.None;

        public RuleAction Action {
            get { return action; }
            set { action = value; }
        }
        
        public OperationRule() {
            this._params.Stub = this;    
        }
        private ListRef _params = new ListRef();
        /// <summary>
        /// Input can be Constant(String||Number), WebElemntAttribute, Parameter, Expression, GlobalFunction
        /// </summary>
        public ListRef Params
        {
            get { return _params; }
            //set { _params = value;}
        }
		private BEList<OperationRule> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<OperationRule> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<OperationRule>) {
                _collection = (BEList<OperationRule>)belist;
            }
        }
        /// <summary>
        /// Whether the two rules are same. Same means that the trigger, action
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool isEqual(OperationRule rule) {
            bool result = false;
            if (rule != null) {
                if (this.Trigger == rule.Trigger && this.Action == rule.Action) {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// return a deep copied rule except the Collection info 
        /// </summary>
        /// <returns></returns>
        public OperationRule Clone() {
            OperationRule rule = ModelFactory.createOperationRule();
            rule.Action = this.Action;
            rule.Description = this.Description;
            ExtFields.AddRange(this.ExtFields);
            rule.Name = this.Name;
            rule.Params.AddRange(this.Params.ToArray());
            rule.Trigger = this.Trigger;

            return rule;
        }
    }
    /// <summary>
    /// This class is used for the paramter update or parameter mapping 
    /// </summary>
    [Serializable]
    public class ParamCmd : BaseElement{
        private PARAM_CMD cmd;
        /// <summary>
        /// command name
        /// </summary>
        public PARAM_CMD Cmd {
            get { return cmd; }
            set { cmd = value; }
        }
        private object paramSrc = null;
        /// <summary>
        /// command source parameters, the command will handle how to use the source.  
        /// allowed value is : String, Number, WebElementAttribute, Parameter, Expression, GlobalFunction
        /// </summary>
        public object Src{
            get { return paramSrc; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(paramSrc, value, this);
                paramSrc = value;            
            }
        }
        private Parameter target = null;
        /// <summary>
        /// Command target parameter.
        /// </summary>
        public Parameter Target {
            get { return target; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(target, value, this);
                target = value;            
            }
        }
        private BEList<ParamCmd> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<ParamCmd> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<ParamCmd>) {
                _collection = (BEList<ParamCmd>)belist;
            }
        }
    }
    /// <summary>
    /// Expression used to handle Number type : + - * /
    /// String type +
    /// </summary>
    [Serializable]
    public class Expression : BaseElement{
        ParamType type = ParamType.STRING;
        /// <summary>
        /// Operated elements type, allowed TYPE = STRING || NUMBER || DATETIME
        /// </summary>
        public ParamType Type {
            get { return type; }
            set { type = value; }
        }
        private object input1 = null;
        /// <summary>
        /// Input can be Constant(String||Number), WebElemntAttribute, Parameter, Expression, GlobalFunction
        /// </summary>
        public object Input1 {
            get { return input1; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(input1, value, this);
                input1 = value;            
            }
        }
        private object input2 = null;
        /// <summary>
        /// Input can be Constant(String||Number), WebElemntAttribute, Parameter, Expression, GlobalFunction
        /// </summary>
        public object Input2 {
            get { return input2; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(input2, value, this);
                input2 = value;            
            }
        }
        private string op = "+";
        /// <summary>
        /// Allowed type == NUMBER + - * /
        /// Type = String +
        /// </summary>
        public string Operator {
            get { return op; }
            set { op = value; }
        }
        /// <summary>
        /// if type == String, return a string, if type == number, return a decimal.
        /// return string.Empty if the there are some errors.
        /// </summary>
        public object Result {
            get { return ModelManager.Instance.getExpressionResult(this); }
        }
        private BEList<Expression> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<Expression> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<Expression>) {
                _collection = (BEList<Expression>)belist;
            }
        }
        /// <summary>
        /// Deep clone a new Expression that has the same structure and value with the original one itself, not include 
        /// the container info. Just the Expression itself attributes. 
        /// </summary>
        /// <returns></returns>
        public Expression Clone() {
            Expression exp = ModelFactory.createExpression();
            exp.Name = this.Name;
            exp.Description = this.Description;
            ExtFields.AddRange(this.ExtFields);
            exp.Input1 = this.Input1;
            exp.Input2 = this.Input2;
            exp.Operator = this.Operator;
            exp.Type = this.Type;
            
            return exp;
        }
    }
    /// <summary>
    /// GlobalFunction is used to calculate some values and return a result, that can be 
    /// consumed by paramCmd or expression or other global functions. 
    /// </summary>
    [Serializable]
    public class GlobalFunction : BaseElement{
        private GF_CMD cmd = GF_CMD.NONE;

        public GF_CMD Cmd {
            get { return cmd; }
            set { cmd = value; }
        }

        public GlobalFunction() {
            this.paramlist.Stub = this;
        }
        
        private ListRef paramlist = new ListRef();
        
        /// <summary>
        /// Input can be Constant(String||Number), WebElemntAttribute, Parameter, Expression, GlobalFunction
        /// </summary>
        public ListRef Params {
            get { return paramlist; }
            //set { paramlist = value; }
        }
        private ParamType type = ParamType.STRING;
        /// <summary>
        /// GlobalFunction result value type, default is string, allowed ParamType.STRING || ParamType.NUMBER || DATETIME
        /// </summary>
        public ParamType Type {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Result is a string or decimal or string.Empty if errors 
        /// </summary>
        public object Result {
            get { return GFManager.getGFResult(this); }
        }
        private BEList<GlobalFunction> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<GlobalFunction> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<GlobalFunction>) {
                _collection = (BEList<GlobalFunction>)belist;
            }
        }
        /// <summary>
        /// Clone a GlobalFunction object that has the same attributes with this one. 
        /// Only the GlobalFunction 's own attributes. For parameters, just add the existed parameters 
        /// into new one's parameters list. 
        /// </summary>
        /// <returns></returns>
        public GlobalFunction Clone() {
            GlobalFunction gf = ModelFactory.createGlobalFunction();
            gf.Name = this.Name;
            gf.Description = this.Description;
            ExtFields.AddRange(this.ExtFields);
            gf.Cmd = this.Cmd;
            gf.Type = this.Type;           
            gf.Params.AddRange(paramlist.ToArray());            
            return gf;
        }
    }
    /// <summary>
    /// It is used to record the user log info 
    /// </summary>
    [Serializable]
    public class UserLog : BaseElement{        
        public UserLog(){
            logItems.Owner = this;
        }
        private BEList<UserLogItem> logItems = new BEList<UserLogItem>();
        /// <summary>
        /// This list log items in order, in the runtime, it will be translated to 
        /// real logs. e.g the items can be WebElementAttribute, Parameter or constant string | number.
        /// For the WebElementAttribute, just only can see that Operation/Process's itself WebElement's attriubte
        /// for the Parameter just can see the Process's(if operation, it is the container) parameter
        /// </summary>
        public BEList<UserLogItem> LogItems {
            get { return logItems; }
            set { logItems = value; }
        }        
        /// <summary>
        /// get the item display text, it maybe show on UI
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getItemText(UserLogItem item) {
            if (item == null) {
                return "Null item";
            }
            if (item.Item is WebElementAttribute) {
                WebElementAttribute wea = item.Item as WebElementAttribute;
                return wea.RValue;
            } else if (item.Item is Parameter) {
                Parameter param = item.Item as Parameter;
                return param.ToString();
            } else if (item.Item is string) {
                return item.Item.ToString();
            } else if (item.Item is decimal) {
                return ModelManager.Instance.getDecimal(item.Item)+"";
            }else {
                return "ErrorType" ;
            }
        }
        private BEList<UserLog> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<UserLog> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<UserLog>) {
                _collection = (BEList<UserLog>)belist;
            }
        }
    }
    /// <summary>
    /// An User log item, it defined the item object, maybe WEA or Parameter or Constant string or number. 
    /// </summary>
    [Serializable]
    public class UserLogItem : BaseElement{
        object item = null;

        public object Item {
            get { return item; }
            set {
                ModelManager.Instance.updateWeakRef_AssginValue(item, value, this);
                item = value; }
        }
        public static readonly int DEFAULT_COLOR = int.MinValue;
        int argb = DEFAULT_COLOR;
        /// <summary>
        /// Item Color, if it is DEFAULT_COLOR, it will use the default color defined in script color map, 
        /// else it should be an int value for the argb color. 
        /// </summary>
        public int Color {
            get { return argb; }
            set { argb = value; }
        }
        private BEList<UserLogItem> _collection = null;
        /// <summary>
        /// container BEList of the current element. 
        /// </summary>
        public BEList<UserLogItem> Collection {
            get { return _collection; }
        }
        /// <summary>
        /// this method is used to update the _collections outside. 
        /// </summary>
        /// <param name="belist"></param>
        public void setupBEList(object belist) {
            if (belist == null) {
                _collection = null;
            } else if (belist is BEList<UserLogItem>) {
                _collection = (BEList<UserLogItem>)belist;
            }
        }
    }
    /// <summary>
    /// pure model model root
    /// </summary>
    [Serializable]
    public class ScriptRoot : BaseElement
    {
        #region common data
        private string uid = "";

        public new string UID {
            get { return uid; }
        }
        private long _time;
        /// <summary>
        /// binary time of the script creation
        /// </summary>
        public long CreationTime {
            get { return _time; }
            set { _time = value; }
        }
        private string _modelVersion = "1.0.0.0";
        /// <summary>
        /// This will be used to do model migration later
        /// </summary>
        public string ModelVersion {
            get { return _modelVersion; }
            set { _modelVersion = value; }
        }

        private string _author = null;
        /// <summary>
        /// The originator of the script. 
        /// </summary>
        public string Author {
            get { return _author; }
            set { _author = value; }
        }
        private List<string> _contributors = new List<string>();
        /// <summary>
        /// Contrubutor of the scripts, maybe more people will help to improve the script.
        /// </summary>
        public List<string> Contributors {
            get { return _contributors; }
            set { _contributors = value; }
        }        
        private string _version = "1.0.0.0";
        /// <summary>
        /// version of the script
        /// </summary>
        public string Version {
            get { return _version; }
            set { _version = value; }
        }

        private string _icoPath = null;
        /// <summary>
        /// path of the script icon 
        /// </summary>
        public string IconPath {
            get { return _icoPath; }
            set { _icoPath = value; }
        }
        private string _targetWebURL = null;
        /// <summary>
        /// targe Web URL this script will apply on 
        /// </summary>
        public string TargetWebURL {
            get { return _targetWebURL; }
            set {
                _targetWebURL = value;
                if (!ModelManager.Instance.isTrustedURL(_targetWebURL, TrustedUrls)) {
                    TrustedUrls.Add(_targetWebURL);
                }
            }
        }
        private List<String> urls = new List<string>();
        /// <summary>
        /// how many urls are trusted in the script, it can be used for security check.
        /// </summary>
        public List<String> TrustedUrls {
            get { return urls; }
            set { urls = value; }
        }
        private Boolean urlsLocked = true;
        /// <summary>
        /// whether to apply the candidate url check, default is true. 
        /// true : only candidate urls operation can be executed
        /// false: any url operations can be executed
        /// </summary>
        public Boolean UrlsLocked {
            get { return urlsLocked; }
            set { urlsLocked = value; }
        }
        private Hashtable _colorMap = new Hashtable();
        /// <summary>
        /// This map is used for User log. It will defined which Script Object with with color. 
        /// the key is Script object class name, value is the color's argb int value. 
        /// </summary>
        public Hashtable ColorMap {
            get { return _colorMap; }
            set { _colorMap = value; }
        }
        private string _history = string.Empty;
        /// <summary>
        /// Published script update history info. History+UpdateInfo will be the total history info.
        /// </summary>
        public string History {
            get { return _history; }
            set { _history = value; }
        }
        private string _updateInfo = string.Empty;
        /// <summary>
        /// This version update info. If a published script is download for edit. It will first add the 
        /// updateInfo into history and clean updateInfo. so if there are some upadate, the script will 
        /// be in a new version with new updateInfo. 
        /// 
        /// A script whether is an published version depends on the server side response. 
        /// </summary>
        public string UpdateInfo {
            get { return _updateInfo; }
            set { _updateInfo = value; }
        }       
        #endregion common data
        #region scripts manangement area
        private int timeout = Constants.WE_CHECK_TIMEOUT;
        /// <summary>
        /// How long time(ms) will wait until the WebElement was not null. A script has a default value
        /// defined with Constants.WE_CHECK_TIMEOUT.
        /// </summary>
        public int Timeout {
            get { return timeout; }
            set { timeout = value; }
        }

        private BEList<WebElement> _iframes = new BEList<WebElement>();
        /// <summary>
        /// It is used to maintain all iframe WebElements, all iframe web elements 
        /// are managed by system, and it is transparent for user. 
        /// 
        /// A iframe webElement is identified by the name = id+src value. 
        /// </summary>
        public BEList<WebElement> IFrames {
            get { return _iframes; }
        }
        private WebElementGroup _rawElemGrp = null;
        /// <summary>
        /// This group is used to maintain the newly added WebElements, it need be refined
        /// to used, element in this list can not be used directly.
        /// accepted element must be WebElement. 
        /// </summary>
        public WebElementGroup RawElemsGrp {
            get { return _rawElemGrp; }
            //set { _rawElems = value; }
        }
        private BEList<WebElement> internalRefWEs = new BEList<WebElement>();
        /// <summary>
        /// It is used to maintain the internal ref WE for locate an WebElement. 
        /// </summary>
        public BEList<WebElement> InternalRefWEs {
            get { return internalRefWEs; }
            set { internalRefWEs = value; }
        }

        private WebElementGroup _weroot = null;
        /// <summary>
        /// the root node of all updated WebElement and WebElementGroup, the RawElementGroup was not include. 
        /// </summary>
        public WebElementGroup WERoot {
            get { return _weroot; }
        }
        private Process _procRoot = null;
        /// <summary>
        /// Process and Operations root node, all operation and process are included. 
        /// </summary>
        public Process ProcRoot {
            get { return _procRoot; }
        }

        private List<string> _refScripts = new List<string>();
        /// <summary>
        /// referenced script list, a value is a unique key represent 
        /// the referred script, it is a hash value of the script path, The value is Script UID. 
        /// </summary>
        public List<string> RefScripts {
            get { return _refScripts; }
            private set { _refScripts = value; }
        }

        private Dictionary<string, ListRef> refScriptsConfig = new Dictionary<string, ListRef>();
        /// <summary>
        /// It is the RefScipts relative parameter configuration. Key = Script UID, Value = Confiugred script public parameters. 
        /// </summary>
        public Dictionary<string, ListRef> RefScriptsConfig {
            get { return refScriptsConfig; }
            set { refScriptsConfig = value; }
        }

        private List<string> _citedScripts = new List<string>();
        /// <summary>
        /// how many scripts refer this script, a value is a unique key represent the referred 
        /// script, it is a hash value of the script path
        /// </summary>
        public List<string> CitedScripts {
            get { return _citedScripts; }
            private set { _citedScripts = value; }
        }        
        private bool timeSensitive = false;
        /// <summary>
        /// whether the script is time sensitive, default is false ;
        /// if true, when a frame document is load completed, the op will check whether the relative WebElement 
        /// is available, if yes, the op will executed, if no, it will wait the next document completed event to check
        /// until the relative WebElement is avaible.
        /// 
        /// if false, the op will wait until all the document is loaded completed. and check necessary WebElement used. 
        /// </summary>
        public bool TimeSensitive {
            get { return timeSensitive; }
            set { timeSensitive = value; }
        }
        #endregion scripts manangement area
        /// <summary>
        /// Script Constructor 
        /// </summary>
        public ScriptRoot() {
            uid = System.Guid.NewGuid().ToString();
            //this._citedScripts.Owner = this;            
            //this._refScripts.Owner = this;
            this._iframes.Owner = this;
            this.internalRefWEs.Owner = this;
            // update WERoot 
            this._weroot = ModelFactory.createWebElementGroup();
            this._weroot.Name = LangUtil.getMsg("model.SRoot.WERoot.Name");
            this._weroot.Description = LangUtil.getMsg("model.SRoot.WERoot.Des");
            ModelManager.Instance.updateWeakRef_AssginValue(null, this._weroot, this);
            // update ProcRoot 
            this._procRoot = ModelFactory.createProcess();
            this._procRoot.Name = LangUtil.getMsg("model.SRoot.ProcRoot.Name");
            this._procRoot.Description = LangUtil.getMsg("model.SRoot.ProcRoot.Des");
            ModelManager.Instance.updateWeakRef_AssginValue(null, this._procRoot, this);
            // update Paramemter public root 
            this._procRoot.ParamPublic = ModelFactory.createParamGroup();
            this._procRoot.ParamPublic.Name = LangUtil.getMsg("model.SRoot.ProcRoot.PubParam.Name");
            this._procRoot.ParamPublic.Description = LangUtil.getMsg("model.SRoot.ProcRoot.PubParam.Des");
            // update parameter private root 
            this._procRoot.ParamPrivate = ModelFactory.createParamGroup();
            this._procRoot.ParamPrivate.Name = LangUtil.getMsg("model.SRoot.ProcRoot.PriParam.Name");
            this._procRoot.ParamPrivate.Description = LangUtil.getMsg("model.SRoot.ProcRoot.PriParam.Des");
            
            ModelManager.Instance.buildScriptDefaultRules(this._procRoot);
            // update RawDataGroup
            this._rawElemGrp = ModelFactory.createWebElementGroup();
            this._rawElemGrp.Name = LangUtil.getMsg("model.SRoot.RawWEGrp.Name");
            this._rawElemGrp.Description = LangUtil.getMsg("model.SRoot.RawWEGrp.Des");
            ModelManager.Instance.updateWeakRef_AssginValue(null, this._rawElemGrp, this);
            // initial default User log color map
            ModelManager.Instance.initDefaultLogColorMap(this);
        }

    }
    #endregion model entry definition
    #region enumeration definition.
    /// <summary>
    /// Here it will support four types for identify a WebElement
    /// 0. by a specific combined string to identify the web element
    /// 1. by attributes
    /// 2. by location, absolute to the browser window or relative another WebElement
    /// 3. by a region of image
    /// 4. by some colors
    /// </summary>
    [Serializable]
    public enum WEType
    {
        // a specific combined string to identify the WebElement. 
        // a string include: 
        // tag + id + name + gloable tag index + innerText
        // when the type is code, there will be a WebElementAttribute object
        // to record the text info if have. 
        CODE,
        // locate by attributes, used for Html element, 
        ATTRIBUTE,
        // locate by location, used for flash
        LOCATION,
        // locate by image, used for flash
        IMAGE,
        // locate by color on some points, used for flash
        COLOR
    }
    /// <summary>
    /// how to access the set element
    /// </summary>
    [Serializable]
    public enum SET_ACCESS
    {
        /// <summary>
        /// access the set values in order, if it is the last set value, next access will start from the 
        /// first set value(a new round)
        /// </summary>
        LOOP,
        /// <summary>
        /// access the set values without duplicated, It will get a value from the set that is no acessed in this round.
        /// A round means that N times access of the Set parameter(N is the set elements number)
        /// If there is no element can be accessed this round, it will start a new round if accessed. 
        /// </summary>
        RANDOM_NO_DUPLICATE,
        /// <summary>
        /// Acess the set values random, the value maybe repeated in a round
        /// </summary>
        RANDOM
    }
    /// <summary>
    /// type of parameter 
    /// </summary>
    [Serializable]
    public enum ParamType
    {
        /// <summary>
        /// value is a string, it should be visible chars 
        /// </summary>
        STRING,
        /// <summary>
        /// value is a number, it will be translated as C# Decimal type 
        /// </summary>
        NUMBER,
        /// <summary>
        /// value is a string, it will be translated as C# DateTime type in runtime.
        /// value format is HH:mm:ss. 
        /// </summary>
        DATETIME,
        /// <summary>
        /// A set type, the value is a list that can be Number, and String, so the available value can be 
        /// constant string, number, Parameter(must be string or number type).
        /// All set children should be the same type. 
        /// 
        /// Set can not be nested, this means that if the element can be Parameter, the type can not be Set. 
        /// 
        /// for operation, all will be treated as a string
        /// for condition, it depends on the apply pattern, default is treated as a string 
        ///                On condition targetValue, the pattern can be SET_CONTAIN, SET_EXCLUDE
        /// On process value, there will be some rule to guide the value usage. 
        ///     FOR_EACH_VALUE, it means that it will use a loop to iterate all the elements in the set with the process. 
        ///     it will generate a parameter to refer a Current element, other operation and condition in the process can refer the parameter to use
        /// </summary>
        SET,
        /// <summary>
        /// value is a file path, from the path the real file can be accessed, by default a element is a line in the file 
        /// it can be a string or number 
        /// 
        /// A file value is like n:[file accessable path], while the first char is element seperator. default is blank
        /// it means seperated by line. each line in the file is an element
        /// developer can define the seperator, refer Set description. 
        /// 
        /// for operation, all file context will be treated as a string
        /// for condition, it depends on the apply pattern, default is treated as a string 
        /// Each line of the file will be treated as an element. Include black space. 
        /// 
        /// On process value, there will be some rule to guide the value usage. 
        ///     FOR_EACH_VALUE, it means that it will use a loop to iterate all the elements in the set with the process. it will generate a parameter to refer a Current element, other operation and condition in the process can refer the parameter to use
        /// </summary>
        FILE
    }
    /// <summary>
    /// constants and methods for operation element 
    /// </summary>
    [Serializable]
    public enum OPERATION
    {
        /// <summary>
        /// EMPTY Operation do nothing, but it can be used to updated some parameter
        /// or add a waittime. 
        /// </summary>
        NOP,
        /// <summary>
        /// just a node for start, the operation do nothing 
        /// just a flag
        /// </summary>
        START,
        /// <summary>
        /// just a node for end, the operation do nothing 
        /// just a flag
        /// </summary>
        END,
        /// <summary>
        /// just a node for process, it is a container of other
        /// operations
        /// </summary>
        PROCESS,
        /// <summary>
        /// input operation
        /// </summary>
        INPUT,
        /// <summary>
        /// mouse click operation
        /// </summary>
        CLICK,
        /// <summary>
        /// used to refresh the current HTMLDocument content. 
        /// </summary>
        REFRESH,
        /// <summary>
        /// open a url in a new Tab if there is a Browser opened. 
        /// else open a new browser window. 
        /// </summary>
        OPEN_URL_N_T,
        /// <summary>
        /// open url in current tab
        /// </summary>
        OPEN_URL_T
    }
    /// <summary>
    /// this class is used to record the mouse click operation
    /// </summary>
    [Serializable]
    public class Click
    {
        // which keyboard key was pressed with the mouse button click 
        Keys key = Keys.None;
        // which keyboard key was pressed with the mouse button click 
        public Keys Key {
            get { return key; }
            set { key = value; }
        }
        // which mouse button clicked. defect is the left button
        Keys button = Keys.LButton;
        // which mouse button clicked. defect is the left button
        public Keys Button {
            get { return button; }
            set { button = value; }
        }
        // whether it is double click, default is no
        bool dbclick = false;
        // whether it is double click, default is no
        public bool Dbclick {
            get { return dbclick; }
            set { dbclick = value; }
        }
        CLICK_TYPE type = CLICK_TYPE.CLICK;
        // default type is CLICK
        public CLICK_TYPE Type {
            get { return type; }
            set { type = value; }
        }
    }

    [Serializable]
    public enum CLICK_TYPE
    {
        CLICK,
        DBCLICK,
        MOUSE_DOWN,
        MOUSE_UP,
        MOUSE_MOVE,
        MOUSE_HOVER
    }
    /// <summary>
    /// constants and operations for condition or condition 
    /// group elements 
    /// 
    /// Currently for number pattern, if the the input is WebElementAttribute,
    /// it will first get the runtime value as a number for check, if it is not a number
    /// it will auto get the first number in the value as the input for check. 
    /// 
    /// </summary>
    [Serializable]
    public enum CONDITION
    {
        /// <summary>
        /// internal use, the condition do nothing
        /// </summary>
        EMPTY = 0,
        /// <summary>
        /// used to connect two condition unit, AND operation.
        /// 2 parameters, can be Condition or ConditionGroup
        /// </summary>
        AND,
        /// <summary>
        /// used to connect two condition unit, OR operation.
        /// 2 parameters, can are Condition or ConditionGroup
        /// </summary>
        OR,
        /// <summary>
        /// full match condition PATTERN.
        /// 2 parameters, first is a String, second is a String.
        /// if input1 full match input2, return true. 
        /// </summary>
        STR_FULLMATCH,
        /// <summary>
        /// Not full match condition pattern
        /// 2 parameters, first is String, Second is a string, 
        /// if input1 Not full match input2, return true. 
        /// </summary>
        STR_NOT_FULLMATCH,
        /// <summary>
        /// string contain PATTERN.
        /// 2 parameters, first is a String, second is a String.
        /// if input1 contains input2, return true. 
        /// </summary>
        STR_CONTAIN,
        /// <summary>
        /// string contain PATTERN.
        /// 2 parameters, first is a String, second is a String.
        /// if input1 Not contains input2, return true. 
        /// </summary>
        STR_NOT_CONTAIN,
        /// <summary>
        /// string start with PATTERN.
        /// 2 parameters, first is a String, second is a String.
        /// If input1 start with input2, return true. 
        /// </summary>
        STR_STARTWITH,
        /// <summary>
        /// string start with PATTERN.
        /// 2 parameters, first is a String, second is a String.
        /// If input1 NOT start with input2, return true. 
        /// </summary>
        STR_NOT_STARTWITH,
        /// <summary>
        /// string end with PATTERN.
        /// 2 parameters, first is a String, second is a String.
        /// If input1 end with input2, return true. 
        /// </summary>
        STR_ENDWIDTH,
        /// <summary>
        /// string end with PATTERN.
        /// 2 parameters, first is a String, second is a String.
        /// If input1 NOT end with input2, return true. 
        /// </summary>
        STR_NOT_ENDWIDTH,
        /// <summary>
        /// to deal with number VALUE equal. 
        /// 2 parameters, first is a number, second is a number, maybe int,long,double. 
        /// If input1==input2, return true. 
        /// </summary>
        NUM_EQUAL,
        /// <summary>
        /// number not equal. 
        /// 2 parameters, first is a number, second is a number, maybe int,long,double.
        /// If input1!=input2, return true. 
        /// </summary>
        NUM_NOT_EQUAL,
        /// <summary>
        /// to deal with number VALUE bigger.
        /// 2 parameters, first is a number, second is a number, maybe int,long,double.
        /// If input1>input2, return true. 
        /// </summary>
        NUM_BIGGER,
        /// <summary>
        /// to deal with number VALUE less.
        /// 2 parameters, first is a number, second is a number, maybe int,long,double.
        /// If input1 < input2, return true. 
        /// </summary>
        NUM_LESS,
        /// <summary>
        /// to deal with the number VALUE equal and bigger than.
        /// 2 parameters, first is a number, second is a number, maybe int,long,double.
        /// If input1>=input2, return true.
        /// </summary>
        NUM_EQ_BIGGER,
        /// <summary>
        /// to deal with the number VALUE less and equal than.
        /// 2 parameters, first is a number, second is a number, maybe int,long,double.
        /// If input1 <= input2, return true.
        /// </summary>
        NUM_EQ_LESS,
        /// <summary>
        /// whether the input Object/value of condition existed in current state.
        /// 1 parameter, can be WebElement, WebElementAttribute, Parameter.
        /// If input1(WebElement) existed(has real value in runtime), return true. 
        /// </summary>
        INPUT_EXISTED,
        /// <summary>
        /// whether the set(condition input1) contain the element(condition input2).
        /// 2 parameters, first is a set parameter, item type must be string or number, the second 
        /// must be string or number that has the same type with set item.
        /// If input2 can be found in input1, return true. 
        /// </summary>
        SET_CONTAIN,
        /// <summary>
        /// whether the set(condition input1) doesn't contain the element(condition input2).
        /// 2 parameters, first is a set parameter, item type must be string or number, the second 
        /// must be string or number that has the same type with set item, it can be Parameter/WebElementAttribute/Constant string
        /// If input2 can NOT be found in input1, return true. 
        /// </summary>
        SET_EXCLUDE,
        /// <summary>
        /// Whether the input1 is after input2, parameter must be Date type parameter or a Date string, like "HH:mm:ss"
        /// 2 parameters, both are date string or date type parameter
        /// If input1 is after input2, return true, else return false 
        /// </summary>
        DATETIME_AFTER,
        /// <summary>
        /// Whether the input1 is before input2, parameter must be Date type parameter or a Date string, like "HH:mm:ss"
        /// 2 parameters, both are date string or date type parameter
        /// If input1 is before input2, return true, else return false 
        /// </summary>
        DATETIME_BEFORE,
        /// <summary>
        /// Whether the input 1 is equal input2, parameter must be Date type parameter or a Date string, like "HH:mm:ss"
        /// 2 parameters, both are date string or date type parameter
        /// If input1 is equal input2, return true, else return false 
        /// </summary>
        DATETIME_EQUAL
    }
    /// <summary>
    /// Trigger of the Operation Rule 
    /// </summary>
    [Serializable]
    public enum RuleTrigger
    {
        /// <summary>
        /// used for internal, it means that the trigger is invalid
        /// </summary>
        INVALID,
        /// <summary>
        /// WebElement is null or not found will trigger the rule 
        /// </summary>
        NULL_ELEMENT,
        /// <summary>
        /// Operation executed error will trigger the rule 
        /// </summary>
        OP_EXE_ERROR,
        /// <summary>
        /// operation request time out will trigger the rule
        /// </summary>
        REQ_TIMEOUT,
        /// <summary>
        /// If an non-end operation with out one proper next OpCondition, it will
        /// trigger the rule. 
        /// </summary>
        NO_NEXT_OP_FOUND
    }
    [Serializable]
    public enum RuleAction
    {
        /// <summary>
        /// empty action, default value 
        /// </summary>
        None,
        /// <summary>
        /// This rule is used to handle the Null WebElement scenario, it maybe Operaiton operated element, or Parameter value
        /// is a WebElementAttribute. 
        /// 
        /// Scenario: are seperated by the parameters 
        /// 1. Wait until Element find.    
        /// 2. Wait until Element find or timeout. If time out, stop engine.
        ///    [Time]
        /// 3. Wait until Element find or timeout. if time out, goto another pair operation. 
        ///    [Time][rule next operation]
        /// </summary>
        WaitUntilElemFind,
        /// <summary>
        /// Restart the script. No parameter.
        /// </summary>
        RestartScript,
        /// <summary>
        /// Go to next operation
        /// Scenario: 
        /// 1. go to another pair operation as next op
        ///    [rule next operation]
        /// </summary>
        Goto_Operation,
        /// <summary>
        /// refresh the web page, then wait a time span, then re-execute operation.
        /// 
        /// </summary>
        Refresh
    }    
    [Serializable]
    public enum PARAM_CMD
    {
        /// <summary>
        /// Assign a source value to the target parameter value. 
        /// params format : 
        /// [src obj][target parameter]
        /// 
        /// target parameter: must be String/Number type 
        /// src obj : it must match the target parameter type
        ///   1. Constants
        ///   2. WebElementAttribute,         
        ///   3. Parameter
        ///   4. Expression
        ///   5. Global functions
        /// </summary>
        ASSIGN,
        /// <summary>
        /// Update an existed Set parameter, add a new constant string or number into the set. 
        /// params format:
        /// [src obj][target parameter]
        /// Target parameter: must be a Set type.
        /// src obj : it must be the same type with set item type (String or number)
        ///   1. Constants, 
        ///   2. WebElementAttribute,        
        ///   3. Parameter(it can be another set), its value must match the type of target set item type
        ///   4. Expression
        ///   5. Global functions
        /// </summary>
        UPDATE_SET_ADD,
        /// <summary>
        /// /// <summary>
        /// Update an existed Set parameter, remove the first item that matched the src value, or do nothing if not found.
        /// params format:
        /// [src obj][target parameter]
        /// Target parameter: must be a Set type.
        /// src obj : it must be the same type with set item type (String or number)
        ///   1. Constants, 
        ///   2. WebElementAttribute,        
        ///   3. Parameter(it can be another set), its value must match the type of target set item type
        ///   4. Expression
        ///   5. Global functions
        /// </summary>
        /// </summary>
        UPDATE_SET_DEL
    }
    /// <summary>
    /// Global Function command types, each command defined its action and parameters. 
    /// it will be executed by engine. 
    /// </summary>
    [Serializable]
    public enum GF_CMD
    {
        NONE,
        /// <summary>
        /// Get the min value of the parameters, 
        /// it will return decimal.MinValue if not found 
        /// format:[number1][number2]...[numberN...]...
        /// Note: all parameters must be number
        /// </summary>
        NUM_GET_MIN,
        /// <summary>
        /// Get the max value of the parameters 
        /// it will return decimal.MinValue if not found 
        /// format:[number1][number2]...[numberN...]...
        /// Note: all parameters must be number
        /// </summary>
        NUM_GET_MAX,
        /// <summary>
        /// Get the average value of all the parameters 
        /// it will return decimal.MinValue if not found 
        /// format:[number1][number2]...[numberN...]...
        /// Note: all parameters must be number
        /// </summary>
        NUM_GET_AVG,
        /// <summary>
        /// Get new string from src string, that is start with start string and end with end string
        /// it will return empty string if not find
        /// format:
        /// [src string][start string][end string]
        /// </summary>
        STR_GET_BY_START_END,
        /// <summary>
        /// Get new string from src string, that is between start and end string. 
        /// it will return empty string if not find
        /// format:
        /// [src string][start string][end string]
        /// </summary>
        STR_GET_BTN_START_END,
        /// <summary>
        /// Split the string with specific string or regex. 
        /// all the splited sub strings will be assigned to substring parameters in order
        /// 
        /// format: [src string][regex string][substr1][substr2]...[substrN]...
        /// </summary>
        STR_SPLIT
    }
    #endregion enumeration definition.
    /// <summary>
    /// ============ WARNING !!!===============================================================================
    /// WARNING: BEList is only designed for ScriptModel internal use !!! If it is used outer, maybe caused strange behaviour. 
    /// e.g If you new a BEList belistA, then beListA add an existed Operation, it will destroy the operation's relationship
    /// in model, in another words, it will destroy the model, or if it do remove operation, it both destroy the model structure. 
    /// =======================================================================================================
    /// A BEList must be a member of a BaseElement, it's designed to used only under 
    /// BaseElement objects. all the T type should be a BaseElement (sub)class
    /// 
    /// Notes that use the BEList is used to reduce the complexity for list->children handling
    /// in model, You can see that a BEList is a true container of the children element. 
    /// The previous one is add add()/remove() method for each class, so that the class itself
    /// can handling the adding/remove operation, here use the BEList to delegate such operations to
    /// list itself, but there is a urgly thing that it must be a updateCollection() method specific for 
    /// each candidate types. But more, I think the BEList is seems a bit better. e.g. if a class has many 
    /// list for a same type, it is hard to use only one add() method to seperate which list to use. 
    /// </summary>
    [Serializable]
    public class BEList<T> {
        BaseElement _owner = null;
        /// <summary>
        /// owner of the list
        /// </summary>
        public BaseElement Owner {
            get { return _owner; }
            set { _owner = value; }
        }
        private List<T> list = new List<T>();
        
        public BEList() { }        
        /// <summary>
        /// Add a item, maybe it is duplicated. the same behaviour with System.Collections.Generic.List
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item) {
            list.Add(item);
            updateCollection(item, this);
        }
        /// <summary>
        /// Add all items, maybe it has some duplicated obj, the same behaviour with System.Collections.Generic.List
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<T> collection) {
            foreach (T item in collection) {
                Add(item);
            }
        }
        /// <summary>
        /// Add a unique item into the list, if have, it will do nothing
        /// </summary>
        /// <param name="item"></param>
        public void AddUnique(T item) {
            if (!list.Contains(item)) {
                list.Add(item);
                updateCollection(item, this);
            }
        }
        /// <summary>
        /// Add all items that is not contained in the list 
        /// </summary>
        /// <param name="collection"></param>
        public void AddRangeUnique(IEnumerable<T> collection) {
            foreach (T item in collection) {
                AddUnique(item);
            }            
        }
        public void Clear() {
            foreach (T item in this.list) {
                updateCollection(item, null);
            }
            list.Clear();
        }
        public bool Remove(T item) {
            updateCollection(item, null);
            return list.Remove(item);
        }
        public void RemoveAt(int index) {
            T item = list[index];
            updateCollection(item, null);
            list.RemoveAt(index);    
        }        
        public IEnumerator<T> GetEnumerator() {
            return list.GetEnumerator();
        }
        public List<T> GetRange(int index, int count) {
            return list.GetRange(index, count);
        }
        public int Count {
            get { return list.Count; }
        }
        public int IndexOf(T item) {
            return list.IndexOf(item);
        }
        public void Insert(int index, T item) {
            list.Insert(index, item);
            updateCollection(item,this);
        }
        /// <summary>
        /// get the object with index, or default(T) if errors 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T get(int index) {
            if (index >= 0 && index < list.Count) {
                return list[index];
            }
            return default(T);
        }
        /// <summary>
        /// Summary:
        ///     Determines whether an element is in the System.Collections.Generic.List<T>.
        ///
        /// Parameters:
        ///   item:
        ///     The object to locate in the System.Collections.Generic.List<T>. The value
        ///     can be null for reference types.
        ///
        /// Returns:
        ///     true if item is found in the System.Collections.Generic.List<T>; otherwise,
        ///     false.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item) {
            return list.Contains(item);
        }
        /// <summary>
        /// Summary:
        ///     Copies the elements of the System.Collections.Generic.List<T> to a new array.
        ///
        /// Returns:
        ///     An array containing copies of the elements of the System.Collections.Generic.List<T>.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray() {
            return list.ToArray();
        }
        /// <summary>
        /// update the item's collection info, carefully for this function. 
        /// e.g process should be ahead of Operation. 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="list">BEList<T> list </param>
        private void updateCollection(T item, object list) {
            // update the weakReference
            if (item is BaseElement) {
                BaseElement be = item as BaseElement;
                if (list == null) {
                    be.WeakRef.Remove(this);
                } else {
                    be.WeakRef.Add(this);
                }

                if (item is WebElementAttribute) {
                    WebElementAttribute wea = item as WebElementAttribute;
                    wea.setupBEList(list);
                } else if (item is WebElementGroup) {
                    WebElementGroup weg = item as WebElementGroup;
                    weg.setupBEList(list);
                } else if (item is WebElement) {
                    WebElement we = item as WebElement;
                    we.setupBEList(list);
                } else if (item is Process) {
                    Process proc = item as Process;
                    proc.setupBEList(list);
                } else if (item is Operation) {
                    Operation op = item as Operation;
                    op.setupBEList(list);
                } else if (item is ParamGroup) {
                    ParamGroup pgrp = item as ParamGroup;
                    pgrp.setupBEList(list);
                } else if (item is Parameter) {
                    Parameter param = item as Parameter;
                    param.setupBEList(list);
                } else if (item is OperationRule) {
                    OperationRule rule = item as OperationRule;
                    rule.setupBEList(list);
                } else if (item is OpCondition) {
                    OpCondition opc = item as OpCondition;
                    opc.setupBEList(list);
                } else if (item is ConditionGroup) {
                    ConditionGroup conGrp = item as ConditionGroup;
                    conGrp.setupBEList(list);
                } else if (item is Condition) {
                    Condition con = item as Condition;
                    con.setupBEList(list);
                } else if (item is UserLog) {
                    UserLog log = item as UserLog;
                    log.setupBEList(list);
                } else if (item is UserLogItem) {
                    UserLogItem logitem = item as UserLogItem;
                    logitem.setupBEList(list);
                } else if (item is Expression) {
                    Expression exp = item as Expression;
                    exp.setupBEList(list);
                } else if (item is GlobalFunction) {
                    GlobalFunction gf = item as GlobalFunction;
                    gf.setupBEList(list);
                } else if (item is ParamCmd) {
                    ParamCmd cmd = item as ParamCmd;
                    cmd.setupBEList(list);
                }
            }                        
        }
    }
    /// <summary>
    /// NOTES : Model Internal USED. 
    /// This list is an enhance list to help to resolve the delete element problem with dirty reference problem. 
    /// When add an new item, it will update the object of list into item(BaseElement)'s weakRef list. 
    /// </summary>
    [Serializable]
    public class ListRef
    {
        public static readonly int OP_ADD = 0;
        public static readonly int OP_RM = 1;

        private List<object> list = new List<object>();

        public ListRef() { }
        private BaseElement stub = null;
        /// <summary>
        /// Stub element of the list. maybe WEA, or WE, ParamCmd or others, that the ListRef object
        /// is one property of the stub. 
        /// </summary>
        public BaseElement Stub {
            get { return stub; }
            set { stub = value; }
        }

        /// <summary>
        /// Add a item, maybe it is duplicated. the same behaviour with System.Collections.Generic.List
        /// </summary>
        /// <param name="item"></param>
        public void Add(object item) {
            updateWeakRef(item, OP_ADD);
            list.Add(item);            
        }
        /// <summary>
        /// Add all items, maybe it has some duplicated obj, the same behaviour with System.Collections.Generic.List
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<object> collection) {
            foreach (object item in collection) {
                Add(item);
            }
        }
        /// <summary>
        /// Add a unique item into the list, if have, it will do nothing
        /// </summary>
        /// <param name="item"></param>
        public void AddUnique(object item) {
            if (!list.Contains(item)) {
                updateWeakRef(item, OP_ADD);
                list.Add(item);                
            }
        }
        /// <summary>
        /// Add all items that is not contained in the list 
        /// </summary>
        /// <param name="collection"></param>
        public void AddRangeUnique(IEnumerable<object> collection) {
            foreach (object item in collection) {
                AddUnique(item);
            }
        }
        public void Clear() {
            foreach (object item in this.list) {
                updateWeakRef(item, OP_RM);
            }
            list.Clear();
        }
        public bool Remove(object item) {
            updateWeakRef(item, OP_RM);
            return list.Remove(item);
        }
        public void RemoveAt(int index) {
            object item = list[index];
            updateWeakRef(item, OP_RM);
            list.RemoveAt(index);
        }
        public IEnumerator<object> GetEnumerator() {
            return list.GetEnumerator();
        }
        public List<object> GetRange(int index, int count) {
            return list.GetRange(index, count);
        }
        public int Count {
            get { return list.Count; }
        }
        public int IndexOf(object item) {
            return list.IndexOf(item);
        }
        public void Insert(int index, object item) {
            updateWeakRef(item, OP_ADD);
            list.Insert(index, item);            
        }
        /// <summary>
        /// get the object with index, or default(T) if errors 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object get(int index) {
            if (index >= 0 && index < list.Count) {
                return list[index];
            }
            return default(object);
        }
        /// <summary>
        /// Summary:
        ///     Determines whether an element is in the System.Collections.Generic.List<T>.
        ///
        /// Parameters:
        ///   item:
        ///     The object to locate in the System.Collections.Generic.List<T>. The value
        ///     can be null for reference types.
        ///
        /// Returns:
        ///     true if item is found in the System.Collections.Generic.List<T>; otherwise,
        ///     false.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(object item) {
            return list.Contains(item);
        }
        /// <summary>
        /// Summary:
        ///     Copies the elements of the System.Collections.Generic.List<T> to a new array.
        ///
        /// Returns:
        ///     An array containing copies of the elements of the System.Collections.Generic.List<T>.
        /// </summary>
        /// <returns></returns>
        public object[] ToArray() {
            return list.ToArray();
        }
        /// <summary>
        /// If item is a BaseElement, update the weakRef list. if it is a add operation, add current list into 
        /// the weakRef list, if it is a remove operation, remove current list in the item's weakRef list. 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="OP">0: add, 1, remove</param>
        private void updateWeakRef(object item, int OP) {
            if (item is BaseElement) {
                BaseElement be = item as BaseElement;                
                if (OP == OP_ADD) {
                    be.WeakRef.Add(this);
                } else if (OP == OP_RM) {
                    be.WeakRef.Remove(this);
                }
            }
        }
    }
}
