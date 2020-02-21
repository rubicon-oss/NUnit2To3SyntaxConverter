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
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit2To3SyntaxConverter.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NUnit2To3SyntaxConverter.Extensions.SyntaxFactoryUtils;

namespace NUnit2To3SyntaxConverter.ExpectedException
{
  public class ExpectedExceptionModel : IExpectedExceptionModel
  {
    private readonly AttributeData _attributeData;

    public ExpressionSyntax ExceptionType { get; }
    public ExpressionSyntax? UserMessage { get; }
    public ExpressionSyntax? MatchType { get; }
    public ExpressionSyntax? ExpectedMessage { get; }

    public ExpectedExceptionModel (
        AttributeData attributeData,
        ExpressionSyntax exceptionType,
        ExpressionSyntax? userMessage,
        ExpressionSyntax? expectedMessage,
        ExpressionSyntax? matchType)
    {
      _attributeData = attributeData;
      ExceptionType = exceptionType;
      UserMessage = userMessage;
      MatchType = matchType;
      ExpectedMessage = expectedMessage;
    }

    public async Task<AttributeSyntax> GetAttributeSyntax ()
    {
      return await _attributeData.ApplicationSyntaxReference.GetSyntaxAsync() as AttributeSyntax
             ?? throw new InvalidOperationException ($"member {_attributeData} is not a AttributeSyntax");
    }

    public ExpressionSyntax AsConstraintExpression (string baseIndent)
    {
      var exception = (ExceptionType as TypeOfExpressionSyntax)
                      ?.Type.ToString()
                      ?? nameof(Exception);

      var throwsWithInstanceOfExpressionType = ExceptionConstraintSyntaxFactory.CreateThrowsExceptionConstrainSyntax (exception)
          .WithLeadingTrivia (Whitespace (baseIndent));

      return ExpectedMessage == null
          ? throwsWithInstanceOfExpressionType
          : CreateWithMessage (throwsWithInstanceOfExpressionType, baseIndent);
    }

    public static ExpectedExceptionModel CreateFromAttributeData (AttributeData attribute)
    {
      var attributeSyntax = attribute.ApplicationSyntaxReference.GetSyntax() as AttributeSyntax
                            ?? throw new ArgumentException ($"{nameof(attribute)} does not support a syntax tree");

      var attributeArguments = attributeSyntax.ArgumentList?.Arguments;

      return attributeArguments == null
          ? CreateNoArgExpectedExceptionModel (attribute)
          : CreateWithArgsExpectedExceptionModel (attribute, attributeArguments.Value);
    }

    private static ExpectedExceptionModel CreateWithArgsExpectedExceptionModel (
        AttributeData attribute,
        SeparatedSyntaxList<AttributeArgumentSyntax> attributeArguments)
    {
      var builder = new ExpectedExceptionModelBuilder();
      foreach (var attributeArgument in attributeArguments)
      {
        var value = attributeArgument.Expression
                    ?? throw new InvalidOperationException ($"Attribute argument {attributeArgument} does not have a value");

        var namedArgumentName = attributeArgument.NameColon?.Name
                                ?? attributeArgument.NameEquals?.Name;

        builder = namedArgumentName?.ToString() switch
        {
            "UserMessage" => builder.WithUserMessage (value),
            "ExpectedException" => builder.WithExceptionType (value),
            "ExpectedMessage" => builder.WithExpectedMessage (value),
            "MatchType" => builder.WithMatchType (value),
            "ExpectedExceptionName" => builder.WithExceptionName (value),
            null => builder.WithExceptionTypeOrName (value),
            _ => builder
        };
      }

      return builder.Build (attribute);
    }

    private static ExpectedExceptionModel CreateNoArgExpectedExceptionModel (AttributeData attribute)
    {
      var builder = new ExpectedExceptionModelBuilder();
      return builder.WithExceptionType (TypeOfExpression (IdentifierName (nameof(Exception))))
          .Build (attribute);
    }

    private ExpressionSyntax CreateWithMessage (ExpressionSyntax expression, string baseIndent)
    {
      var indent = new string (Formatting.IndentationCharacter, Formatting.IndentationSize);
      var withMessage = MemberAccess (
          expression.WithTrailingTrivia (Whitespace (Formatting.NewLine + baseIndent + indent + indent)),
          "With",
          "Message");

      var matchTypeFunctionName = MatchType?.ToFullString() switch
      {
          "MessageMatch.Contains" => "Contains",
          "MessageMatch.StartsWith" => "StartsWith",
          "MessageMatch.Regex" => "Matches",
          "MessageMatch.Exact" => "EqualTo",
          null => "EqualTo",
          _ => "UNKNOWN_MATCH_TYPE"
      };

      var withMessageMatchType = MemberAccess (withMessage, matchTypeFunctionName);

      var withMessageMatchTypeInvocation = SimpleInvocation (
          withMessageMatchType,
          new[]
          {
              ExpectedMessage.WithLeadingTrivia (Whitespace (""))!
                  .WithIndentation (indent: baseIndent + indent + indent + indent + indent)
          });

      return withMessageMatchTypeInvocation;
    }
  }
}