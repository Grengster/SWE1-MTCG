using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TCG.Card;

namespace TCG.Card.Subclasses
{
    public class Orc : Card
    {
        public Orc() : base()
        {
            cardType = CardTypes.Monster;
            cardElement = CardElements.Normal;
            health = 120;
            damage = 80;

        }
        /*
        public new void AttackCard()
        {
            Console.WriteLine("The orc attacks!");
        }
        */
    }
}
