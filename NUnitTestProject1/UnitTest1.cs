using NUnit.Framework;
using TCG;
using System.IO;
using System;

namespace NUnitTestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Class1 calc = new Class1();
            int x = 12;
            int y = 10;

            int expectedVal = calc.multVal(x, y);
            int fakeVal = 120;

            Assert.AreEqual(expectedVal, fakeVal);

            //calc.multVal()
        }
    }
}