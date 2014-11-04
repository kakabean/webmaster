using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.lib.engine;

namespace WebMaster.ide.ui
{
    public interface IWEPropView
    {
        /// <summary>
        /// update the property view based on the element. e.g by specific string, by attributes, 
        /// by color, by location or by image, it depends on the WEType.
        /// if isNew == true, just reset UI and propview status and do update
        /// if isNew == false, jsut reset UI(except name and description area) and propview status and do update
        /// </summary>
        /// <param name="elem">input object of the property view</param>
        /// <param name="isNew">whether the element is a new created or just update the existed WebElement</param>
        void updateView(Object elem, bool isNew);
        /// <summary>
        /// clean the view as initialized, including ui info and maintained data info 
        /// </summary>
        void resetView();
        /// <summary>
        /// get the WebElement specified by the property view. 
        /// </summary>
        /// <returns>get the WebElement from the property view</returns>
        WebElement getWebElement();
        /// <summary>
        /// show the view from the parent container 
        /// </summary>
        void showView();
        /// <summary>
        /// hidden the view from the parent container 
        /// </summary>
        void hideView();
        /// <summary>
        /// set the view editable areas enabled 
        /// </summary>
        void enableView();
        /// <summary>
        /// set the view editable areas disabled
        /// </summary>
        void disableView();
        ///// <summary>
        ///// property view will show the proper error msg on demand, itself knows where and 
        ///// what message will be shown,
        ///// outer validation function can invoke this method if validation failed.
        ///// </summary>
        //void showErrorMsg();
        /// <summary>
        /// check whether the current modeled WebElement is valid. 
        /// 1. UI fields validation, whether the current filled fields are valid, e.g name area are mandatory.
        ///    or the WebElement name is used in script root.        
        /// </summary>
        /// <returns></returns>
        bool isValid();
        /// <summary>
        /// get the validation message if validation errors 
        /// </summary>
        /// <returns></returns>
        string getInvalidMsg();
        /// <summary>
        /// set up the script root unless it will be used. e.g check the WebElment unique
        /// </summary>
        /// <param name="sroot"></param>
        void setScriptRoot(ScriptRoot sroot);
        /// <summary>
        /// this method is called by outer control if its sized changed
        /// and need the property view updated its size
        /// </summary>
        void handleSizeChangedEvt();
        /// <summary>
        /// this class it called by outer control if it want the properties view 
        /// reset to the original size
        /// </summary>
        void resetUISize();
    }
}
