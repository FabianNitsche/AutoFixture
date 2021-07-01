using AutoFixture.Idioms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AutoFixture.IdiomsUnitTest
{
    public class ReflectionScaffoldingTests
    {
        [Fact]
        public void Problem()
        {
            var emptyListWithHistory = new List<object>() { new object() };
            emptyListWithHistory.Clear();

            var emptyList = new List<object>();

            var fieldsForEmptyListWithHistory = new ReflectionScaffolding(typeof(List<object>), emptyListWithHistory);
            var fieldsForEmptyList = new ReflectionScaffolding(typeof(List<object>), emptyList);

            Assert.Equal(fieldsForEmptyListWithHistory.IntegerFields[0].Get(), fieldsForEmptyList.IntegerFields[0].Get());
            Assert.Equal(fieldsForEmptyListWithHistory.IntegerFields[1].Get(), fieldsForEmptyList.IntegerFields[1].Get());
        }

        [Fact]
        public void CloneDemo()
        {
            var testInstance = new TestClass();
            var sut = new ReflectionScaffolding(typeof(TestClass), testInstance);
            sut.IntegerFields[0].Set(1);
            sut.IntegerFields[1].Set(2);
            sut.StringFields[0].Set("Test");

            var clonedSut = sut.Clone();
            Assert.False(ReferenceEquals(sut.Instance, clonedSut.Instance));
            Assert.Equal(sut.IntegerFields[0].Get(), clonedSut.IntegerFields[0].Get());
            Assert.Equal(sut.IntegerFields[1].Get(), clonedSut.IntegerFields[1].Get());
            Assert.Equal(sut.StringFields[0].Get(), clonedSut.StringFields[0].Get());
        }

        [Fact]
        public void DataAccessDemo()
        {
            var testInstance = new TestClass { NestedInstance = new TestClass() };
            testInstance.NestedInstance.NestedInstance = testInstance; // cyclic reference

            var sut = new ReflectionScaffolding(typeof(TestClass), testInstance);

            Assert.Equal(4, sut.IntegerFields.Length);
            Assert.Equal(2, sut.StringFields.Length);

            sut.IntegerFields[0].Set(1);
            sut.IntegerFields[1].Set(2);
            sut.IntegerFields[2].Set(3);
            sut.IntegerFields[3].Set(4);
            sut.StringFields[0].Set("Test");
            sut.StringFields[1].Set("Test2");

            Assert.Equal(1, testInstance.IntTest);
            Assert.Equal(2, testInstance.IntAutoProperty);
            Assert.Equal("Test", testInstance.StringTest);
            Assert.Equal(3, testInstance.NestedInstance.IntTest);
            Assert.Equal(4, testInstance.NestedInstance.IntAutoProperty);
            Assert.Equal("Test2", testInstance.NestedInstance.StringTest);
        }

        public class TestClass
        {
            private readonly int intTest;

            private string stringTest;

            public int IntTest => intTest;

            public string StringTest => stringTest;

            public int IntAutoProperty { get; }

            public TestClass NestedInstance { get; set; }
        }
    }
}
