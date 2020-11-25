using NUnit.Framework;
using MockServer;
using Request;

namespace HTTPNUnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            MyTcpListener server = new MyTcpListener();
            RequestContext test = new RequestContext();
            Assert.IsNotNull(server);
        }
    }
}