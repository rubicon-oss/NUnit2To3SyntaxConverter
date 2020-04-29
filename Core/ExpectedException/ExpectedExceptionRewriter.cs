#region copyright

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
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit2To3SyntaxConverter.ExpectedException.Validators;
using NUnit2To3SyntaxConverter.Validation;
using Serilog;

namespace NUnit2To3SyntaxConverter.ExpectedException
{
  public class ExpectedExceptionRewriter : CSharpSyntaxRewriter
  {
    private readonly ExpectedExceptionAttributeRemover _attributeRemover;
    private readonly ExpectedExceptionMethodBodyTransformer _methodBodyTransformer;
    private readonly SemanticModel _semanticModel;
    private readonly IValidator<MethodDeclarationSyntax> _validator;

    public ExpectedExceptionRewriter (
        SemanticModel semanticModel,
        ExpectedExceptionMethodBodyTransformer methodBodyTransformer,
        ExpectedExceptionAttributeRemover attributeRemover)
    {
      _semanticModel = semanticModel;
      _methodBodyTransformer = methodBodyTransformer;
      _attributeRemover = attributeRemover;
      _validator = new ExpectedExceptionValidator();
    }

    public override SyntaxNode VisitMethodDeclaration (MethodDeclarationSyntax node)
    {
      var expectedExceptionAttribute = QueryExpectedExceptionAttributes (node);

      if (expectedExceptionAttribute == null)
        return node;

      var errors = _validator.Validate (node).ToList();

      foreach (var error in errors)
      {
        Log.Warning ("{@file} - {@method}: {@category}:\n {@message}", error.FileName, error.MethodName, error.Category, error.Reason);
      }

      if (errors.Count != 0)
        return node;

      var withTransformedBody = _methodBodyTransformer.Transform (node, expectedExceptionAttribute);
      return _attributeRemover.Transform (withTransformedBody, expectedExceptionAttribute);
    }

    private ExpectedExceptionModel QueryExpectedExceptionAttributes (BaseMethodDeclarationSyntax node)
    {
      var methodSymbol = _semanticModel.GetDeclaredSymbol (node);
      var attributes = methodSymbol.GetAttributes();
      return attributes
          .Where (attribute => attribute.AttributeClass.Name == "ExpectedExceptionAttribute")
          .Select (ExpectedExceptionModel.CreateFromAttributeData)
          .SingleOrDefault();
    }
  }
}