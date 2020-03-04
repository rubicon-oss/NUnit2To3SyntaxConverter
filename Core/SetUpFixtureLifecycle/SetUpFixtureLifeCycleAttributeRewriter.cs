using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnit2To3SyntaxConverter.SetUpFixtureLifecycle
{
  public class SetUpFixtureLifeCycleAttributeRewriter : CSharpSyntaxRewriter
  {
    private readonly SetUpFixtureSetUpAttributeRenamer _setUpRenamer;
    private readonly SetUpFixtureTearDownAttributeRenamer _tearDownRenamer;

    public SetUpFixtureLifeCycleAttributeRewriter (
        SetUpFixtureSetUpAttributeRenamer setUpRenamer,
        SetUpFixtureTearDownAttributeRenamer tearDownRenamer)
    {
      _setUpRenamer = setUpRenamer;
      _tearDownRenamer = tearDownRenamer;
    }

    public override SyntaxNode VisitAttribute (AttributeSyntax node)
    {
      return _tearDownRenamer.Transform (_setUpRenamer.Transform (node, default), default);
    }
  }
}