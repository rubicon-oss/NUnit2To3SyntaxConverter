using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace NUnit2To3SyntaxConverter.SetUpFixtureLifecycle
{
  public class SetUpFixtureDocumentConverter : IDocumentConverter
  {
    public async Task<SyntaxNode> Convert (Document document)
    {
      var syntax = await document.GetSyntaxRootAsync()
                   ?? throw new ArgumentException ("Document does not support a syntax tree");
      var semantic = await document.GetSemanticModelAsync()
                     ?? throw new ArgumentException ("Document does not support a semantic model");

      return new SetUpFixtureLifeCycleRewriter (semantic, new SetUpFixtureSetUpAttributeRenamer(), new SetUpFixtureTearDownAttributeRenamer())
          .Visit (syntax);
    }
  }
}