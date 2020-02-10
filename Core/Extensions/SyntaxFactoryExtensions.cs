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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnit2To3SyntaxConverter.Extensions
{
    public static class SyntaxFactoryUtils
    {
        public static InvocationExpressionSyntax SimpleInvocation (ExpressionSyntax expression, IEnumerable<ExpressionSyntax> arguments)
        {
            return InvocationExpression (expression, ArgumentList( SeparatedList(arguments.Select (Argument))));
        }

        public static MemberAccessExpressionSyntax MemberAccess (ExpressionSyntax expression, params string[] accessor)
        {
            Debug.Assert(accessor.Length > 0);
            var access = MemberAccessExpression (SyntaxKind.SimpleMemberAccessExpression, expression, IdentifierName (accessor[0]));
            return accessor.Length == 1 
                    ? access 
                    : MemberAccess(access, accessor.Skip(1).ToArray());
        }
        
        public static MemberAccessExpressionSyntax MemberAccess (ExpressionSyntax expression, SimpleNameSyntax nameSyntax)
        {
            return MemberAccessExpression (SyntaxKind.SimpleMemberAccessExpression, expression, nameSyntax);
        }
        
    }
}