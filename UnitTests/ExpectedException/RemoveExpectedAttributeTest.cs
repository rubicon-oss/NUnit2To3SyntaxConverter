using System;
using System.Buffers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.ExpectedException;

namespace NUnit2To3SyntaxConverter.UnitTests.ExpectedException
{
  public class RemoveExpectedAttributeTest
  {
    [Test]
    [TestCase("resources/RewriteMethodBodyTests.cs", "MultipleAttributesInAttributeListFirst")]
    [TestCase("resources/RewriteMethodBodyTests.cs", "MultipleAttributesInAttributeListLast")]
    [TestCase("resources/RewriteMethodBodyTests.cs", "MultipleAttributesInAttributeListFirst")]
    [TestCase("resources/RewriteMethodBodyTests.cs", "MultipleAttributesInAttributeListLast")]
    public void RemovesSingleAttribute (string fileName, string methodName)
    {
      var (methodSymbol, methodSyntax) = CompiledSourceFileProvider.LoadMethod (fileName, methodName);
      var model = new Mock<IExpectedExceptionModel>();
      
      model.Setup (m => m.GetAttributeSyntax())
          .Returns (Task<SyntaxNode>.FromResult(methodSyntax.AttributeLists
              .SelectMany(list => list.Attributes).First(attr => attr.Name.ToString() == "ExpectedException")));
      
      
      var rewritten = new ExpectedExceptionAttributeRemover()
          .Transform(methodSyntax, model.Object);
      
      Assert.That(rewritten.AttributeLists.ToString(), Is.EqualTo("[Test]"));
    
    }
  }
}