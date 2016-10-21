using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace WebBrowser
{
    //There should only be one instance of this class - singleton pattern used
    class Browser
    {
        private const string BookmarkFileName = "BrowserBookmarks.dat";
        private const string HistoryFileName = "BrowserHistory.dat";
        private const string HomeFileName = "BrowserHome.dat";


        public readonly List<Tab> Tabs = new List<Tab>();
        public Tab ActiveTab { get; private set; }
        public StorableCollection<Bookmark> Bookmarks { get; private set; }
        public StorableCollection<HistoryItem> History { get; private set; }
        public MainWindow MainWindow { get; set; }
        private static Browser _browser;
        private static readonly object Padlock = new object();

        public static Browser Instance {
            get {
                lock (Padlock) {
                    return _browser ?? (_browser = new Browser());
                }
            }
        }

        private HomePage _homePage;
        public HomePage HomePage =>
            _homePage ??
                (_homePage = HomePage.LoadHomePage(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), HomeFileName)));

        private Browser() {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var historyLoc = Path.Combine(appData, HistoryFileName);
            var bookmarkLoc = Path.Combine(appData, BookmarkFileName);

            History = new StorableCollection<HistoryItem>(historyLoc);
            Bookmarks = new StorableCollection<Bookmark>(bookmarkLoc);
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
                _homePage = HomePage.CreateHomePage(webPage, title);
        }
    }
}
