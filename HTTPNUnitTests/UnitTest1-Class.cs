using NUnit.Framework;
using MockServer;
using Request;
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
using System.Runtime.InteropServices.WindowsRuntime;

namespace HTTPNUnitTests
{
    public class Tests
    {
        SessUser userTest = new SessUser();
        SessUser userTestFail = new SessUser();

        [SetUp]
        public void UserSetup()
        {
            userTest.SetUser("kienboec", "daniel");
        }
        [Test]
        public void CheckUserNull()
        {
            Assert.IsNotEmpty(userTest.GetUser());
            Assert.IsNotEmpty(userTest.GetPwd());
        }

        [Test]
        public void CheckUserEmpty()
        {
            Assert.IsEmpty(userTestFail.GetUser());
            Assert.IsEmpty(userTestFail.GetPwd());
        }

        [Test]
        public void CheckUserDeck()
        {
            Assert.IsEmpty(userTest.userDeck);
        }
    }
}