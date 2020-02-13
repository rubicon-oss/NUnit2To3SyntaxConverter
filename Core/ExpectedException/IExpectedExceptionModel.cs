using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnit2To3SyntaxConverter.ExpectedException
{
  public interface IExpectedExceptionModel
  {
    Task<AttributeSyntax> GetAttributeSyntax ();
    ExpressionSyntax ExceptionType { get; }
    ExpressionSyntax UserMessage { get; }
    ExpressionSyntax MatchType { get; }
    ExpressionSyntax ExpectedMessage { get; }

    ExpressionSyntax AsConstraintExpression ();
  }
}