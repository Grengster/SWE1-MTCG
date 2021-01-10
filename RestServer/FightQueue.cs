using RestServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace FightQueueClass
{
    public class FightQueue
    {
        public SessUser Player1 { get; set; }
        public SessUser Player2 { get; set; }

        public string BattleLog { get; set; }

        public bool FightOver { get; set; } = false;
        public override bool Equals(object obj)
        {
            return obj is FightQueue queue &&
                   EqualityComparer<SessUser>.Default.Equals(Player1, queue.Player1) &&
                   EqualityComparer<SessUser>.Default.Equals(Player2, queue.Player2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player1, Player2, BattleLog, FightOver);
        }
    }
}
