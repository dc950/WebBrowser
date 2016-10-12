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

        private enum SidePanelStatus {Bookmarks, History, None}
        private SidePanelStatus _sidePanelStatus = SidePanelStatus.None;

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

        public Control GetPanel() { //TODO make property
            return TabPanel;
        }

        public void AddContentPanel(Control contentPanel) {
            tableLayoutPanel1.Controls.Add(contentPanel, 0, 2);
            var htmlPanel = contentPanel as HtmlPanel;
            if(htmlPanel != null)
                htmlPanel.LinkClicked += OnLinkClicked;
        }

        protected virtual void OnLinkClicked(object sender, HtmlLinkClickedEventArgs e) {
            var baseUri = new Uri(_browser.ActiveTab.CurrentPage.Url);
            var url = new Uri(baseUri, e.Link);
            //link has just been clicked so should base uri should be on the current tab
            Browser.Instance.GoToLinkInCurrentTab(url.AbsoluteUri);
        }
        
        public void SetCurrentPanel(HtmlPanel panel) {
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
                tab.MainPanel.SetContent(content);
                if (css != "")
                    tab.MainPanel.SetCss(css);
                tab.SetTitle(content);
            });
        }

        public void LoadPageIntoContentWindow(string content, HtmlPanel panel) {
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


        //TODO theese methods are very similar - can something smart be done here?
        //TODO put into new class?
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void btnToggleHtml_Click(object sender, EventArgs e)
        {
            MainWindowPanelManager.Instance.ToggleMode();
            if (MainWindowPanelManager.Instance.ActiveDisplayPanelMode == MainWindowPanelManager.DisplayPanelMode.Render)
                btnToggleHtml.Text = "Show html source";
            else
                btnToggleHtml.Text = "Show rendered webpage";
        }
    }

    //manages the global state of the main window
    //TODO change to normal singleton
    public class MainWindowPanelManager
    {
        private readonly List<MainWindowPanel> _mainWindowPanelListiners = new List<MainWindowPanel>();
        public enum DisplayPanelMode { Source, Render }

        public DisplayPanelMode ActiveDisplayPanelMode { get; private set; }
        private static MainWindowPanelManager _mainWindowPanelManager;

        public static MainWindowPanelManager Instance => _mainWindowPanelManager ?? (_mainWindowPanelManager = new MainWindowPanelManager());

        private MainWindowPanelManager() {
            ActiveDisplayPanelMode = DisplayPanelMode.Render;
        }

        public void ToggleMode() {
            switch (ActiveDisplayPanelMode) {
                case DisplayPanelMode.Render:
                    SetMode(DisplayPanelMode.Source);
                    break;
                case DisplayPanelMode.Source:
                    SetMode(DisplayPanelMode.Render);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetMode(DisplayPanelMode mode) {
            ActiveDisplayPanelMode = mode;
            foreach (var window in _mainWindowPanelListiners)
                window.ChangeMode(mode);
        }

        public void Subscribe(MainWindowPanel panel) {
            _mainWindowPanelListiners.Add(panel);
        }
    }

    public class MainWindowPanel
    {
        private readonly HtmlPanel _htmlPanel;
        readonly RichTextBox _sourceBox;
        public bool IsActive { get; set; }

        public MainWindowPanel() {
            _htmlPanel = new HtmlPanel {
                Dock = DockStyle.Fill
            };
            _htmlPanel.Hide();
            Browser.Instance.MainWindow.AddContentPanel(_htmlPanel);
            _sourceBox = new RichTextBox {
                ReadOnly = true,
                Dock = DockStyle.Fill
            };
            _sourceBox.Hide();
            Browser.Instance.MainWindow.AddContentPanel(_sourceBox);
            MainWindowPanelManager.Instance.Subscribe(this);
        }

        public void SetContent(string content) {
            _htmlPanel.Text = content;
            _sourceBox.Text = content;
        }

        public void SetCss(string css) {
            _htmlPanel.BaseStylesheet = css;
        }

        public void ChangeMode(MainWindowPanelManager.DisplayPanelMode mode) {
            if (mode == MainWindowPanelManager.DisplayPanelMode.Source)
                ActivateSource();
            else
                ActivateRender();
            
        }

        public void Activate() {
            IsActive = true;
            switch (MainWindowPanelManager.Instance.ActiveDisplayPanelMode) {
                case MainWindowPanelManager.DisplayPanelMode.Render:
                    _htmlPanel.Show();
                    break;
                case MainWindowPanelManager.DisplayPanelMode.Source:
                    _sourceBox.Show();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public void Disable() {
            IsActive = false;
            _htmlPanel.Hide();
            _sourceBox.Hide();
        }

        private void ActivateSource() {
            _htmlPanel.Hide();
            if (IsActive)
                _sourceBox.Show();
        }

        private void ActivateRender() {
            _sourceBox.Hide();
            if (IsActive)
                _htmlPanel.Show();
        }
    }
}
