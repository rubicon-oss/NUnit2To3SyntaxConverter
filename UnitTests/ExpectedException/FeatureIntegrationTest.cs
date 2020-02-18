using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.ExpectedException;

namespace NUnit2To3SyntaxConverter.UnitTests.ExpectedException
{
  public class FeatureIntegrationTest
  {
    public (ExpectedExceptionRewriter, MethodDeclarationSyntax) RewriterFor (string file, string methodName)
    {
      var (semantic, root) = CompiledSourceFileProvider.LoadSemanticModel (file);
      var rewriter = new ExpectedExceptionRewriter (semantic, new ExpectedExceptionMethodBodyTransformer(), new ExpectedExceptionAttributeRemover());
      var method = root.DescendantNodes()
          .OfType<MethodDeclarationSyntax>()
          .Single (syntax => syntax.Identifier.ToString() == methodName);
      return (rewriter, method);
    }

    [Test]
    [TestCase (
        "/resources/ExpectedExceptionIntegrationTestCases.cs",
        "/resources/ExpectedExceptionIntegrationTestCasesExpected.cs",
        "SimpleBaseCase")]
    [TestCase (
        "/resources/ExpectedExceptionIntegrationTestCases.cs",
        "/resources/ExpectedExceptionIntegrationTestCasesExpected.cs",
        "WithCustomExceptionType")]
    [TestCase (
        "/resources/ExpectedExceptionIntegrationTestCases.cs",
        "/resources/ExpectedExceptionIntegrationTestCasesExpected.cs",
        "WithWellKnownExceptionType")]
    [TestCase (
        "/resources/ExpectedExceptionIntegrationTestCases.cs",
        "/resources/ExpectedExceptionIntegrationTestCasesExpected.cs",
        "WithCustomExpectedMessage")]
    [TestCase (
        "/resources/ExpectedExceptionIntegrationTestCases.cs",
        "/resources/ExpectedExceptionIntegrationTestCasesExpected.cs",
        "WithCustomExpectedMessageAndMatchTypeRegex")]
    [TestCase (
        "/resources/ExpectedExceptionIntegrationTestCases.cs",
        "/resources/ExpectedExceptionIntegrationTestCasesExpected.cs",
        "WithAllCustomFieldsAndLongMessage")]
    [TestCase (
        "/resources/ExpectedExceptionIntegrationTestCases.cs",
        "/resources/ExpectedExceptionIntegrationTestCasesExpected.cs",
        "WithMultiLineString")]
    public void FormatSimpleCase (string filename, string filenameExpected, string method)
    {
      var (rewriter, syntax) = RewriterFor (filename, method);
      var (_, expectedSyntax) = CompiledSourceFileProvider.LoadMethod (filenameExpected, method);

      Assert.That (
          rewriter.VisitMethodDeclaration (syntax).ToFullString().Trim(),
          Is.EqualTo (expectedSyntax.ToFullString().Trim()));
    }
  }
}