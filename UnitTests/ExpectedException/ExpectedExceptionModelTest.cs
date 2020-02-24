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
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using NUnit2To3SyntaxConverter.ExpectedException;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NUnit2To3SyntaxConverter.Extensions.SyntaxFactoryUtils;

namespace NUnit2To3SyntaxConverter.UnitTests.ExpectedException
{
  public class ExpectedExceptionModelTest
  {
    [Test]
    [TestCase ("[ExpectedException]")]
    public async Task CreateFromAttributeData_NoArgsAttribute (string attribute)
    {
      var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);

      var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (attributeData);

      Assert.That (expectedExceptionModel, Is.Not.Null);
      Assert.That (
          expectedExceptionModel.ExceptionType,
          Is.EquivalentTo (TypeOfExpression (IdentifierName ("Exception"))));
      Assert.That (
          await expectedExceptionModel.GetAttributeSyntax(),
          Is.SameAs (await attributeData.ApplicationSyntaxReference.GetSyntaxAsync()));
      Assert.That (expectedExceptionModel.ExpectedMessage, Is.Null);
    }


    [Test]
    [TestCase ("[ExpectedException(typeof(DivideByZeroException))]")]
    public void CreateFromAttributeData_UsesCustomExceptionType (string attribute)
    {
      var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);

      var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (attributeData);

      Assert.That (expectedExceptionModel, Is.Not.Null);
      Assert.That (
          expectedExceptionModel.ExceptionType,
          Is.EquivalentTo (TypeOfExpression (IdentifierName ("DivideByZeroException"))));
    }

    [Test]
    [TestCase ("[ExpectedException (UserMessage = \"test user message\")]")]
    public void CreateFromAttributeData_UsesUserMessage (string attribute)
    {
      var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);

      var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (attributeData);
      Assert.That (expectedExceptionModel, Is.Not.Null);
      Assert.That (
          expectedExceptionModel.UserMessage,
          Is.EquivalentTo (LiteralExpression (SyntaxKind.StringLiteralExpression, Literal ("test user message"))));
    }

    [Test]
    [TestCase ("[ExpectedException (ExpectedMessage = \"test message\")]", "test message", "Exact")]
    [TestCase ("[ExpectedException (ExpectedMessage = \"test message regex\", MatchType = MessageMatch.Regex)]", "test message regex", "Regex")]
    [TestCase ("[ExpectedException (ExpectedMessage = \"test message exact\", MatchType = MessageMatch.Exact)]", "test message exact", "Exact")]
    [TestCase (
        "[ExpectedException (ExpectedMessage = \"test message startsWith\", MatchType = MessageMatch.StartsWith)]",
        "test message startsWith",
        "StartsWith")]
    [TestCase (
        "[ExpectedException (ExpectedMessage = \"test message contains\", MatchType = MessageMatch.Contains)]",
        "test message contains",
        "Contains")]
    public void CreateFromAttributeData_UsesExpectedMessage (string attribute, string message, string matchType)
    {
      var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);

      var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (attributeData);

      Assert.That (expectedExceptionModel, Is.Not.Null);
      Assert.That (
          expectedExceptionModel.ExpectedMessage,
          Is.EquivalentTo (LiteralExpression (SyntaxKind.StringLiteralExpression, Literal (message))));
      Assert.That (
          expectedExceptionModel.MatchType,
          Is.EquivalentTo (MemberAccess (IdentifierName ("MessageMatch"), matchType)));
    }

    [Test]
    [TestCase (
        "[ExpectedException ("
        + "typeof(DivideByZeroException), "
        + "ExpectedMessage = \"test message\", "
        + "UserMessage = \"test user message\", "
        + "MatchType = MessageMatch.Contains)]")]
    public void CreateFromAttributeData_UsesAllCustomFields (string attribute)
    {
      var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);

      var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (attributeData);

      Assert.That (expectedExceptionModel, Is.Not.Null);
      Assert.That (
          expectedExceptionModel.ExceptionType,
          Is.EquivalentTo (TypeOfExpression (IdentifierName ("DivideByZeroException"))));
      Assert.That (
          expectedExceptionModel.ExpectedMessage,
          Is.EquivalentTo (LiteralExpression (SyntaxKind.StringLiteralExpression, Literal ("test message"))));
      Assert.That (
          expectedExceptionModel.MatchType,
          Is.EquivalentTo (MemberAccess (IdentifierName ("MessageMatch"), "Contains")));
      Assert.That (
          expectedExceptionModel.UserMessage,
          Is.EquivalentTo (LiteralExpression (SyntaxKind.StringLiteralExpression, Literal ("test user message"))));
    }

    [Test]
    [TestCase ("[ExpectedException (\"DivideByZeroException\")]")]
    [TestCase ("[ExpectedException (ExpectedExceptionName = \"DivideByZeroException\")]")]
    public void CreateFromAttributeData_ExceptionNameIsSetCorrectly (string attribute)
    {
        var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);

      var expectedExceptionModel = ExpectedExceptionModel.CreateFromAttributeData (attributeData);

      Assert.That (
          expectedExceptionModel.ExceptionType,
          Is.EquivalentTo (TypeOfExpression (IdentifierName ("DivideByZeroException"))));
    }

    [Test]
    [TestCase ("[ExpectedException (typeof(DivideByZeroException), ExpectedExceptionName = \"DivideByZeroException\")]")]
    public void CreateFromAttributeData_ThrowsWhenTypeAndNameIsSpecified (string attribute)
    {
        var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);

      Assert.That (
          () => { ExpectedExceptionModel.CreateFromAttributeData (attributeData); },
          Throws.Exception.With.InstanceOf<InvalidOperationException>());
    }
    
    [Test]
    [TestCase ("[ExpectedException]")]
    public void ConvertToConstraint_ConvertsForSimpleExceptionCase (string attribute)
    {
        var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);
        IExpectedExceptionModel expectedException 
                = new ExpectedExceptionModel(attributeData, IdentifierName("Exception"), null, null, null);

        var constraint = expectedException.AsConstraintExpression ("");
        
        Assert.That(constraint, Is.EquivalentTo(ParseExpression("Throws.Exception")));
    }
    
    [Test]
    [TestCase ("[ExpectedException(typeof(TestException))]")]
    public void ConvertToConstraint_ConvertsForCustomException (string attribute)
    {
        var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);
        IExpectedExceptionModel expectedException 
                = new ExpectedExceptionModel(attributeData, ParseExpression ("typeof(TestException)"), null, null, null);

        var constraint = expectedException.AsConstraintExpression ("");
        
        Assert.That(constraint, Is.EquivalentTo(ParseExpression("Throws.InstanceOf<TestException>()")));
    }
    
    [Test]
    [TestCase ("[ExpectedException(typeof(InvalidOperationException))]")]
    public void ConvertToConstraint_ConvertsForCustomWellKnownException (string attribute)
    {
        var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);
        IExpectedExceptionModel expectedException 
                = new ExpectedExceptionModel(attributeData, ParseExpression ("typeof(InvalidOperationException)"), null, null, null);

        var constraint = expectedException.AsConstraintExpression ("");
        
        Assert.That(constraint, Is.EquivalentTo(ParseExpression("Throws.InvalidOperationException")));
    }
    
    [Test]
    [TestCase ("[ExpectedException(ExpectedMessage = \"Test Message\")]")]
    public void ConvertToConstraint_ConvertsExpectedExceptionMessage (string attribute)
    {
        var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);
        IExpectedExceptionModel expectedException 
                = new ExpectedExceptionModel(attributeData, ParseExpression ("Exception"), null, ParseExpression("\"Test Message\""), null);

        var constraint = expectedException.AsConstraintExpression ("");
        
        Assert.That(constraint, Is.EquivalentTo(ParseExpression("Throws.Exception.With.Message.EqualTo(\"Test Message\")")));
    }
    
    [Test]
    [TestCase ("[ExpectedException(ExpectedMessage = \"Test Message\"), MatchType = MessageMatch.Regex]")]
    public void ConvertToConstraint_ConvertsExpectedExceptionMessageWithCustomMatcher (string attribute)
    {
        var (attributeData, _) = CompiledSourceFileProvider.CompileAttribute (attribute);
        IExpectedExceptionModel expectedException 
                = new ExpectedExceptionModel(attributeData, ParseExpression ("Exception"), null, ParseExpression("\"Test Message\""), ParseExpression("MessageMatch.Regex"));

        var constraint = expectedException.AsConstraintExpression ("");
        
        Assert.That(constraint, Is.EquivalentTo(ParseExpression("Throws.Exception.With.Message.Matches(\"Test Message\")")));
    }


  }
}