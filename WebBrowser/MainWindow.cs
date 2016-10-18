using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.WinForms;

namespace WebBrowser
{
    public partial class MainWindow : Form
    {
        private readonly Browser _browser;
        private HtmlPanel _currentHtmlPanel;
        private TableLayoutPanel _sidePanel;

        private enum SidePanelStatus
        {
            Bookmarks,
            History,
            None
        }

        private SidePanelStatus _sidePanelStatus = SidePanelStatus.None;

        public FlowLayoutPanel TabFlowPanel => TabPanel;

        public MainWindow() {
            InitializeComponent();

            //make the first two row heights absolute
            tableLayoutPanel1.RowStyles[0].SizeType = SizeType.Absolute;
            tableLayoutPanel1.RowStyles[1].SizeType = SizeType.Absolute;
            tableLayoutPanel1.RowStyles[0].Height = 35;
            tableLayoutPanel1.RowStyles[1].Height = 35;

            //initialise browser
            _browser = Browser.Instance;
            _browser.MainWindow = this;
            _browser.OpenNewTab();
            if(_browser.HomePage != null)
                _browser.GoToLinkInCurrentTab(_browser.HomePage.WebPage);

            //fix issues with textbox height
            UrlBar.Height = 35;
            UrlBar.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, 35);
            UrlBar.MinimumSize = new Size(100, 35);
        }

        public Control GetPanel() {
            //TODO make property
            return TabPanel;
        }

        public void AddContentPanel(Control contentPanel) {
            tableLayoutPanel1.Controls.Add(contentPanel, 0, 2);
            var htmlPanel = contentPanel as HtmlPanel;
            if (htmlPanel != null)
                htmlPanel.LinkClicked += OnLinkClicked;
        }

        protected virtual void OnLinkClicked(object sender, HtmlLinkClickedEventArgs e) {
            var baseUri = new Uri(_browser.ActiveTab.CurrentPage.Url);
            var url = new Uri(baseUri, e.Link);
            //link has just been clicked so should base uri should be on the current tab
            Browser.Instance.GoToLinkInCurrentTab(url.AbsoluteUri);
        }

        public void SetCurrentPanel(HtmlPanel panel) {
            Contract.Requires(panel != null);
            _currentHtmlPanel?.Hide();
            _currentHtmlPanel = panel;
            _currentHtmlPanel.Show();
        }

        public void SetUrlBar(WebPageReference webPage) {
            UrlBar.Text = webPage != null ? webPage.Url : "http://www.";
        }

        public void LoadPageIntoContextWindowFromThread(string content, string css, Tab tab) {
            //TODO possible to invoke this from tab?
            Invoke((MethodInvoker) delegate {
                if (content == null) content = "";
                tab.SetTitle(content);
                tab.MainPanel.SetContent(content);
                if (css != "")
                    tab.MainPanel.SetCss(css);
            });
        }

        public void LoadPageIntoContentWindow(string content, HtmlPanel panel) {
            Contract.Requires(panel != null);
            panel.Text = content;
        }

        private void btnBack_Click(object sender, EventArgs e) {
            try {
                _browser.ActiveTab.GoBack();
            }
            catch (InvalidOperationException) {
                //ignore
            }
        }

        private void btnForwards_Click(object sender, EventArgs e) {
            try {
                _browser.ActiveTab.GoForwards();
            }
            catch (InvalidOperationException) {
                //ignore
            }
        }

        private void NewTab_Click(object sender, EventArgs e) {
            _browser.OpenNewTab();
        }

        private void btnGo_Click(object sender, EventArgs e) {
            _browser.GoToLinkInCurrentTab(UrlBar.Text);
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {}


        //TODO put all side panel stuff in new class
        private void btnFavourites_Click(object sender, EventArgs e) {
            switch (_sidePanelStatus) {
                case SidePanelStatus.Bookmarks:
                    CloseSidePanel();
                    break;
                case SidePanelStatus.History:
                    CloseSidePanel();
                    ShowBookMarkPanel();
                    break;
                case SidePanelStatus.None:
                    ShowBookMarkPanel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void btnHistory_Click(object sender, EventArgs e) {
            switch (_sidePanelStatus) {
                case SidePanelStatus.History:
                    CloseSidePanel();
                    break;
                case SidePanelStatus.Bookmarks:
                    CloseSidePanel();
                    ShowHistoryPanel();
                    break;
                case SidePanelStatus.None:
                    ShowHistoryPanel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowBookMarkPanel() {
            var bookmarks = Bookmarks.Instance.BookmarkList;
            _sidePanelStatus = SidePanelStatus.Bookmarks;
            ShowSidePanel(bookmarks.Cast<SavedUrl>().ToList());
        }

        private void ShowHistoryPanel() {
            var historyItems = GlobalHistory.Instance.HistoryItems;
            _sidePanelStatus = SidePanelStatus.History;
            ShowSidePanel(historyItems.Cast<SavedUrl>().ToList());
        }

        private void ShowSidePanel(List<SavedUrl> items) {
            var table = new TableLayoutPanel {
                Dock = DockStyle.Right,
                RowCount = 2,
                ColumnCount = 1,
                Width = 300
                
            };
            //get the elements needed and add them to the panel
            table.Controls.Add(SetupSidePanelCloseButton(), 0, 0);
            table.Controls.Add(SetupSidePanelButtons(items), 0, 1);

            Controls.Add(table); //add panel to form
            _sidePanel = table; //update current side panel
        }

        private Button SetupSidePanelCloseButton() {
            var close = new Button {
                Text = "Close",
                Dock = DockStyle.Bottom
            };
            close.Click += delegate { CloseSidePanel(); };
            return close;
        }

        private FlowLayoutPanel SetupSidePanelButtons(List<SavedUrl> items) {
            var panel = SetupSidePanelPanel();
            panel.Width = 300;
            foreach (var item in items) {
                var innerPannel = new FlowLayoutPanel {
                    FlowDirection = FlowDirection.LeftToRight,
                    Width = 300
                };
                var button = SetupSidePanelContentButton(item);
                innerPannel.Height = button.Height+2;
                var removeButton = new Button {Text = "X"};
                if(item is HistoryItem)
                    removeButton.Click += delegate { GlobalHistory.Instance.Remove(item as HistoryItem); panel.Controls.Remove(innerPannel); };
                else if(item is Bookmark)
                    removeButton.Click += delegate { Bookmarks.Instance.Remove(item as Bookmark); panel.Controls.Remove(innerPannel); };
                innerPannel.Controls.Add(button);
                innerPannel.Controls.Add(removeButton);
                panel.Controls.Add(innerPannel);
            }
            return panel;
        }

        private static FlowLayoutPanel SetupSidePanelPanel() {
            var panel = new FlowLayoutPanel {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
            };
            return panel;
        }

        private Button SetupSidePanelContentButton(SavedUrl item) {
            var button = new Button {
                Text = item.Title,
                Width = 200
            };
            var historyItem = item as HistoryItem;
            if (historyItem != null)
                button.Text += " | " + historyItem.Time;
            button.Click += delegate { GoToHistoryItem(item.WebPage); };
            return button;
        }

        private void GoToHistoryItem(WebPageReference reference) {
            Browser.Instance.GoToLinkInCurrentTab(reference);
        }

        private void CloseSidePanel() {
            foreach (var control in _sidePanel.Controls)
                _sidePanel.Controls.Remove((Control) control);
            Controls.Remove(_sidePanel);
            _sidePanel = null;
            _sidePanelStatus = SidePanelStatus.None;
        }

        private void btnFavourite_Click(object sender, EventArgs e) {
            var bookmark = new Bookmark(_browser.ActiveTab.CurrentPage, _browser.ActiveTab.Title);
            Bookmarks.Instance.AddBookmark(bookmark);
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            Browser.Instance.ActiveTab.LoadNewPage(Browser.Instance.ActiveTab.CurrentPage);
        }

        private void btnToggleHtml_Click(object sender, EventArgs e) {
            MainPanelManager.Instance.ToggleMode();
            btnToggleHtml.Text = MainPanelManager.Instance.ActiveDisplayPanelMode == MainPanelManager.DisplayPanelMode.Render ? "Show html source" : "Show rendered webpage";
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch (keyData) {
                case Keys.Alt | Keys.Left:
                    btnBack_Click(null, null);
                    break;
                case Keys.Alt | Keys.Right:
                    btnForwards_Click(null, null);
                    break;
                case Keys.F5:
                    btnRefresh_Click(null, null);
                    break;
                case Keys.Control | Keys.H:
                    btnHistory_Click(null, null);
                    break;
                case Keys.Control | Keys.B:
                    break;
                case Keys.Control | Keys.T:
                    NewTab_Click(null, null);
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CheckEnterKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Return)
                btnGo_Click(null, null);
        }

        private void UrlBar_TextChanged(object sender, EventArgs e) {}

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnBack_Click(sender, e);
        }

        private void forwardsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnForwards_Click(sender, e);
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnHistory_Click(sender, e);
        }

        private void favouritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnFavourites_Click(sender, e);
        }

        private void toggleSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnToggleHtml_Click(sender, e);
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Browser.Instance.HomePage != null)
                Browser.Instance.ActiveTab.LoadNewPage(Browser.Instance.HomePage.WebPage);
        }

        public void SetNavButtons(bool forwardActive, bool backActive) {
            btnForwards.Enabled = forwardActive;
            btnBack.Enabled = backActive;
        }

        private void setCurrentPageAsHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Browser.Instance.SetHomePage(Browser.Instance.ActiveTab.CurrentPage, Browser.Instance.ActiveTab.Title);
        }
    }
}
