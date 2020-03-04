using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnit2To3SyntaxConverter.TestFixtureLifecycle
{
  public class TestFixtureSetupAttributeRenamer: ISyntaxTransformer<AttributeSyntax, ValueTuple>
  {
    public AttributeSyntax Transform (AttributeSyntax node, ValueTuple context)
    {
      return node.Name.ToString() == "TestFixtureSetUp"
          ? node.WithName (SyntaxFactory.IdentifierName ("OneTimeSetUp")) 
          : node;
    }
  }
}