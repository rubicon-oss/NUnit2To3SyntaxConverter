using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.SetUpFixtureLifecycle;
using NUnit2To3SyntaxConverter.Unittests.Helpers;

namespace NUnit2To3SyntaxConverter.UnitTests.SetupFixtureLifeCycleAttributes
{
  public class SetUpFixtureRewriteTest
  {
    public (SetUpFixtureLifeCycleRewriter, ClassDeclarationSyntax) RewriterFor (string file, string className)
    {
      var (semantic, root) = CompiledSourceFileProvider.LoadCompilationFromFile (file);
      var rewriter = new SetUpFixtureLifeCycleRewriter (
          semantic,
          new SetUpFixtureSetUpAttributeRenamer(),
          new SetUpFixtureTearDownAttributeRenamer());
      var method = root.DescendantNodes()
          .OfType<ClassDeclarationSyntax>()
          .Single (syntax => syntax.Identifier.ToString() == className);
      return (rewriter, method);
    }

    [Test]
    [TestCase ("resources/SetUpFixtureTests.cs", "SetUpFixtureWithSetupOnly")]
    [TestCase ("resources/SetUpFixtureTests.cs", "SetUpFixtureWithSetupAndTeardown")]
    [TestCase ("resources/SetUpFixtureTests.cs", "SetUpFixtureWithTeardownOnly")]
    [TestCase ("resources/SetUpFixtureTests.cs", "SetUpFixtureWithOtherMethods")]
    [TestCase ("resources/SetUpFixtureTests.cs", "SetUpFixtureByInheritance")]
    public void TestSetUpFixtureRewrite (string file, string className)
    {
      var expectedClass = className + "Transformed";
      var expectedSyntax = CompiledSourceFileProvider.LoadClass (file, expectedClass);
      var (rewriter, classSyntax) = RewriterFor (file, className);

      var result = (ClassDeclarationSyntax) rewriter.Visit (classSyntax);

      Assert.That (
          result.Members.ToFullString(),
          Is.EqualTo (expectedSyntax.Members.ToFullString()));
    }
  }
}