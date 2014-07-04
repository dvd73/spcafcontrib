using SPCAF.Sdk;
using System.ComponentModel;

namespace SPCAFContrib.Consts
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
