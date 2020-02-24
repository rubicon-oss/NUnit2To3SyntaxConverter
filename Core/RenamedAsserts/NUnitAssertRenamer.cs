using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnit2To3SyntaxConverter.RenamedAsserts
{
  public class NUnitAssertRenamer: ISyntaxTransformer<MemberAccessExpressionSyntax, (ExpressionSyntax, SimpleNameSyntax)>
  {
    public MemberAccessExpressionSyntax Transform (MemberAccessExpressionSyntax node, (ExpressionSyntax, SimpleNameSyntax) context)
    {

      var (newExpression, newAccessor) = context;

      var exprWithTrivia = newExpression.WithTriviaFrom (node.Expression);
      var accessorWithTrivia = newAccessor.WithTriviaFrom (node.Name);
      
      return node.WithExpression (exprWithTrivia)
          .WithName (accessorWithTrivia);
    }
  }
}