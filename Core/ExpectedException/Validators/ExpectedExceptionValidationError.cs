using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit2To3SyntaxConverter.Validation;

namespace NUnit2To3SyntaxConverter.ExpectedException.Validators
{
  public class ExpectedExceptionValidationError : IValidationError

  {
    private readonly MethodDeclarationSyntax _method;
    private readonly string _reason;

    public ExpectedExceptionValidationError (MethodDeclarationSyntax method, string reason)
    {
      _method = method;
      _reason = reason;
    }

    public string Category => "ExpectedException";
    
    public string Reason => _reason;

    public string FileName => _method.GetLocation().GetMappedLineSpan().Path;

    public string MethodName => _method.Identifier.ToString();
  }
}