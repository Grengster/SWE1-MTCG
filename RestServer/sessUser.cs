using System;
using System.Collections.Generic;
using System.Text;

namespace RestServer
{
    public class SessUser
    {
        public string username="", password="";
        private DateTime lastlogin;
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
