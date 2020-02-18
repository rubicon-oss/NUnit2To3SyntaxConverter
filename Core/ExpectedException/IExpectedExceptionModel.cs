using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnit2To3SyntaxConverter.ExpectedException
{
  public interface IExpectedExceptionModel
  {
    ExpressionSyntax ExceptionType { get; }
    ExpressionSyntax UserMessage { get; }
    ExpressionSyntax MatchType { get; }
    ExpressionSyntax ExpectedMessage { get; }
    Task<AttributeSyntax> GetAttributeSyntax ();

    ExpressionSyntax AsConstraintExpression (string baseIndent);
  }
}