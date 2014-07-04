using System;
using System.Collections.Generic;
using Microsoft.SharePoint.WebControls;

namespace MOSS.Common.Code
{
    /// <summary>
    /// Helper class to store display exception rules
    /// </summary>    
    public class FieldDisplayRuleItem
    {
        /// <summary>
        /// list of field names the rule applies to
        /// </summary> 
        public List<String> FieldNames { get; set; }
        /// <summary>
        /// list of field control modes the rule applies to
        /// </summary> 
        public List<SPControlMode> ControlModes { get; set; }
        /// <summary>
        /// the resulting display exeption rule
        /// </summary> 
        public FieldDisplayRule Rule { get; set; }
    }
    
}
