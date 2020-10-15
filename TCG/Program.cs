using System;
using TCG.Card.Subclasses;
using TCG.Card;

namespace TCG.Card
{
    public class Program
    {
        
        static void Main(string[] args)
        {
            Necromancer necro = new Necromancer();
            Orc orc = new Orc();

            necro.attackCard();
            orc.attackCard();

            Console.WriteLine(necro.getHealth());
            Console.WriteLine(orc.getHealth());
        }
    }
}
