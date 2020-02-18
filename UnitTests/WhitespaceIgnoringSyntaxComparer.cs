using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NUnit2To3SyntaxConverter.UnitTests
{
  public class WhitespaceIgnoringSyntaxComparer : IEqualityComparer<SyntaxNode>
  {
    public bool Equals (SyntaxNode x, SyntaxNode y)
    {
      if (x == null && y == null) return true;
      return x?.IsEquivalentTo (y, false) ?? false;
    }

    public int GetHashCode (SyntaxNode obj)
    {
      return obj.WithoutTrivia().ToFullString().GetHashCode();
    }
  }
}