using System.Text.RegularExpressions;

namespace SharePoint.Common.Utilities
{
    public class ListURLParser
    {
        // Fields
        private string _hostPattern = "(http://[^/]*)";
        private string _listUrlPattern = "/lists.+";
        private string _subWebPattern = @"(?<=http://[^/]*/)(\S*)(?=Lists)";
        private string _URL;

        // Methods
        public ListURLParser(string url)
        {
            this._URL = url;
        }

        private string GetMatch(string sample, string regexExpression)
        {
            if (Regex.IsMatch(sample, regexExpression, RegexOptions.Multiline | RegexOptions.IgnoreCase))
            {
                return Regex.Match(sample, regexExpression, RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
            }
            return string.Empty;
        }

        // Properties
        public bool IsUrlValid
        {
            get
            {
                return ((this.SPHostUrl != string.Empty) && (this.SPListUrl != string.Empty));
            }
        }

        public string SPHostUrl
        {
            get
            {
                return this.GetMatch(this._URL, this._hostPattern);
            }
        }

        public string SPListUrl
        {
            get
            {
                return this.GetMatch(this._URL, this._listUrlPattern);
            }
        }

        public string SPWebUrl
        {
            get
            {
                return this.GetMatch(this._URL, this._subWebPattern);
            }
        }
    }
}
