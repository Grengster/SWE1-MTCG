using NUnit.Framework;
using TCG;
using System.IO;
using System;
using TCG.Card.Subclasses;
using TCG.Card;


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
            Orc orc_ = new Orc();

            Assert.NotNull(orc_);

            //calc.multVal()
        }
    }
}