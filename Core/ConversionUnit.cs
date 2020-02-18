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
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace NUnit2To3SyntaxConverter
{
  public class ConversionUnit
  {
    private const string c_supportedLanguage = "C#";
    private readonly Document _document;
    private readonly SemanticModel _semanticModel;

    private readonly SyntaxNode _syntaxRoot;

    private ConversionUnit (Document document, SyntaxNode syntaxRoot, SemanticModel semanticModel)
    {
      _document = document;
      _syntaxRoot = syntaxRoot;
      _semanticModel = semanticModel;
    }

    public static async Task<ConversionUnit> CreateFromDocument (Document document)
    {
      if (!SupportsDocument (document)) return null;

      var syntaxTree = (await document.GetSyntaxTreeAsync())?.GetRoot();
      var semanticModel = await document.GetSemanticModelAsync();
      Debug.Assert (syntaxTree != null);
      Debug.Assert (semanticModel != null);


      return new ConversionUnit (document, syntaxTree, semanticModel);
    }

    private static bool SupportsDocument (Document document)
    {
      return document.Project.Language == c_supportedLanguage
             && document.SupportsSemanticModel
             && document.SupportsSyntaxTree;
    }
  }
}