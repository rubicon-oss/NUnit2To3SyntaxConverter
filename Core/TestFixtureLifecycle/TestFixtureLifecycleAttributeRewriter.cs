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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnit2To3SyntaxConverter.TestFixtureLifecycle
{
  /// <summary>
  /// Roslyn SyntaxRewriter which replaces all TestFixtureSetUp and -TearDown attributes with the new OneTimeSetUp and OneTimeTearDown attributes
  /// </summary>
  public class TestFixtureLifecycleAttributeRewriter : CSharpSyntaxRewriter
  {
    private readonly TestFixtureSetupAttributeRenamer _setupRewriter;
    private readonly TestFixtureTearDownAttributeRenamer _teardownRewriter;

    public TestFixtureLifecycleAttributeRewriter (
        TestFixtureSetupAttributeRenamer setupRewriter,
        TestFixtureTearDownAttributeRenamer teardownAttributeRewriter)
    {
      _setupRewriter = setupRewriter;
      _teardownRewriter = teardownAttributeRewriter;
    }

    public override SyntaxNode VisitAttribute (AttributeSyntax node)
    {
      var withRenamedSetUps = _setupRewriter.Transform (node);
      return _teardownRewriter.Transform (withRenamedSetUps);
    }
  }
}