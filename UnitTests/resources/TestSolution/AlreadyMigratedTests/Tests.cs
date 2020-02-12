using System;
using NUnit.Framework;

namespace AlreadyMigratedTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1 ()
        {
            Assert.True (true);

            Assert.Throws (typeof (Exception), () => { throw new Exception();});
            Assert.Throws (typeof (Exception), () => { throw new Exception();});
            Assert.Throws (typeof (Exception), () => { throw new Exception();});
        }

        [Test]
        public void Test2 ()
        {
            Assert.That(() => { throw new Exception("abc");}, Throws.InstanceOf<Exception>().With.Message.EqualTo("abc"));
            Assert.Throws (
                    typeof (Exception),
                    () =>
                    {
                        Assert.True (true);
                        throw new Exception();
                    });
        }
    }
}