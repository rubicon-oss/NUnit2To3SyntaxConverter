using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests
{
    public class RewriteMethodBodyTests
    {
        [Test]
        [ExpectedException]
        public void SingleAttributeInAttributeListLast ()
        {
        }
        
        [ExpectedException]
        [Test]
        public void SingleAttributeInAttributeListFirst ()
        {
        }
        
        [Test, ExpectedException]
        public void MultipleAttributesInAttributeListLast ()
        {
        }
        
        [ExpectedException, Test]
        public void MultipleAttributesInAttributeListFirst ()
        {
        }
        
    }
}