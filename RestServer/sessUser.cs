using System;
using System.Collections.Generic;
using System.Text;

namespace RestServer
{
    public class SessUser
    {
        public string username="", password="";
        private DateTime lastlogin;
        public List<deckData> userDeck = new List<deckData>();
        public SessUser()
        {
        }

        public void setUser(string name, string pwd)
        {
            username = name;
            password = pwd;
        }

        public string GetInfo()
        {
            return "Here is your name: " + username + "\nLast login: " + GetLastLogin();
        }

        public string SeeDeck(SessUser user)
        {
            string output = "Decklist:\n";
            bool deckFilled = false;
            foreach (var deckData in user.userDeck)
            {
                output = output + deckData.UserDeckInfo() + "\n";
                deckFilled = true;
            }
            if (deckFilled == true)
                return output;
            else
                return null;
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
