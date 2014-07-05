using System.ComponentModel;
using SPCAF.Sdk;

namespace SPCAFContrib.Groups
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
