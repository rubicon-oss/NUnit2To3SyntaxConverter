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

using System.Linq;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.ExpectedException.Validators;
using NUnit2To3SyntaxConverter.Unittests.Helpers;

namespace NUnit2To3SyntaxConverter.UnitTests.ExpectedException
{
  [TestFixture]
  public class ValidatorsTest
  {
    private const string c_validatorTestCasesFileName = "resources/ExpectedExceptionValidatorTestCases.cs";
    private const string c_errorCategory = "ExpectedException";


    [Test]
    [TestCase ("HasEmptyBody")]
    public void MethodBodyValidator_ValidatesWithError (string method)
    {
      var methodSyntax = CompiledSourceFileProvider.LoadMethod (c_validatorTestCasesFileName, method);
      var validator = new MethodBodyNotEmptyValidator();

      var errors = validator.Validate (methodSyntax).ToList();

      Assert.That (errors, Has.One.Items);
      var error = errors.Single();

      Assert.That (error.Category, Is.EqualTo (c_errorCategory));
      Assert.That (error.Reason, Is.EqualTo ("Unable to convert method with empty body."));
      Assert.That (error.MethodName, Is.EqualTo (method));
    }

    [Test]
    [TestCase ("HasAssertInLastPosition")]
    [TestCase ("HasOnlyAssert")]
    [TestCase ("HasIfInLastPosition")]
    [TestCase ("HasTryInLastPosition")]
    [TestCase ("HasLoopInLastPosition")]
    [TestCase ("ShouldValidateWithoutErrors")]
    public void MethodBodyValidator_ValidatesWithoutError (string method)
    {
      var methodSyntax = CompiledSourceFileProvider.LoadMethod (c_validatorTestCasesFileName, method);
      var validator = new MethodBodyNotEmptyValidator();

      var errors = validator.Validate (methodSyntax);

      Assert.That (errors, Is.Empty);
    }

    [Test]
    [TestCase ("HasAssertInLastPosition")]
    [TestCase ("HasOnlyAssert")]
    public void AssertInLastStatementValidator_ValidatesWithError (string method)
    {
      var methodSyntax = CompiledSourceFileProvider.LoadMethod (c_validatorTestCasesFileName, method);
      var validator = new AssertInLastStatementValidator();

      var errors = validator.Validate (methodSyntax).ToList();

      Assert.That(errors, Has.One.Items);
      var error = errors.Single();
      
      Assert.That (error.Category, Is.EqualTo (c_errorCategory));
      Assert.That (error.Reason, Is.EqualTo ("Unable to convert method with Assertion in last statement."));
      Assert.That (error.MethodName, Is.EqualTo (method));
    }

    [Test]
    [TestCase ("HasEmptyBody")]
    [TestCase ("HasIfInLastPosition")]
    [TestCase ("HasTryInLastPosition")]
    [TestCase ("HasLoopInLastPosition")]
    [TestCase ("ShouldValidateWithoutErrors")]
    public void AssertInLastStatementValidator_ValidatesWithoutError (string method)
    {
      var methodSyntax = CompiledSourceFileProvider.LoadMethod (c_validatorTestCasesFileName, method);
      var validator = new AssertInLastStatementValidator();

      var errors = validator.Validate (methodSyntax);

      Assert.That (errors, Is.Empty);
    }

    [Test]
    [TestCase ("HasIfInLastPosition")]
    [TestCase ("HasTryInLastPosition")]
    [TestCase ("HasLoopInLastPosition")]
    public void LastStatementIsExpressionStatementValidator_ValidatesWithError (string method)
    {
      var methodSyntax = CompiledSourceFileProvider.LoadMethod (c_validatorTestCasesFileName, method);
      var validator = new LastStatementIsExpressionStatementValidator();

      var errors = validator.Validate (methodSyntax).ToList();

      Assert.That(errors, Has.One.Items);
      var error = errors.Single();

      Assert.That (error.Category, Is.EqualTo (c_errorCategory));
      Assert.That (error.Reason, Is.EqualTo ("Unable to convert method with non expression statement in last position."));
      Assert.That (error.MethodName, Is.EqualTo (method));
    }

    [Test]
    [TestCase ("HasEmptyBody")]
    [TestCase ("HasAssertInLastPosition")]
    [TestCase ("HasOnlyAssert")]
    [TestCase ("ShouldValidateWithoutErrors")]
    public void LastStatementIsExpressionStatementValidator_ValidatesWithoutError (string method)
    {
      var methodSyntax = CompiledSourceFileProvider.LoadMethod (c_validatorTestCasesFileName, method);
      var validator = new LastStatementIsExpressionStatementValidator();

      var errors = validator.Validate (methodSyntax);

      Assert.That (errors, Is.Empty);
    }

    [Test]
    [TestCase ("HasEmptyBody", "Unable to convert method with empty body.")]
    [TestCase ("HasAssertInLastPosition", "Unable to convert method with Assertion in last statement.")]
    [TestCase ("HasOnlyAssert", "Unable to convert method with Assertion in last statement.")]
    [TestCase ("HasIfInLastPosition", "Unable to convert method with non expression statement in last position.")]
    [TestCase ("HasTryInLastPosition", "Unable to convert method with non expression statement in last position.")]
    [TestCase ("HasLoopInLastPosition", "Unable to convert method with non expression statement in last position.")]
    public void AllValidators_ValidateWithError (string method, params string[] reasons)
    {
      var methodSyntax = CompiledSourceFileProvider.LoadMethod (c_validatorTestCasesFileName, method);
      var validator = new ExpectedExceptionValidator();

      var errors = validator.Validate (methodSyntax).ToList();

      Assert.That (errors, Is.Not.Empty);
      Assert.That (errors.Select (e => e.Reason), Is.EquivalentTo (reasons));
    }

    [Test]
    [TestCase ("ShouldValidateWithoutErrors")]
    public void AllValidators_ValidateWithoutErrors (string method)
    {
      var methodSyntax = CompiledSourceFileProvider.LoadMethod (c_validatorTestCasesFileName, method);
      var validator = new ExpectedExceptionValidator();

      var errors = validator.Validate (methodSyntax);

      Assert.That (errors, Is.Empty);
    }
  }
}