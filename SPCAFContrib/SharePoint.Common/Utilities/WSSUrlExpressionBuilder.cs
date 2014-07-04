using System;
using System.CodeDom;
using System.Web.Compilation;
using Microsoft.SharePoint;
using System.Web.UI;

namespace SharePoint.Common.Utilities
{
    [ExpressionPrefix("WSSUrl")]
    public class WSSUrlExpressionBuilder : ExpressionBuilder
    {

        public override CodeExpression GetCodeExpression(System.Web.UI.BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            CodeTypeReferenceExpression thisType = new CodeTypeReferenceExpression(base.GetType());

            CodePrimitiveExpression expression = new CodePrimitiveExpression(entry.Expression.Trim().ToString());

            string evaluationMethod = "GetKeyValue";

            return new CodeMethodInvokeExpression(thisType, evaluationMethod, new CodeExpression[] { expression });
        }

        public static object GetKeyValue(string expression)
        {
            SPWeb web = SPContext.Current.Web;

            string key = "~SiteCollection";
            if (expression.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == 0)
                return web.Site.Url + expression.Substring(key.Length);

            key = "~Site";
            if (expression.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == 0)
                return web.Url + expression.Substring(key.Length);

            return expression;
        }

        /// <summary>
        /// Gets a flag that indicates whether the expression builder
        /// supports no-compile evaluation.
        /// Returns true, as the target type can be validated at runtime as well.
        /// </summary>
        public override bool SupportsEvaluate
        {
            get { return true; }
        }

        /// <summary>
        /// Evaluates the expression at runtime.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="entry">The entry for the property bound to the expression.</param>
        /// <param name="parsedData">The parsed expression data.</param>
        /// <param name="context">The current expression builder context.</param>
        /// <returns>A string representing the target type or member.</returns>
        public override object EvaluateExpression(object target, BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            return (string)parsedData;
        }

    }
}
