using System.Media;
using System.Windows.Forms;

namespace WebBrowser
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UrlBar = new System.Windows.Forms.TextBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnForwards = new System.Windows.Forms.Button();
            this.NewTab = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.TabPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnFavourite = new System.Windows.Forms.Button();
            this.btnHistory = new System.Windows.Forms.Button();
            this.btnFavourites = new System.Windows.Forms.Button();
            this.btnToggleHtml = new System.Windows.Forms.Button();
            this.TabPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UrlBar
            // 
            this.UrlBar.AcceptsReturn = true;
            this.UrlBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UrlBar.Location = new System.Drawing.Point(95, 8);
            this.UrlBar.Name = "UrlBar";
            this.UrlBar.Size = new System.Drawing.Size(446, 26);
            this.UrlBar.TabIndex = 1;
            this.UrlBar.TextChanged += new System.EventHandler(this.UrlBar_TextChanged);
            this.UrlBar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CheckEnterKeyPress);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(3, 8);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(40, 35);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "<-";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnForwards
            // 
            this.btnForwards.Location = new System.Drawing.Point(49, 8);
            this.btnForwards.Name = "btnForwards";
            this.btnForwards.Size = new System.Drawing.Size(40, 35);
            this.btnForwards.TabIndex = 4;
            this.btnForwards.Text = "->";
            this.btnForwards.UseVisualStyleBackColor = true;
            this.btnForwards.Click += new System.EventHandler(this.btnForwards_Click);
            // 
            // NewTab
            // 
            this.NewTab.AutoSize = true;
            this.NewTab.Location = new System.Drawing.Point(3, 3);
            this.NewTab.Name = "NewTab";
            this.NewTab.Size = new System.Drawing.Size(44, 46);
            this.NewTab.TabIndex = 0;
            this.NewTab.Text = "+";
            this.NewTab.UseVisualStyleBackColor = true;
            this.NewTab.Click += new System.EventHandler(this.NewTab_Click);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.Location = new System.Drawing.Point(547, 8);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(50, 35);
            this.btnGo.TabIndex = 7;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // TabPanel
            // 
            this.TabPanel.Controls.Add(this.NewTab);
            this.TabPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabPanel.Location = new System.Drawing.Point(3, 269);
            this.TabPanel.Name = "TabPanel";
            this.TabPanel.Size = new System.Drawing.Size(1906, 219);
            this.TabPanel.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TabPanel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 54.21687F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.78313F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 546F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1912, 1038);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnBack);
            this.flowLayoutPanel1.Controls.Add(this.btnForwards);
            this.flowLayoutPanel1.Controls.Add(this.UrlBar);
            this.flowLayoutPanel1.Controls.Add(this.btnGo);
            this.flowLayoutPanel1.Controls.Add(this.btnRefresh);
            this.flowLayoutPanel1.Controls.Add(this.btnFavourite);
            this.flowLayoutPanel1.Controls.Add(this.btnHistory);
            this.flowLayoutPanel1.Controls.Add(this.btnFavourites);
            this.flowLayoutPanel1.Controls.Add(this.btnToggleHtml);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1906, 260);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(604, 10);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(87, 35);
            this.btnRefresh.TabIndex = 11;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnFavourite
            // 
            this.btnFavourite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFavourite.Location = new System.Drawing.Point(698, 8);
            this.btnFavourite.Name = "btnFavourite";
            this.btnFavourite.Size = new System.Drawing.Size(152, 35);
            this.btnFavourite.TabIndex = 9;
            this.btnFavourite.Text = "Add to Favourites";
            this.btnFavourite.UseVisualStyleBackColor = true;
            this.btnFavourite.Click += new System.EventHandler(this.btnFavourite_Click);
            // 
            // btnHistory
            // 
            this.btnHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHistory.Location = new System.Drawing.Point(856, 8);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(75, 35);
            this.btnHistory.TabIndex = 8;
            this.btnHistory.Text = "History";
            this.btnHistory.UseVisualStyleBackColor = true;
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // btnFavourites
            // 
            this.btnFavourites.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFavourites.Location = new System.Drawing.Point(937, 8);
            this.btnFavourites.Name = "btnFavourites";
            this.btnFavourites.Size = new System.Drawing.Size(98, 35);
            this.btnFavourites.TabIndex = 10;
            this.btnFavourites.Text = "Favourites";
            this.btnFavourites.UseVisualStyleBackColor = true;
            this.btnFavourites.Click += new System.EventHandler(this.btnFavourites_Click);
            // 
            // btnToggleHtml
            // 
            this.btnToggleHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToggleHtml.Location = new System.Drawing.Point(1042, 10);
            this.btnToggleHtml.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnToggleHtml.Name = "btnToggleHtml";
            this.btnToggleHtml.Size = new System.Drawing.Size(184, 34);
            this.btnToggleHtml.TabIndex = 12;
            this.btnToggleHtml.Text = "Show Html";
            this.btnToggleHtml.UseVisualStyleBackColor = true;
            this.btnToggleHtml.Click += new System.EventHandler(this.btnToggleHtml_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1912, 1038);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.TabPanel.ResumeLayout(false);
            this.TabPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox UrlBar;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnForwards;
        private System.Windows.Forms.Button NewTab;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.FlowLayoutPanel TabPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.Button btnFavourite;
        private System.Windows.Forms.Button btnFavourites;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnToggleHtml;
    }
}
