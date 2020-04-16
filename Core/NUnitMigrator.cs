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
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Serilog;

namespace NUnit2To3SyntaxConverter
{
  public class NUnitMigration
  {
    private readonly MigrationOptions _options;

    public NUnitMigration (MigrationOptions options)
    {
      _options = options;
      Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Warning()
          .WriteTo.Console()
          .CreateLogger();
    }

    public async Task Migrate (Solution solution)
    {
      var tasks = GetDocuments (solution)
          .Select (async document => await ConvertDocument (document, _options.Converters))
          .Select (async documentResult => WriteBack (await documentResult));

      await Task.WhenAll (tasks);
    }

    private IEnumerable<Document> GetDocuments (Solution solution)
    {
      return solution.Projects
          .Where (_options.ProjectFilter)
          .SelectMany (project => project.Documents)
          .Where (_options.SourceFileFilter);
    }

    private async Task<(Document, Document)> ConvertDocument (Document document, IEnumerable<IDocumentConverter> converters)
    {
      var newDoc = document;

      foreach (var converter in converters)
        newDoc = newDoc.WithSyntaxRoot (await converter.Convert (newDoc));

      return (document, document.WithSyntaxRoot ((await newDoc.GetSyntaxRootAsync())!));
    }

    private async Task WriteBack ((Document, Document) documentResult)
    {
      var (originalDocument, newDocument) = documentResult;

      var originalRootNode = await originalDocument.GetSyntaxRootAsync();
      var newRootNode = await newDocument.GetSyntaxRootAsync();
      if (originalRootNode == newRootNode) return;

      try
      {
        using var fileStream = new FileStream (newDocument.FilePath!, FileMode.Truncate);
        using var writer = new StreamWriter (fileStream, _options.Encoding);

        newRootNode!.WriteTo (writer);
      }
      catch (IOException ex)
      {
        throw new InvalidOperationException ($"Unable to write source file '{newDocument.FilePath}'.", ex);
      }
    }
  }
}