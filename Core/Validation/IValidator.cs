using System;

namespace NUnit2To3SyntaxConverter.Validation
{
  public interface IValidator<in TInput>
  {
    IValidationError? Validate (TInput input);
  }
}