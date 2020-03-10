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
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace NUnit2To3SyntaxConverter.Unittests.Helpers
{
  public static class CompiledSourceFileProvider
  {
    public static AttributeData CompileAttribute (string attribute)
    {
      var (methodSymbol, _) = CompileAttributeLists (attribute);
      return methodSymbol.GetAttributes().Single();
    }

    public static (IMethodSymbol, MethodDeclarationSyntax) CompileAttributeLists (string attributeList)
    {
      var compilationTemplate =
          $"namespace TestCompilations {{ using System; using Nunit.Framework; public class TestClass {{ {attributeList} public void Test(){{}} }}";
      var (model, root) = LoadSemanticModel (compilationTemplate);

      var methodSyntax = root.DescendantNodes()
          .OfType<MethodDeclarationSyntax>()
          .First();

      var methodSymbol = model.GetDeclaredSymbol (methodSyntax);
      return (methodSymbol, methodSyntax);
    }

    public static MethodDeclarationSyntax LoadMethod (string file, string methodName)
    {
      var (_, root) = LoadCompilationFromFile (file);
      var method = root.DescendantNodes()
          .OfType<MethodDeclarationSyntax>()
          .Single (syntax => syntax.Identifier.ToString() == methodName);
      return method;
    }

    public static ClassDeclarationSyntax LoadClass (string file, string className)
    {
      var (_, root) = LoadCompilationFromFile (file);
      var classSyntax = root.DescendantNodes()
          .OfType<ClassDeclarationSyntax>()
          .Single (syntax => syntax.Identifier.ToString() == className);
      return classSyntax;
    }

    public static (SemanticModel, SyntaxNode) LoadCompilationFromFile (string file)
      => LoadSemanticModel (File.ReadAllText (TestContext.CurrentContext.TestDirectory + "/" + file));

    public static (SemanticModel, SyntaxNode) LoadSemanticModel (string srcText)
    {
      var tree = CSharpSyntaxTree.ParseText (srcText);
      var root = (CompilationUnitSyntax) tree.GetRoot();
      var compilation = CSharpCompilation.Create ("TestCompilation")
          .AddReferences (
              MetadataReference.CreateFromFile (
                  typeof (object).Assembly.Location))
          .AddReferences (MetadataReference.CreateFromFile (TestContext.CurrentContext.TestDirectory + @"\resources\nunit.framework.v2.6.2.dll"))
          .AddSyntaxTrees (tree);

      var model = compilation.GetSemanticModel (tree);
      return (model, root);
    }
  }
}