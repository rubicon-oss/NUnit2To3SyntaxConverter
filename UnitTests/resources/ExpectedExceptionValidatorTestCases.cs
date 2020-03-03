using System;
using NUnit.Framework;

namespace UnitTests
{
  public class ExpectedExceptionValidatorTestCases
  {
    public void HasAssertInLastPosition ()
    {
      Console.WriteLine ();
      if (true)
      {
        Console.WriteLine ();
      }
      
      Assert.That(true, Is.True);
    }
    
    public void HasOnlyAssert ()
    {
      Assert.That(true, Is.True);
    }
    
    public void HasEmptyBody ()
    {
     
    }
    
    public void HasIfInLastPosition ()
    {
      if (true)
      {
        Console.WriteLine ();
      }
    }
    
    public void HasTryInLastPosition ()
    {
      try
      {
        throw new Exception();
      }
      catch (Exception ex)
      {
        Console.WriteLine (ex);
        throw;
      }
    }
    
    public void HasLoopInLastPosition ()
    {
      for (int i = 0; i < 10; i++)
      {
        Console.WriteLine ();
      }
    }

    public void ShouldValidateWithoutErrors ()
    {
      Assert.That(true, Is.True);
      if ("true" == true.ToString())
      {
        Console.WriteLine ();
      }
      try
      {
        for (int i = 0; i < 10; i++)
        {
          Console.WriteLine ("Loop");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine (ex);
        throw;
      }
      
      throw new Exception();
    }

  }
}