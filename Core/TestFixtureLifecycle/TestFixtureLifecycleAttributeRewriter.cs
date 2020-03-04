using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnit2To3SyntaxConverter.TestFixtureLifecycle
{
  public class TestFixtureLifecycleAttributeRewriter : CSharpSyntaxRewriter
  {
    private readonly TestFixtureSetupAttributeRenamer _setupRewriter;
    private readonly TestFixtureTeardownAttributeRenamer _teardownRewriter;

    public TestFixtureLifecycleAttributeRewriter (
        TestFixtureSetupAttributeRenamer setupRewriter,
        TestFixtureTeardownAttributeRenamer teardownAttributeRewriter)
    {
      _setupRewriter = setupRewriter;
      _teardownRewriter = teardownAttributeRewriter;
    }

    public override SyntaxNode VisitAttribute (AttributeSyntax node)
    {
      return _teardownRewriter.Transform (
          _setupRewriter.Transform (node, default),
          default);
    }
  }
}