using System;
using System.Collections.Generic;
using System.Text;
using TCG.Card.Subclasses;

namespace TCG.Card
{
    public abstract class Card  // Base class (parent) 
    {
        protected int Health, Damage, Soulpower, Lightpower;
        protected string Name;
        protected CardTypes CardType;
        protected CardElements CardElement;
        protected Random rnd = new Random();


        public Card()
        {
            Health = 100;
            Damage = 100;

        }

        public void GetStats()
        {
            Console.WriteLine("-------------CARD STATS-------------");
            Console.WriteLine("Card: " + this.Name);
            Console.WriteLine("Health: " + this.Health);
            Console.WriteLine("Type: " + this.GetType());
            Console.WriteLine("Element: " + this.GetElement());
            Console.WriteLine("\n");
        }

        public void AttackCard(Card other)
        {
            /*check for both class types -> check for effectiveness and choose damage output scaled by effectiveness 
             * super effective = dead
             * effective = x1.25
             * normal = x1
             * low effective = x0.85
             * not effective = x0
            */
            other.GetDamaged(this.Damage);
        }

        public int GetHealth()
        {
            return this.Health;
        }

        public void GetDamaged(int damage)
        {
            this.Health -= damage;
        }

        public new CardTypes GetType()
        {
            return CardType;
        }

        public CardElements GetElement()
        {
            return CardElement;
        }

    }
}
