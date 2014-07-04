using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace SPCAFContrib.Scripting
{
    public class PowerShellScriptingHost : IDisposable
    {
        #region properties

        private Runspace _powerShellRunspace;

        protected Runspace PowerShellRunspace
        {
            get
            {
                return _powerShellRunspace ?? (_powerShellRunspace = RunspaceFactory.CreateRunspace());
            }
            set
            {
                _powerShellRunspace = value;
            }
        }

        private Dictionary<int, string> ScriptHash = new Dictionary<int, string>();

        private string AddTemplate =
                "if ( (Get-PSSnapin -Name \"{#ReferenceName#}\" -ErrorAction SilentlyContinue) -eq $null )" +
                "{" +
                "   Add-PsSnapin \"{#ReferenceName#}\"" +
                "}";

        #endregion

        #region methods

        public ExecutionResult Execute(ExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return ExecuteContext(context);
        }

        private string BuildScript(string script, IEnumerable<string> referencedAssemblies)
        {
            string result = string.Empty;

            if (referencedAssemblies != null)
            {
                result += "# aurogenerated external referencies begin";
                result += Environment.NewLine;

                foreach (string asmReference in referencedAssemblies)
                {
                    result += AddTemplate.Replace("{#ReferenceName#}", asmReference);
                    result += Environment.NewLine;
                }

                result += "# aurogenerated external referencies end";
                result += Environment.NewLine;
            }

            result += script;

            return result;
        }

        private ExecutionResult ExecuteContext(ExecutionContext context)
        {
            string scriptFile = BuildScript(context.Script, context.ReferencedAssemblies);
            string eventName = context.StartMethodName;

            int scriptHash = scriptFile.GetHashCode();

            if (!ScriptHash.ContainsKey(scriptHash))
            {
                if (PowerShellRunspace.RunspaceStateInfo.State != RunspaceState.Opened)
                    PowerShellRunspace.Open();

                Pipeline mainPipe = PowerShellRunspace.CreatePipeline(scriptFile);
                mainPipe.Invoke();

                ScriptHash.Add(scriptHash, scriptFile);

                List<string> functions = GetFunctions(PowerShellRunspace);

                if (!functions.Contains(eventName.ToLower()))
                    throw new ScriptingRuntimeException(string.Format("Can't find entry point with name: {0}", eventName));
            }

            // refresh global params
            foreach (ScriptMethodParameter globalParam in context.GlobalParameters)
                PowerShellRunspace.SessionStateProxy.PSVariable.Set(globalParam.Name, globalParam.Value);

            Pipeline pipeline = PowerShellRunspace.CreatePipeline();

            Command scriptCommand = new Command(eventName);
            Collection<CommandParameter> commandParameters = new Collection<CommandParameter>();

            foreach (ScriptMethodParameter scriptParameter in context.Parameters)
            {
                CommandParameter commandParm = new CommandParameter(scriptParameter.Name, scriptParameter.Value);
                commandParameters.Add(commandParm);
                scriptCommand.Parameters.Add(commandParm);
            }

            pipeline.Commands.Add(scriptCommand);
            Collection<PSObject> result = pipeline.Invoke();

            object resultValue;

            if (result.Count == 0)
            {
                resultValue = null;
            }
            else if (result.Count == 1)
            {
                resultValue = result[0].BaseObject;
            }
            else
            {
                // have no idea yet why we might have a lot of nulls
                resultValue = result.Select(d => d != null ? d.BaseObject : null)
                                    .ToArray();
            }

            return new ExecutionResult
            {
                ResultValue = resultValue
            };
        }

        internal static List<string> GetFunctions(Runspace runspace)
        {
            List<string> functions = new List<string>();
            Pipeline pipe = runspace.CreatePipeline("get-childitem function:\\");

            Collection<PSObject> result = pipe.Invoke();

            foreach (PSObject obj in result)
            {
                FunctionInfo func = obj.BaseObject as FunctionInfo;
                if (func != null) functions.Add(func.Name.ToLower());
            }

            return functions;
        }

        #endregion

        public void Dispose()
        {
            if (PowerShellRunspace != null)
            {
                PowerShellRunspace.Dispose();
                PowerShellRunspace = null;
            }
        }

        public CompileResult Compile(CompileContext context)
        {
            CompileResult result = new CompileResult();

            string scriptFile = BuildScript(context.Script, context.ReferencedAssemblies);

            try
            {
                if (PowerShellRunspace.RunspaceStateInfo.State != RunspaceState.Opened)
                    PowerShellRunspace.Open();

                // there is a big security issue here as script goingt to be executed
                // suppose, there might be an option to extract command to make sure the won't be any immediate run
                Pipeline mainPipe = PowerShellRunspace.CreatePipeline(scriptFile);
                mainPipe.Invoke();

                if (mainPipe.Error.Count == 0)
                {
                    result.ResultValue = true;
                }
                else
                {
                    result.ResultValue = false;

                    // TODO, make up an error string from mainPipe.Error!
                    result.ResultMessage = "There is one or more errors in the main pipeline";
                }
            }
            catch (Exception compileException)
            {
                result.ResultValue = false;
                result.ResultException = compileException;
            }

            return result;
        }
    }
}
