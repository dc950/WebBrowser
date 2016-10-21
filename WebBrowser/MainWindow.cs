using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.WinForms;

namespace WebBrowser
{
    public partial class MainWindow : Form
    {
        private readonly Browser _browser;
        private HtmlPanel _currentHtmlPanel;
        private readonly SidePanel _sidePanel;
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
            if (_browser.HomePage != null)
                _browser.GoToLinkInCurrentTab(_browser.HomePage.WebPage);

            //fix issues with textbox height
            UrlBar.Height = 35;
            UrlBar.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, 35);
            UrlBar.MinimumSize = new Size(100, 35);

            _sidePanel = new SidePanel(this);
            Text = "Web Browser";
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

        private void btnBack_Click(object sender, EventArgs e) {
                _browser.ActiveTab.GoBack();
        }

        private void btnForwards_Click(object sender, EventArgs e) {
                _browser.ActiveTab.GoForwards();
        }

        private void NewTab_Click(object sender, EventArgs e) {
            _browser.OpenNewTab();
        }

        private void btnGo_Click(object sender, EventArgs e) {
            _browser.GoToLinkInCurrentTab(UrlBar.Text);
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {}


        private void btnFavourite_Click(object sender, EventArgs e) {
            var bookmark = new Bookmark(_browser.ActiveTab.CurrentPage, _browser.ActiveTab.Title);
            Browser.Instance.Bookmarks.AddItem(bookmark);
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            Browser.Instance.ActiveTab.Refresh();
        }

        private void btnToggleHtml_Click(object sender, EventArgs e) {
            MainPanelManager.Instance.ToggleMode();
            btnToggleHtml.Text = MainPanelManager.Instance.ActiveDisplayPanelMode ==
                                 MainPanelManager.DisplayPanelMode.Render ? "Show html source" : "Show rendered page";
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
                    btnFavourites_Click(null, null);
                    break;
                case Keys.Control | Keys.T:
                    NewTab_Click(null, null);
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnFavourites_Click(object sender, EventArgs e) {
            _sidePanel.FavouritesClicked();
        }

        private void btnHistory_Click(object sender, EventArgs e) {
            _sidePanel.HistoryClicked();
        }

        private void CheckEnterKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Return)
                btnGo_Click(null, null);
        }

        private void UrlBar_TextChanged(object sender, EventArgs e) {}

        private void backToolStripMenuItem_Click(object sender, EventArgs e) {
            btnBack_Click(sender, e);
        }

        private void forwardsToolStripMenuItem_Click(object sender, EventArgs e) {
            btnForwards_Click(sender, e);
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e) {
            btnHistory_Click(sender, e);
        }

        private void favouritesToolStripMenuItem_Click(object sender, EventArgs e) {
            btnFavourites_Click(sender, e);
        }

        private void toggleSourceToolStripMenuItem_Click(object sender, EventArgs e) {
            btnToggleHtml_Click(sender, e);
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e) {
            if (Browser.Instance.HomePage != null)
                Browser.Instance.ActiveTab.LoadNewPage(Browser.Instance.HomePage.WebPage);
        }

        public void SetNavButtons(bool forwardActive, bool backActive) {
            btnForwards.Enabled = forwardActive;
            btnBack.Enabled = backActive;
        }

        private void setCurrentPageAsHomeToolStripMenuItem_Click(object sender, EventArgs e) {
            Browser.Instance.SetHomePage(Browser.Instance.ActiveTab.CurrentPage, Browser.Instance.ActiveTab.Title);
        }
    }
}
