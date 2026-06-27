using System;
using System.Drawing;
using System.Windows.Forms;
using CyberSecurityGUI.Database;
using CyberSecurityGUI.Models;

namespace CyberSecurityGUI.Forms
{
    public partial class ActivityLogForm : Form
    {
        private ListBox lstLog;
        private Button btnRefresh;
        private Label lblTitle;
        private NumericUpDown nudCount;
        private System.ComponentModel.IContainer components = null;

        public ActivityLogForm()
        {
            InitializeComponent();
            LoadLog();
        }

        private void InitializeComponent()
        {
            this.Text = "📊 Activity Log";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.MinimumSize = new Size(400, 300);

            lblTitle = new Label
            {
                Text = "📊 Recent Activity Log",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 255, 200),
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            lstLog = new ListBox
            {
                Location = new Point(20, 60),
                Size = new Size(540, 340),
                BackColor = Color.FromArgb(40, 40, 60),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(lstLog);

            Label lblCount = new Label
            {
                Text = "Show:",
                ForeColor = Color.White,
                Location = new Point(20, 415),
                AutoSize = true
            };
            this.Controls.Add(lblCount);

            nudCount = new NumericUpDown
            {
                Location = new Point(70, 412),
                Size = new Size(60, 25),
                Minimum = 5,
                Maximum = 50,
                Value = 10,
                BackColor = Color.FromArgb(40, 40, 60),
                ForeColor = Color.White
            };
            nudCount.ValueChanged += (s, e) => LoadLog();
            this.Controls.Add(nudCount);

            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(150, 410),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
            btnRefresh.Click += (s, e) => LoadLog();
            this.Controls.Add(btnRefresh);

            Button btnClear = new Button
            {
                Text = "🗑 Clear Display",
                Location = new Point(270, 410),
                Size = new Size(130, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 60, 60),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 }
            };
            btnClear.Click += (s, e) => {
                lstLog.Items.Clear();
                lstLog.Items.Add("Display cleared. Click Refresh to reload.");
            };
            this.Controls.Add(btnClear);

            Button btnClose = new Button
            {
                Text = "✕ Close",
                Location = new Point(480, 410),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 80),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 }
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadLog()
        {
            lstLog.Items.Clear();
            using (var db = new TaskDbContext())
            {
                var entries = db.GetRecentActivityLog((int)nudCount.Value);
                if (entries.Count == 0)
                {
                    lstLog.Items.Add("No activity logged yet.");
                }
                else
                {
                    lstLog.Items.Add("Time".PadRight(22) + "| Action".PadRight(20) + "| Description");
                    lstLog.Items.Add("".PadRight(60, '-'));

                    foreach (var entry in entries)
                    {
                        string time = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                        string action = entry.ActionType.PadRight(20);
                        if (action.Length > 20) action = action.Substring(0, 17) + "...";
                        string desc = entry.Description;
                        if (desc.Length > 30) desc = desc.Substring(0, 27) + "...";
                        lstLog.Items.Add($"{time} | {action} | {desc}");
                    }
                }
            }
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}