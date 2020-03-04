using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace NUnit2To3SyntaxConverter.TestFixtureLifecycle
{
  public class TestFixtureDocumentConverter : IDocumentConverter
  {
    private readonly CSharpSyntaxRewriter _attributeRewriter;

    public TestFixtureDocumentConverter ()
    {
      _attributeRewriter = new TestFixtureLifecycleAttributeRewriter (
          new TestFixtureSetupAttributeRenamer(),
          new TestFixtureTeardownAttributeRenamer()
          );
    }

    public async Task<SyntaxNode> Convert (Document document)
    { 
      var syntaxRoot = await document.GetSyntaxRootAsync()
          ?? throw new ArgumentException("Document does not support syntax root.");
      return _attributeRewriter.Visit (syntaxRoot);
    }
  }
}