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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NUnit2To3SyntaxConverter.Extensions.SyntaxFactoryUtils;

namespace NUnit2To3SyntaxConverter.ExpectedException
{
   
    public class ExpectedExceptionModel
    {
        public AttributeData AttributeData { get; }
        public ExpressionSyntax? ExceptionType { get; }
        public ExpressionSyntax? ExceptionName { get; }
        public ExpressionSyntax? UserMessage { get; }
        public ExpressionSyntax? MatchType { get; }
        public ExpressionSyntax? ExpectedMessage { get; }
        public ExpressionSyntax? Handler { get; }

        public ExpectedExceptionModel (
                AttributeData attributeData,
                ExpressionSyntax? exceptionType,
                ExpressionSyntax? exceptionName,
                ExpressionSyntax? userMessage,
                ExpressionSyntax? expectedMessage,
                ExpressionSyntax? matchType,
                ExpressionSyntax? handler)
        {
            AttributeData = attributeData;
            ExceptionType = exceptionType;
            ExceptionName = exceptionName;
            UserMessage = userMessage;
            MatchType = matchType;
            ExpectedMessage = expectedMessage;
            Handler = handler;
        }

        public ExpressionSyntax AsConstraintExpression ()
        {
            var exception = (ExceptionType as TypeOfExpressionSyntax)?.Type;
            exception ??= GenericName (nameof(Exception));

            var throwsWith = MemberAccess (IdentifierName ("Throws"), "Exception", "With");

            var throwsWithInstanceOfExpressionType = MemberAccess (
                    throwsWith,
                    GenericName (Identifier ("InstanceOf"), TypeArgumentList (SeparatedList (new[] { exception })))
                    );

            var throwsWithInstanceOfExpressionTypeInvocation = SimpleInvocation (throwsWithInstanceOfExpressionType, new ExpressionSyntax[0]);

            return ExpectedMessage == null
                    ? throwsWithInstanceOfExpressionTypeInvocation
                    : WithMessage (throwsWithInstanceOfExpressionTypeInvocation);
        }

        private ExpressionSyntax WithMessage (ExpressionSyntax expression)
        {
            Debug.Assert (ExpectedMessage != null);
            var withMessage = MemberAccess (expression, "With", "Message");

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
                    new[] { ExpectedMessage });

            return withMessageMatchTypeInvocation;
        }
    }
}