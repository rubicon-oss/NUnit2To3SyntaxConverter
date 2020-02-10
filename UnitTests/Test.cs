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
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace NUnit2To3SyntaxConverter.UnitTests
{
    public class Test
    {
        [Test]
        public void Test1 ()
        {
            Assert.True(true);
        }

        [Test]
        public void TestCompilation ()
        {
            var src = File.ReadAllText ("resources/ExpectedExceptionBaseCase.txt");
            
            var tree = CSharpSyntaxTree.ParseText (src);
            var root = (CompilationUnitSyntax) tree.GetRoot();
            var compilation = CSharpCompilation.Create("HelloWorld")
                    .AddReferences(
                            MetadataReference.CreateFromFile(
                                    typeof(object).Assembly.Location))
                    .AddReferences(MetadataReference.CreateFromFile(@"resources\nunit.framework.dll"))
                    .AddSyntaxTrees(tree);

            var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            var model = compilation.GetSemanticModel(tree);
            var symbolInfo = model.GetSymbolInfo (method);
            
            var myTypeSyntax = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            var myTypeInfo = model.GetDeclaredSymbol(myTypeSyntax);
            
            Assert.That(() => { throw new Exception(""); }, Throws.Exception.With.InstanceOf<Exception>().With.Message.Empty);
            
            
            Console.WriteLine (myTypeInfo.GetAttributes());

        }
    }
}