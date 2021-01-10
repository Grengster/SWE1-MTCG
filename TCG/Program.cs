using System;
using TCG.Card.Subclasses;
using TCG.Card;

namespace TCG.Card
{
    public class Program
    {
        
        static void Main()
        {
            Necromancer necro = new Necromancer();
            Orc orc = new Orc();

            necro.GetStats();
            orc.GetStats();

            necro.AttackCard(orc);
            orc.AttackCard(necro);

            necro.GetStats();
            orc.GetStats();

        }
    }
}
