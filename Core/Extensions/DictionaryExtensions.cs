using System.Collections;
using System.Collections.Generic;

namespace NUnit2To3SyntaxConverter.Extensions
{
  public static class DictionaryExtensions
  {
    public static TValue GetValueOrDefault<TKey, TValue> (this IReadOnlyDictionary<TKey, TValue> _this, TKey key, TValue defaultValue = default)
    {
      return _this.TryGetValue (key, out var value)
          ? value
          : defaultValue;
    }
  }
}