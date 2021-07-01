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
        public void Test()
        {
            var testInstance = new TestClass { NestedInstance = new TestClass() };
            var sut = new ReflectionScaffolding(typeof(TestClass), testInstance);

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
