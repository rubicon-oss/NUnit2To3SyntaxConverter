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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.TestFixtureLifecycle;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnit2To3SyntaxConverter.UnitTests.TestFixtureLifecycleAttributes
{
  public class RewriteTestFixtureLifecycleAttributes
  {
    private AttributeListSyntax ParseAttributeList (string attributeList)
    {
      var member = attributeList + "void m(){}";
      var memberSyntax = ParseMemberDeclaration (member);
      return memberSyntax.AttributeLists.First();
    }

    [Test]
    [TestCase ("[TestFixtureSetUp]", "[OneTimeSetUp]")]
    [TestCase ("[TestFixtureSetUp()]", "[OneTimeSetUp()]")]
    [TestCase ("[TestFixtureSetUp ()]", "[OneTimeSetUp ()]")]
    [TestCase ("[TestFixtureSetUp () ]", "[OneTimeSetUp () ]")]
    [TestCase ("[ TestFixtureSetUp () ]", "[ OneTimeSetUp () ]")]
    public void TestSingleAttributeSetUpRewrites (string attribute, string expected)
    {
      var attributeList = ParseAttributeList (attribute);
      var rewriter = new TestFixtureLifecycleAttributeRewriter (
          new TestFixtureSetupAttributeRenamer(),
          new TestFixtureTeardownAttributeRenamer());

      var result = rewriter.Visit (attributeList);

      Assert.That (result, Is.EquivalentTo (ParseAttributeList (expected)));
    }

    [Test]
    [TestCase ("[TestFixtureTearDown]", "[OneTimeTearDown]")]
    [TestCase ("[TestFixtureTearDown()]", "[OneTimeTearDown()]")]
    [TestCase ("[TestFixtureTearDown ()]", "[OneTimeTearDown ()]")]
    [TestCase ("[TestFixtureTearDown () ]", "[OneTimeTearDown () ]")]
    [TestCase ("[ TestFixtureTearDown () ]", "[ OneTimeTearDown () ]")]
    public void TestSingleAttributeTearDownRewrites (string attribute, string expected)
    {
      var attributeList = ParseAttributeList (attribute);
      var rewriter = new TestFixtureLifecycleAttributeRewriter (
          new TestFixtureSetupAttributeRenamer(),
          new TestFixtureTeardownAttributeRenamer());

      var result = rewriter.Visit (attributeList);

      Assert.That (result, Is.EquivalentTo (ParseAttributeList (expected)));
    }

    [Test]
    [TestCase ("[TestFixtureTearDown][Test] void m(){}", "[OneTimeTearDown][Test] void m(){}")]
    [TestCase ("[TestFixtureTearDown()]\n[Test] void m(){}", "[OneTimeTearDown()]\n[Test] void m(){}")]
    [TestCase ("[Test][TestFixtureSetUp ()] void m(){}", "[Test][OneTimeSetUp ()] void m(){}")]
    [TestCase ("[Test]\n[TestFixtureTearDown () ] void m(){}", "[Test]\n[OneTimeTearDown () ] void m(){}")]
    public void TestMultipleAttributeTearDownRewrites (string attribute, string expected)
    {
      var attributeList = ParseMemberDeclaration (attribute);
      var rewriter = new TestFixtureLifecycleAttributeRewriter (
          new TestFixtureSetupAttributeRenamer(),
          new TestFixtureTeardownAttributeRenamer());

      var result = rewriter.Visit (attributeList);

      Assert.That (result, Is.EquivalentTo (ParseMemberDeclaration (expected)));
    }
  }
}