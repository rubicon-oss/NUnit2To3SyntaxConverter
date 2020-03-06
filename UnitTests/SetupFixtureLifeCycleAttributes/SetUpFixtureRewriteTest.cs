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

using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.SetUpFixtureLifecycle;
using NUnit2To3SyntaxConverter.Unittests.Helpers;

namespace NUnit2To3SyntaxConverter.UnitTests.SetupFixtureLifeCycleAttributes
{
  public class SetUpFixtureRewriteTest
  {
    private const string c_testCasesSourceFileName = "resources/SetUpFixtureTests.cs";
    
    [Test]
    [TestCase ("SetUpFixtureWithSetupOnly")]
    [TestCase ("SetUpFixtureWithSetupAndTeardown")]
    [TestCase ("SetUpFixtureWithTeardownOnly")]
    [TestCase ("SetUpFixtureWithOtherMethods")]
    [TestCase ("SetUpFixtureByInheritance")]
    public void TestSetUpFixtureRewrite_RewriteAttributeToNewNames (string className)
    {
      var expectedClass = className + "Transformed";
      var expectedSyntax = CompiledSourceFileProvider.LoadClass (c_testCasesSourceFileName, expectedClass);
      var (rewriter, classSyntax) = CreateRewriterFor (c_testCasesSourceFileName, className);

      var result = (ClassDeclarationSyntax) rewriter.Visit (classSyntax);

      Assert.That (
          result.Members.ToFullString(),
          Is.EqualTo (expectedSyntax.Members.ToFullString()));
    }

    private (SetUpFixtureLifeCycleRewriter, ClassDeclarationSyntax) CreateRewriterFor (string file, string className)
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
  }
}