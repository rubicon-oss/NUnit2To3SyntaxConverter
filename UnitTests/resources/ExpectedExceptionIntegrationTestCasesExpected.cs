using System;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.ExpectedException;

namespace UnitTests
{
  public class ExpectedExceptionIntegrationTests
  {
    
    public void SimpleBaseCase ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.Exception);
    }
    
    public void WithCustomExceptionType ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.InstanceOf<DivideByZeroException>());
    }
    
    public void WithWellKnownExceptionType ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.ArgumentNullException);
    }

    public void WithCustomExpectedMessage ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.Exception
              .With.Message.EqualTo ("test message"));
    }

    public void WithCustomExpectedMessageAndMatchTypeRegex ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.Exception
              .With.Message.Matches ("test message regex"));
    }

    public void WithAllCustomFieldsAndLongMessage ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.InstanceOf<DivideByZeroException>()
              .With.Message.Contains ("test message reallllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllly long"));
    }
    
    public void WithMultiLineString ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.InstanceOf<DivideByZeroException>()
              .With.Message.EqualTo ("multi"
                  + "line"
                  + "string"));
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