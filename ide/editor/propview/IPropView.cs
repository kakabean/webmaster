using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMaster.ide.editor.propview
{
    interface IPropView
    {
        /// <summary>
        /// update the input object with properties view values, in this method, it should not raise any event
        /// in this method
        /// </summary>
        /// <returns></returns>
        void updatedInput();
        /// <summary>
        /// update the property view based on input element. e.g a diagram, operation 
        /// or a connection. 
        /// </summary>
        /// <param name="input">input object of the property view</param>
        void setInput(Object input);
        /// <summary>
        /// get the input object 
        /// </summary>
        /// <returns></returns>
        Object getInput();
        /// <summary>
        /// clean the view as initialized 
        /// </summary>
        void cleanView();
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
    }
}
