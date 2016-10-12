using System;
using NUnit.Framework;
using System.Net;


namespace WebBrowser
{
    [TestFixture]
    public class Tests
    {
        private void Setup() {
            var mainWindow = new MainWindow();
        }

        [Test]
        public void TestBrowserInstantiation() {
            var mainWindow = new MainWindow();
            Assert.True(Browser.Instance.MainWindow != null);
        }

        [Test]
        public void TestNewTab() {
            Setup();
            Browser.Instance.OpenNewTab();
            var tab = Browser.Instance.ActiveTab;
            Assert.True(tab != null);
        }

        public void TestLoadingPage() {
            
        }

        public void TestSavingAndLoadingBookmarks() {
            
        }

        public void TestSavingAndLoadingHistory() {
            
        }

        public void TestForwardsAndBackNavigation() {
            
        }
    }
}

