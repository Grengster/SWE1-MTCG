using System;
using System.Collections.Generic;
using System.Text;

namespace RestServer
{
    public class SessUser
    {
        private string username="", password="";
        private DateTime lastlogin;
        public SessUser(string name, string pwd)
        {
            username = name;
            password = pwd;
        }

        public void GetUser()
        {
            Console.WriteLine("Here is your fucking name: " + username + "\nLast login: " + GetLastLogin());
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
