using System.Drawing.Design;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Win32;
using NUnit2To3SyntaxConverter.Validation;

namespace NUnit2To3SyntaxConverter.ExpectedException.Validators
{
  public class EmptyMethodBodyValidator: IValidator<MethodDeclarationSyntax>
  {
    public IValidationError? Validate (MethodDeclarationSyntax method)
    {
      if(method.Body == null || method.Body.Statements.Count == 0)
        return new ExpectedExceptionValidationError(method, "Unable to convert method with empty body.");
      return null;
    }
  }
}