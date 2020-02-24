#region copyright

// 
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

#endregion

using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.ExpectedException;
using NUnit2To3SyntaxConverter.Unittests.Helpers;

namespace NUnit2To3SyntaxConverter.UnitTests.ExpectedException
{
  public class FeatureIntegrationTest
  {
    private const string c_testCasesSourceFileName = "/resources/ExpectedExceptionIntegrationTestCases.cs";
    private const string c_expectedTestResultsSourceFileName = "/resources/ExpectedExceptionIntegrationTestCasesExpected.cs";

    [Test]
    [TestCase ("SimpleBaseCase")]
    [TestCase ("WithCustomExceptionType")]
    [TestCase ("WithWellKnownExceptionType")]
    [TestCase ("WithCustomExpectedMessage")]
    [TestCase ("WithCustomExpectedMessageAndMatchTypeRegex")]
    [TestCase ("WithAllCustomFieldsAndLongMessage")]
    [TestCase ("WithMultiLineString")]
    public void ExpectedExceptionRewritesToExpectedCases (string method)
    {
      var (rewriter, syntax) = CreateRewriterFor (c_testCasesSourceFileName, method);
      var expectedSyntax = CompiledSourceFileProvider.LoadMethod (c_expectedTestResultsSourceFileName, method);
      
      var rewrittenMethodSyntax = rewriter.VisitMethodDeclaration (syntax);

      Assert.That (rewrittenMethodSyntax.ToFullString().Trim(), Is.EqualTo (expectedSyntax.ToFullString().Trim()));
    }

    private (ExpectedExceptionRewriter, MethodDeclarationSyntax) CreateRewriterFor (string file, string methodName)
    {
      var (semantic, root) = CompiledSourceFileProvider.LoadCompilationFromFile (file);
      var rewriter = new ExpectedExceptionRewriter (semantic, new ExpectedExceptionMethodBodyTransformer(), new ExpectedExceptionAttributeRemover());
      var method = root.DescendantNodes()
          .OfType<MethodDeclarationSyntax>()
          .Single (syntax => syntax.Identifier.ToString() == methodName);
      return (rewriter, method);
    }
  }
}