using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Npgsql;
using RestServer;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using DatabaseHandler;
using System.Xml.XPath;
using NpgsqlTypes;
using System.ComponentModel;
using MockServer;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BattleHandler
{
    public class BattleHandlerClass
    {
        public string StartBattle(SessUser player1, SessUser player2)
        {
            string battleLog = "Battlelog:\n";

            var P1Deck = new List<DeckData>(MyTcpListener.loggedUsers[player1.username].userDeck);
            var P2Deck = new List<DeckData>(MyTcpListener.loggedUsers[player2.username].userDeck);
            //MyTcpListener.loggedUsers[player1.username]
            int roundCount = 1;
            //int p1Health = 100, p2Health = 100;
            if (P1Deck.Count() != 4)
                return "Player1Err";
            if (P2Deck.Count() != 4)
                return "Player2Err";
            var rand = new Random();
            while (P1Deck.Count() != 0 && P2Deck.Count() != 0 && roundCount <= 100)
            {
                int randomCardP1 = rand.Next(0, P1Deck.Count());
                int randomCardP2 = rand.Next(0, P2Deck.Count());
                if(P1Deck[randomCardP1].Damage > P2Deck[randomCardP2].Damage)
                {
                    battleLog += "Round Nr." + roundCount + "\nPlayer 1's Card [Damage: " + P1Deck[randomCardP1].Damage + "] defeated Player 2's Card [Damage: " + P2Deck[randomCardP2].Damage + "]\n";
                    P1Deck.Add(P2Deck[randomCardP2]);
                    P2Deck.RemoveAt(randomCardP2);
                }
                else if(P1Deck[randomCardP1].Damage < P2Deck[randomCardP2].Damage)
                {
                    battleLog += "Round Nr." + roundCount + "\nPlayer 2's Card [Damage: " + P2Deck[randomCardP2].Damage + "] defeated Player 1's Card [Damage: " + P1Deck[randomCardP1].Damage + "]\n";
                    P2Deck.Add(P1Deck[randomCardP1]);
                    P1Deck.RemoveAt(randomCardP1);
                }
                else
                    battleLog += "Round Nr." + roundCount + "\nPlayer 2's Card [Damage: " + P2Deck[randomCardP2].Damage + "] is equal to Player 1's Card [Damage: " + P1Deck[randomCardP1].Damage + "]\n";


                roundCount++;
            }
            if (P1Deck.Count() > 4)
                battleLog = "1" + battleLog + "Player 1 won the battle, he took Player 2's Deck.";
            else if (P2Deck.Count() > 4)
                battleLog = "2" + battleLog + "Player 2 won the battle, he took Player 1's Deck.";
            else
                battleLog = "0" + battleLog + "Nobody won, it's a draw!";


            return battleLog;
        }


    }
}
