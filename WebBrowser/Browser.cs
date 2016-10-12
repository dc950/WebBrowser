using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        //GUI buttons and stuff will go here
        private static Browser _browser;
        public static Browser Instance => _browser ?? (_browser = new Browser());

        private Browser()
        {
            History = GlobalHistory.GetGlobalHistory();
            //TODO get bookmarks
        }

        public void AddItemToGlobalHistory(HistoryItem item)
        {
            History.Add(item);
        }

        public void SetActiveTab(Tab tab)
        {
            ActiveTab = tab;
        }

        public void OpenNewTab()
        {
            var tab = Tab.NewTab();
            Tabs.Add(tab);
            ActiveTab = tab;
            //MainWindow.Controls.Add(button);
        }

        public void OpenLinkInNewTab(string url) { }

        public void GoToLinkInCurrentTab(string url)
        {
            Contract.Requires(ActiveTab != null);
            var webPage = new WebPageReference(url);
            GoToLinkInCurrentTab(webPage);
        }

        internal void GoToLinkInCurrentTab(WebPageReference webPage)
        {
            ActiveTab.LoadPage(webPage);
        }
    }
}
