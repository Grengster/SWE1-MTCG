using NUnit.Framework;
using TCG;
using System.IO;
using System;
using TCG.Card.Subclasses;
using TCG.Card;


namespace TCG_Test
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AttackAndTypeTest()
        {
            Orc orc_ = new Orc();
            Necromancer nec_ = new Necromancer();
            Assert.NotNull(orc_);
            int orcHealth = orc_.GetHealth();
            int necHealth = nec_.GetHealth();
            CardTypes CardType;
            CardType = (CardTypes)2;

            nec_.AttackCard(orc_);
            Assert.AreNotEqual(orc_.GetHealth(), orcHealth);

            Assert.AreEqual(orc_.GetType(), CardType);

        }

        [Test]
        public void NecromancerTest()
        {
            Necromancer nec_ = new Necromancer();
            Assert.NotNull(nec_);
            Assert.AreEqual(nec_.GetHealth(), 50);
        }
    }
}