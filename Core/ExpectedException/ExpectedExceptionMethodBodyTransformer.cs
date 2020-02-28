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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit2To3SyntaxConverter.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NUnit2To3SyntaxConverter.Extensions.SyntaxFactoryUtils;

namespace NUnit2To3SyntaxConverter.ExpectedException
{
  public class ExpectedExceptionMethodBodyTransformer : ISyntaxTransformer<MethodDeclarationSyntax, IExpectedExceptionModel>
  {
    public MethodDeclarationSyntax Transform (MethodDeclarationSyntax node, IExpectedExceptionModel model)
    {
      if (node.Body == null || node.Body.Statements.Count == 0)
      {
          throw new ConversionWarning (node.GetLocation(), node.Identifier.ToFullString(), "Unable to convert method with empty body.");
      }
      
      var indentation = Whitespace (new string (' ', 2));
      var baseIndentation = node.Body.GetLeadingTrivia();
      var bodyIndentation = baseIndentation.Add (indentation);
      var assertThatArgsIndentation = bodyIndentation.Add (indentation).Add (indentation);

      var bodyStatement = node.Body.Statements.Last();
      if (bodyStatement.WithoutTrivia().ToFullString().StartsWith ("Assert."))
      {
          throw new ConversionWarning(node.GetLocation(), node.Identifier.ToFullString(), "Unable to convert method with assertion in last statement.");
      }

      var lambdaExpression = CreateLambdaExpression (bodyStatement);

      var assertThat = MemberAccess (IdentifierName ("Assert"), "That")
          .WithLeadingTrivia (bodyIndentation);


      var assertThrowsArgumentList = SeparatedList<ArgumentSyntax> (
          NodeOrTokenList (
              Argument (lambdaExpression.WithLeadingTrivia (assertThatArgsIndentation)),
              Token (SyntaxKind.CommaToken).WithTrailingTrivia (Whitespace (Formatting.NewLine)),
              Argument (model.AsConstraintExpression (assertThatArgsIndentation.ToFullString()))
              ));

      var assertThatThrows = InvocationExpression (
          assertThat.WithTrailingTrivia (Whitespace (" ")),
          ArgumentList (
              Token (SyntaxKind.OpenParenToken).WithTrailingTrivia (Whitespace (Formatting.NewLine)),
              assertThrowsArgumentList,
              Token (SyntaxKind.CloseParenToken)));

      return node.WithBody (
          node.Body.WithStatements (
              node.Body.Statements.Replace (
                  node.Body.Statements.Last(),
                  ExpressionStatement (assertThatThrows).WithTrailingTrivia (Whitespace (Formatting.NewLine)))));
    }

    private static LambdaExpressionSyntax CreateLambdaExpression (StatementSyntax bodyStatement)
    {
      return bodyStatement switch
      {
          ExpressionStatementSyntax exprStmt => CreateExpressionLambda (exprStmt),
          _ => CreateStatementLambda (bodyStatement)
      };
    }

    private static LambdaExpressionSyntax CreateStatementLambda (StatementSyntax bodyStatement)
    {
      var lambdaBody = Block (bodyStatement.WithLeadingTrivia (Whitespace (" ")).WithTrailingTrivia (Whitespace (" ")))
          .WithExtraIndentation (new string (Formatting.IndentationCharacter, Formatting.IndentationSize * 2));

      var lambdaExpression = ParenthesizedLambdaExpression (
          ParameterList().WithTrailingTrivia (Whitespace (" ")),
          lambdaBody.WithLeadingTrivia (Whitespace (" ")));

      return lambdaExpression;
    }

    private static LambdaExpressionSyntax CreateExpressionLambda (ExpressionStatementSyntax bodyExpressionStmt)
    {
      var lambdaBody = bodyExpressionStmt.Expression;
      var lambdaExpression = ParenthesizedLambdaExpression (
          ParameterList().WithTrailingTrivia (Whitespace (" ")),
          null,
          lambdaBody.WithLeadingTrivia (Whitespace (" ")));
      return lambdaExpression;
    }
  }
}