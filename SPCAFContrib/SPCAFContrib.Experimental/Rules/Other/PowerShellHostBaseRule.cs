using System;
using System.IO;
using System.Linq.Expressions;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Scripting;

namespace SPCAFContrib.Experimental.Rules.Other
{
    // made as abstract caause SPCAF doesn't validate empty metadata on Rules
    public abstract class PowerShellHostBaseRule<TTarget> : Rule<TTarget>
         where TTarget : class
    {
        #region contructors

        #endregion

        #region properties

        internal static class DefaultSettings
        {
            public const string PowerShellRepositoryFolder = "SPCAFContrib.PowerShellHostRules";
            public const string PowerShellFileExtension = "*.ps1";

            public const string StartMethodName = "Visit";

            public static class ParamNames
            {
                public const string This = "__this";
                public const string Target = "target";
                public const string Notifications = "notifications";
            }
        }

        protected virtual string StartMethodName
        {
            get
            {
                return DefaultSettings.StartMethodName;
            }
        }

        protected virtual string PowerShellRepositoryFolder
        {
            get
            {
                return DefaultSettings.PowerShellRepositoryFolder;
            }
        }

        protected virtual string PowerShellRulesFolder
        {
            get
            {
                return typeof(TTarget).Name;
            }
        }

        protected virtual string PowerShellFileExtension
        {
            get
            {
                return DefaultSettings.PowerShellFileExtension;
            }
        }

        #endregion

        #region methods

        public override void Visit(TTarget target, NotificationCollection notifications)
        {
            string currentDirectory = DictionaryProvider.GetCurrentDirectory();
            string psRepositoryDirectory = Path.Combine(currentDirectory, PowerShellRepositoryFolder);
            string psRulesDirectory = Path.Combine(psRepositoryDirectory, PowerShellRulesFolder);

            if (Directory.Exists(psRulesDirectory))
            {
                string[] psFiles = Directory.GetFiles(psRulesDirectory, PowerShellFileExtension);

                foreach (string file in psFiles)
                {
                    string scriptContent = File.ReadAllText(file);

                    using (PowerShellScriptingHost host = new PowerShellScriptingHost())
                    {
                        ExecutionContext context = new ExecutionContext
                        {
                            Script = scriptContent,
                            StartMethodName = StartMethodName,
                        };

                        context.Parameters.Add(new ScriptMethodParameter { Name = DefaultSettings.ParamNames.This, Value = this });
                        context.Parameters.Add(new ScriptMethodParameter { Name = DefaultSettings.ParamNames.Target, Value = target });
                        context.Parameters.Add(new ScriptMethodParameter { Name = DefaultSettings.ParamNames.Notifications, Value = notifications });

                        host.Execute(context);
                    }
                }
            }
        }

        #region overrides to open Notify to PS scripts

        public void PsNotify(object element, string message, NotificationCollection collection)
        {
            Notify(element, message, collection);
        }

        public void PsNotify<TElement>(TElement element, string message, ElementSummary elementSummary, NotificationCollection collection)
        {
            Notify(element, message, elementSummary, collection);
        }

        public void PsNotify<TElement, TProperty>(TElement element, string message, NotificationCollection collection, Expression<Func<TElement, TProperty>> propertySelector) where TElement : class
        {
            Notify(element, message, collection, propertySelector);
        }

        public void PsNotify<TElement>(TElement element, string message, NotificationCollection collection, Func<string> propertySelector)
        {
            Notify(element, message, collection, propertySelector);
        }

        public void PsNotify<TElement>(TElement element, string message, NotificationCollection collection, string propertyName)
        {
            Notify(element, message, collection, propertyName);
        }

        public void PsNotify<TElement>(VisitorMetadata visitorMetaData, TElement element, string message, NotificationCollection collection)
        {
            Notify(visitorMetaData, element, message, collection);
        }

        public void PsNotify<TElement>(VisitorMetadata visitorMetaData, TElement element, string message, ElementSummary elementSummary, NotificationCollection collection)
        {
            Notify(visitorMetaData, element, message, elementSummary, collection);
        }

        #endregion

        #endregion
    }
}
