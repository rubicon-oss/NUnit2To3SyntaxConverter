using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnit2To3SyntaxConverter.TestFixtureLifecycle
{
  public class TestFixtureTeardownAttributeRenamer: ISyntaxTransformer<AttributeSyntax, ValueTuple>
  {
    public AttributeSyntax Transform (AttributeSyntax node, ValueTuple context)
    {
      return node.Name.ToString() == "TestFixtureTearDown"
          ? node.WithName (SyntaxFactory.IdentifierName ("OneTimeTearDown"))
          : node;
    }
  }
}