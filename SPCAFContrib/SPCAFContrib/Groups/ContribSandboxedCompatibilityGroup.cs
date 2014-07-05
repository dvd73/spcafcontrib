﻿using System.ComponentModel;
using SPCAF.Sdk;

namespace SPCAFContrib.Groups
{
    [Category("SPCAFContrib: Sandboxed Compatibility")]
    [Description("Checks files and artefacts whether they are compatible with Sandboxed solutions requirements.")]
    [VisitorGroupMetadata(Category = "SPCAFContrib: Sandboxed Compatibility", Description = "Checks files and artefacts whether they are compatible with Sandboxed solutions requirements.")]
    public class ContribSandboxedCompatibilityGroup : IVisitorGroup
    {
        #region properties

        public const string CategoryId = "56";

        #endregion
    }
}