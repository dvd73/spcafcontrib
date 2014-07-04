using SPCAF.Sdk;
using System.ComponentModel;

namespace SPCAFContrib.Consts
{
    [Category("SPCAFContrib: Recommendations")]
    [Description("Checks for minor issues and suggestions.")]
    [VisitorGroupMetadata(Category = "SPCAFContrib: Recommendations", Description = "Checks for minor issues and suggestions.")]
    public class ContribRecommendationsGroup : IVisitorGroup
    {
        #region properties

        public const string CategoryId = "53";

        #endregion
    }
}
