// Copyright (c) rubicon IT GmbH
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit2To3SyntaxConverter.Validation;

namespace NUnit2To3SyntaxConverter.ExpectedException.Validators
{
  /// <summary>
  /// Validator for the ExpectedException transformation. This validator is a compound object of all validations for this transformation.
  /// </summary>
  public class ExpectedExceptionValidator : IValidator<MethodDeclarationSyntax>
  {
    private const string c_errorCategory = "ExpectedException";

    private readonly IEnumerable<IValidator<MethodDeclarationSyntax>> _innerValidators
        = ImmutableList.Create<IValidator<MethodDeclarationSyntax>> (
            new MethodBodyNotEmptyValidator(c_errorCategory),
            new AssertInLastStatementValidator(c_errorCategory),
            new LastStatementIsExpressionStatementValidator(c_errorCategory));

    public IEnumerable<ValidationError> Validate (MethodDeclarationSyntax input)
    {
      return _innerValidators.SelectMany (validator => validator.Validate (input));
    }
  }
}