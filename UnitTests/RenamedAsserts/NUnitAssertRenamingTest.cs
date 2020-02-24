using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.RenamedAsserts;

namespace NUnit2To3SyntaxConverter.UnitTests.RenamedAsserts
{
  public class NUnitAssertRenamingTest
  {
    [Test]
    [TestCase ("Text.DoesNotMatch ($1)", "Does.Not.Match ($1)")]
    [TestCase ("Text.Matches ($1)", "Does.Match ($1)")]
    [TestCase ("Text.DoesNotEndWith ($1)", "Does.Not.EndWith ($1)")]
    [TestCase ("Text.EndsWith ($1)", "Does.EndWith ($1)")]
    [TestCase ("Text.DoesNotStartWith ($1)", "Does.Not.StartWith ($1)")]
    [TestCase ("Text.StartsWith ($1)", "Does.StartWith ($1)")]
    [TestCase ("Text.Contains ($1)", "Does.Contain ($1)")]
    [TestCase ("Text.DoesNotContain ($1)", "Does.Not.Contain ($1)")]
    [TestCase ("Is.InstanceOfType<T> ($1)", "Is.InstanceOf<T> ($1)")]
    [TestCase ("Is.StringStarting ($1)", "Does.StartWith ($1)")]
    [TestCase ("Is.StringContaining ($1)", "Does.Contain ($1)")]
    [TestCase ("Is.StringEnding ($1)", "Does.EndWith ($1)")]
    [TestCase ("Is.StringMatching ($1)", "Does.Match ($1)")]
    public void TestAllSimpleCases (string toRename, string expected)
    {
      var expression = SyntaxFactory.ParseExpression (toRename);
      var actual = new NUnitAssertRewriter().Visit (expression);
      Assert.That (actual.ToFullString(), Is.EqualTo (expected));
    }

    [Test]
    [TestCase ("Text\n.DoesNotMatch ($1)", "Does.Not\n.Match ($1)")]
    [TestCase ("Text.\nMatches ($1)", "Does.\nMatch ($1)")]
    [TestCase ("Text. DoesNotEndWith ($1)", "Does.Not. EndWith ($1)")]
    [TestCase ("Text .EndsWith ($1)", "Does .EndWith ($1)")]
    [TestCase ("Text.\n        DoesNotStartWith ($1)", "Does.Not.\n        StartWith ($1)")]
    [TestCase ("Text\n        .StartsWith ($1)", "Does\n        .StartWith ($1)")]
    [TestCase ("Text\n        .StartsWith($1)", "Does\n        .StartWith($1)")]
    public void DoesHandleWhitespace (string toRename, string expected)
    {
      var expression = SyntaxFactory.ParseExpression (toRename);
      var actual = new NUnitAssertRewriter().Visit (expression);
      Assert.That (actual.ToFullString(), Is.EqualTo (expected));
    }

    [Test]
    [TestCase ("")]
    [TestCase ("Text")]
    [TestCase ("Text.Other ($1)")]
    [TestCase ("Is.Other ($1)")]
    public void DoesNotRewriteNonChangedAsserts (string toRename)
    {
      var expression = SyntaxFactory.ParseExpression (toRename);
      var actual = new NUnitAssertRewriter().Visit (expression);
      Assert.That (actual, Is.EqualTo (expression));
    }
  }
}