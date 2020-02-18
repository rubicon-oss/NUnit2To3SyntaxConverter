using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit2To3SyntaxConverter.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnit2To3SyntaxConverter.RenamedAsserts
{
  public class RenamedAsserts
  {
    private IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>
        _newNameMap => new Dictionary<string, IReadOnlyDictionary<string, string>>
                       {
                           {
                               "Text", new Dictionary<string, string>
                                       {
                                           { "DoesNotMatch", "Does.Not.Match" },
                                           { "Matches", "Does.Match" },
                                           { "DoesNotEndWith", "Does.Not.EndWith" },
                                           { "EndsWith", "Does.EndWith" },
                                           { "StartsWith", "Does.StartWith" },
                                           { "DoesNotStartWith", "Does.Not.StartWith" },
                                           { "Contains", "Does.Contain" },
                                           { "DoesNotContain", "Does.Not.Contain" },
                                           { "All", "Has.All" }
                                       }
                           },
                           {
                               "Is", new Dictionary<string, string>
                                     {
                                         { "InstanceOfType", "Is.InstanceOf" },
                                         { "StringStarting", "Does.StartWith" },
                                         { "StringEnding", "Does.EndWith" },
                                         { "StringContaining", "Does.Contain" },
                                         { "StringMatching", "Does.Match" },
                                     }
                           }
                       };

    public (ExpressionSyntax, SimpleNameSyntax)? TryGetSyntax (ExpressionSyntax expression, SimpleNameSyntax access)
    {
      var newName = _newNameMap
          .GetValueOrDefault (expression.WithoutTrivia().ToFullString())
          ?.GetValueOrDefault (access.Identifier.WithoutTrivia().ToString());
          
      if (newName == null) return null;

      var nameParts = newName.Split ('.');
      
      if (nameParts.Length == 2)
      {
        return (IdentifierName (nameParts[0]), access.WithIdentifier (Identifier (nameParts[1])));
      }
      
      return (SyntaxFactoryUtils.MemberAccess (IdentifierName (nameParts[0]), nameParts[1]), access.WithIdentifier (Identifier (nameParts[2])));
    }
  }
}