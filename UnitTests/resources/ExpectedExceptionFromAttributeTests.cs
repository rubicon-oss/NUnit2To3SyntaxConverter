using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests
{
    public class ExpectedExceptionFromAttributeTests
    {
        [ExpectedException]
        public void SimpleBaseCase ()
        {
        }

        [ExpectedException(typeof(DivideByZeroException)]
        public void WithCustomExceptionType ()
        {
        }

        [ExpectedException (ExpectedMessage = "test message")]
        public void WithCustomExpectedMessage ()
        {
        }
        
        [ExpectedException (ExpectedMessage = "test message regex", MatchType = MessageMatch.Regex)]
        public void WithCustomExpectedMessageAndMatchTypeRegex ()
        {
        }
        
        [ExpectedException (ExpectedMessage = "test message contains", MatchType = MessageMatch.Contains)]
        public void WithCustomExpectedMessageAndMatchTypeContains ()
        {
        }
        
        [ExpectedException (ExpectedMessage = "test message startsWith", MatchType = MessageMatch.StartsWith)]
        public void WithCustomExpectedMessageAndMatchTypeStartsWith ()
        {
        }
        
        [ExpectedException (ExpectedMessage = "test message exact", MatchType = MessageMatch.Exact)]
        public void WithCustomExpectedMessageAndMatchTypeExact ()
        {
        }
        
        [ExpectedException (UserMessage = "test user message")]
        public void WithCustomUserMessage ()
        {
        }
        
        [ExpectedException (typeof(DivideByZeroException), ExpectedMessage = "test message", UserMessage = "test user message", MatchType = MessageMatch.Contains)]
        public void WithAllCustomFields ()
        {
        }
        
        [ExpectedException (ExpectedException = typeof(DivideByZeroException))]
        public void WithExplicitlyNamedExceptionType ()
        {
        }
        
        [ExpectedException ("DivideByZeroException")]
        public void WithExceptionNameStringLiteral ()
        {
        }
        
        
        [ExpectedException (ExpectedExceptionName = "DivideByZeroException")]
        public void WithExplicitlyNamedExceptionNameStringLiteral ()
        {
        }
        
        [ExpectedException (typeof(DivideByZeroException), ExpectedExceptionName = "DivideByZeroException")]
        public void DoubleSpecifiedException ()
        {
        }

    }
}