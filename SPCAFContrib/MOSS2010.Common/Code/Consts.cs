using System;

namespace MOSS.Common.Code
{
    public class Consts
    {
        public static string IFRAME_TMP_FILE_PATH = String.Format("~/{0}", IFRAME_TMP_FILE_NAME);
        public const string IFRAME_TMP_FILE_NAME = "Iframe4357.aspx";

        public static string DISABLE_TEXT_BOX_SCRIPT = "\r\n$(\"[id^='{0}']\").attr(\"disabled\", \"disabled\");\r\n";
        public static string DISABLE_CALENDAR_SCRIPT = "\r\n$(\"img[id='{0}']\").attr(\"src\", \"/_layouts/images/calendar_grey.gif\");\r\n   \r\n$(\"img[id='{0}']\").parent().prop(\"onclick\", null);\r\n";
        public static string DISABLE_PEOPLE_PICKER_SCRIPT = "\r\n$(\"div.ms-inputuserfield\").attr(\"contentEditable\", \"false\");\r\n" + 
            "\r\n$(\"img[id='{0}']\").attr(\"src\", \"/_layouts/images/MOSS.Common/checknames_gray.png\");\r\n   \r\n$(\"img[id='{0}']\").parent().prop(\"onclick\", null);\r\n" +
            "\r\n$(\"img[id='{0}']\").attr(\"src\", \"/_layouts/images/MOSS.Common/addressbook_gray.gif\");\r\n   \r\n$(\"img[id='{0}']\").parent().prop(\"onclick\", null);\r\n";
        public static string DISABLE_CHOISE_SCRIPT = "\r\n$(\"[id^='{0}']\").attr(\"disabled\", \"disabled\");\r\n";

        public static string DISABLE_ALL_TEXT_BOX_SCRIPT = "\r\n$(\"[id$='TextField']\").attr(\"readonly\", \"readonly\").css(\"background-color\", \"#EBEBE4\");\r\n";
        public static string DISABLE_ALL_CALENDAR_SCRIPT = "\r\n$(\"[id$='DateTimeFieldDate']\").attr(\"disabled\", \"disabled\");\r\n$(\"img[id$='DateTimeFieldDateDatePickerImage']\").attr(\"src\", \"/_layouts/images/calendar_grey.gif\");\r\n   \r\n$(\"img[id$='DateTimeFieldDateDatePickerImage']\").parent().css( \"cursor\",\"default\" ).prop(\"onclick\", null);\r\n";
        public static string DISABLE_ALL_PEOPLE_PICKER_SCRIPT = "\r\n$(\"div.ms-inputuserfield\").attr(\"contentEditable\", \"false\");\r\n" + 
            "\r\n$(\"a[id$='UserField_checkNames'] > img\").attr(\"src\", \"/_layouts/images/MOSS.Common/checknames_gray.png\");\r\n   \r\n$(\"a[id$='UserField_checkNames'] > img\").parent().css( \"cursor\",\"default\" ).prop(\"onclick\", null);\r\n" +
            "\r\n$(\"a[id$='UserField_browse'] > img\").attr(\"src\", \"/_layouts/images/MOSS.Common/addressbook_gray.gif\");\r\n   \r\n$(\"a[id$='UserField_browse']  > img\").parent().css( \"cursor\",\"default\" ).prop(\"onclick\", null);\r\n";
        public static string DISABLE_ALL_CHOISE_SCRIPT = "\r\n$(\"[id$='DropDownChoice']\").attr(\"disabled\", \"disabled\");\r\n";
        public static string DISABLE_ALL_LOOKUP_SCRIPT = "\r\n$(\"[id$='Lookup']\").attr(\"disabled\", \"disabled\");\r\n";

        public static string CANCEL_POPUP_DIALOG_SCRIPT = "<script type='text/javascript'>window.frameElement.cancelPopUp();</script>";
        public static string COMMIT_POPUP_DIALOG_SCRIPT = "<script type='text/javascript'>window.frameElement.commitPopUp();</script>";
        public static string COMMIT_POPUP_DIALOG2_SCRIPT = "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose(1, 1);</script>";
        public static string AUTO_LENGTH_TEXT_BOX_SCRIPT = "\r\n$('{0}').bind('keypress', function(){{$(this).attr('size', $(this).val().length);}})\r\n";

        public static string NEW_ITEM_ATTACHMENT_URL = "http://newitemurl/";

        public const string PAGING_PARAM_NAME = "p";
        public const string FIELD_PARAM_NAME = "f";
        public const string MONTH_PARAM_NAME = "m";
        public const string YEAR_PARAM_NAME = "y";

        public static Guid ARTICLE_START_DATE_FIELD_ID = new Guid("71316CEA-40A0-49f3-8659-F0CEFDBDBD4F");

        public const string NO_RECORDS_MARK = "No records";        
    }
}
