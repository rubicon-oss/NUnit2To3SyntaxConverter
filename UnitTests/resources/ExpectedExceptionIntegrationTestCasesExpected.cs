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
using NUnit.Framework;

namespace UnitTests
{
  public class ExpectedExceptionIntegrationTests
  {

    [Test]
    public void SimpleBaseCase ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.Exception);
    }

    [Test]
    public void WithCustomExceptionType ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.InstanceOf<DivideByZeroException>());
    }

    [Test]
    public void WithWellKnownExceptionType ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.ArgumentException);
    }

    [Test]
    public void WithCustomExpectedMessage ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.Exception
              .With.Message.EqualTo ("test message"));
    }

    [Test]
    public void WithCustomExpectedMessageAndMatchTypeRegex ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.Exception
              .With.Message.Matches ("test message regex"));
    }

    [Test]
    public void WithAllCustomFieldsAndLongMessage ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.InstanceOf<DivideByZeroException>()
              .With.Message.Contains (
                  "test message reallllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllly long"));
    }

    [Test]
    public void WithMultiLineString ()
    {
      Assert.That (
          () => Console.WriteLine(),
          Throws.InstanceOf<DivideByZeroException>()
              .With.Message.EqualTo (
                  "multi"
                  + "line"
                  + "string"));
    }

    [Test]
    [ExpectedException (ExpectedMessage = "test message regex", MatchType = MessageMatch.Regex)]
    public void WontConvertWithNonExpressionStatementInLastPosition ()
    {
      if (true)
      {
        Assert.That (true, Is.EqualTo (true));
      }
      else
      {
        throw Exception ("test message regex");
      }
    }

    [Test]
    [ExpectedException]
    public void WontConvertAssertionStatement ()
    {
      Console.WriteLine();
      if (true)
      {
      }
      else
      {
      }
      Assert.That(1, Is.EqualTo(1));
    }

    [Test]
    [ExpectedException]
    public void WontConvertEmptyMethod ()
    {
    }
  }
}