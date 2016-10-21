using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.WinForms;

namespace WebBrowser
{
    public class MainPanelManager
    {
        private readonly List<MainPanel> _mainWindowPanelListiners = new List<MainPanel>();

        public enum DisplayPanelMode {Source, Render}

        public DisplayPanelMode ActiveDisplayPanelMode { get; private set; }
        private static MainPanelManager _mainPanelManager;

        public static MainPanelManager Instance
            => _mainPanelManager ?? (_mainPanelManager = new MainPanelManager());

        private MainPanelManager() {
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

        public void Subscribe(MainPanel panel) {
            _mainWindowPanelListiners.Add(panel);
        }
    }

    public class MainPanel
    {
        private readonly HtmlPanel _htmlPanel;
        private readonly TextBox _sourceBox;
        private string _content;
        //these keep track of what is set
        private bool _htmlSet, _sourceSet;
        public bool IsActive { get; set; }

        public MainPanel() {
            _htmlPanel = new HtmlPanel {
                Dock = DockStyle.Fill
            };
            _htmlPanel.Hide();
            Browser.Instance.MainWindow.AddContentPanel(_htmlPanel);
            _sourceBox = new TextBox {
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Multiline = true,
                WordWrap = true,
                ScrollBars = ScrollBars.Vertical
            };
            _sourceBox.Hide();
            Browser.Instance.MainWindow.AddContentPanel(_sourceBox);
            MainPanelManager.Instance.Subscribe(this);
        }

        public void SetContent(string content) {
            _content = content;
            _htmlSet = false;
            _sourceSet = false;
            if (MainPanelManager.Instance.ActiveDisplayPanelMode == MainPanelManager.DisplayPanelMode.Source) {
                _sourceBox.Text = _content;
                _sourceSet = true;
            }
            else {
                _htmlSet = true;
                _htmlPanel.Text = _content;
            }
        }

        public void ChangeMode(MainPanelManager.DisplayPanelMode mode) {
            if (mode == MainPanelManager.DisplayPanelMode.Source)
                ActivateSource();
            else
                ActivateRender();
        }

        public void Activate() {
            IsActive = true;
            switch (MainPanelManager.Instance.ActiveDisplayPanelMode) {
                case MainPanelManager.DisplayPanelMode.Render:
                    SetHtml();
                    _htmlPanel.Show();
                    break;
                case MainPanelManager.DisplayPanelMode.Source:
                    SetSource();
                    _sourceBox.Show();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetSource() {
            if (_sourceSet) return;
            _sourceBox.Text = _content;
            _sourceSet = true;
        }

        private void SetHtml() {
            if (_htmlSet) return;
            _htmlPanel.Text = _content;
            _htmlSet = true;
        }

        public void Disable() {
            IsActive = false;
            _htmlPanel.Hide();
            _sourceBox.Hide();
        }

        private void ActivateSource() {
            _htmlPanel.Hide();
            if (!IsActive) return;
            SetSource();
            _sourceBox.Show();
        }

        private void ActivateRender() {
            _sourceBox.Hide();
            if (!IsActive) return;
            SetHtml();
            _htmlPanel.Show();
        }
    }
}
