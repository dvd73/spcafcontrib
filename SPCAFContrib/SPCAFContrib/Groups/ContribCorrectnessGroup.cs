using System.ComponentModel;
using SPCAF.Sdk;

namespace SPCAFContrib.Groups
{
    [Category("SPCAFContrib: Correctness")]
    [Description("Checks for major issues and errors.")]
    [VisitorGroupMetadata(Category = "SPCAFContrib: Correctness", Description = "Checks for major issues and errors.")]
    public class ContribCorrectnessGroup : IVisitorGroup
    {
        #region properties

        public const string CategoryId = "51";

        #endregion
    }
}
