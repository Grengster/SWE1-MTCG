using System;
using System.Collections.Generic;
using System.Text;
using TCG.Card.Subclasses;

namespace TCG.Card
{
    public class Card  // Base class (parent) 
    {
        protected int health, damage, soulpower, lightpower;
        protected CardTypes cardType;
        protected CardElements cardElement;



        public Card()
        {
            health = 100;
            damage = 100;

        }
        public void AttackCard(Card other)
        {
            //getDamaged other.getDamaged(this.getDamage())
            other.GetDamaged(this.GetDamage());
        }


        public int GetHealth()
        {
            return health;
        }

        public int GetDamage()
        {
            return damage;
        }

        public void GetDamaged(int damage)
        {
            this.health -= damage;
        }

        public new CardTypes GetType()

        {
            return cardType;
        }

        public CardElements GetElement()
        {
            return cardElement;
        }

    }
}
