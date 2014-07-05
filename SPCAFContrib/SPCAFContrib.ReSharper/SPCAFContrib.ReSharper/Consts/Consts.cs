using System;
using JetBrains.ReSharper.Daemon;
using SPCAFContrib.ReSharper.Consts;

[assembly: RegisterConfigurableHighlightingsGroup(Consts.CORRECTNESS_GROUP, Consts.CORRECTNESS_GROUP)]
[assembly: RegisterConfigurableHighlightingsGroup(Consts.BEST_PRACTICE_GROUP, Consts.BEST_PRACTICE_GROUP)]
[assembly: RegisterConfigurableHighlightingsGroup(Consts.SANDBOX_COMPATIBILITY_GROUP, Consts.SANDBOX_COMPATIBILITY_GROUP)]

namespace SPCAFContrib.ReSharper.Consts
{
    public sealed class Consts
    {
        public const string CORRECTNESS_GROUP = "SPCAF Contrib. Correctness";
        public const string BEST_PRACTICE_GROUP = "SPCAF Contrib. Best practice";
        public const string SANDBOX_COMPATIBILITY_GROUP = "SPCAF Contrib. Sandbox compatibility";

        public static Guid[] SP2010SolutionProjectTypeGuids = new[] { Guid.Parse("{BB1F664B-9266-4fd6-B973-E1E44974B511}"), Guid.Parse("{14822709-B5A1-4724-98CA-57A101D1B079}"), Guid.Parse("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") };
        public static Guid[] SP2013SolutionProjectTypeGuids = new[] { Guid.Parse("{C1CDDADD-2546-481F-9697-4EA41081F2FC}"), Guid.Parse("{14822709-B5A1-4724-98CA-57A101D1B079}"), Guid.Parse("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") };
        public static Guid[] SP2013AppsSolutionProjectTypeGuids = new[] { Guid.Parse("{349c5851-65df-11da-9384-00065b846f21}"), Guid.Parse("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") };
    }
}
