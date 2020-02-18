using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnit2To3SyntaxConverter.RenamedAsserts
{
  public class NUnitAssertRenamer : CSharpSyntaxRewriter
  {
    private RenamedAsserts _renamedAsserts = new RenamedAsserts();

    public override SyntaxNode VisitMemberAccessExpression (MemberAccessExpressionSyntax node)
    {
      var maybeNewName = _renamedAsserts.TryGetSyntax (node.Expression, node.Name);

      if (!maybeNewName.HasValue)
        return node;
      
      Console.WriteLine($"Rewriting old assert:\n   {node}\n   in {node.SyntaxTree.FilePath} on pos {node.GetLocation().GetLineSpan().StartLinePosition}\n");
      
      var (newExpression, newAccessor) = maybeNewName.Value;

      var exprWithTrivia = newExpression.WithTriviaFrom (node.Expression);
      var accessorWithTrivia = newAccessor.WithTriviaFrom (node.Name);
      
      return node.WithExpression (exprWithTrivia)
          .WithName (accessorWithTrivia);

    }
  }
}