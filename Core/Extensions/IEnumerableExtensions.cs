using System.Collections.Generic;
using System.Linq;

namespace NUnit2To3SyntaxConverter.Extensions
{
  public static class IEnumerableExtensions
  {
    public static IEnumerable<T> WhereNotNull<T> (this IEnumerable<T?> source)
        where T : class
    {
      return source.Where (t => t != null)!;
    }
  }
}