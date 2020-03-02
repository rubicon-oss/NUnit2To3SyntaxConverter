using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit2To3SyntaxConverter.Validation;

namespace NUnit2To3SyntaxConverter.ExpectedException.Validators
{
  public class AssertInLastStatementValidator: IValidator<MethodDeclarationSyntax>
  {
    public IValidationError? Validate (MethodDeclarationSyntax input)
    {
      if (input?.Body?.Statements.LastOrDefault()?.WithoutTrivia()?.ToString().StartsWith ("Assert.") == true)
        return new ExpectedExceptionValidationError(input, "Unable to convert method with Assertion in last statement.");
      return null;
    }
  }
}