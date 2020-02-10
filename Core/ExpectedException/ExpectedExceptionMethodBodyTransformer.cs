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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NUnit2To3SyntaxConverter.Extensions.SyntaxFactoryUtils;

namespace NUnit2To3SyntaxConverter.ExpectedException
{
    public class ExpectedExceptionMethodBodyTransformer : ISyntaxTransformer<MethodDeclarationSyntax, ExpectedExceptionModel>
    {

        public MethodDeclarationSyntax Transform (MethodDeclarationSyntax node, ExpectedExceptionModel model)
        {
            var wrappingLambdaExpression =
                    ParenthesizedLambdaExpression (
                            ParameterList (SeparatedList<ParameterSyntax>()),
                            node.Body);

            var assertThat = MemberAccess (IdentifierName ("Assert"), "That");
            var assertThrows = SimpleInvocation (
                    assertThat,
                    new[] { wrappingLambdaExpression, model.AsConstraintExpression() });


            return node
                    .WithBody (Block (ExpressionStatement (assertThrows)));
        }
    }
}