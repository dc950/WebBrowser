using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using HtmlAgilityPack;
using TheArtOfDev.HtmlRenderer.WinForms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WebBrowser
{
    public class Tab
    {
        public LocalHistory TabHistory { get; } = new LocalHistory();
        public WebPageReference CurrentPage { get; private set; }
        private Thread _thread;
        private string _content;
        public string Title { get; private set; }
        //public HtmlPanel HtmlPanel { get; } = new HtmlPanel();
        public MainWindowPanel MainPanel { get; } = new MainWindowPanel();

        private readonly Button _button;
        private string _css;

        private Tab() {
            
            _button = new Button();
            _button.Click += delegate { ActivateTab(); };
            var panel = Browser.Instance.MainWindow.GetPanel();
            _button.Text = "New Tab";
            _button.BackColor = Color.Silver;
            panel.Controls.Add(_button);
            ActivateTab();
        }

        public void SetTitle(string htmlString) {
            var regex = Regex.Match(htmlString, @"<title>\s*(.+?)\s*</title>");
            var url = CurrentPage != null ? CurrentPage.Url : "Tab";
            var title = regex.Success ? regex.Groups[1].Value : url;
            _button.Text = title;
            Title = title;
        }

        public void GoBack() {
            LoadPage(TabHistory.GoBack(CurrentPage, Title).WebPage);
        }

        public void GoForwards() {
            LoadPage(TabHistory.GoForwards(CurrentPage, Title).WebPage);
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

        public void CloseTab() {}

        public static Tab NewTab() {
            var tab = new Tab();
            return tab;
        }

        public void LoadPage(WebPageReference webPage) {
            //TODO move all multithreading and http stuff to new class?
            Browser.Instance.MainWindow.SetUrlBar(webPage);
            if (_thread != null && _thread.IsAlive) {
                _thread.Abort();
            }
            ThreadStart loadPageThreadStart = delegate { TryLoadPageThread(webPage); };
            _thread = new Thread(loadPageThreadStart);
            _thread.Start();
        }

        private void TryLoadPageThread(WebPageReference webPage) {
            try {
                LoadPageThread(webPage);
            }
            catch (WebException webExcp) {
                ProcessWebException(webExcp);
            }
            catch (Exception e) {
                _content = "Failed to load page: " + e.Message;
                if (Browser.Instance.ActiveTab == this) DisplayWebPage();
            }
        }

        private void LoadPageThread(WebPageReference webPage) {
            _content = GetPageContentAsString(webPage);
            _css = GetCss(_content, webPage);
            if (Browser.Instance.ActiveTab == this)
                DisplayWebPage();
            UpdateHistories(webPage);
            CurrentPage = webPage;
        }

        private void DisplayWebPage() {
            Browser.Instance.MainWindow.LoadPageIntoContextWindowFromThread(_content, _css, this);
        }

        //TODO store history and add it to back after so its not being remade?
        public void UpdateHistories(WebPageReference webPage) {
            //Global history is getting updated for new page - back history is getting updated for last page 
            Browser.Instance.History.Add(new HistoryItem(webPage, Title));
            if (CurrentPage != null) TabHistory.AddBack(CurrentPage);
        }

        //TODO remove and change old one to return content, not set it so it can be reused
        private string GetCss(string content, WebPageReference webPage) {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var nodes = doc.DocumentNode.SelectNodes("//head/link[@type='text/css']");
            if (nodes == null) return "";
            return nodes.Aggregate("", (current, node) => LoadCssFromNode(webPage, current, node));
        }

        private string LoadCssFromNode(WebPageReference webPage, string combinedCss, HtmlNode node) {
            try {
                combinedCss = TryLoadCssFromNode(webPage, node);
            }
            catch (Exception e) {
                Console.WriteLine("Error occured while getting css: {0}", e.Message);
            }
            return combinedCss;
        }

        private string TryLoadCssFromNode(WebPageReference webPage, HtmlNode node) {
            var combinedCss = "";
            var href = node.Attributes["href"].Value;
            var uri = new Uri(webPage.Url);
            var absoluteLink = new Uri(uri, href);
            //now need to make an html request to get the file
            combinedCss += LoadCssData(absoluteLink.AbsoluteUri);
            //TODO would be good to create more threads here to simultanously get multiple css files
            return combinedCss;
        }

        private string LoadCssData(string url) {
            try {
                return GetPageContentAsString(new WebPageReference(url));
            }
            catch (WebException e) {
                Console.WriteLine("Web exception occured during CSS request: {0}", e.Message);
                return ""; //something went wrong, just don't load any css
            }
            catch (Exception e) {
                Console.WriteLine("Exception occured during CSS request: {0}", e.Message);
                return "";
            }
        }


        //TODO move web stuff to new class
        private static string GetPageContentAsString(WebPageReference webPage) {
            //Create web request and get responseStream
            var webRequest = WebRequest.Create(webPage.Url);
            var response = webRequest.GetResponse();
            var responseStream = response.GetResponseStream();
            //Get the html as a string from the responseStream
            var steamReader = new StreamReader(responseStream);
            var content = steamReader.ReadToEnd();
            return content;
        }

        private void ProcessWebException(WebException webExcp) {
            var status = webExcp.Status;
            //if the error is a protocol error, the response should still be set
            if (status != WebExceptionStatus.ProtocolError) return;
            var response = (HttpWebResponse) webExcp.Response;
            var statusCode = response.StatusCode;
            var content = statusCode.ToString();
            Browser.Instance.MainWindow.LoadPageIntoContextWindowFromThread(content, "", this);
                //TODO create overload with no css?
        }
    }
}
