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
      var (newExpression, newAccessor) = _renamedAsserts.TryGetSyntax (node.Expression, node.Name)
          .GetValueOrDefault ((node.Expression, node.Name));

      var exprWithTrivia = newExpression.WithTriviaFrom (node.Expression);
      var accessorWithTrivia = newAccessor.WithTriviaFrom (node.Name);
      return node.WithExpression (exprWithTrivia)
          .WithName (accessorWithTrivia);
    }
  }
}