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
            CardType = (CardTypes)2;
            CardElement = (CardElements)rnd.Next(1,7);
            Health = 120;
            Damage = 80;
            Name = "Orc";
        }
        /*
        public new void AttackCard()
        {
            Console.WriteLine("The orc attacks!");
        }
        */
    }
}
