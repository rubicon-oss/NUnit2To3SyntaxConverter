using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit2To3SyntaxConverter.Validation;

namespace NUnit2To3SyntaxConverter.ExpectedException.Validators
{
  public class LastStatementIsExpressionStatementValidator: IValidator<MethodDeclarationSyntax>
  {
    public IValidationError? Validate (MethodDeclarationSyntax input)
    {
      if(!(input.Body?.Statements.LastOrDefault() is ExpressionStatementSyntax))
        return new ExpectedExceptionValidationError(input, "Unable to convert method with non expression statement in last position.");
      return null;
    }
  }
}