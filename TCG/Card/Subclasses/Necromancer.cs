using System;
using System.Collections.Generic;
using System.Text;
using TCG.Card;

namespace TCG.Card.Subclasses
{
    public class Necromancer : Card
    {
        public Necromancer() : base()
        {
            cardType = CardTypes.Monster;
            cardElement = CardElements.Darkness;
            health = 50;
            damage = 100;
            soulpower = 100;
        }
        /*
        public void AttackCard()
        {
            Console.WriteLine("The Necromancer attacks!");
        }
        */

    }
}
