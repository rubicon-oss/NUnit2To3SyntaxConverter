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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NUnit2To3SyntaxConverter.Extensions.SyntaxFactoryUtils;

namespace NUnit2To3SyntaxConverter.ExpectedException
{
  internal class ExpectedExceptionModelBuilder
  {
    private ExpressionSyntax? ExceptionType { get; set; }
    private ExpressionSyntax? ExceptionName { get; set; }
    private ExpressionSyntax? UserMessage { get; set; }
    private ExpressionSyntax? ExpectedMessage { get; set; }
    private ExpressionSyntax? MatchType { get; set; }

    public ExpectedExceptionModelBuilder WithExceptionType (ExpressionSyntax exceptionType)
    {
      ExceptionType = exceptionType;
      return this;
    }

    public ExpectedExceptionModelBuilder WithExceptionName (ExpressionSyntax exceptionName)
    {
      ExceptionName = exceptionName;
      return this;
    }

    public ExpectedExceptionModelBuilder WithUserMessage (ExpressionSyntax userMessage)
    {
      UserMessage = userMessage;
      return this;
    }

    public ExpectedExceptionModelBuilder WithExpectedMessage (ExpressionSyntax expectedMessage)
    {
      ExpectedMessage = expectedMessage;
      return this;
    }

    public ExpectedExceptionModelBuilder WithMatchType (ExpressionSyntax matchType)
    {
      MatchType = matchType;
      return this;
    }

    public ExpectedExceptionModelBuilder WithExceptionTypeOrName (ExpressionSyntax value)
    {
      if (value is LiteralExpressionSyntax literal)
        ExceptionName = literal;
      else if (value is TypeOfExpressionSyntax typeOf)
        ExceptionType = typeOf;

      return this;
    }

    public ExpectedExceptionModel Build (AttributeData attributeData)
    {
      if (MatchType == null && ExpectedMessage != null)
        MatchType = MemberAccess (IdentifierName ("MessageMatch"), "Exact");

      if (ExceptionName != null && ExceptionType != null)
        throw new InvalidOperationException (
            "Unable to convert ExpectedException attribute, "
            + $"both a name: {ExceptionName.ToString()} and a type: {ExceptionType.ToString()} are specified");

      if (ExceptionName is LiteralExpressionSyntax nameLiteral && ExceptionType == null)
      {
        var value = nameLiteral.Token.ValueText;
        ExceptionType = TypeOfExpression (IdentifierName (value));
      }

      if (attributeData == null)
        throw new InvalidOperationException ($"Required field {nameof(attributeData)} is null.");

      ExceptionType ??= IdentifierName ("Exception");

      return new ExpectedExceptionModel (
          attributeData,
          ExceptionType,
          UserMessage,
          ExpectedMessage,
          MatchType);
    }
  }
}