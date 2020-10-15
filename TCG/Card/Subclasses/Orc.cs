using System;
using System.Collections.Generic;
using System.Text;
using TCG.Card;

namespace TCG.Card.Subclasses
{
    public class Orc : Card
    {
        public Orc() : base()
        {
            health = 120;
            damage = 80;

        }
        public new void attackCard()
        {
            Console.WriteLine("The orc attacks!");
        }
    }
}
