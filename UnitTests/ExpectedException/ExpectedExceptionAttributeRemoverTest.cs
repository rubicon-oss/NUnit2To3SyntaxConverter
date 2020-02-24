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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.ExpectedException;
using NUnit2To3SyntaxConverter.Unittests.Helpers;

namespace NUnit2To3SyntaxConverter.UnitTests.ExpectedException
{
  public class RemoveExpectedAttributeTest
  {
    private const string c_testCaseFile = "resources/RemoveExpectedAttributeTestCases.cs";
    
    [Test]
    [TestCase ("MultipleAttributesInAttributeListFirst")]
    [TestCase ("MultipleAttributesInAttributeListLast")]
    [TestCase ("MultipleAttributesInAttributeListFirst")]
    [TestCase ("MultipleAttributesInAttributeListLast")]
    public void ExpectedExceptionAttributeRemover_RemovesSingleAttribute (string methodName)
    {
      var attributeRemover = new ExpectedExceptionAttributeRemover();
      var methodSyntax = CompiledSourceFileProvider.LoadMethod (c_testCaseFile, methodName);
      var model = new Mock<IExpectedExceptionModel>();
      model.Setup (m => m.GetAttributeSyntax())
          .Returns (
              Task<SyntaxNode>.FromResult (
                  methodSyntax.AttributeLists
                      .SelectMany (list => list.Attributes).First (attr => attr.Name.ToString() == "ExpectedException")));


      var rewritten = attributeRemover.Transform (methodSyntax, model.Object);

      Assert.That (rewritten.AttributeLists.ToString(), Is.EqualTo ("[Test]"));
    }
  }
}