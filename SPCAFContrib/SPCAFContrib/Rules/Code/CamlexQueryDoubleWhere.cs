using System;
using System.Collections.Generic;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Common;
using SPCAFContrib.Consts;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
     [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.CamlexQueryDoubleWhere,
     Help = CheckIDs.Rules.Assembly.CamlexQueryDoubleWhere_HelpUrl,

     DisplayName = "Multiple WHERE clauses is not supported for Camlex.Query.",
     Message = "Multiple WHERE clauses is not supported for Camlex.Query.",
     Description = "CamlexNET.Interfaces.IQuery interface is remains last WHERE clause only.",
     Resolution = "Use ExpressionHelper static class to combine set of conditions to single WHERE clause.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new[]
     {
         "Camlex.NET",
         "https://camlex.codeplex.com/",
     })]
    public class CamlexQueryDoubleWhere : SearchMethodRuleBase
    {
         MultiValueDictionary<Mono.Cecil.MethodDefinition, MethodReference> instructions = new MultiValueDictionary<Mono.Cecil.MethodDefinition, MethodReference>();

         protected override void PopulateTypeMap()
         {
            TargetTypeMap.Add(TypeKeys.CamlexIQuery, new List<string>(){"Where", "WhereAll", "WhereAny"});
         }

         protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction, NotificationCollection notifications,
             Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
         {
             if (!instructions.TryAdd(instruction.MethodDefinition, instruction.Instruction.Operand as MethodReference))
             {
                 base.OnMatch(assembly, instruction, notifications, getNotificationMessage, getSummary);  
             }
             else
             {
                 if (instructions[instruction.MethodDefinition].Count > 1)
                     base.OnMatch(assembly, instruction, notifications, getNotificationMessage, getSummary);  
             }
         }
    }
}
