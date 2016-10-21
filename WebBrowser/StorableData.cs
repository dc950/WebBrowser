using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace WebBrowser
{
    [Serializable]
    public class WebPageReference
    {
        public readonly string Url;

        public WebPageReference(string url)
        {
            Url = url;
        }
    }

    class StorableCollection<T>
    {
        public List<T> Collection { get; }
        private readonly DataStorer<List<T>> _dataStorer;

        public StorableCollection(string fileLoaction)
        {
            _dataStorer = new DataStorer<List<T>>(fileLoaction);
            Collection = _dataStorer.LoadData();
            if (Collection != null) return;
            Collection = new List<T>();
            _dataStorer.SetNewItem(Collection);
        }

        public void AddItem(T item)
        {
            Collection.Add(item);
            _dataStorer.SaveData();
        }

        public void Remove(T item)
        {
            Collection.Remove(item);
            _dataStorer.SaveData();
        }

        public void Save()
        {
            _dataStorer.SaveData();
        }
    }

    public class DataStorer<T>
    {
        public T Item { get; private set; }
        private readonly string _fileLocation;
        public DataStorer(string fileLocation)
        {
            _fileLocation = fileLocation;
        }

        public T LoadData()
        {
            try
            {
                TryLoadData();
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
            catch (SerializationException)
            {
                Console.WriteLine("Failed to load data of type {0}", typeof(T));
                return default(T);
            }
            return Item;
        }

        private void TryLoadData()
        {
            using (Stream stream = File.Open(_fileLocation, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                Item = (T)formatter.Deserialize(stream);
            }
        }

        public void SaveData()
        {
            using (Stream stream = File.Open(_fileLocation, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, Item);
            }
        }

        public void SetNewItem(T item)
        {
            Item = item;
            SaveData();
        }
    }

    [Serializable]
    public abstract class SavedUrl
    {
        public WebPageReference WebPage { get; set; }
        public string Title { get; set; }

        protected SavedUrl(WebPageReference page, string title)
        {
            WebPage = page;
            Title = title;
        }
    }

    [Serializable]
    public class HomePage : SavedUrl
    {
        private HomePage(WebPageReference page, string title) : base(page, title)
        { }

        private static HomePage _homePage;
        private static DataStorer<HomePage> _dataStorer;

        public static HomePage LoadHomePage(string fileLocation)
        {
            if (_homePage != null) return _homePage;
            _dataStorer = new DataStorer<HomePage>(fileLocation);
            _homePage = _dataStorer.LoadData();
            return _homePage;
        }

        public static HomePage CreateHomePage(WebPageReference page, string title)
        {
            if(_dataStorer == null)
                throw new Exception("Must attempt to load home page before creating one");
            _homePage = new HomePage(page, title);
            _dataStorer.SetNewItem(_homePage);
            _dataStorer.SaveData();
            return _homePage;
        }

        public void SetHomePage(WebPageReference page, string title)
        {
            WebPage = page;
            Title = title;
            _dataStorer.SaveData();
        }
    }

    [Serializable]
    public class Bookmark : SavedUrl
    {
        public Bookmark(WebPageReference page, string title) : base(page, title) { }
    }

    [Serializable]
    public class HistoryItem : SavedUrl
    {
        public DateTime Time { get; set; }

        public HistoryItem(WebPageReference page, string title) : base(page, title)
        {
            Time = DateTime.Now;
        }
    }

    public class LocalHistory
    {
        public readonly Stack<HistoryItem> Back = new Stack<HistoryItem>();
        public Stack<HistoryItem> Forwards = new Stack<HistoryItem>();

        public HistoryItem GoBack(HistoryItem currentItem)
        {
            Contract.Requires(Back.Count > 0);
            Forwards.Push(currentItem);
            return Back.Pop();
        }

        public HistoryItem GoForwards(HistoryItem currentItem)
        {
            Contract.Requires(Forwards.Count > 0);
            Back.Push(currentItem);
            return Forwards.Pop();
        }

        public void ClearForwards()
        {
            Forwards = new Stack<HistoryItem>();
        }

        public void AddBack(HistoryItem historyItem)
        {
            Back.Push(historyItem);
        }
    }
}
