using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WebBrowser
{
    public class Tab
    {
        private const string NewTabText = "New Tab";
        public LocalHistory TabHistory { get; } = new LocalHistory();
        public WebPageReference CurrentPage { get; private set; }
        private HistoryItem _currentHistoryItem;
        
        private string _content;
        public string Title { get; private set; }
        public MainPanel MainPanel { get; } = new MainPanel();

        private readonly Button _button;
        private readonly FlowLayoutPanel _tabPanel;

        //these bools are used to determine if the forwards and back buttons should be active for this tab
        private bool _forwardActive;
        private bool _backActive;

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
                Text = NewTabText,
                BackColor = Color.Silver,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
            _button.Click += delegate { ActivateTab(); };
            _tabPanel.Controls.Add(_button);
            _tabPanel.Controls.Add(closeButton);
            var panel = Browser.Instance.MainWindow.TabFlowPanel;
            panel.Controls.Add(_tabPanel);
            ActivateTab();
        }

        public void SetTitle(string htmlString) {
            Contract.Requires(htmlString != null);
            var regex = Regex.Match(htmlString, @"<title>\s*(.+?)\s*</title>");
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

        public void LoadNewPage(WebPageReference webPage) {
            if(_currentHistoryItem != null)
                TabHistory.AddBack(_currentHistoryItem);
            WebHandler.LoadPage(webPage, ActivateNewPageContent);
        }

        public void GoBack() {
            try {
                if(TabHistory.Back.Count <= 1)
                    Browser.Instance.MainWindow.SetNavButtons(_forwardActive, false); //Disble button if going back too far
                WebHandler.LoadPage(TabHistory.GoBack(_currentHistoryItem).WebPage, ActivateBackContent);
            }
            catch (NullReferenceException) {
                //can't go back - keep current active
            }
        }

        public void GoForwards() {
            try {
                WebHandler.LoadPage(TabHistory.GoForwards(_currentHistoryItem).WebPage, ActivateForwardsContent);
            }
            catch {
                //can't go forwards - do nothing
            }
        }

        public void Refresh() {
            WebHandler.LoadPage(TabHistory.GoForwards(_currentHistoryItem).WebPage, ActivateRefreshContent);
        }

        private void ActivateNewPageContent(string content, WebPageReference webPage) {
            UpdateTabInfo(content, webPage);
            _forwardActive = false;
            if (TabHistory.Back.Count > 0)
                _backActive = true;
            DisplayWebPage();

        }

        private void ActivateBackContent(string content, WebPageReference webPage)
        {
            //only update global history
            UpdateTabInfo(content, webPage);
            if (TabHistory.Back.Count == 0)
                _backActive = false;
            if(TabHistory.Forwards.Count > 0)
                _forwardActive = true;
            DisplayWebPage();

        }

        private void ActivateForwardsContent(string content, WebPageReference webPage) {
            UpdateTabInfo(content, webPage);
            _backActive = true;
            if (TabHistory.Forwards.Count == 0)
                _forwardActive = false;
            DisplayWebPage();

        }

        private void ActivateRefreshContent(string content, WebPageReference webPage) {
            UpdateTabInfo(content, webPage);
            DisplayWebPage();
        }

        private void DisplayWebPage() {
            if (Browser.Instance.ActiveTab != this) return;
            Browser.Instance.MainWindow.Invoke((MethodInvoker) delegate {
                if (_content == null) _content = "";
                SetTitle(_content);
                MainPanel.SetContent(_content);
                Browser.Instance.MainWindow.SetNavButtons(_forwardActive, _backActive);
            });
        }

        private void UpdateTabInfo(string content, WebPageReference webPage) {
            CurrentPage = webPage;
            _content = content;
            _currentHistoryItem = new HistoryItem(webPage, Title);
            Browser.Instance.History.AddItem(_currentHistoryItem);
        }
    }
}
