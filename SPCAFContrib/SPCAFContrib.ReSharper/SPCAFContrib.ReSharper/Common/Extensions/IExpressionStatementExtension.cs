using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Feature.Services.LinqTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using SPCAFContrib.ReSharper.Consts;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class IExpressionStatementExtension
    {
        public static bool IsOneOfTheTypes(this IExpressionStatement statement, IEnumerable<IClrTypeName> typeNames)
        {
            bool result = false;

            if (statement.Expression is IInvocationExpression)
            {
                var invokedExpression = (statement.Expression as IInvocationExpression).InvokedExpression;
                result |= invokedExpression.IsOneOfType(typeNames);
            }

            return result;
        }
        
        public static bool CheckExpression(this IExpressionStatement statement,
            Func<IExpressionStatement, bool> checkMethod,
            Func<IMethod, bool> checkNoBodyMethod, int deep)
        {
            bool result = false;

            if (deep < 0) return result;
            
            result |= checkMethod(statement);

            if (!result)
            {
                if (statement.Expression is IInvocationExpression)
                {
                    var invokedExpression =
                        (statement.Expression as IInvocationExpression).InvokedExpression;

                    IDeclaredElement referenceExpressionTarget = invokedExpression.ReferenceExpressionTarget();
                    if (referenceExpressionTarget is IMethod)
                    {
                        IMethod method = referenceExpressionTarget as IMethod;
                        foreach (var methodDeclaration in method.GetDeclarations())
                        {
                            var body = methodDeclaration.Children<IChameleonNode>().FirstOrDefault();
                            if (body != null)
                                foreach (IExpressionStatement child in body.Children<IExpressionStatement>())
                                {
                                    result |= CheckExpression(child, checkMethod, checkNoBodyMethod, deep - 1);

                                    if (result) break;
                                }

                            if (result) break;
                        }

                        if (!result && method.GetDeclarations().Count == 0 && checkNoBodyMethod != null)
                        {
                            result |= checkNoBodyMethod(method);
                        }
                    }
                }
            }

            return result;
        }
    }
}
