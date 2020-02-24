using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnit2To3SyntaxConverter.RenamedAsserts
{
  public class NUnitAssertRewriter : CSharpSyntaxRewriter
  {
    private readonly RenamedAsserts _renamedAsserts = new RenamedAsserts();
    private readonly NUnitAssertRenamer _assertRenamer = new NUnitAssertRenamer();

    public override SyntaxNode VisitMemberAccessExpression (MemberAccessExpressionSyntax node)
    {
      var maybeNewName = _renamedAsserts.TryGetSyntax (node.Expression, node.Name);
      
      return maybeNewName.HasValue 
          ? _assertRenamer.Transform(node, maybeNewName.Value) 
          : node;
    }
  }
}