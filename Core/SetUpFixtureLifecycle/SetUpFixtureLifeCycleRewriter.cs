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
          tearDownAttributeRenamer
          );
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