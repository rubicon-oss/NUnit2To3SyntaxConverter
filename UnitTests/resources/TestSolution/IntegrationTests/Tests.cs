using System;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [ExpectedException (typeof (DivideByZeroException))]
        public void Test1 ()
        {
            var zero = 0;
            var i = 1 / zero;
        }
        
        [Test]
        [ExpectedException (typeof (Exception), ExpectedMessage = "")]
        public void Test2 ()
        {
            throw new Exception ("");
        }
        
        [Test]
        [ExpectedException (typeof (DivideByZeroException), ExpectedMessage = ".*", MatchType = MessageMatch.Regex)]
        public void Test3 ()
        {
            var zero = 0;
            var i = 1 / zero;
        }
        
        [Test]
        [ExpectedException (typeof (Exception), ExpectedMessage = "abc", MatchType = MessageMatch.StartsWith)]
        public void Test4 ()
        {
            throw new Exception ("abcdef");
        }
    }
}