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
using BattleHandler;

namespace HTTPNUnitTests
{
    public class DatabaseTest
    {
        DatabaseHandlerClass database = new DatabaseHandlerClass();
        SessUser userTest = new SessUser();
        SessUser userTest2 = new SessUser();
        SessUser fakeUser = new SessUser();
        [SetUp]
        public void UserSetup()
        {
            userTest.SetUser("kienboec", "daniel");
            //database.LoginUser("kienboec", "daniel", ref userTest);
            //database.LoginUser("altenhof", "markus", ref userTest2);
            //Assert.Positive
        }

        [Test]
        public void RegisterAndLoginTest()
        {
            database.LoginUser("kienboec", "daniel", ref userTest);
            Assert.IsNotNull(userTest.GetUser());
            Assert.IsNotNull(userTest.GetPwd());
            Assert.IsFalse(database.RegisterUser("kienboec", "daniel"));
            var loginFunc = database.LoginUser("kienboec", "daniel", ref userTest);
            Assert.AreEqual(-2, loginFunc);
        }
        [Test]
        public void ScoreboardTest()
        {
            Assert.IsNotNull(database.CheckScore(userTest));
        }
        [Test]
        public void GetUserDecksTest()
        {
            database.LoginUser("kienboec", "daniel", ref userTest);
            var deckFunc = database.GetDecks(userTest);
            Assert.AreEqual(-1, deckFunc); //fails because of no bought cards
        }
        [Test]
        public void SeeBoughtCardsTest()
        {
            var deckFunc = database.SeeBoughtCards(fakeUser);
            Assert.IsNotNull(deckFunc, "zero"); //fails because of no bought cards
            var deckFuncTrue = database.SeeBoughtCards(userTest);
            Assert.AreNotEqual(deckFuncTrue, "zero");
        }
        [Test]
        public void InsertCardsIntoDeckTest()
        {
            string jsonText = "[\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"171f6076-4eb5-4a7d-b3f2-2d650cc3d237\"]";

            var deckFunc = database.InsertCards(userTest, jsonText);
            var deckFuncFake = database.InsertCards(fakeUser, "");

            Assert.AreEqual(deckFuncFake, -1); //fails because of no bought cards
            Assert.AreEqual(deckFunc, 2);
        }

        [Test]
        public void RetrieveCardsTest()
        {
            var deckFunc = database.RetrieveCards(userTest);
            var deckFuncFake = database.RetrieveCards(fakeUser);

            Assert.AreEqual(deckFuncFake, 1); //fails because of no bought cards
            Assert.AreEqual(deckFunc, 2);
        }

        [Test]
        public void CheckUserInfoTest()
        {
            var deckFunc = database.CheckUserInfo(userTest, "get", "");
            var deckFuncFake = database.CheckUserInfo(fakeUser, "set", "");

            Assert.AreEqual(deckFuncFake, -1); //wrong info
            Assert.AreEqual(deckFunc, 1);
        }

        [Test]
        public void BattleTest()
        {
            BattleHandlerClass battle = new BattleHandlerClass();
            var battleFunc = battle.StartBattle(userTest, userTest2);

            Assert.AreEqual(battleFunc, "Player1Err"); //no cards in deck
        }

        [Test]
        public void CheckUserAvailable()
        {
            var loginFunc = database.CheckUser("kienboec");
            var loginFuncFail = database.CheckUser("keinbock");

            Assert.IsFalse(loginFunc); //false when available
            Assert.IsTrue(loginFuncFail);
        }
    }
}