using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WebBrowser
{
    class SidePanel
    {
        private readonly Form _mainForm;
        private enum SidePanelStatus {Bookmarks, History, None}
        private TableLayoutPanel _sidePanel;
        private SidePanelStatus _sidePanelStatus = SidePanelStatus.None;

        public SidePanel(Form mainForm) {
            _mainForm = mainForm;
        }
        
        private void ShowBookMarkPanel() {
            var bookmarks = Browser.Instance.Bookmarks.Collection;
            _sidePanelStatus = SidePanelStatus.Bookmarks;
            ShowSidePanel(bookmarks.Cast<SavedUrl>().ToList());
        }

        private void ShowHistoryPanel() {
            var historyItems = Browser.Instance.History.Collection;
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

            _mainForm.Controls.Add(table); //add panel to form
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
                var innerPannel = SetupPanelButton(item, panel);
                panel.Controls.Add(innerPannel);
            }
            return panel;
        }

        private FlowLayoutPanel SetupPanelButton(SavedUrl item, FlowLayoutPanel panel) {
            var innerPannel = new FlowLayoutPanel {
                FlowDirection = FlowDirection.LeftToRight,
                Width = 300
            };
            var button = SetupSidePanelContentButton(item);
            innerPannel.Height = button.Height + 2;
            var editButton = new Button();
            //Add delete button for history and edit button for bookmark
            if (item is HistoryItem) {
                editButton.Text = "X";
                editButton.Click += delegate {
                    Browser.Instance.History.Remove(item as HistoryItem);
                    panel.Controls.Remove(innerPannel);
                };
            }
            else if (item is Bookmark) {
                editButton.Text = "Edit";
                editButton.Click += delegate {
                    var bookmarkEdit = new BookmarkEdit(item as Bookmark,
                        delegate {
                            Browser.Instance.Bookmarks.Remove(item as Bookmark);
                            panel.Controls.Remove(innerPannel);
                        },
                        delegate {
                            innerPannel.Controls.Remove(button);
                            button = SetupSidePanelContentButton(item);
                            innerPannel.Controls.Add(button);
                            innerPannel.Controls.SetChildIndex(button, 0);//set to right position - no Insert() for controls
                        });
                    bookmarkEdit.Show();
                };
            }
            innerPannel.Controls.Add(button);
            innerPannel.Controls.Add(editButton);
            return innerPannel;
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
            _mainForm.Controls.Remove(_sidePanel);
            _sidePanel = null;
            _sidePanelStatus = SidePanelStatus.None;
        }

        public void FavouritesClicked() {
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

        public void HistoryClicked() {
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
    }
}
