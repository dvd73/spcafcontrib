using System;
using MOSS.Common.Pages;

namespace MOSS.Common.Code
{
    public class ScriptRequestEventArgs : EventArgs, ICustomScript
    {
        public CustomScriptType ScriptType { get; set; }        
    }
}
