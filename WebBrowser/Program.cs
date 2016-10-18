using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

//TODO bugs:
//If favourites is empty the close button does not appear
//Don't think css is working - test on simple example on uni server
//If favourite is added while window open, it will not be added to the window
//TODO essential features
//menus
//home page
//make nicer - less buttons
//TODO extra features
//unit tests
//Tidy/abstract more code - more classes
//More contracts and try - catches
//Options? just some singleton/static class with a bunch of settings that can be configured

namespace WebBrowser
{
    class MainClass
    {
        [STAThread]
        public static void Main() {
            var mainWindow = new MainWindow();
            Application.Run(mainWindow);
        }
    }

    [Serializable]
    public class WebPageReference
    {
        public readonly string Url;

        public WebPageReference(string url) {
            Url = url;
        }
    }

    [Serializable]
    public abstract class SavedUrl
    {
        public WebPageReference WebPage { get; }
        public string Title { get; }

        protected SavedUrl(WebPageReference page, string title) {
            WebPage = page;
            Title = title;
        }
    }

    public class DataStorer<T>
    {
        public List<T> Items { get; private set; }
        private readonly string _fileLocation;

        public DataStorer(string fileLocation) {
            _fileLocation = fileLocation;
        }

        public List<T> LoadData() {
            try {
                TryLoadData();
            }
            catch (FileNotFoundException) {
                Items = new List<T>();
            }
            catch (SerializationException) {
                Console.WriteLine("Failed to load data of type {0}", typeof(T));
                Items = new List<T>();
            }
            return Items;
        }

        private void TryLoadData() {
            using (Stream stream = File.Open(_fileLocation, FileMode.Open)) {
                var formatter = new BinaryFormatter();
                Items = (List<T>) formatter.Deserialize(stream);
            }
        }

        public void SaveData() {
            using (Stream stream = File.Open(_fileLocation, FileMode.Create)) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, Items);
            }
        }
    }

    class Bookmarks
    {
        private readonly string _bookmarksFileLocation =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "BrowserBookmarks.dat");

        public List<Bookmark> BookmarkList { get; }
        private readonly DataStorer<Bookmark> _dataStorer;
        private static Bookmarks _instance;
        public static Bookmarks Instance => _instance ?? (_instance = new Bookmarks());

        private Bookmarks() {
            _dataStorer = new DataStorer<Bookmark>(_bookmarksFileLocation);
            BookmarkList = _dataStorer.LoadData();
        }

        public void AddBookmark(Bookmark bookmark) {
            BookmarkList.Add(bookmark);
            _dataStorer.SaveData();
        }

        public void Remove(Bookmark bookmark) {
            BookmarkList.Remove(bookmark);
            _dataStorer.SaveData();
        }
    }

    [Serializable]
    class Bookmark : SavedUrl
    {
        public Bookmark(WebPageReference page, string title) : base(page, title) {}
    }

    [Serializable]
    public class HistoryItem : SavedUrl
    {
        public DateTime Time { get; set; }

        public HistoryItem(WebPageReference page, string title) : base(page, title) {
            Time = DateTime.Now;
        }
    }
    //TODO shold these classes inherit instead of referncing
    public class GlobalHistory
    {
        private readonly string _historyFileLocation =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "BrowserHistory.dat");

        public List<HistoryItem> HistoryItems { get; }
        private static GlobalHistory _globalHistory;
        private readonly DataStorer<HistoryItem> _dataStorer;
        public static GlobalHistory Instance => _globalHistory ?? (_globalHistory = new GlobalHistory());

        private GlobalHistory() {
            _dataStorer = new DataStorer<HistoryItem>(_historyFileLocation);
            HistoryItems = _dataStorer.LoadData();
        }

        public void Add(HistoryItem item) {
            HistoryItems.Add(item);
            SaveHistory();
        }

        public void Remove(HistoryItem item) {
            HistoryItems.Remove(item);
            SaveHistory();
        }

        public void SaveHistory() {
            _dataStorer.SaveData();
        }
    }

    public class LocalHistory
    {
        public readonly Stack<HistoryItem> Back = new Stack<HistoryItem>();
        public Stack<HistoryItem> Forwards = new Stack<HistoryItem>();

        public HistoryItem GoBack(HistoryItem currentItem) {
            Forwards.Push(currentItem);
            return Back.Pop();
        }

        public HistoryItem GoForwards(HistoryItem currentItem) {
            Contract.Requires(Forwards.Count > 0);
            Back.Push(currentItem);
            return Forwards.Pop();
        }

        public void ClearForwards() {
            Forwards = new Stack<HistoryItem>();
        }

        public void AddBack(HistoryItem historyItem) {
           Back.Push(historyItem); 
        }
    }
}
