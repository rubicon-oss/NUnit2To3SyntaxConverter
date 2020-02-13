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
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.ExpectedException;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NUnit2To3SyntaxConverter.Extensions.SyntaxFactoryUtils;

namespace NUnit2To3SyntaxConverter.UnitTests.ExpectedException
{
    public class ParseExpectedExceptionModelTest
    {
        [Test]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "SimpleBaseCase")]
        public async Task NoArgsAttribute (string fileName, string methodName)
        {
            var (methodSymbol, methodSyntax) = new CompiledSourceFileProvider().LoadMethod (fileName, methodName);
            var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (methodSymbol.GetAttributes()[0]);

            Assert.That (expectedExceptionModel, Is.Not.Null);

            Assert.That (
                    expectedExceptionModel.ExceptionType,
                    Is.EqualTo (TypeOfExpression (IdentifierName ("Exception")))
                            .Using<ExpressionSyntax> ((t1, t2) => t1.IsEquivalentTo (t2, false)));

            Assert.That (await expectedExceptionModel.GetAttributeSyntax(), Is.SameAs (await methodSymbol.GetAttributes()[0].ApplicationSyntaxReference.GetSyntaxAsync() ));
            Assert.That (expectedExceptionModel.ExpectedMessage, Is.Null);
        }


        [Test]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "WithCustomExceptionType")]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "WithExplicitlyNamedExceptionType")]
        public void UsesCustomExceptionType (string fileName, string methodName)
        {
            var (methodSymbol, methodSyntax) = new CompiledSourceFileProvider().LoadMethod (fileName, methodName);
            var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (methodSymbol.GetAttributes()[0]);
            Assert.That (expectedExceptionModel, Is.Not.Null);
            Assert.That (
                    expectedExceptionModel.ExceptionType,
                    Is.EqualTo (TypeOfExpression (IdentifierName ("DivideByZeroException")))
                            .Using<ExpressionSyntax> ((t1, t2) => t1.IsEquivalentTo (t2, false)));
        }

        [Test]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "WithCustomUserMessage")]
        public void UsesUserMessage (string fileName, string methodName)
        {
            var (methodSymbol, methodSyntax) = new CompiledSourceFileProvider().LoadMethod (fileName, methodName);
            var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (methodSymbol.GetAttributes()[0]);
            Assert.That (expectedExceptionModel, Is.Not.Null);
            Assert.That (
                    expectedExceptionModel.UserMessage,
                    Is.EqualTo (LiteralExpression (SyntaxKind.StringLiteralExpression, Literal ("test user message")))
                            .Using<ExpressionSyntax> ((t1, t2) => t1.IsEquivalentTo (t2, false)));
        }

        [Test]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "WithCustomExpectedMessage", "test message", "Exact")]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "WithCustomExpectedMessageAndMatchTypeRegex", "test message regex", "Regex")]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "WithCustomExpectedMessageAndMatchTypeExact", "test message exact", "Exact")]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "WithCustomExpectedMessageAndMatchTypeStartsWith", "test message startsWith", "StartsWith")]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "WithCustomExpectedMessageAndMatchTypeContains", "test message contains", "Contains")]
        public void UsesExpectedMessage (string fileName, string methodName, string message, string matchType)
        {
            var (methodSymbol, methodSyntax) = new CompiledSourceFileProvider().LoadMethod (fileName, methodName);
            var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (methodSymbol.GetAttributes()[0]);
            Assert.That (expectedExceptionModel, Is.Not.Null);
            Assert.That (
                    expectedExceptionModel.ExpectedMessage,
                    Is.EqualTo (LiteralExpression (SyntaxKind.StringLiteralExpression, Literal (message)))
                            .Using<ExpressionSyntax> ((t1, t2) => t1.IsEquivalentTo (t2, false)));
            Assert.That (
                    expectedExceptionModel.MatchType,
                    Is.EqualTo (MemberAccess (IdentifierName ("MessageMatch"), matchType))
                            .Using<ExpressionSyntax> ((t1, t2) => t1.IsEquivalentTo (t2, false)));
        }

        [Test]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "WithAllCustomFields")]
        public void UsesAllCustomFields (string fileName, string methodName)
        {
            var (methodSymbol, methodSyntax) = new CompiledSourceFileProvider().LoadMethod (fileName, methodName);
            var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (methodSymbol.GetAttributes()[0]);
            Assert.That (expectedExceptionModel, Is.Not.Null);
            Assert.That (
                    expectedExceptionModel.ExceptionType,
                    Is.EqualTo (TypeOfExpression (IdentifierName ("DivideByZeroException")))
                            .Using<ExpressionSyntax> ((t1, t2) => t1.IsEquivalentTo (t2, false)));


            Assert.That (
                    expectedExceptionModel.ExpectedMessage,
                    Is.EqualTo (LiteralExpression (SyntaxKind.StringLiteralExpression, Literal ("test message")))
                            .Using<ExpressionSyntax> ((t1, t2) => t1.IsEquivalentTo (t2, false)));

            Assert.That (
                    expectedExceptionModel.MatchType,
                    Is.EqualTo (MemberAccess (IdentifierName ("MessageMatch"), "Contains"))
                            .Using<ExpressionSyntax> ((t1, t2) => t1.IsEquivalentTo (t2, false)));

            Assert.That (
                    expectedExceptionModel.UserMessage,
                    Is.EqualTo (LiteralExpression (SyntaxKind.StringLiteralExpression, Literal ("test user message")))
                            .Using<ExpressionSyntax> ((t1, t2) => t1.IsEquivalentTo (t2, false)));
        }

        [Test]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "WithExceptionNameStringLiteral")]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "WithExplicitlyNamedExceptionNameStringLiteral")]
        public void ExceptionNameIsSetCorrectly (string fileName, string methodName)
        {
            var (methodSymbol, methodSyntax) = new CompiledSourceFileProvider().LoadMethod (fileName, methodName);
            var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (methodSymbol.GetAttributes()[0]);

            Assert.That (
                    expectedExceptionModel.ExceptionType,
                    Is.EqualTo (TypeOfExpression (IdentifierName ("DivideByZeroException")))
                            .Using<ExpressionSyntax> ((t1, t2) => t1.IsEquivalentTo (t2, false)));
        }

        [Test]
        [TestCase ("resources/ExpectedExceptionFromAttributeTests.cs", "DoubleSpecifiedException")]
        public void ThrowsWhenTypeAndNameIsSpecified (string fileName, string methodName)
        {
            var (methodSymbol, methodSyntax) = new CompiledSourceFileProvider().LoadMethod (fileName, methodName);
            Assert.That (
                    () => { ExpectedExceptionModel.CreateFromAttributeData (methodSymbol.GetAttributes()[0]); },
                    Throws.Exception.With.InstanceOf<ArgumentException>());
        }
    }
}