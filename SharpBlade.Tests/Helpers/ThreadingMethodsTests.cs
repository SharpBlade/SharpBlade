using System.Threading;

using NUnit.Framework;

using SharpBlade.Helpers;

namespace SharpBlade.Tests.Helpers
{
    [TestFixture]
    public class ThreadingMethodsTests
    {
        [Test]
        public void ShouldSetNameWhenNameEmpty()
        {
            const string Expected = "NewName";
            string actual = null;
            var thread = new Thread(
                () =>
                {
                    ThreadingMethods.SetCurrentThreadName(Expected);
                    actual = Thread.CurrentThread.Name;
                });
            thread.Start();
            thread.Join();
            Assert.AreEqual(Expected, actual);
        }

        [Test]
        public void ShouldAbortSettingNameWhenNameExists()
        {
            const string Attempt = "NewName";
            const string Expected = "OldName";
            string actual = null;
            var thread = new Thread(
                () =>
                {
                    ThreadingMethods.SetCurrentThreadName(Expected);
                    ThreadingMethods.SetCurrentThreadName(Attempt);
                    actual = Thread.CurrentThread.Name;
                });
            thread.Start();
            thread.Join();
            Assert.AreEqual(Expected, actual);
        }
    }
}
