using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace NUnit2To3SyntaxConverter.RenamedAsserts
{
  public class NUnitAssertRenamingDocumentConverter: IDocumentConverter
  {
    
    public async Task<SyntaxNode> Convert (Document document)
    {
      var syntaxRoot = await document.GetSyntaxRootAsync();
      return new NUnitAssertRenamer().Visit (syntaxRoot);
    }
  }
}