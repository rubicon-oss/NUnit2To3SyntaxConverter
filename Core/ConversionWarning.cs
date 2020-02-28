using System;
using Microsoft.CodeAnalysis;

namespace NUnit2To3SyntaxConverter
{
  public class ConversionWarning : Exception
  {
    public Location Location { get; }
    public string MethodName { get; }

    public ConversionWarning (Location location, string methodName, string message)
        : base (message)
    {
      Location = location;
      MethodName = methodName;
    }
  }
}