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
            CardType = (CardTypes)2;
            CardElement = (CardElements)rnd.Next(1, 7);
            Health = 50;
            Damage = 100;
            Name = "Necromancer";
            Soulpower = 100;
        }
        /*
        public void AttackCard()
        {
            Console.WriteLine("The Necromancer attacks!");
        }
        */

    }
}
