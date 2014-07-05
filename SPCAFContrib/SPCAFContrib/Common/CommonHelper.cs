using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SPCAF.Sdk.Common;
using SPCAF.Sdk.Logging;
using SPCAFContrib.Entities.Consts;

namespace SPCAFContrib.Common
{
    internal class CommonHelper
    {
        public static bool FileHasExcluded(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                string fileName = Path.GetFileName(path);
                if (!String.IsNullOrEmpty(fileName))
                {
                    bool result = ExcludedItems.Files.Any(
                            s => new Wildcard(s, RegexOptions.IgnoreCase).IsMatch(fileName));
                    //LoggingService.Log(LogLevel.Info, result + " Check for excluded file: " + fileName);

                    return result;
                }
            }
            return false;
        }
    }
}
