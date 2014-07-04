using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Common;
using SPCAFContrib.Consts;
using SPCAF.Sdk.Logging;

namespace SPCAFContrib.Extensions
{
    public static class MethodDefinitionExtensions
    {
        #region classes

        private enum InstructionRangeType
        {
            Try,
            Catch
        }

        private class InstrustionRange
        {
            public int Start { get; set; }
            public int End { get; set; }

            public InstructionRangeType RangeType { get; set; }
        }

        #endregion

        #region properties

        private static readonly List<string> AllowedUnwrappedInstructions = new List<string>{
             "nop",
             "ret"
        };

        private static readonly List<string> RequiredCatchInstructions = new List<string>
            {
             "call",
             "rethrow",
             "throw"
        };

        #endregion

        #region utils

        private static IEnumerable<Instruction> GetTryInstructions(this MethodDefinition method)
        {
            List<Instruction> result = new List<Instruction>();

            if (!method.HasBody)
                return result;

            List<InstrustionRange> coveredExcecutionRanges = GetTryCatchExcecutionRanges(method);
            Collection<Instruction> instructionCollection = method.Body.Instructions;

            if (coveredExcecutionRanges.Count > 0)
            {
                for (int i = 0; i <= instructionCollection.Count - 1; i++)
                {
                    Instruction currentOperation = instructionCollection[i];

                    if (coveredExcecutionRanges
                                        .Where(r => r.RangeType == InstructionRangeType.Try)
                                        .All(r => (currentOperation.Offset >= r.Start && currentOperation.Offset <= r.End)))
                    {
                        result.Add(currentOperation);
                    }
                }
            }

            return result;
        }

        private static List<Instruction> GetCatchInstructions(this MethodDefinition method)
        {
            List<Instruction> result = new List<Instruction>();

            if (!method.HasBody)
                return result;

            List<InstrustionRange> coveredExcecutionRanges = GetTryCatchExcecutionRanges(method);
            Collection<Instruction> instructionCollection = method.Body.Instructions;

            if (coveredExcecutionRanges.Count > 0)
            {
                for (int i = 0; i <= instructionCollection.Count - 1; i++)
                {
                    Instruction currentOperation = instructionCollection[i];

                    if (coveredExcecutionRanges
                                        .Where(r => r.RangeType == InstructionRangeType.Catch)
                                        .Any(r => (currentOperation.Offset >= r.Start && currentOperation.Offset <= r.End)))
                    {
                        result.Add(currentOperation);
                    }
                }
            }

            return result;
        }

        private static List<Instruction> GetInstructionsWithinRange(this MethodDefinition method, InstrustionRange range)
        {
            List<Instruction> result = new List<Instruction>();

            if (!method.HasBody)
                return result;

            Collection<Instruction> instructionCollection = method.Body.Instructions;

            for (int i = 0; i <= instructionCollection.Count - 1; i++)
            {
                Instruction currentOperation = instructionCollection[i];

                if (currentOperation.Offset >= range.Start && currentOperation.Offset <= range.End)
                    result.Add(currentOperation);
            }

            return result;
        }

        private static List<Instruction> GetNotCoveredInstructions(this MethodDefinition method)
        {
            List<Instruction> result = new List<Instruction>();

            if (!method.HasBody)
                return result;

            List<InstrustionRange> coveredExcecutionRanges = GetTryCatchExcecutionRanges(method);
            Collection<Instruction> instructionCollection = method.Body.Instructions;

            for (int i = 0; i <= instructionCollection.Count - 1; i++)
            {
                Instruction currentOperation = instructionCollection[i];

                if (coveredExcecutionRanges.All(r => currentOperation.Offset < r.Start || currentOperation.Offset > r.End))
                    result.Add(currentOperation);
            }

            return result;
        }

        private static bool HasNotEmptyAndAllowedInstuction(IEnumerable<Instruction> instructions)
        {
            Instruction[] array = instructions.ToArray();

            return array.Any() &&
                   array.Any(i => RequiredCatchInstructions.Contains(i.OpCode.Name, StringComparer.CurrentCultureIgnoreCase));
        }

        private static bool HasNotEmptyAndAllowedUnwrappedInstructions(IEnumerable<Instruction> instructions)
        {
            Instruction[] array = instructions.ToArray();

            return array.Any() &&
                   array.All(i => AllowedUnwrappedInstructions.Contains(i.OpCode.Name, StringComparer.CurrentCultureIgnoreCase));
        }

        private static List<InstrustionRange> GetExcecutionRanges(this MethodDefinition method, ExceptionHandlerType type)
        {
            List<InstrustionRange> result = new List<InstrustionRange>();

            if (!method.HasBody)
                return result;

            Collection<ExceptionHandler> handlers = method.Body.ExceptionHandlers;

            foreach (ExceptionHandler handler in handlers.Where(h => h.HandlerType == type))
            {
                InstrustionRange tryRange = new InstrustionRange
                {
                    Start = handler.TryStart.Offset,
                    End = handler.TryEnd.Offset,
                    RangeType = InstructionRangeType.Try
                };

                InstrustionRange catchRange = new InstrustionRange
                {
                    Start = handler.HandlerStart.Offset,
                    End = handler.HandlerEnd.Offset,
                    RangeType = InstructionRangeType.Catch
                };

                List<Instruction> tangeInstructions = GetInstructionsWithinRange(method, tryRange);

                if (!HasForEachInside(tangeInstructions) &&
                    !HasUsingInside(GetInstructionsWithinRange(method, tryRange), GetInstructionsWithinRange(method, catchRange)))
                {
                    result.Add(tryRange);
                    result.Add(catchRange);
                }
            }

            return result;
        }

        private static List<Instruction> WithoutNop(IEnumerable<Instruction> instructions)
        {
            return instructions.Where(i => i.OpCode != OpCodes.Nop).ToList();
        }

        private static bool HasUsingInside(IEnumerable<Instruction> tryInstructions, IEnumerable<Instruction> finallyInstructions)
        {
            List<Instruction> fixedTryInstructions = WithoutNop(tryInstructions);
            List<Instruction> fixedFinallyInstructions = finallyInstructions
                                                .Where(i => i.OpCode == OpCodes.Callvirt ||
                                                           i.OpCode == OpCodes.Endfinally)
                                                .ToList();

            //callvirt System.Void System.IDisposable::Dispose()]
            //Op:[IL_002c: endfinally]

            // TODO, CRAP CRAP CRAP
            // donno how to etec using() in other way, sorry

            if (fixedTryInstructions.Count < 1 || fixedFinallyInstructions.Count < 2)
                return false;

            Instruction lastTry = fixedTryInstructions[fixedTryInstructions.Count - 1];
            //var prevLastTry = fixedTryInstructions[fixedTryInstructions.Count - 2];

            Instruction lastFinally = fixedFinallyInstructions[fixedFinallyInstructions.Count - 1];
            Instruction prevlastFinally = fixedFinallyInstructions[fixedFinallyInstructions.Count - 2];

            List<OpCode> lsDoc = new List<OpCode>
            {
                OpCodes.Ldloc,
                OpCodes.Ldloc_0,
                OpCodes.Ldloc_1,
                OpCodes.Ldloc_2,
                OpCodes.Ldloc_3,
                OpCodes.Ldloc_S
            };

            //result = lsDoc.Contains(lastTry.OpCode) && prevLastTry.OpCode == OpCodes.Leave_S &&
            return lsDoc.Contains(lastTry.OpCode) &&
                          lastFinally.OpCode == OpCodes.Endfinally && prevlastFinally.OpCode == OpCodes.Callvirt;
        }

        private static bool HasForEachInside(List<Instruction> instructions)
        {
            // TODO
            // ad-hoc, hsd to be fixed later
            // call enumerator ::get_Current()   

            if (instructions.Count > 2)
            {
                return CheckForEachCase(instructions.ToList()[2]);
            }

            return false;
        }

        private static bool CheckForEachCase(Instruction instruction)
        {
            MethodReference methodRef = instruction.Operand as MethodReference;

            if (methodRef != null)
            {
                string methodRefString = methodRef.ToString();

                return methodRefString.Contains("::get_Current()") &&
                       methodRefString.Contains("Enumerator");
            }
            return false;
        }

        //private static List<InstrustionRange> GetFinallyExcecutionRanges(this MethodDefinition method)
        //{
        //    return GetExcecutionRanges(method, ExceptionHandlerType.Finally);
        //}

        private static List<InstrustionRange> GetTryCatchExcecutionRanges(this MethodDefinition method)
        {
            List<InstrustionRange> result = new List<InstrustionRange>();

            result.AddRange(GetExcecutionRanges(method, ExceptionHandlerType.Catch));
            result.AddRange(GetExcecutionRanges(method, ExceptionHandlerType.Finally));

            return result;
        }

        private static void TraceMessage(string message)
        {
            Trace.WriteLine(message);
        }

        private static void TraceInstructions(MethodDefinition method, IEnumerable<Instruction> instructions)
        {
            TraceMessage(string.Format("Method:[{0}]", method.Name));

            List<Instruction> t = instructions.ToList();

            for (int i = 0; i <= t.Count - 1; i++)
            {
                Instruction currentOperation = t[i];
                Trace.WriteLine(currentOperation);
                TraceMessage(string.Format("   Op:[{0}]", currentOperation));
            }
        }

        #endregion

        #region unsafe sp object string comporations

        private static bool IsSafeSPObjectStringCoporatinOperator(Instruction instruction)
        {
            if (instruction == null)
                return true;

            if (instruction.OpCode != OpCodes.Call &&
                instruction.OpCode != OpCodes.Calli &&
                instruction.OpCode != OpCodes.Callvirt)
                return true;

            if (instruction.Operand == null)
                return true;

            return !instruction.Operand.ToString().StartsWith("System.String Microsoft.SharePoint");
        }

        public static IEnumerable<Instruction> GetUnsafeSPObjectStringComparison(this MethodDefinition method)
        {
            // TODO
            // does not imclude LINQ expresion detection yet

            if (!method.HasBody)
                return Enumerable.Empty<Instruction>();

            IEnumerable<Instruction> comporationInstructions = method.Body.Instructions
                                                .Where(i => (i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Calli || i.OpCode == OpCodes.Callvirt) &&
                                                            (i.Operand != null) &&
                                                            (i.Operand.ToString().StartsWith("System.Boolean System.String::op_Equality")));

            List<Instruction> result = new List<Instruction>();

            foreach (Instruction comporationInstruction in comporationInstructions)
            {
                if (comporationInstruction.Previous != null && comporationInstruction.Previous.Previous != null)
                {
                    Instruction prevInstruction = comporationInstruction.Previous;
                    Instruction prevPrevInstruction = prevInstruction.Previous;

                    if (!IsSafeSPObjectStringCoporatinOperator(prevInstruction) || !IsSafeSPObjectStringCoporatinOperator(prevPrevInstruction))
                        result.Add(comporationInstruction);
                }
            }

            return result;
        }

        #endregion

        #region managed metadata extentions

        public static IEnumerable<Instruction> GetUnsafeTaxonomyTermSetCollectionStringIndexCall(this MethodDefinition method)
        {
            if (!method.HasBody)
                return Enumerable.Empty<Instruction>();

            return method.Body.Instructions
                                          .Where(i => (i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Calli || i.OpCode == OpCodes.Callvirt) &&
                                                       (i.OpCode != null) &&
                                                       (i.Operand.ToString().Contains("Microsoft.SharePoint.Taxonomy.Generic.IndexedCollection") &&
                                                        i.Operand.ToString().Contains("Microsoft.SharePoint.Taxonomy.TermSet") &&
                                                        i.Operand.ToString().Contains("get_Item(System.String)")));
        }

        public static IEnumerable<Instruction> GetUnsafeTaxonomyGroupStringIndexCall(this MethodDefinition method)
        {
            if (!method.HasBody)
                return Enumerable.Empty<Instruction>();

            return method.Body.Instructions
                                          .Where(i => (i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Calli || i.OpCode == OpCodes.Callvirt) &&
                                                       (i.OpCode != null) &&
                                                       (i.Operand.ToString().Contains("Microsoft.SharePoint.Taxonomy.Generic.IndexedCollection") &&
                                                        i.Operand.ToString().Contains("Microsoft.SharePoint.Taxonomy.Group") &&
                                                        i.Operand.ToString().Contains("get_Item(System.String)")));
        }

        #endregion

        #region SPLIstCollection methods

        public static IEnumerable<Instruction> GetSPListCollectionStringIndexCall(this Mono.Cecil.MethodDefinition method)
        {
            List<Instruction> result = new List<Instruction>();

            if (method.Body == null)
                return result;

            IEnumerable<Instruction> getListStringIndexCalls = method.Body.Instructions
                                          .Where(i => i.OpCode == OpCodes.Callvirt &&
                                                      i.Operand != null &&
                                                      i.Operand.ToString().EndsWith("Microsoft.SharePoint.SPListCollection::get_Item(System.String)", StringComparison.CurrentCultureIgnoreCase));

            result.AddRange(getListStringIndexCalls);

            return result;
        }

        public static IEnumerable<Instruction> GetSPListCollectionEnumeratorCalls(this Mono.Cecil.MethodDefinition method)
        {
            List<Instruction> result = new List<Instruction>();

            if (method.Body == null)
                return result;

            IEnumerable<Instruction> getListCalls = method.Body.Instructions
                                          .Where(i => i.OpCode == OpCodes.Callvirt &&
                                                      i.Operand != null &&
                                                      i.Operand.ToString().EndsWith("Microsoft.SharePoint.SPWeb::get_Lists()", StringComparison.CurrentCultureIgnoreCase));

            foreach (Instruction getListCall in getListCalls)
            {
                if (getListCall.Next != null)
                {
                    Instruction nextCall = getListCall.Next;

                    if (nextCall.OpCode == OpCodes.Callvirt &&
                        nextCall.Operand != null &&
                        nextCall.Operand.ToString().EndsWith("Microsoft.SharePoint.SPBaseCollection::GetEnumerator()", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add(nextCall);
                    }
                }
            }

            return result;
        }

        public static IEnumerable<Instruction> GetSPListCollectionEnumeratorLinqCastCall(this Mono.Cecil.MethodDefinition method)
        {
            if (method.Body == null)
                return Enumerable.Empty<Instruction>();

            return GetFollowingCall(method,
                                        "Microsoft.SharePoint.SPWeb::get_Lists()",
                                        "System.Linq.Enumerable::Cast<Microsoft.SharePoint.SPList>(System.Collections.IEnumerable)");
        }

        public static IEnumerable<Instruction> GetSPListCollectionEnumeratorLinqOfTypeCall(this Mono.Cecil.MethodDefinition method)
        {
            if (method.Body == null)
                return Enumerable.Empty<Instruction>();

            return GetFollowingCall(method,
                                        "Microsoft.SharePoint.SPWeb::get_Lists()",
                                        "System.Linq.Enumerable::OfType<Microsoft.SharePoint.SPList>(System.Collections.IEnumerable)");
        }

        public static IEnumerable<Instruction> GetFollowingCall(this MethodDefinition method, string currentCall, string followingCall)
        {
            List<Instruction> result = new List<Instruction>();

            if (method.Body == null)
                return result;

            IEnumerable<Instruction> getListCalls = method.Body.Instructions
                                          .Where(i => i.OpCode == OpCodes.Callvirt &&
                                                      i.Operand != null &&
                                                      i.Operand.ToString().EndsWith(currentCall, StringComparison.CurrentCultureIgnoreCase));

            foreach (Instruction getListCall in getListCalls)
            {
                if (getListCall.Next != null)
                {
                    Instruction nextCall = getListCall.Next;

                    if (nextCall.OpCode == OpCodes.Call &&
                       nextCall.Operand != null &&
                       nextCall.Operand.ToString().EndsWith(followingCall, StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add(nextCall);
                    }
                }
            }

            return result;
        }

        #endregion

        #region unsafe urls

        // has to be reengineered, raw sketch yet
        private static List<OpCode> AllowedUrlConcatinationRamgeInstructions = new List<OpCode>{
            OpCodes.Ldstr,
            OpCodes.Ldarg,
            OpCodes.Ldarg_0,
            OpCodes.Ldarg_1,
            OpCodes.Ldarg_2,
            OpCodes.Ldarg_3,
            OpCodes.Ldarg_S,
            OpCodes.Ldarga,
            OpCodes.Ldarga_S,
            OpCodes.Ldloc,
            OpCodes.Ldloc_0,
            OpCodes.Ldloc_1,
            OpCodes.Ldloc_2,
            OpCodes.Ldloc_3,
            OpCodes.Ldloc_S,
            OpCodes.Ldloca,
            OpCodes.Ldloca_S,
            OpCodes.Box,
            OpCodes.Ldc_I4,
            OpCodes.Ldc_I4_0,
            OpCodes.Ldc_I4_1, OpCodes.Ldc_I4_2, OpCodes.Ldc_I4_3, OpCodes.Ldc_I4_4, OpCodes.Ldc_I4_5, OpCodes.Ldc_I4_6,
            OpCodes.Ldc_I4_7, OpCodes.Ldc_I4_8, OpCodes.Ldc_I4_M1, OpCodes.Ldc_I4_S, OpCodes.Ldc_I8, OpCodes.Ldc_R4, OpCodes.Ldc_R8};

        private static IEnumerable<Instruction> GetUnsafeUrlConcatinations(this MethodDefinition methodDefinition, IEnumerable<string> targetMethodNames)
        {
            List<Instruction> result = new List<Instruction>();
            IEnumerable<Instruction> stringConcatInstructions = methodDefinition.GetStringConcatinatinInstructions();

            foreach (Instruction stringConcatInstruction in stringConcatInstructions)
            {
                // get previous *.Url* call

                List<Instruction> stringOpRange = new List<Instruction>();
                Instruction currentOperation = stringConcatInstruction.Previous;

                do
                {
                    if (currentOperation == null ||
                       (currentOperation.OpCode == OpCodes.Callvirt && targetMethodNames.Contains(currentOperation.Operand.ToString())))
                        break;

                    stringOpRange.Add(currentOperation);

                    currentOperation = currentOperation.Previous;
                } while (currentOperation != null);

                bool hasRightRange = true;

                foreach (Instruction op in stringOpRange)
                {
                    if (!AllowedUrlConcatinationRamgeInstructions.Contains(op.OpCode))
                    {
                        // call with string return?
                        if ((op.OpCode == OpCodes.Call || op.OpCode == OpCodes.Calli || op.OpCode == OpCodes.Callvirt) &&
                            op.Operand != null && op.Operand.ToString().StartsWith("System.String"))
                        {
                            continue;
                        }

                        hasRightRange = false;
                        break;
                    }
                }

                if (hasRightRange)
                    result.Add(stringConcatInstruction);
            }

            return result;
        }

        private static IEnumerable<Instruction> GetStringConcatinatinInstructions(this MethodDefinition methodDefinition)
        {
            if (!methodDefinition.HasBody)
                return Enumerable.Empty<Instruction>();

            return methodDefinition.Body.Instructions
                                        .Where(i => i.OpCode == OpCodes.Call &&
                                                    i.Operand != null &&
                                                    i.Operand.ToString().Contains("System.String::Concat"));
        }

        public static IEnumerable<Instruction> GetUnsafeSPWebUrlConcatinations(this MethodDefinition methodDefinition)
        {
            if (!methodDefinition.HasBody)
                return Enumerable.Empty<Instruction>();

            return methodDefinition.GetUnsafeUrlConcatinations(new[]
            {
                "System.String Microsoft.SharePoint.SPWeb::get_Url()",
                "System.String Microsoft.SharePoint.SPWeb::get_ServerRelativeUrl()"
            });
        }

        public static IEnumerable<Instruction> GetUnsafeSPSiteUrlConcatinations(this MethodDefinition methodDefinition)
        {
            if (!methodDefinition.HasBody)
                return Enumerable.Empty<Instruction>();

            return methodDefinition.GetUnsafeUrlConcatinations(new[]
            {
                "System.String Microsoft.SharePoint.SPSite::get_Url()",
                "System.String Microsoft.SharePoint.SPSite::get_ServerRelativeUrl()"
            });
        }

        public static IEnumerable<Instruction> GetUnsafeSPFolderConcatinations(this MethodDefinition methodDefinition)
        {
            if (!methodDefinition.HasBody)
                return Enumerable.Empty<Instruction>();

            return methodDefinition.GetUnsafeUrlConcatinations(new[]
            {
                "System.String Microsoft.SharePoint.SPFolder::get_Url()",
                "System.String Microsoft.SharePoint.SPFolder::get_ServerRelativeUrl()"
            });
        }

        #endregion

        #region methods

        public static bool HasFullTryCatchCover(this MethodDefinition method)
        {
            if (!method.HasBody)
                return false;

            if (method.HasExceptionHandlers())
            {
                List<Instruction> notCoveredIstructions = GetNotCoveredInstructions(method);

                Trace.WriteLine("   -not covered instructions");
                foreach (Instruction instruction in notCoveredIstructions)
                    Trace.WriteLine(instruction);

                return HasNotEmptyAndAllowedUnwrappedInstructions(notCoveredIstructions);
                //!method.HasEmptyExceptionHandler();
            }

            return false;
        }

        public static bool HasEmptyExceptionHandler(this MethodDefinition method)
        {
            if (!method.HasBody)
                return false;

            Trace.WriteLine(string.Format("Method: [{0}]", method.FullName));
            TraceInstructions(method, method.Body.Instructions.ToList());

            if (method.HasExceptionHandlers())
            {
                IEnumerable<Instruction> tryInstructions = GetTryInstructions(method);

                Trace.WriteLine(string.Format(" Try instructions"));
                TraceInstructions(method, tryInstructions);

                List<Instruction> catchInstructions = GetCatchInstructions(method);

                Trace.WriteLine(string.Format(" Catch instructions"));
                TraceInstructions(method, catchInstructions);

                return !HasNotEmptyAndAllowedInstuction(catchInstructions);
            }

            return false;
        }

        public static bool HasExceptionHandlers(this MethodDefinition method)
        {
            if (!method.HasBody)
                return true;

            TraceInstructions(method, method.Body.Instructions);

            return GetTryCatchExcecutionRanges(method).Count > 0;

            //var tryInstructions = GetTryInstructions(method);

            //Trace.WriteLine(string.Format(" Try instructions"));
            //TraceInstructions(method, tryInstructions);

            //var catchInstructions = GetCatchInstructions(method);

            //Trace.WriteLine(string.Format(" Catch instructions"));
            //TraceInstructions(method, catchInstructions);

            //return method.Body.HasExceptionHandlers;
        }

        public static bool HasOneInsideMethodCall(this MethodDefinition method)
        {
            return method.InsideMethodsCallsCount() == 1;
        }
        
        public static void InsideMethodsCalls(this MethodDefinition methodDefinition, MultiValueDictionary<string, string> methodList, HashSet<string> handledMethods, bool ignoreSystemCalls, Action<MethodDefinition> report)
        {
            if (methodDefinition.HasBody && !LoopbackCheck(handledMethods, methodDefinition))
            {
				//LoggingService.Log(LogLevel.Debug, methodDefinition.ToString()); 
                foreach (MethodReference methodInvokation in methodDefinition.AllMethodInvokationsIn())
                {
                    string key = methodInvokation.DeclaringType.FullName;
                    if (!ignoreSystemCalls || (!key.StartsWith("Microsoft.") && !key.StartsWith("System.")))
                    {
                        if (!methodInvokation.Module.Assembly.AssemblyHasExcluded())
                        {
                            if (methodList.ContainsKey(key) && methodList[key].Contains(methodInvokation.Name)) continue;

                            MethodDefinition md = methodInvokation.Resolve();
                            if (md != null)
                            {
                                md.InsideMethodsCalls(methodList, handledMethods, ignoreSystemCalls, report);
                            }
                            else if (methodInvokation.DeclaringType is TypeDefinition)
                            {
                                (methodInvokation.DeclaringType as TypeDefinition).InsideMethodCalls(methodList, handledMethods, 
                                    ignoreSystemCalls, report);
                            }
                        }

                        if (!methodList.TryAdd(key, methodInvokation.Name))
                            continue;
                        else
                        {
                            if (report != null)
                                report(methodDefinition);
                        }
                    }
                }
            }
        }

        private static bool LoopbackCheck(HashSet<string> handledMethods, MethodDefinition methodDefinition)
        {
            if (!handledMethods.Contains(methodDefinition.FullName))
            {
                handledMethods.Add(methodDefinition.FullName);
                return false;
            }
            
            return true;
        }

        public static MethodDefinition GetFirstOrDefaultInsideMethod(this MethodDefinition method)
        {
            if (method.InsideMethodsCallsCount() != 1)
                return null;

            Instruction firstCallMethodInsruction = method.Body
                                                  .Instructions
                                                  .FirstOrDefault(i => i.OpCode == OpCodes.Call);

            if (firstCallMethodInsruction != null)
                return firstCallMethodInsruction.Operand as MethodDefinition;

            return null;
        }

        public static int InsideMethodsCallsCount(this MethodDefinition method)
        {
            int result = -1;

            if (method.HasBody)
            {
                result = method.Body
                               .Instructions
                               .Count(i => i.OpCode == OpCodes.Call);
            }

            return result;
        }

        public static bool HasSPMonitoredScope(this MethodDefinition method)
        {
            if (!method.HasBody)
                return false;

            return method.Body.Variables.ToList().Exists(z => z.VariableType.FullName == TypeKeys.SPMonitoredScope);
        }

        public static int GetSPMonitoredScopeUsageCount(this MethodDefinition method)
        {
            if (!method.HasBody)
                return 0;

            return method.Body.Variables.Where(z => z.VariableType.FullName == TypeKeys.SPMonitoredScope).Count();
        }

        public static IEnumerable<Instruction> GetUnsafeSPListItemCastInstructions(this MethodDefinition method)
        {
            if (!method.HasBody)
                return Enumerable.Empty<Instruction>();

            List<Instruction> result = new List<Instruction>();

            Collection<Instruction> methodInstructions = method.Body.Instructions;
            IEnumerable<Instruction> getSPItemValueCalls = methodInstructions.Where(i => i.OpCode == OpCodes.Callvirt &&
                                                                    i.Operand != null &&
                                                                    i.Operand.ToString().Contains("Microsoft.SharePoint.SPItem::get_Item(System.String)"));

            foreach (Instruction getValueCall in getSPItemValueCalls)
            {
                // TODO, trace, should be?
                if (getValueCall.Next == null) continue;

                // analyse most of the cases
                // we might create Dictionary/ResultMap in the future to see trace for the method calls
                Instruction nextCall = getValueCall.Next;

                //  list.Items[0]["Date"].ToString()
                if (nextCall.OpCode == OpCodes.Callvirt &&
                    nextCall.Operand != null &&
                    nextCall.Operand.ToString().Contains("System.Object::ToString()"))
                {
                    result.Add(getValueCall);
                }

                //  (DateTime)list.Items[0]["Date"];
                if (nextCall.OpCode == OpCodes.Unbox_Any)
                {
                    result.Add(getValueCall);
                }

                //  list.Items[0]["Date"] as DateTime?;
                if (nextCall.OpCode == OpCodes.Isinst)
                {
                    // result.Add(getValueCall);
                }

                // ((SPFieldUserValue)list.Items[0]["User"]).LookupId;
                if (nextCall.OpCode == OpCodes.Castclass)
                {
                    result.Add(getValueCall);
                }

                if (nextCall.OpCode == OpCodes.Call &&
                    nextCall.Operand != null &&
                    nextCall.Operand.ToString().Contains("System.Convert::To"))
                {
                    // result.Add(getValueCall);
                }
            }

            return result;
        }

        public static bool IsDataListItemEventHandler(this MethodDefinition method)
        {
            // TODO

            //  Instruction:[IL_0071: newobj System.Void System.Web.UI.WebControls.DataListItemEventHandler::.ctor(System.Object,System.IntPtr)] OpCode:[newobj] Operand:[System.Void System.Web.UI.WebControls.DataListItemEventHandler::.ctor(System.Object,System.IntPtr)]
            //  Instruction:[IL_0076: callvirt System.Void System.Web.UI.WebControls.DataList::add_ItemCreated(System.Web.UI.WebControls.DataListItemEventHandler)] OpCode:[callvirt] Operand:[System.Void System.Web.UI.WebControls.DataList::add_ItemCreated(System.Web.UI.WebControls.DataListItemEventHandler)]

            if (!method.HasBody)
                return false;

            string methodName = method.FullName;

            Collection<MethodDefinition> classMethods = method.DeclaringType.Resolve().Methods;

            foreach (MethodDefinition classMethod in classMethods)
            {
                if (!classMethod.HasBody)
                    continue;

                IEnumerable<Instruction> repeaterItemHandlers = classMethod.Body.Instructions.Where(i => i.OpCode == OpCodes.Newobj &&
                                                                               i.Operand != null &&
                                                                               i.Operand.ToString().Contains("System.Web.UI.WebControls.DataListItemEventHandler"));

                foreach (Instruction repeaterItemHandler in repeaterItemHandlers.Where(i => i.Previous != null))
                {
                    Instruction prevInstruction = repeaterItemHandler.Previous;

                    //  Instruction:[IL_0025: ldftn System.Void SPCAFContrib.Sandbox.ItemBindingEvents.BadItemBindingImpl::Positive_ConstructorAssignedView_ItemDataBound(System.Object,System.Web.UI.WebControls.RepeaterItemEventArgs)] OpCode:[ldftn] Operand:[System.Void SPCAFContrib.Sandbox.ItemBindingEvents.BadItemBindingImpl::Positive_ConstructorAssignedView_ItemDataBound(System.Object,System.Web.UI.WebControls.RepeaterItemEventArgs)]
                    //  Instruction:[IL_002b: newobj System.Void System.Web.UI.WebControls.RepeaterItemEventHandler::.ctor(System.Object,System.IntPtr)] OpCode:[newobj] Operand:[System.Void System.Web.UI.WebControls.RepeaterItemEventHandler::.ctor(System.Object,System.IntPtr)]

                    if (prevInstruction.OpCode == OpCodes.Ldftn &&
                        prevInstruction.Operand != null &&
                        prevInstruction.Operand.ToString().Contains(methodName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsRepeaterItemEventHandler(this MethodDefinition method)
        {
            // TODO

            //System.Void System.Web.UI.WebControls.RepeaterItemEventHandler
            //Instruction:[IL_002b: newobj System.Void System.Web.UI.WebControls.RepeaterItemEventHandler::.ctor(System.Object,System.IntPtr)] OpCode:[newobj] Operand:[System.Void System.Web.UI.WebControls.RepeaterItemEventHandler::.ctor(System.Object,System.IntPtr)]

            if (!method.HasBody)
                return false;

            string methodName = method.FullName;

            Collection<MethodDefinition> classMethods = method.DeclaringType.Resolve().Methods;

            foreach (MethodDefinition classMethod in classMethods)
            {
                if (!classMethod.HasBody)
                    continue;

                IEnumerable<Instruction> repeaterItemHandlers = classMethod.Body.Instructions.Where(i => i.OpCode == OpCodes.Newobj &&
                                                                               i.Operand != null &&
                                                                               i.Operand.ToString().Contains("System.Web.UI.WebControls.RepeaterItemEventHandler"));

                foreach (Instruction repeaterItemHandler in repeaterItemHandlers.Where(i => i.Previous != null))
                {
                    Instruction prevInstruction = repeaterItemHandler.Previous;

                    //  Instruction:[IL_0025: ldftn System.Void SPCAFContrib.Sandbox.ItemBindingEvents.BadItemBindingImpl::Positive_ConstructorAssignedView_ItemDataBound(System.Object,System.Web.UI.WebControls.RepeaterItemEventArgs)] OpCode:[ldftn] Operand:[System.Void SPCAFContrib.Sandbox.ItemBindingEvents.BadItemBindingImpl::Positive_ConstructorAssignedView_ItemDataBound(System.Object,System.Web.UI.WebControls.RepeaterItemEventArgs)]
                    //  Instruction:[IL_002b: newobj System.Void System.Web.UI.WebControls.RepeaterItemEventHandler::.ctor(System.Object,System.IntPtr)] OpCode:[newobj] Operand:[System.Void System.Web.UI.WebControls.RepeaterItemEventHandler::.ctor(System.Object,System.IntPtr)]

                    if (prevInstruction.OpCode == OpCodes.Ldftn &&
                        prevInstruction.Operand != null &&
                        prevInstruction.Operand.ToString().Contains(methodName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool HasUserProfileEnumeration(this MethodDefinition method)
        {
            bool result = false;

            if (method != null)
            {
                foreach (Instruction instruction in method.Body.Instructions)
                {
                    MethodReference methodRef = instruction.Operand as MethodReference;

                    if (methodRef != null)
                    {
                        if (String.Equals(methodRef.DeclaringType.FullName,
                            TypeKeys.ProfileManagerBase,
                            StringComparison.InvariantCulture) &&
                            String.Equals(methodRef.Name,
                            "GetEnumerator",
                            StringComparison.InvariantCulture) &&
                            String.Equals(methodRef.ReturnType.Name,
                            "IEnumerator",
                            StringComparison.InvariantCulture) &&
                            !methodRef.HasParameters)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public static bool HasMethodUsage(this MethodDefinition method, Dictionary<string, List<string>> typeMethodMap, bool shoulHaveParameters)
        {
            bool result = false;

            if (method != null)
            {
                foreach (Instruction instruction in method.Body.Instructions)
                {
                    MethodReference methodRef = instruction.Operand as MethodReference;
                    if (methodRef != null && methodRef.DeclaringType != null)
                    {
                        foreach (string typeName in typeMethodMap.Keys)
                        {
                            foreach (string methodName in typeMethodMap[typeName])
                            {
                                if (
                                    (
                                        methodRef.DeclaringType is TypeDefinition && String.Equals((methodRef.DeclaringType as TypeDefinition).BaseType.FullName, typeName, StringComparison.InvariantCulture) ||
                                        String.Equals(methodRef.DeclaringType.FullName, typeName, StringComparison.InvariantCulture)
                                    ) &&
                                    String.Equals(methodRef.Name, methodName, StringComparison.InvariantCulture) &&
                                    (!shoulHaveParameters || methodRef.HasParameters)
                                   )
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static bool HasSPDatabase(this MethodDefinition method)
        {
            const string typeName = "Microsoft.SharePoint.Administration.SPDatabase";
            const string methodName = "get_DatabaseConnectionString";

            bool result = false;

            if (method != null)
            {
                foreach (Instruction instruction in method.Body.Instructions)
                {
                    MethodReference methodRef = instruction.Operand as MethodReference;
                    if (methodRef != null)
                    {
                        if (String.Equals(methodRef.DeclaringType.FullName,
                           typeName,
                           StringComparison.InvariantCulture) &&
                           String.Equals(methodRef.Name,
                           methodName,
                           StringComparison.InvariantCulture) &&
                           String.Equals(methodRef.ReturnType.FullName,
                           "System.String",
                           StringComparison.InvariantCulture) &&
                           !methodRef.HasParameters)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public static bool IsOversized(this MethodDefinition method, int lineLimit)
        {
            if (method.HasBody)
            {
                return method.Body.CodeSize > lineLimit;
            }

            return false;
        }

        #endregion
    }
}
