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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnit2To3SyntaxConverter.Extensions
{
  public static class SyntaxNodeExtension
  {
    public static T WithExtraIndentation<T> (this T _this, string newLine = Formatting.NewLine, string indent = "  ")
        where T : SyntaxNode
    {
      var node = (T) new ExtraIndentationNodeVisitor (newLine, indent).Visit (_this);
      return node.WithLeadingTrivia (node.GetLeadingTrivia().Add (Whitespace (indent)));
    }

    public static T WithIndentation<T> (this T _this, string newLine = Formatting.NewLine, string indent = "  ")
        where T : SyntaxNode
    {
      var node = (T) new IndentationNodeVisitor (indent).Visit (_this);
      return node.WithLeadingTrivia (node.GetLeadingTrivia().Add (Whitespace (indent)));
    }

    private class ExtraIndentationNodeVisitor : CSharpSyntaxRewriter
    {
      private readonly string _extraIndentation;
      private readonly string _newLine;

      public ExtraIndentationNodeVisitor (string newLine, string extraIndentation)
      {
        _newLine = newLine;
        _extraIndentation = extraIndentation;
      }

      public override SyntaxNode DefaultVisit (SyntaxNode node)
      {
        var indentLeadingTrivia = node.WithLeadingTrivia (
            Whitespace (
                node.GetLeadingTrivia().ToFullString()
                    .Replace (_newLine, _newLine + _extraIndentation)));

        var indentTrailingTrivia = indentLeadingTrivia.WithTrailingTrivia (
            Whitespace (
                indentLeadingTrivia.GetTrailingTrivia().ToFullString()
                    .Replace (_newLine, _newLine + _extraIndentation)));

        return indentTrailingTrivia;
      }
    }

    private class IndentationNodeVisitor : CSharpSyntaxRewriter
    {
      private readonly string _indentation;

      public IndentationNodeVisitor (string indentation)
      {
        _indentation = indentation;
      }

      public override SyntaxTrivia VisitTrivia (SyntaxTrivia trivia)
      {
        if (trivia.IsKind (SyntaxKind.WhitespaceTrivia) && trivia.ToFullString().Length > 1)
          return Whitespace (_indentation);

        return trivia;
      }
    }
  }
}