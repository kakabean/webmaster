using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.lib.engine;

namespace WebMaster.ide.editor.mapping
{
    /// <summary>
    /// Parameter mapping source part interface 
    /// </summary>
    interface IMappingSrc
    {
        /// <summary>
        /// Get the mapping src current expression text, or string.Empty if errors 
        /// </summary>
        /// <returns></returns>
        string getExpression();
        /// <summary>
        /// return the validatoin msg, or return null if it is valid
        /// </summary>
        /// <returns></returns>
        string getValidMsg();
        /// <summary>
        /// return true if the source is valid, or return false if it is invalid. 
        /// </summary>
        /// <returns></returns>
        bool isValid();       
        /// <summary>
        /// Get mapping source object or null if errors . 
        /// src obj allowed type is :
        ///   1. Constants
        ///   2. WebElementAttribute,         
        ///   3. Parameter
        ///   4. Expression
        ///   5. Global functions
        /// </summary>
        /// <returns></returns>
        object getMappingSrc();
        /// <summary>
        /// update the panel content with src object and show. 
        /// src obj allowed type is :
        ///   1. Constants
        ///   2. WebElementAttribute,         
        ///   3. Parameter
        ///   4. Expression
        ///   5. Global functions
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcType">allowed value should be String or Number. 
        /// It is the final source value type, default type is ParamType.String.</param>
        void show(object src,ParamType srcType);
        void hidden();
    }
}
