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
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace NUnit2To3SyntaxConverter.UnitTests
{
    public class CompiledSourceFileProvider
    {
        public (IMethodSymbol, MethodDeclarationSyntax) LoadMethod (string file, string methodName)
        {
            var src = File.ReadAllText (TestContext.CurrentContext.TestDirectory + "/" + file);


            var tree = CSharpSyntaxTree.ParseText (src);
            var root = (CompilationUnitSyntax) tree.GetRoot();
            var compilation = CSharpCompilation.Create ("TestCompilation")
                    .AddReferences (
                            MetadataReference.CreateFromFile (
                                    typeof (object).Assembly.Location))
                    .AddReferences (MetadataReference.CreateFromFile (TestContext.CurrentContext.TestDirectory + @"\resources\nunit.framework.dll"))
                    .AddSyntaxTrees (tree);

            var method = root.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Single (syntax => syntax.Identifier.ToString() == methodName);
            var model = compilation.GetSemanticModel (tree);
            var methodSymbol = model.GetDeclaredSymbol (method);
            return (methodSymbol, method);
        }
    }
}