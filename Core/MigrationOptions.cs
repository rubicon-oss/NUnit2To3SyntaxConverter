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
using System.Text;
using Microsoft.CodeAnalysis;

namespace NUnit2To3SyntaxConverter
{
  public class MigrationOptions
  {
    public static readonly MigrationOptions DefaultOptions = new MigrationOptions();

    public Func<Project, bool> ProjectFilter { get; } = _ => true;

    public Func<Document, bool> SourceFileFilter { get; } = _ => true;

    public Encoding Encoding { get; } = Encoding.Default;

    public IEnumerable<IDocumentConverter> Converters { get; } = new List<IDocumentConverter>();

    private MigrationOptions ()
    {
    }

    private MigrationOptions (
        Func<Project, bool> projectFilter,
        Func<Document, bool> documentFilter,
        Encoding encoding,
        IEnumerable<IDocumentConverter> converters)
    {
      ProjectFilter = projectFilter;
      SourceFileFilter = documentFilter;
      Encoding = encoding;
      Converters = converters;
    }

    public MigrationOptions WithConverters (IEnumerable<IDocumentConverter> converters)
    {
      return new MigrationOptions (ProjectFilter, SourceFileFilter, Encoding, converters);
    }
  }
}