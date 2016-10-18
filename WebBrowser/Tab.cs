using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WebBrowser
{
    public class Tab
    {
        public LocalHistory TabHistory { get; } = new LocalHistory();
        public WebPageReference CurrentPage { get; private set; }
        private HistoryItem _currentHistoryItem;

        //TODO create a WebPage item with html, css, title and url to abstract it away from tab
        private string _content;
        public string Title { get; private set; }
        public MainPanel MainPanel { get; } = new MainPanel();

        private readonly Button _button;
        private readonly FlowLayoutPanel _tabPanel;
        private string _css = "";

        //these bools are used to determine if the forwards and back buttons should be active for this tab
        private bool _forwardActive = false;
        private bool _backActive = false;

        public Tab() {
            _tabPanel = new FlowLayoutPanel {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };
            var closeButton = new Button {
                Text = "X",
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            closeButton.Size = new Size(15, closeButton.Size.Height);
            closeButton.Click += delegate { CloseTab(); };

            _button = new Button {
                Text = "New Tab",
                BackColor = Color.Silver,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
            _button.Click += delegate { ActivateTab(); };
            _tabPanel.Controls.Add(_button);
            _tabPanel.Controls.Add(closeButton);
            var panel = Browser.Instance.MainWindow.GetPanel();
            panel.Controls.Add(_tabPanel);
            ActivateTab();
        }

        public void SetTitle(string htmlString) {
            Contract.Requires(htmlString != null);
            var regex = Regex.Match(htmlString, @"<title>\s*(.+?)\s*</title>"); //TODO get from 
            var url = CurrentPage != null ? CurrentPage.Url : "Tab";
            var title = regex.Success ? regex.Groups[1].Value : url;
            _button.Text = title;
            Title = title;
        }

        public void DisableTab() {
            _button.BackColor = Color.Silver;
            MainPanel.Disable();
        }

        public void ActivateTab() {
            if (Browser.Instance.ActiveTab != null) Browser.Instance.ActiveTab.DisableTab();
            Browser.Instance.SetActiveTab(this);
            MainPanel.Activate();
            Browser.Instance.MainWindow.SetUrlBar(CurrentPage);
            _button.BackColor = Color.CornflowerBlue;
        }

        public void CloseTab() {
            MainPanel.Disable();
            _content = null;
            _tabPanel.Controls.Clear();
            Browser.Instance.Tabs.Remove(this);
            Browser.Instance.MainWindow.TabFlowPanel.Controls.Remove(_tabPanel);
        }
        
        private void DisplayWebPage() {
            if (Browser.Instance.ActiveTab == this) {
                Browser.Instance.MainWindow.Invoke((MethodInvoker) delegate {
                    if (_content == null) _content = "";
                    SetTitle(_content);
                    MainPanel.SetContent(_content);
                    Browser.Instance.MainWindow.SetNavButtons(_forwardActive, _backActive);
                });
            }
//            Browser.Instance.MainWindow.LoadPageIntoContextWindowFromThread(_content, _css, this);
        }

        public void LoadNewPage(WebPageReference webPage) {
            TabHistory.AddBack(_currentHistoryItem);
            WebPageLoader.LoadPage(webPage, ActivateNewPageContent);
        }

        public void GoBack() {
            try {
                WebPageLoader.LoadPage(TabHistory.GoBack(_currentHistoryItem).WebPage, ActivateBackContent);
            }
            catch (NullReferenceException) {
                //can't go back - show blank page
                _content = "";
                _currentHistoryItem = null;
                _css = "";
                _button.Text = "New tab"; //TODO get from somewhere
            }
        }

        public void GoForwards() {
            try {
                WebPageLoader.LoadPage(TabHistory.GoForwards(_currentHistoryItem).WebPage, ActivateForwardsContent);
            }
            catch {
                //can't go forwards - do nothing
            }
        }
        
        public void ActivateNewPageContent(string content, WebPageReference webPage) {
            // _css = GetCss(content, webPage);
            UpdateTabInfo(content, webPage);
            _forwardActive = false;
            if (TabHistory.Back.Count > 0)
                _backActive = true;
            DisplayWebPage();

        }

        public void ActivateBackContent(string content, WebPageReference webPage)
        {
            //only update global history
            UpdateTabInfo(content, webPage);
            if (TabHistory.Back.Count == 0)
                _backActive = false;
            if(TabHistory.Forwards.Count > 0)
                _forwardActive = true;
            DisplayWebPage();

        }

        public void ActivateForwardsContent(string content, WebPageReference webPage) {
            UpdateTabInfo(content, webPage);
            _backActive = true;
            if (TabHistory.Forwards.Count == 0)
                _forwardActive = false;
            DisplayWebPage();

        }

        public void ActivateRefreshContent(string content, WebPageReference webPage) {
            UpdateTabInfo(content, webPage);
            DisplayWebPage();
        }

        private void UpdateTabInfo(string content, WebPageReference webPage) {
            CurrentPage = webPage;
            _content = content;
            _currentHistoryItem = new HistoryItem(webPage, Title);
            Browser.Instance.History.Add(_currentHistoryItem);
        }
    }
}

//old CSS stuff

//TODO remove and change old one to return content, not set it so it can be reused
//        private string GetCss(string content, WebPageReference webPage) {
//            var doc = new HtmlDocument();
//            doc.LoadHtml(content);
//            var nodes = doc.DocumentNode.SelectNodes("//head/link[@type='text/css']");
//            if (nodes == null) return "";
//            return nodes.Aggregate("", (current, node) => LoadCssFromNode(webPage, current, node));
//        }
//
//        private string LoadCssFromNode(WebPageReference webPage, string combinedCss, HtmlNode node) {
//            try {
//                combinedCss = TryLoadCssFromNode(webPage, node);
//            }
//            catch (Exception e) {
//                Console.WriteLine("Error occured while getting css: {0}", e.Message);
//            }
//            return combinedCss;
//        }
//
//        private string TryLoadCssFromNode(WebPageReference webPage, HtmlNode node) {
//            var combinedCss = "";
//            var href = node.Attributes["href"].Value;
//            var uri = new Uri(webPage.Url);
//            var absoluteLink = new Uri(uri, href);
//            //now need to make an html request to get the file
//            combinedCss += LoadCssData(absoluteLink.AbsoluteUri);
//            //TODO would be good to create more threads here to simultanously get multiple css files
//            return combinedCss;
//        }
//
//        private string LoadCssData(string url) {
//            try {
//                return GetPageContentAsString(new WebPageReference(url));
//            }
//            catch (WebException e) {
//                Console.WriteLine("Web exception occured during CSS request: {0}", e.Message);
//                return ""; //something went wrong, just don't load any css
//            }
//            catch (Exception e) {
//                Console.WriteLine("Exception occured during CSS request: {0}", e.Message);
//                return "";
//            }
//        }
