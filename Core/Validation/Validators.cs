using System;
using System.Collections.Generic;
using System.Linq;

namespace NUnit2To3SyntaxConverter.Validation
{
  public static class Validators
  {
    public static List<IValidationError> ValidateMultiple<TInput> (TInput input, params IValidator<TInput>[] validators)
    {
      var result = new List<IValidationError>();
      foreach (var validator in validators)
      {
        var error = validator.Validate (input);
        if(error != null)
          result.Add(error);
      }

      return result;
    }
  }
}