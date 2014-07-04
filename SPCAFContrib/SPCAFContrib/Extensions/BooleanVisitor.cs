using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCAFContrib.Extensions
{
    public class BooleanOrVisitor : DepthFirstAstVisitor<bool>
    {
        protected override bool VisitChildren(AstNode node)
        {
            bool result = false;
            AstNode nextSibling;
            for (AstNode astNode = node.FirstChild; astNode != null; astNode = nextSibling)
            {
                nextSibling = astNode.NextSibling;
                result = result || astNode.AcceptVisitor(this);
            }
            return result;
        }
    }
    public class BooleanOrVisitor<T> : DepthFirstAstVisitor<T, bool>
    {
        protected override bool VisitChildren(AstNode node, T data)
        {
            bool result = false;
            AstNode nextSibling;
            for (AstNode astNode = node.FirstChild; astNode != null; astNode = nextSibling)
            {
                nextSibling = astNode.NextSibling;
                result = result || astNode.AcceptVisitor(this, data);
            }
            return result;
        }
    }
}
