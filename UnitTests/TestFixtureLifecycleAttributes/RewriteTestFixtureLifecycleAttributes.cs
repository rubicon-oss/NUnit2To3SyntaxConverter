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