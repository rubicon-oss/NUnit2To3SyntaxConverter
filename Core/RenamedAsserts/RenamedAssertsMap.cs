#region copyright

// 
// Copyright (c) rubicon IT GmbH
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

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
  public class RenamedAssertsMap
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
             },
             {
                 "Is.Not", new Dictionary<string, string>
                           {
                               { "InstanceOfType", "Is.Not.InstanceOf" },
                               { "StringStarting", "Does.Not.StartWith" },
                               { "StringEnding", "Does.Not.EndWith" },
                               { "StringContaining", "Does.Not.Contain" },
                               { "StringMatching", "Does.Not.Match" },
                           }
             },
         };

    public (ExpressionSyntax, SimpleNameSyntax)? TryGetSyntax (ExpressionSyntax expression, SimpleNameSyntax access)
    {
      if (NewNameMap.TryGetValue (expression.WithoutTrivia().ToFullString(), out var methodMap))
      {
        if (methodMap.TryGetValue (access.Identifier.WithoutTrivia().ToString(), out var newName))
        {
          return BuildNewSyntax (newName, access);
        }
      }

      return null;
    }

    private (ExpressionSyntax, SimpleNameSyntax) BuildNewSyntax (string newName,  SimpleNameSyntax access)
    {
      var nameParts = newName.Split ('.');

      Trace.Assert (nameParts.Length > 1);

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