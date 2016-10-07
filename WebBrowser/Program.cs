using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace WebBrowser
{
    class MainClass
    {
        public static void Main(){
            Request webRequester = new Request("http://google.co.uk");
            var responseStream = webRequester.Response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            Console.WriteLine(reader.ReadToEnd());
        }
    }



    //There should only be one instance of this class - singleton pattern used
    class Browser
    {
        List<Tab> tabs;
        Tab activeTab;
        public List<Bookmark> Bookmarks { get; private set; }
        public GlobalHistory History { get; private set; }
        //GUI buttons and stuff will go here

        private static Browser browser;

        private Browser() {
            //call methods to load bookmarks and history
            //setup initial tab?
        }

        public static Browser Instance {
            get {
                if(browser==null)
                    browser = new Browser();
                return browser;
            }
        }

        public void addItemToGlobalHistory(HistoryItem item){
            History.Add(item);
        }

        public void OpenTab(){}
        public void OpenNewTab(){}
        public void OpenLinkInNewTab(string url){}
    }

    class Tab
    {
        LocalHistory tabHistory;
        ContentWindow renderWindow;
    }

    class URLBar
    {
        string currentUrl;
    }

    class ContentWindow
    {
        //html is rendered here
    }

    class WebPageReference
    {
        string url;
    }

    abstract class SavedUrl {
        WebPageReference url;
        string title;

        //public abstract void ConvertToJson();
    }

    class Bookmark : SavedUrl
    {

    }

    class HistoryItem : SavedUrl
    {
        DateTime time;
    }

    class GlobalHistory
    {
        List<HistoryItem> references;

        public void Add(HistoryItem item){}
        public void Remove(HistoryItem item){}
    }

    class LocalHistory
    {
        List<HistoryItem> Back;
        List<HistoryItem> Forwards;
    }

    class Request
    {
        public HttpWebResponse Response { get; private set; }
        public String Url { get; private set; }

        public Request(string url) {
            try {
                Url = url;
                WebRequest webRequest = WebRequest.Create(url);
                Response = webRequest.GetResponse() as HttpWebResponse;
            }
            catch (WebException webExcp) {
                WebExceptionStatus status =  webExcp.Status;
                //if the error is a protocol error, the response should still be set
                if (status == WebExceptionStatus.ProtocolError) {
                    Response = (HttpWebResponse)webExcp.Response;
                }
            }
            catch {
                //TODO: something if there is another type of exception
            }
        }
    }
}
