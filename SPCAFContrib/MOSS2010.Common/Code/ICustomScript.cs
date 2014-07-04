using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOSS.Common.Code
{
    public interface ICustomScript
    {
        CustomScriptType ScriptType { get; set;}
    }

    [Flags]    
    public enum CustomScriptType : uint
    {
        /// <summary>
        /// jquery library
        /// </summary>
        JQueryScript            = 0,        
        /// <summary>
        /// jquery ui library
        /// </summary>
        JQueryUIScript          = 1,
        /// <summary>
        /// jquery theme css
        /// </summary>
        JQueryUITheme           = 2 << 0,
        /// <summary>
        /// This is a jQuery UI time picker plugin build to match with other official jQuery UI widgets. Based on the existing date picker, it will blend nicely with your form and use your selected jQuery UI theme. The plugin is very easy to integrate in your form for you time (hours / minutes) inputs.
        /// </summary>
        TimePickerScript        = 2 << 1,
        /// <summary>
        /// Expand/Collapse Complex Content
        /// </summary>
        ExpandScript            = 2 << 2,
        /// <summary>
        /// SPServices is a jQuery library which abstracts SharePoint's Web Services and makes them easier to use. It also includes functions which use the various Web Service operations to provide more useful (and cool) capabilities. It works entirely client side and requires no server install.
        /// </summary>
        SPServiceScript         = 2 << 3,
        /// <summary>
        /// List all attachments when clicking the paperclip in a list view
        /// </summary>
        ListAttachments         = 2 << 4,
        /// <summary>
        /// Resize popup dialog frame
        /// </summary>
        ResizeIframe            = 2 << 5,
        /// <summary>
        /// Ribbon будет скрыт, только если страница запущена в модальном диалоге, и только если она находится в режиме просмотра. В режиме редактирования лента будет отображаться.
        /// </summary>
        HideRibbonInDlg         = 2 << 6,
        /// <summary>
        /// Ensure that current poge ribbon set to Browse tab
        /// </summary>
        ActivateRibbonReadTab   = 2 << 7,
        /// <summary>        
        /// The video player for Web
        /// <see href="http://flowplayer.org/">http://flowplayer.org/</see>
        /// </summary>
        FlowPlayer              = 2 << 8,
        /// <summary>
        /// Superfish menu
        /// </summary>
        SuperFishMenu           = 2 << 9,
        /// <summary>
        /// jQuery Cookie Plugin
        /// </summary>
        JQueryCookies           = 2 << 10,
        /// <summary>
        /// jQuery Hotkeys Plugin
        /// </summary>
        JQueryHotkeys           = 2 << 11,
        /// <summary>
        /// JQUERY accordion for sharepoint 2010 quick launch. 
        /// <see href="http://www.lifeinsharepoint.co.uk/2011/12/15/quick-tip-jquery-accordion-quick-launch/">http://www.lifeinsharepoint.co.uk/2011/12/15/quick-tip-jquery-accordion-quick-launch/</see>
        /// </summary>
        JQueryQuickLaunch       = 2 << 12,
        /// <summary>
        /// Tree component. 
        /// <see href="http://www.jstree.com/">http://www.jstree.com/</see>
        /// </summary>
        JSTree                  = 2 << 13,
        /// <summary>
        /// Dynamic JavaScript tree view control with support for checkboxes, drag'n'drop, and lazy loading
        /// </summary>
        DynaTree                = 2 << 14,
        /// <summary>
        /// Sleek, intuitive, and powerful front-end framework for faster and easier web development.
        /// <see href="http://getbootstrap.com/">http://getbootstrap.com/</see>
        /// </summary>
        Bootstrap               = 2 << 15,
        /// <summary>
        /// This jQuery plugin makes simple clientside form validation easy, whilst still offering plenty of customization options.
        /// <see href="http://jqueryvalidation.org/">http://jqueryvalidation.org/</see>
        /// </summary>
        JQueryValidation        = 2 << 16
    }
}
