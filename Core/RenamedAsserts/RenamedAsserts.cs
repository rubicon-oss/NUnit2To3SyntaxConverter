using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit2To3SyntaxConverter.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnit2To3SyntaxConverter.RenamedAsserts
{
  public class RenamedAsserts
  {
    private IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> NewNameMap
      => new Dictionary<string, IReadOnlyDictionary<string, string>>
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
      if (NewNameMap.TryGetValue (expression.WithoutTrivia().ToFullString(), out var methodMap))
      {
        if (methodMap.TryGetValue (access.Identifier.WithoutTrivia().ToString(), out var newName))
        {
          return BuildNewSyntax (newName, expression, access);
        }
      }

      return null;
    }

    private (ExpressionSyntax, SimpleNameSyntax) BuildNewSyntax (string newName, ExpressionSyntax expression, SimpleNameSyntax access)
    {
      var nameParts = newName.Split ('.');
      
      Trace.Assert(nameParts.Length > 1);
      
      if (nameParts.Length == 2)
      {
        return (IdentifierName (nameParts[0]), access.WithIdentifier (Identifier (nameParts[1])));
      }

      var first = nameParts.First();

      // copy all but the first and last elements
      var middle = new string[nameParts.Length - 2];
      Array.Copy (nameParts, 1, middle, 0, middle.Length);

      var last = nameParts.Last();

      return (SyntaxFactoryUtils.MemberAccess (IdentifierName (first), middle), access.WithIdentifier (Identifier (last)));
    }
  }
}