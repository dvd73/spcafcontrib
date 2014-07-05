using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.LinqTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common.CodeAnalysis;
using SPCAFContrib.ReSharper.Consts;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    //To get type inheritance you should use the GetAllSuperTypes or GetAllSuperClasses extension method defined in TypeElementsUtil. 
    //This will return you an array of types. 
    //GetAllSuperTypes will include implemented interfaces, while GetAllSuperClasses returns only classes.
    //IEnumerable<IDeclaredType> parentTypes = operandType.GetAllSuperTypes();

    public static class IExpressionExtension
    {
        public static bool IsResolvedAsMethodCall(this IExpression element, IClrTypeName typeName,
            IEnumerable<MethodCriteria> methodCriterias)
        {
            bool result = false;

            if (element != null)
            {
                IDeclaredElement referenceExpressionTarget = element.ReferenceExpressionTarget();
                if (referenceExpressionTarget is IMethod)
                {
                    IMethod method = referenceExpressionTarget as IMethod;
                    ITypeElement containingType = method.GetContainingType();

                    if (containingType != null)
                    {
                        IEnumerable<IDeclaredType> parentTypes = containingType.GetSuperTypesWithoutCircularDependent();

                        if ((containingType.GetClrName().Equals(typeName) || parentTypes.Any(parentType => parentType.GetClrName().Equals(typeName))) &&
                            methodCriterias.Any(methodCriteria => 
                                String.Equals(method.ShortName, methodCriteria.ShortName, StringComparison.OrdinalIgnoreCase) && 
                                method.HasSameParameters(methodCriteria.Parameters)))
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public static bool IsResolvedAsPropertyUsage(this IExpression element, IClrTypeName typeName,
            IEnumerable<string> propertyNames)
        {
            bool result = false;

            if (element != null)
            {
                IDeclaredElement referenceExpressionTarget = element.ReferenceExpressionTarget();

                if (referenceExpressionTarget == null)
                {
                    if (element is IElementAccessExpression)
                    {
                        IElementAccessExpression elementAccessExpression = element as IElementAccessExpression;
                        result = elementAccessExpression.Operand.IsResolvedAsPropertyUsage(typeName, propertyNames);
                    }
                }
                else if (referenceExpressionTarget is IProperty)
                {
                    IProperty property = referenceExpressionTarget as IProperty;
                    ITypeElement containingType = property.GetContainingType();
                    
                    if (containingType != null)
                    {
                        IEnumerable<IDeclaredType> parentTypes = containingType.GetSuperTypesWithoutCircularDependent();

                        if ((containingType.GetClrName().Equals(typeName) || parentTypes.Any(parentType => parentType.GetClrName().Equals(typeName))) &&
                            propertyNames.Any(m => String.Equals(property.ShortName, m, StringComparison.Ordinal)))
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public static bool IsOneOfType(this IExpression element, IEnumerable<IClrTypeName> typeNames)
        {
            bool result = false;

            if (element != null)
            {
                IDeclaredType scalarType = element.Type().GetScalarType();
                if (scalarType != null && !scalarType.IsUnknown)
                {
                    IEnumerable<IDeclaredType> parentTypes = scalarType.GetSuperTypesWithoutCircularDependent();
                    result = typeNames.Any(typeName => scalarType.GetClrName().Equals(typeName) ||
                                                       parentTypes.Any(
                                                           parentType => parentType.GetClrName().Equals(typeName)));
                }
                else
                {
                    IDeclaredElement referenceExpressionTarget = element.ReferenceExpressionTarget();
                    if (referenceExpressionTarget == null)
                    {
                        scalarType = element.Type().GetScalarType();
                        if (scalarType != null && !scalarType.IsUnknown)
                        {
                            IEnumerable<IDeclaredType> parentTypes =
                                scalarType.GetSuperTypesWithoutCircularDependent();
                            result = typeNames.Any(typeName => scalarType.GetClrName().Equals(typeName) ||
                                                                parentTypes.Any(
                                                                    parentType =>
                                                                        parentType.GetClrName().Equals(typeName)));
                        }
                    }
                    else if (referenceExpressionTarget is IClrDeclaredElement)
                    {
                        IClrDeclaredElement clrDeclaredElement = referenceExpressionTarget as IClrDeclaredElement;
                        ITypeElement containingType = clrDeclaredElement.GetContainingType();
                        if (containingType != null)
                        {
                            IEnumerable<IDeclaredType> parentTypes =
                                containingType.GetSuperTypesWithoutCircularDependent();
                            result =
                                typeNames.Any(
                                    typeName =>
                                        containingType.GetClrName().Equals(typeName) ||
                                        parentTypes.Any(parentType => parentType.GetClrName().Equals(typeName)));
                        }
                        else
                        {
                            IDeclaration declaration = referenceExpressionTarget.GetDeclarations().FirstOrDefault();
                            if (declaration != null)
                            {
                                if (declaration.DeclaredElement is ITypeElement)
                                {
                                    ITypeElement typeElement = declaration.DeclaredElement as ITypeElement;
                                    IEnumerable<IDeclaredType> parentTypes =
                                        typeElement.GetSuperTypesWithoutCircularDependent();
                                    result = typeNames.Any(typeName => typeElement.GetClrName().Equals(typeName) ||
                                                                        parentTypes.Any(
                                                                            parentType =>
                                                                                parentType.GetClrName()
                                                                                    .Equals(typeName)));
                                }
                            }
                            else
                                result =
                                    typeNames.Any(
                                        clrTypeName => clrDeclaredElement.ToString().Contains(clrTypeName.FullName));
                        }
                    }
                    
                }
            }

            return result;
        }

        public static bool IsOutOfSPContext(this IExpression element, ITypeDeclaration elementContainingTypeDeclaration)
        {
            bool result = false;
            List<IClrTypeName> typeNames = new List<IClrTypeName> { ClrTypeKeys.SPFeatureReceiver };
            typeNames.AddRange(ClrTypeKeys.SPTimerJobs);
            typeNames.AddRange(ClrTypeKeys.SPEventReceivers);
            typeNames.AddRange(ClrTypeKeys.SPWFActivities);

            if (element != null && elementContainingTypeDeclaration.DeclaredElement != null)
            {
                IDeclaredElement referenceExpressionTarget = element.ReferenceExpressionTarget();
                IEnumerable<IDeclaredType> parenClasses = elementContainingTypeDeclaration.DeclaredElement.GetAllSuperClasses();
                
                result =
                    typeNames.Any(
                        typeName =>
                            parenClasses.Any(parenClass => parenClass.GetClrName().Equals(typeName)) &&
                            (!ClrTypeKeys.SPEventReceivers.Contains(typeName) ||
                                IsAsyncEventReceiver(referenceExpressionTarget.ShortName)));
                
            }

            return result;
        }

        public static string ReadableName(this IExpression element)
        {
            string result = String.Empty;

            if (element != null)
            {
                IDeclaredElement referenceExpressionTarget = element.ReferenceExpressionTarget();

                if (referenceExpressionTarget is IClrDeclaredElement)
                {
                    IClrDeclaredElement clrDeclaredElement = referenceExpressionTarget as IClrDeclaredElement;
                    ITypeElement containingType = clrDeclaredElement.GetContainingType();
                    if (containingType != null)
                    {
                        result = String.Format("{0}: {1}", containingType.GetClrName(), clrDeclaredElement.ShortName);
                    }
                }
                else
                    result = referenceExpressionTarget == null ? element.ToString() : referenceExpressionTarget.ToString();
            }

            return result;
        }

        private static bool IsAsyncEventReceiver(string methodName)
        {
            return
                TypeInfo.SPItemEventReceiverAsynchronousEvents.Contains(methodName) ||
                TypeInfo.SPListEventReceiverAsynchronousEvents.Contains(methodName) ||
                TypeInfo.SPWebEventReceiverAsynchronousEvents.Contains(methodName);
        }
    }
}
