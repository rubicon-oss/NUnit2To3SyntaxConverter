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

    [ExpectedException ("DivideByZeroException")]
    public void WithExceptionNameStringLiteral ()
    {
    }


    [ExpectedException (ExpectedExceptionName = "DivideByZeroException")]
    public void WithExplicitlyNamedExceptionNameStringLiteral ()
    {
    }

    [ExpectedException (typeof (DivideByZeroException), ExpectedExceptionName = "DivideByZeroException")]
    public void DoubleSpecifiedException ()
    {
    }
  }
}