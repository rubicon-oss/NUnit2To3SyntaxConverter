using System.Linq;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnit2To3SyntaxConverter.Extensions
{
  public static class SyntaxNodeExtension
  {

    public static T WithExtraIndentation<T> (this T _this, string newLine = Formatting.NewLine, string indent = "  ")
      where T: SyntaxNode
    {
      var node = (T) new IndentationNodeVisitor(newLine, indent).Visit(_this);
      return node.WithLeadingTrivia (node.GetLeadingTrivia().Add (Whitespace (indent)));
    }

    private class IndentationNodeVisitor : CSharpSyntaxRewriter
    {
      private readonly string _newLine;
      private readonly string _extraIndentation;

      public IndentationNodeVisitor (string newLine, string extraIndentation)
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
  }
}