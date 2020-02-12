﻿#region copyright

// 
// Copyright (c) rubicon IT GmbH
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NUnit2To3SyntaxConverter.Extensions.SyntaxFactoryUtils;

namespace NUnit2To3SyntaxConverter.ExpectedException
{
    public class ExpectedExceptionRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;
        private readonly NUnitAssemblyFilter _assemblyFilter;
        private readonly ISyntaxTransformer<MethodDeclarationSyntax, ExpectedExceptionModel> _methodBodyTransformer;
        private readonly ISyntaxTransformer<MethodDeclarationSyntax, ExpectedExceptionModel> _attributeRemover;

        public ExpectedExceptionRewriter (
                SemanticModel semanticModel,
                ISyntaxTransformer<MethodDeclarationSyntax, ExpectedExceptionModel> methodBodyTransformer,
                ISyntaxTransformer<MethodDeclarationSyntax, ExpectedExceptionModel> attributeRemover)
        {
            _semanticModel = semanticModel;
            _methodBodyTransformer = methodBodyTransformer;
            _attributeRemover = attributeRemover;
            _assemblyFilter = new NUnitAssemblyFilter();
        }

        public override SyntaxNode VisitMethodDeclaration (MethodDeclarationSyntax node)
        {
            var expectedExceptionAttribute = QueryExpectedExceptionAttributes (node).SingleOrDefault();

            if (expectedExceptionAttribute == null) return node;

            return _attributeRemover.Transform (_methodBodyTransformer.Transform (node, expectedExceptionAttribute), expectedExceptionAttribute);
        }

        private IEnumerable<ExpectedExceptionModel> QueryExpectedExceptionAttributes (BaseMethodDeclarationSyntax node)
        {
            var methodSymbol = _semanticModel.GetDeclaredSymbol (node);
            var attributes = methodSymbol.GetAttributes();
            return attributes.Where (attribute => _assemblyFilter.IsSupportedAssembly (attribute.AttributeClass.ContainingAssembly.Identity))
                    .Where (attribute => attribute.AttributeClass.Name == "ExpectedExceptionAttribute")
                    .Select (CreateFromAttributeData)
                    .ToList();
        }

        private static ExpectedExceptionModel CreateFromAttributeData (AttributeData attribute)
        {
            var attributeSyntax = attribute.ApplicationSyntaxReference.GetSyntax() as AttributeSyntax;
            Debug.Assert (attributeSyntax != null);

            var attributeArguments = attributeSyntax!.ArgumentList!.Arguments;
            var builder = new ExpectedExceptionModelBuilder();

            foreach (var attributeArgument in attributeArguments)
            {
                var value = attributeArgument.Expression;
                var namedArgumentName = attributeArgument.NameColon?.Name
                                        ?? attributeArgument.NameEquals?.Name;
                Debug.Assert (value != null);

                builder = namedArgumentName?.ToString() switch
                {
                        "UserMessage" => builder.WithUserMessage (value),
                        "ExpectedException" => builder.WithExceptionType (value),
                        "ExpectedMessage" => builder.WithExpectedMessage (value),
                        "MatchType" => builder.WithMatchType(value),
                        "ExpectedExceptionName" => builder.WithExceptionName (value),
                        "Handler" => builder.WithHandler (value),
                        null => builder.WithExceptionType (value),
                        _ => builder
                };
            }

            return builder.Build (attribute);
        }
    }
}