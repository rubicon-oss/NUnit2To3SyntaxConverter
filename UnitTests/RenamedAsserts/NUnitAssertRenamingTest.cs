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

using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.RenamedAsserts;

namespace NUnit2To3SyntaxConverter.UnitTests.RenamedAsserts
{
  [TestFixture]
  public class NUnitAssertRenamingTest
  {
    [Test]
    [TestCase ("Text.DoesNotMatch (...)", "Does.Not.Match (...)")]
    [TestCase ("Text.Matches (...)", "Does.Match (...)")]
    [TestCase ("Text.DoesNotEndWith (...)", "Does.Not.EndWith (...)")]
    [TestCase ("Text.EndsWith (...)", "Does.EndWith (...)")]
    [TestCase ("Text.DoesNotStartWith (...)", "Does.Not.StartWith (...)")]
    [TestCase ("Text.StartsWith (...)", "Does.StartWith (...)")]
    [TestCase ("Text.Contains (...)", "Does.Contain (...)")]
    [TestCase ("Text.DoesNotContain (...)", "Does.Not.Contain (...)")]
    [TestCase ("Is.InstanceOfType<T> (...)", "Is.InstanceOf<T> (...)")]
    [TestCase ("Is.StringStarting (...)", "Does.StartWith (...)")]
    [TestCase ("Is.StringContaining (...)", "Does.Contain (...)")]
    [TestCase ("Is.StringEnding (...)", "Does.EndWith (...)")]
    [TestCase ("Is.StringMatching (...)", "Does.Match (...)")]
    [TestCase ("Is.Not.InstanceOfType<T> (...)", "Is.Not.InstanceOf<T> (...)")]
    [TestCase ("Is.Not.StringStarting (...)", "Does.Not.StartsWith (...)")]
    [TestCase ("Is.Not.StringContaining (...)", "Does.Not.Contains (...)")]
    [TestCase ("Is.Not.StringEnding (...)", "Does.Not.EndsWith (...)")]
    [TestCase ("Is.Not.StringMatching (...)", "Does.Not.Matches (...)")]
    public void NUnitAssertRewriting_RewritesToNewAssertNames (string toRename, string expected)
    {
      var expression = SyntaxFactory.ParseExpression (toRename);
      var rewriter = new AssertRewriter (new RenamedAssertsMap(), new AssertRenamer());

      var actual = rewriter.Visit (expression);

      Assert.That (actual.ToFullString(), Is.EqualTo (expected));
    }

    [Test]
    [TestCase ("Text\n.DoesNotMatch (...)", "Does.Not\n.Match (...)")]
    [TestCase ("Text.\nMatches (...)", "Does.\nMatch (...)")]
    [TestCase ("Text. DoesNotEndWith (...)", "Does.Not. EndWith (...)")]
    [TestCase ("Text .EndsWith (...)", "Does .EndWith (...)")]
    [TestCase ("Text.\n        DoesNotStartWith (...)", "Does.Not.\n        StartWith (...)")]
    [TestCase ("Text\n        .StartsWith (...)", "Does\n        .StartWith (...)")]
    [TestCase ("Text\n        .StartsWith(...)", "Does\n        .StartWith(...)")]
    [TestCase ("Method.Call.Inner( Assert.That(\"string\", Is.StringStarting (...)))", "Method.Call.Inner( Assert.That(\"string\", Does.StartWith (...)))")]
    public void NUnitAssertRewriting_PreservesWhiteSpace (string toRename, string expected)
    {
      var expression = SyntaxFactory.ParseExpression (toRename);
      var rewriter = new AssertRewriter (new RenamedAssertsMap(), new AssertRenamer());

      var actual = rewriter.Visit (expression);

      Assert.That (actual.ToFullString(), Is.EqualTo (expected));
    }

    [Test]
    [TestCase ("")]
    [TestCase ("Text")]
    [TestCase ("Text.Other (...)")]
    [TestCase ("Is.Other (...)")]
    public void NUnitAssertRewriting_DoesNotRewriteNonChangedAsserts (string toRename)
    {
      var expression = SyntaxFactory.ParseExpression (toRename);
      var rewriter = new AssertRewriter (new RenamedAssertsMap(), new AssertRenamer());

      var actual = rewriter.Visit (expression);

      Assert.That (actual, Is.EqualTo (expression));
    }
  }
}