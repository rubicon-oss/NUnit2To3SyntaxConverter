using System;
using NUnit.Framework;

namespace UnitTests
{
  [TestFixture]
  public class Tests
  {
    [Test]
    [ExpectedException (typeof (DivideByZeroException))]
    public void Test1 ()
    {
      var zero = 0;
      var i = 1 / zero;
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "")]
    public void Test2 ()
    {
      throw new Exception ("");
    }

    [Test]
    [ExpectedException (typeof (DivideByZeroException), ExpectedMessage = ".*", MatchType = MessageMatch.Regex)]
    public void Test3 ()
    {
      var zero = 0;
      var i = 1 / zero;
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "abc", MatchType = MessageMatch.StartsWith)]
    public void Test4 ()
    {
      throw new Exception ("abcdef");
    }

    [Test]
    public void Test5 ()
    {
      Assert.That ("", Text.DoesNotMatch (""));
      Assert.That ("", Text.Matches (""));
      Assert.That ("", Text.DoesNotEndWith (""));
      Assert.That ("", Text.EndsWith (""));
      Assert.That ("", Text.DoesNotStartWith (""));
      Assert.That ("", Text.StartsWith (""));
      Assert.That ("", Text.Contains (""));
      Assert.That ("", Text.DoesNotContain (""));
      Assert.That ("", Text.All.Empty);
    }

    [Test]
    public void Test6 ()
    {
      Assert.That ("", Is.InstanceOfType<string>());
      Assert.That ("", Is.StringStarting (""));
      Assert.That ("", Is.StringContaining (""));
      Assert.That ("", Is.StringEnding (""));
      Assert.That ("", Is.StringMatching (""));
    }
  }
}