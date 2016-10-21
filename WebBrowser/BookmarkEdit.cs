using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebBrowser
{
    public partial class BookmarkEdit : Form
    {
        private readonly Bookmark _bookmark;
        private readonly DeleteBookmark _deleteBookmark;
        private readonly ReplaceBoomark _replaceBoomark;

        public delegate void DeleteBookmark();

        public delegate void ReplaceBoomark();

        public BookmarkEdit(Bookmark bookmark, DeleteBookmark deleteBookmark, ReplaceBoomark replaceBoomark) {
            InitializeComponent();
            Text = "Edit Bookmark";
            _bookmark = bookmark;
            _deleteBookmark = deleteBookmark;
            _replaceBoomark = replaceBoomark;
            txtTitle.Text = bookmark.Title;
            txtUrl.Text = bookmark.WebPage.Url;
        }
        

        private void btnConfirm_Click(object sender, EventArgs e) {
            _bookmark.Title = txtTitle.Text;
            _bookmark.WebPage = new WebPageReference(txtUrl.Text);
            Browser.Instance.Bookmarks.Save();
            _replaceBoomark();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void btnDelete_Click(object sender, EventArgs e) { 
            Browser.Instance.Bookmarks.Remove(_bookmark);
            _deleteBookmark();
            Close();
        }
    }
}
