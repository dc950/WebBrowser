using System;
using System.Collections.Generic;
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
        private FlowLayoutPanel _sidePanel;

        private enum SidePanelStatus
        {
            Bookmarks,
            History,
            None
        }

        private SidePanelStatus _sidePanelStatus = SidePanelStatus.None;

        public Control GetPanel() {
            return TabPanel;
        }

        public void AddHtmlPanel(HtmlPanel htmlPanel) {
            tableLayoutPanel1.Controls.Add(htmlPanel, 0, 2);
            htmlPanel.LinkClicked += OnLinkClicked;
        }

        protected virtual void OnLinkClicked(object sender, HtmlLinkClickedEventArgs e) {
            var baseUri = new Uri(_browser.ActiveTab.CurrentPage.Url);
            var url = new Uri(baseUri, e.Link);
            //has just been clicked so should be on current tab
            Browser.Instance.GoToLinkInCurrentTab(url.AbsoluteUri);
        }

        //        public event EventHandler<HtmlLinkClickedEventArgs> LinkClicked;


        public void GoToLink() {}

        public void SetCurrentPanel(HtmlPanel panel) {
            _currentHtmlPanel?.Hide();
            _currentHtmlPanel = panel;
            _currentHtmlPanel.Show();
        }

        public void SetUrlBar(WebPageReference webPage) {
            UrlBar.Text = webPage != null ? webPage.Url : "http://www.";
        }

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

            //fix issues with textbox height
            UrlBar.Height = 35;
            UrlBar.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, 35);
            UrlBar.MinimumSize = new Size(100, 35);
        }

        public void LoadPageIntoContextWindowFromThread(string content, string css, Tab tab) {
            //_context.Post(,null);
            //TODO possible to invoke this from tab?
            Invoke((MethodInvoker) delegate {
                tab.HtmlPanel.Text = content;
                if (css != "") {
                    tab.HtmlPanel.BaseStylesheet = css;
                }
                // TODO: load the css in a string and set it here: 
                //stab.HtmlPanel.BaseStylesheet = GetCss(content)s;
                tab.SetTitle(content);
            });
        }

        public void LoadPageIntoContentWindow(string content, HtmlPanel panel) {
            panel.Text = content;
        }

        private void bookmarksToolStripMenuItem_Click(object sender, EventArgs e) {}

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e) {}

        private void btnBack_Click(object sender, EventArgs e) {
            try {
                _browser.ActiveTab.GoBack();
            }
            catch (InvalidOperationException) {
                //Back has been pressed when there are no items.  Just do nothing...
            }
        }

        private void btnForwards_Click(object sender, EventArgs e) {
            try {
                _browser.ActiveTab.GoForwards();
            }
            catch (InvalidOperationException) {
                // do nothing
            }
        }

        private void NewTab_Click(object sender, EventArgs e) {
            _browser.OpenNewTab();
        }

        private void btnGo_Click(object sender, EventArgs e) {
            _browser.GoToLinkInCurrentTab(UrlBar.Text);
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {}


        //TODO theese methods are very similar - can something smart be done here?
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
            var bookmarks = Bookmarks.GetBookmarks().BookmarkList;
            _sidePanelStatus = SidePanelStatus.Bookmarks;
            ShowSidePanel(bookmarks.Cast<SavedUrl>().ToList());
        }

        private void ShowHistoryPanel() {
            var historyItems = GlobalHistory.GetGlobalHistory().HistoryItems;
            _sidePanelStatus = SidePanelStatus.History;
            ShowSidePanel(historyItems.Cast<SavedUrl>().ToList());
        }

        private void ShowSidePanel(List<SavedUrl> items) {
            var panel = new FlowLayoutPanel {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.TopDown
            };

            //get the elements needed and add them to the panel
            SetupSidePanelCloseButton(panel);
            SetupSidePanelButtons(items, panel);

            Controls.Add(panel); //add panel to form
            _sidePanel = panel; //update current side panel
        }

        private void SetupSidePanelCloseButton(FlowLayoutPanel panel) {
            var close = new Button {
                Text = "Close History",
                Dock = DockStyle.Bottom
            };
            close.Click += delegate { CloseSidePanel(); };
            panel.Controls.Add(close);
        }

        private void SetupSidePanelButtons(List<SavedUrl> items, FlowLayoutPanel panel) {
            foreach (var item in items) {
                var button = new Button {
                    //TODO some better abstraction?
                    Text = item.Title,
                    Width = 200
                };
                if (item is HistoryItem)
                    button.Text += " | " + (item as HistoryItem).Time;
                button.Click += delegate { GoToHistoryItem(item.WebPage); };
                panel.Controls.Add(button);
            }
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
            Bookmarks.GetBookmarks().AddBookmark(bookmark);
        }
    }
}
