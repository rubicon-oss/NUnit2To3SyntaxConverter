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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NUnit2To3SyntaxConverter.ExpectedException;
using NUnit2To3SyntaxConverter.Extensions;

namespace NUnit2To3SyntaxConverter
{
    public class NUnitMigration
    {
        public NUnitMigration ()
        {
        }

        public async Task Migrate (Solution solution)
        {
            var tasks = solution.Projects
                    .SelectMany (project => project.Documents)
                    .Select(document => ConvertDocument(document, new ExpectedExceptionDocumentConverter()))
                    .Select(async document => WriteBack(await document));

            await Task.WhenAll (tasks);
        }

        public async Task<Document> ConvertDocument (Document document, IDocumentConverter converter)
        {
                var syntaxTree = await converter.Convert (document);
                return document.WithSyntaxRoot (syntaxTree);
        }

        public async Task WriteBack (Document document)
        {
            var rootNode = await document.GetSyntaxRootAsync();
            try
            {
                using (var fileStream = new FileStream (document.FilePath!, FileMode.Truncate))
                {
                    using (var writer = new StreamWriter (fileStream))
                    {
                        rootNode!.WriteTo (writer);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException (string.Format ("Unable to write source file '{0}'.", document.FilePath), ex);
            }
        }
    }
}