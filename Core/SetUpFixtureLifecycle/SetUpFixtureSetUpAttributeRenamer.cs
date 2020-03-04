using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnit2To3SyntaxConverter.SetUpFixtureLifecycle
{
  public class SetUpFixtureSetUpAttributeRenamer : ISyntaxTransformer<AttributeSyntax, ValueTuple>
  {
    public AttributeSyntax Transform (AttributeSyntax node, ValueTuple context)
    {
      return node.Name.WithoutTrivia().ToString() == "SetUp" 
          ? node.WithName (IdentifierName ("OneTimeSetUp")) 
          : node;
    }
  }
}