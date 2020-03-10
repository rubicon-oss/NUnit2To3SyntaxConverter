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
//

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnit2To3SyntaxConverter.SetUpFixtureLifecycle
{
  public class SetUpFixtureLifeCycleRewriter : CSharpSyntaxRewriter
  {
    private readonly SemanticModel _documentSemanticModel;
    private readonly SetUpFixtureLifeCycleAttributeRewriter _lifeCycleAttributeRewriter;

    public SetUpFixtureLifeCycleRewriter (
        SemanticModel documentSemanticModel,
        SetUpFixtureSetUpAttributeRenamer setUpAttributeRenamer,
        SetUpFixtureTearDownAttributeRenamer tearDownAttributeRenamer)
    {
      _documentSemanticModel = documentSemanticModel;
      _lifeCycleAttributeRewriter = new SetUpFixtureLifeCycleAttributeRewriter (
          setUpAttributeRenamer,
          tearDownAttributeRenamer);
    }

    public override SyntaxNode VisitClassDeclaration (ClassDeclarationSyntax node)
    {
      var classSymbol = _documentSemanticModel.GetDeclaredSymbol (node);
      return InheritsFromSetUpFixture (classSymbol)
          ? _lifeCycleAttributeRewriter.Visit (node)
          : node;
    }

    private bool InheritsFromSetUpFixture (INamedTypeSymbol typeSymbol)
    {
      var inheritanceChain = new List<INamedTypeSymbol> { typeSymbol };
      for (INamedTypeSymbol? symbol = typeSymbol; symbol != null; symbol = symbol.BaseType)
      {
        inheritanceChain.Add (symbol);
      }

      return inheritanceChain.Any (IsSetUpFixture);
    }

    private bool IsSetUpFixture (INamedTypeSymbol typeSymbol)
    {
      var setupFixtureAttribute = typeSymbol.GetAttributes()
          .SingleOrDefault (attribute => attribute.AttributeClass.Name.Equals ("SetUpFixtureAttribute"));
      return setupFixtureAttribute != null;
    }
  }
}