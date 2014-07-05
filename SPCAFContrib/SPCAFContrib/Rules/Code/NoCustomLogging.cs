using System;
using System.Collections.Generic;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Rules.Code
{
     [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.NoCustomLogging,
     Help = CheckIDs.Rules.Assembly.NoCustomLogging_HelpUrl,

     Message = "Do not use custom logging [{0}].",
     DisplayName = "Do not use custom logging tools.",
     Description = "Avoid introducing 3-rd part logging like EventLog or NLog or log4net. It is required web.config changes or affects to solution security.",
     Resolution = "Use SPDiagnosticsService class to log.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links=new []
     {
         "SPDiagnosticsService class",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.administration.spdiagnosticsservice.aspx"
     })]
    public class NoCustomLogging : Rule<AssemblyFileReference>
     {
         #region methods

         public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
         {
             if (assembly.AssemblyDefinition == null)
                 assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

             if (assembly.AssemblyHasExcluded()) return;

             bool hasSharePointReference = assembly.AssemblyReferencesSharePointAssembly();
             if (!hasSharePointReference) return;

             OnMatch(assembly, notifications, "EventLog", TypeKeys.EventLog, assembly.MethodsUsingType(TypeKeys.EventLog));
             OnMatch(assembly, notifications, "NLog", TypeKeys.NLog, assembly.MethodsUsingType(TypeKeys.NLog));
             OnMatch(assembly, notifications, "log4net", TypeKeys.log4net, assembly.MethodsUsingType(TypeKeys.log4net));
             OnMatch(assembly, notifications, "CommonLogging", TypeKeys.CommonLogging, assembly.MethodsUsingType(TypeKeys.CommonLogging));
             OnMatch(assembly, notifications, "Logging Application Block", TypeKeys.EL_Logging_Application_Block, assembly.MethodsUsingType(TypeKeys.EL_Logging_Application_Block));
             OnMatch(assembly, notifications, "ObjectGuy Framework", TypeKeys.ObjectGuy, assembly.MethodsUsingType(TypeKeys.ObjectGuy));
             OnMatch(assembly, notifications, "C# Logger", TypeKeys.CSharpLogger, assembly.MethodsUsingType(TypeKeys.CSharpLogger));
             OnMatch(assembly, notifications, "C# .NET Logger", TypeKeys.CSharpDotNETLogger, assembly.MethodsUsingType(TypeKeys.CSharpDotNETLogger));
             OnMatch(assembly, notifications, "Logger .NET", TypeKeys.LoggerNET, assembly.MethodsUsingType(TypeKeys.LoggerNET));
             OnMatch(assembly, notifications, "LogThis", TypeKeys.LogThis, assembly.MethodsUsingType(TypeKeys.LogThis));
             OnMatch(assembly, notifications, "NetTrace", TypeKeys.NetTrace, assembly.MethodsUsingType(TypeKeys.NetTrace));
             OnMatch(assembly, notifications, "NSpring", TypeKeys.NSpring, assembly.MethodsUsingType(TypeKeys.NSpring));
             OnMatch(assembly, notifications, "Loggr", TypeKeys.Loggr, assembly.MethodsUsingType(TypeKeys.Loggr));
             OnMatch(assembly, notifications, "ELMAH", TypeKeys.ELMAH_SqlErrorLog, assembly.MethodsUsingType(TypeKeys.ELMAH_SqlErrorLog));
             OnMatch(assembly, notifications, "ELMAH", TypeKeys.ELMAH_SqlServerCompactErrorLog, assembly.MethodsUsingType(TypeKeys.ELMAH_SqlServerCompactErrorLog));
             OnMatch(assembly, notifications, "ELMAH", TypeKeys.ELMAH_SQLiteErrorLog, assembly.MethodsUsingType(TypeKeys.ELMAH_SQLiteErrorLog));
             OnMatch(assembly, notifications, "ELMAH", TypeKeys.ELMAH_MemoryErrorLog, assembly.MethodsUsingType(TypeKeys.ELMAH_MemoryErrorLog));
             OnMatch(assembly, notifications, "ELMAH", TypeKeys.ELMAH_OracleErrorLog, assembly.MethodsUsingType(TypeKeys.ELMAH_OracleErrorLog));
             OnMatch(assembly, notifications, "ELMAH", TypeKeys.ELMAH_MySqlErrorLog, assembly.MethodsUsingType(TypeKeys.ELMAH_MySqlErrorLog));
             OnMatch(assembly, notifications, "ELMAH", TypeKeys.ELMAH_XmlFileErrorLog, assembly.MethodsUsingType(TypeKeys.ELMAH_XmlFileErrorLog));
             OnMatch(assembly, notifications, "ELMAH", TypeKeys.ELMAH_PgsqlErrorLog, assembly.MethodsUsingType(TypeKeys.ELMAH_PgsqlErrorLog));
             OnMatch(assembly, notifications, "ELMAH", TypeKeys.ELMAH_AccessErrorLog, assembly.MethodsUsingType(TypeKeys.ELMAH_AccessErrorLog));
         }

         private void OnMatch(AssemblyFileReference assembly, NotificationCollection notifications, string loggerName,string typeName, IEnumerable<MethodDefinition> customLogUsages)
         {
             foreach (MethodDefinition eventLogUsage in customLogUsages)
             {
                 foreach (CodeInstruction instruction in eventLogUsage.AllMethodReferences())
                 {
                     MethodReference methodInvokation = instruction.Instruction.Operand as MethodReference;
                     if (methodInvokation != null)
                     {
                         if (String.Equals(methodInvokation.DeclaringType.FullName, typeName, StringComparison.OrdinalIgnoreCase))
                         {
                             Notify(eventLogUsage, String.Format(this.MessageTemplate(), loggerName),
                                 instruction.ImproveSummary(assembly.GetSummary()), notifications);
                         }
                     }
                 }
             }
         }

         #endregion
    }
}
