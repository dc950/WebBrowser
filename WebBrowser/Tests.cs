using System;
using NUnit.Framework;
using System.Net;


namespace WebBrowser
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestHttpRequest()
        {
            Request webRequester = new Request("http://www.google.com");
            var status = ((HttpWebResponse)webRequester.Response).StatusDescription;
            Assert.AreEqual("OK",status);
        }

        [Test]
        public void TestHistory(){
            Browser browser = Browser.Instance;
        }

        public void TestNewTab(){
            
        }

        public void TestBookmarks(){
            
        }
    }
}

