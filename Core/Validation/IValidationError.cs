using System;

namespace NUnit2To3SyntaxConverter.Validation
{
  public interface IValidationError
  {
    string FileName { get; }
    string MethodName { get; }
    string Category { get; }
    string Reason { get; }
  }
}