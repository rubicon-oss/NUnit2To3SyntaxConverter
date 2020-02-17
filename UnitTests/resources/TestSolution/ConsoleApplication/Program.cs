using System;

namespace ConsoleApplication
{
  internal class Program
  {
    [ExpectedException]
    public static void Main (string[] args)
    {
    }

    class ExpectedException : Attribute
    {
    }
  }
}