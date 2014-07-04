﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCAFContrib.Consts
{
    public class ExcludedLibraries
    {
        #region properties

        public static readonly List<string> Assemblies = new List<string>()
        {
            "Aspose.*.dll",
            "Telerik.*.dll",
            "System.*.dll",
            "Microsoft.*.dll",
            "DocumentFormat.OpenXml.dll",
            "ClosedXml.dll",
            "NVelocity.dll",
            "Camlex.NET.dll",
            "AjaxControlToolkit.dll",
            "NPOI.dll",
            "NLog.dll",
            "Elmah.dll",
            "log4net.dll",
            "loggr-dotnet.dll",
            "nspring.dll",
            "Logger.NET.dll",
            "NetTrace.dll",
            "csharp-logger.dll",
            "Common.Logging.dll",
            "gsdll32.dll",
            "gsdll64.dll",
            "EntityFramework.dll",
            "EntityFramework.*.dll",
            "Bamboo.*.dll"
        };

        #endregion
    }
}
