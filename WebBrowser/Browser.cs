using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows.Forms;

namespace WebBrowser
{
    //There should only be one instance of this class - singleton pattern used
    class Browser
    {
        public readonly List<Tab> Tabs = new List<Tab>();
        public Tab ActiveTab { get; private set; }
        public List<Bookmark> Bookmarks { get; } = new List<Bookmark>();
        public GlobalHistory History { get; private set; }
        public MainWindow MainWindow { get; set; }
        private static Browser _browser;
        public static Browser Instance => _browser ?? (_browser = new Browser());
        private HomePage _homePage;
        public HomePage HomePage {
            get {
                if (_homePage == null)
                    _homePage = HomePage.LoadHomePage(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HomePage.dat")); 
                return _homePage;
            }
        }

        private Browser() {
            History = GlobalHistory.Instance;
            //TODO move getting bookmarks to here
        }

        public void SetActiveTab(Tab tab) {
            ActiveTab = tab;
        }

        public void OpenNewTab() {
            var tab = new Tab();
            Tabs.Add(tab);
            ActiveTab = tab;
        }
        
        public void GoToLinkInCurrentTab(string url) {
            var webPage = new WebPageReference(url);
            GoToLinkInCurrentTab(webPage);
        }

        internal void GoToLinkInCurrentTab(WebPageReference webPage) {
            Contract.Requires(ActiveTab != null);
            ActiveTab.LoadNewPage(webPage);
        }

        public void SetHomePage(WebPageReference webPage, string title)
        {
            if (HomePage != null)
                HomePage.SetHomePage(webPage, title);
            else
                _homePage = WebBrowser.HomePage.CreateHomePage(webPage, title);
        }
    }
}
