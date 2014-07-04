using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Common.Enums
{
    public enum IssuePriority
    {
        High    = 1,
        Normal  = 2,
        Low     = 2
    }

    public enum IssueSeverity
    {
        Error,
        Warning,
        Information
    }

    public enum IssueStatus
    {
        Active,
        Resolved,
        Closed
    }
}
