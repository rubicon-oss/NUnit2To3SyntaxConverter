using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
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
      var node = (T) new IndentationNodeVisitor (newLine, indent).Visit (_this);
      return node.WithLeadingTrivia (node.GetLeadingTrivia().Add (Whitespace (indent)));
    }

    private class ExtraIndentationNodeVisitor : CSharpSyntaxRewriter
    {
      private readonly string _newLine;
      private readonly string _extraIndentation;

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
      private readonly string _newLine;
      private readonly string _indentation;

      public IndentationNodeVisitor (string newLine, string indentation)
      {
        _newLine = newLine;
        _indentation = indentation;
      }

      public override SyntaxTrivia VisitTrivia (SyntaxTrivia trivia)
      {
        if (trivia.IsKind (SyntaxKind.WhitespaceTrivia) && trivia.ToFullString().Length > 1)
        {
          return Whitespace (_indentation);
        }

        return trivia;
      }
    }
  }
}