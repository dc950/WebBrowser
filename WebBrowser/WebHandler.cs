using System;
using System.IO;
using System.Net;
using System.Threading;

namespace WebBrowser
{
    public class WebPageLoader
    {
        public delegate void ActivateContent(string content, WebPageReference webPage);

        //contentActivator is so different things can be done after finshing e.g. update history correctly, handle output as css etc.
        private readonly ActivateContent _contentActivator;

        private WebPageLoader(ActivateContent contentActivator) {
            _contentActivator = contentActivator;
        }

        public static void LoadPage(WebPageReference webPage, ActivateContent del) {
            Browser.Instance.MainWindow.SetUrlBar(webPage);
            var webPageLoader = new WebPageLoader(del);
            ThreadStart loadPageThreadStart = delegate { webPageLoader.LoadPageThread(webPage); };
            var thread = new Thread(loadPageThreadStart);
            thread.Start();
        }

        private void ProcessWebException(WebException webExcp, WebPageReference webPage) {
            var status = webExcp.Status;
            //if the error is a protocol error, the response should still be set
            if (status != WebExceptionStatus.ProtocolError) return;
            var response = (HttpWebResponse) webExcp.Response;
            var statusCode = response.StatusCode;
            var content = statusCode.ToString();
            _contentActivator(content, webPage);
        }

        public void LoadPageThread(WebPageReference webPage) {
            try {
                TryLoadPageThread(webPage);
            }
            catch (WebException webExcp) {
                ProcessWebException(webExcp, webPage);
            }
            catch (Exception e) {
                var content = "Failed to load page: " + e.Message;
                _contentActivator(content, webPage);
            }
        }

        private void TryLoadPageThread(WebPageReference webPage) {
            //Create web request and get responseStream
            var webRequest = WebRequest.Create(webPage.Url);
            var response = webRequest.GetResponse();
            var responseStream = response.GetResponseStream();
            //Get the html as a string from the responseStream
            var steamReader = new StreamReader(responseStream);
            var content = steamReader.ReadToEnd();
            _contentActivator(content, webPage);
        }
    }
}
