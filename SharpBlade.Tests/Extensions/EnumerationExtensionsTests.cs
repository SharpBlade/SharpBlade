using System;

using NUnit.Framework;

using SharpBlade.Extensions;

namespace SharpBlade.Tests.Extensions
{
    [TestFixture]
    public class EnumerationExtensionsTests
    {
        [Flags]
        private enum TestEnum
        {
            Foo = 0x1,
            Bar = 0x2,
            Baz = 0x4
        }

        [Flags]
        private enum TestLargeEnum : ulong
        {
            Foo = 0x2000000000000000,
            Bar = 0x4000000000000000,
            Baz = 0x8000000000000000
        }

        [Test]
        public void ShouldReturnTrueWhenEnumContainsValues()
        {
            const TestEnum Combined = TestEnum.Foo | TestEnum.Bar;
            Assert.True(Combined.Has(TestEnum.Foo));
            Assert.True(Combined.Has(TestEnum.Bar));
        }

        [Test]
        public void ShouldReturnTrueWhenLargeEnumContainsValue()
        {
            const TestLargeEnum Combined = TestLargeEnum.Bar | TestLargeEnum.Baz;
            Assert.True(Combined.Has(TestLargeEnum.Bar));
            Assert.True(Combined.Has(TestLargeEnum.Baz));
        }

        [Test]
        public void ShouldReturnFalseWhenEnumDoesntContainValue()
        {
            const TestEnum Combined = TestEnum.Foo | TestEnum.Bar;
            Assert.False(Combined.Has(TestEnum.Baz));
        }

        [Test]
        public void ShouldReturnFalseWhenLargeEnumDoesntContainValue()
        {
            const TestLargeEnum Combined = TestLargeEnum.Bar | TestLargeEnum.Baz;
            Assert.False(Combined.Has(TestLargeEnum.Foo));
        }

        [Test]
        public void ShouldReturnTrueWhenEnumMissingValue()
        {
            const TestEnum Combined = TestEnum.Foo | TestEnum.Bar;
            Assert.True(Combined.Missing(TestEnum.Baz));
        }

        [Test]
        public void ShouldReturnTrueWhenLargeEnumMissingValue()
        {
            const TestLargeEnum Combined = TestLargeEnum.Bar | TestLargeEnum.Baz;
            Assert.True(Combined.Missing(TestLargeEnum.Foo));
        }

        [Test]
        public void ShouldCombineEnumWithInclude()
        {
            const TestEnum Expected = TestEnum.Foo | TestEnum.Bar;
            var actual = TestEnum.Foo.Include(TestEnum.Bar);
            Assert.AreEqual(Expected, actual);
        }

        [Test]
        public void ShouldCombineLargeEnumWithInclude()
        {
            const TestLargeEnum Expected = TestLargeEnum.Bar | TestLargeEnum.Baz;
            var actual = TestLargeEnum.Bar.Include(TestLargeEnum.Baz);
            Assert.AreEqual(Expected, actual);
        }

        [Test]
        public void ShouldRemoveValueFromEnum()
        {
            const TestEnum Expected = TestEnum.Bar;
            var actual = TestEnum.Foo.Include(TestEnum.Bar);
            actual = actual.Remove(TestEnum.Foo);
            Assert.AreEqual(Expected, actual);
        }

        [Test]
        public void ShouldRemoveValueFromLargeEnum()
        {
            const TestLargeEnum Expected = TestLargeEnum.Baz;
            var actual = TestLargeEnum.Bar.Include(TestLargeEnum.Baz);
            actual = actual.Remove(TestLargeEnum.Bar);
            Assert.AreEqual(Expected, actual);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenHasCalledOnNull()
        {
            ((Enum)null).Has((object)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenMissingCalledOnNull()
        {
            ((Enum)null).Missing((object)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenIncludeCalledOnNull()
        {
            ((Enum)null).Include((object)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenRemoveCalledOnNull()
        {
            ((Enum)null).Remove((object)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowWhenHasCalledWithDifferentTypes()
        {
            TestEnum.Foo.Has(TestLargeEnum.Foo);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowWhenMissingCalledWithDifferentTypes()
        {
            TestEnum.Foo.Missing(TestLargeEnum.Foo);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowWhenIncludeCalledWithDifferentTypes()
        {
            TestEnum.Foo.Include(TestLargeEnum.Foo);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowWhenRemoveCalledWithDifferentTypes()
        {
            TestEnum.Foo.Remove(TestLargeEnum.Foo);
        }
    }
}
