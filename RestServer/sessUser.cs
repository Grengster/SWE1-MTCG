using System;
using System.Collections.Generic;
using System.Text;

namespace RestServer
{
    public class SessUser
    {
        public string username="", password="", name="", bio="", image="";
        public int wins = 0, losses = 0, points = 0;
        public bool readyToFight = false;
        public bool online = false;
        private DateTime lastlogin;
        public List<DeckData> userDeck = new List<DeckData>();
        public SessUser()
        {
        }

        public void SetUser(string name, string pwd)
        {
            username = name;
            password = pwd;
            online = true;
        }

        public string GetInfo()
        {
            return "Here is your name: " + username + "\nLast login: " + GetLastLogin();
        }

        public string SeeDeck(SessUser user)
        {
            string output = "Decklist:\n";
            bool deckFilled = false;
            foreach (var DeckData in user.userDeck)
            {
                output = output + DeckData.UserDeckInfo() + "\n";
                deckFilled = true;
            }
            if (deckFilled == true)
                return output;
            else
                return null;
        }
        public string SeeUserInfo(SessUser user)
        {
            if (user.name == "")
                user.name = "NOT SET";
            if (user.bio == "")
                user.bio = "NOT SET";
            if (user.image == "")
                user.image = "NOT SET";
            return "Userinfo:\n" + "Name: " + user.name + "\n" + "Bio: " + user.bio + "\n" + "Image: " + user.image + "\n";
        }
        public string SeeUserStats(SessUser user)
        {
            return "Userstats:\n" + "Points: " + user.points + "\n" + "Wins: " + user.wins + "\n" + "Losses: " + user.losses + "\n";
        }

        public string GetUser()
        {
            return username;
        }
        public void SetLastLogin(DateTime time)
        {
            lastlogin = time;
        }

        public DateTime GetLastLogin()
        {
            return lastlogin;
        }

        public string GetPwd()
        {
            return password;
        }


    }
}
