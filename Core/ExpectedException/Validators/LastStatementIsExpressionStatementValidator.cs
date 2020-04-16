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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit2To3SyntaxConverter.Validation;

namespace NUnit2To3SyntaxConverter.ExpectedException.Validators
{
  /// <summary>
  /// Validator for the ExpectedException transformation. This Validator checks if the last statement is an ExpressionStatement. If not (eg if-statement, while-loop)
  /// the validator returns a ValidationError.
  /// </summary>
  public class LastStatementIsExpressionStatementValidator : IValidator<MethodDeclarationSyntax>
  {
    private readonly string _category;

    public LastStatementIsExpressionStatementValidator (string category)
    {
      _category = category;
    }

    public IEnumerable<ValidationError> Validate (MethodDeclarationSyntax input)
    {
      var lastStatement = input.Body?.Statements.LastOrDefault();
      if (lastStatement == null
          || lastStatement is ExpressionStatementSyntax
          || lastStatement is ThrowStatementSyntax)
        yield break;

      yield return ValidationError.CreateFromMethodSyntax (input, _category, "Unable to convert method with non expression statement in last position.");
    }
  }
}