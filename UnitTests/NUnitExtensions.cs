using System;
using Microsoft.CodeAnalysis;
using NUnit.Framework.Constraints;

namespace NUnit2To3SyntaxConverter.UnitTests
{
  public class Is : NUnit.Framework.Is
  {
    public static Constraint EquivalentTo (SyntaxNode expected)
    {
      return EqualTo (expected).Using (new WhitespaceIgnoringSyntaxComparer());
    }
  }
}