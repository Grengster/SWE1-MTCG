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
            health = 50;
            damage = 100;
            soulpower = 100;
        }
        public new void attackCard()
        {
            Console.WriteLine("The Necromancer attacks!");
        }

    }
}
