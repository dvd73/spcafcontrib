using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPCAF.Sdk.Model;

namespace SPCAFContrib.Extensions
{
    public static class ModuleDefinitionExtensions
    {
        #region methods

        public static IEnumerable<FileDefinition> GetFiles(this ModuleDefinition module, string fileExtension)
        {
            if (module.File == null)
                return Enumerable.Empty<FileDefinition>();

            return module.File.Where(f => !string.IsNullOrEmpty(f.Path) &&
                                          !string.IsNullOrEmpty(Path.GetExtension(f.Path)) &&
                                          Path.GetExtension(f.Path).ToUpper().Contains(fileExtension.ToUpper()));
        }

        #endregion
    }
}
