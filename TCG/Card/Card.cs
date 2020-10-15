using System;
using System.Collections.Generic;
using System.Text;

namespace TCG.Card
{
    public class Card  // Base class (parent) 
    {
        public Card()
        {
            health = 100;
            damage = 100;

        }
        protected int health, damage, soulpower, lightpower;


        public void attackCard()
        {
            Console.WriteLine("The card attacks!");
        }


        public int getHealth()
        {
            return health;
        }

        public int getDamage()
        {
            return damage;
        }

    }
}
