using System;
using System.Collections.Generic;
using System.Linq;

namespace SPCAFContrib.ReSharper.Common
{
    [Flags]
    public enum IDEProjectType : uint
    {
        Unknown                 = 0,
        /// <summary>
        /// SharePoint 2010 farm solution
        /// </summary>
        SP2010FarmSolution      = 1,
        /// <summary>
        /// SharePoint sandbox solution
        /// </summary>
        SPSandbox               = 2 << 0,
        /// <summary>
        /// SharePoint 2013 farm solution
        /// </summary>
        SP2013FarmSolution      = 2 << 1,
        /// <summary>
        /// Apps for SharePoint 2013
        /// </summary>
        SP2013App              = 2 << 2,
        /// <summary>
        /// Any project with Microsoft.Sharepoint.dll reference
        /// </summary>
        SPServerAPIReferenced   = 2 << 3
    }

    public enum SPFeatureScope
    {
        Web,
        Site,
        WebApplication,
        Farm
    }

    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
    
}
