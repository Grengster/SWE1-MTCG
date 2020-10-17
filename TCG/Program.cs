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

            necro.GetHealth();
            orc.GetHealth();

            necro.AttackCard(orc);
            orc.AttackCard(necro);

            necro.GetHealth();
            orc.GetHealth();

        }
    }
}
