using System;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.ExpectedException;

namespace UnitTests
{
  public class ExpectedExceptionIntegrationTests
  {
    
    [ExpectedException]
    public void SimpleBaseCase ()
    {
      Console.WriteLine();
    }

    [ExpectedException (typeof (DivideByZeroException)]
    public void WithCustomExceptionType ()
    {
      Console.WriteLine();
    }
    
    [ExpectedException (typeof (ArgumentException)]
    public void WithWellKnownExceptionType ()
    {
      Console.WriteLine();
    }

    [ExpectedException (ExpectedMessage = "test message")]
    public void WithCustomExpectedMessage ()
    {
      Console.WriteLine();
    }

    [ExpectedException (ExpectedMessage = "test message regex", MatchType = MessageMatch.Regex)]
    public void WithCustomExpectedMessageAndMatchTypeRegex ()
    {
      Console.WriteLine();
    }

    [ExpectedException (
        typeof (DivideByZeroException),
        ExpectedMessage = "test message reallllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllly long",
        MatchType = MessageMatch.Contains)]
    public void WithAllCustomFieldsAndLongMessage ()
    {
      Console.WriteLine();
    }

    [ExpectedException (ExpectedException = typeof (DivideByZeroException), 
        ExpectedMessage = 
            "multi"
            + "line"
            + "string")]
    public void WithMultiLineString ()
    {
      Console.WriteLine();
    }
    
    [ExpectedException (ExpectedMessage = "test message regex", MatchType = MessageMatch.Regex)]
    public void WithNonExpressionStatementInLastPosition ()
    {
      if (true)
      {
        Assert.That (true, Is.EqualTo (true));
      }
      else
      {
        throw Exception ("test message regex");
      }
    }
    
    [ExpectedException]
    public void WontConvertAssertionStatement ()
    {
      Console.WriteLine();
      if (true)
      {
      }
      else
      {
      }
      Assert.That(1, Is.EqualTo(1));
    }
    
    [ExpectedException]
    public void WontConvertEmptyMethod ()
    {
    }

  }
}