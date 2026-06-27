using System;
using System.Drawing;
using System.Windows.Forms;
using CyberSecurityGUI.Controllers;
using CyberSecurityGUI.Models;
using CyberSecurityGUI.Database;

namespace CyberSecurityGUI.Forms
{
    public partial class TaskManagerForm : Form
    {
        private ChatbotController _chatbot;
        private ListBox lstTasks;
        private Button btnAdd;
        private Button btnComplete;
        private Button btnDelete;
        private Button btnRefresh;
        private Label lblTitle;
        private Label lblStatus;
        private System.ComponentModel.IContainer components = null;

        public TaskManagerForm(ChatbotController chatbot)
        {
            _chatbot = chatbot;
            InitializeComponent();
            LoadTasks();
        }

        private void InitializeComponent()
        {
            this.Text = "📋 Task Manager";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.MinimumSize = new Size(400, 300);

            lblTitle = new Label
            {
                Text = "📋 Cybersecurity Tasks",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 255, 200),
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            lblStatus = new Label
            {
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(150, 150, 180),
                Location = new Point(20, 45),
                AutoSize = true
            };
            this.Controls.Add(lblStatus);

            lstTasks = new ListBox
            {
                Location = new Point(20, 70),
                Size = new Size(440, 220),
                BackColor = Color.FromArgb(40, 40, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(lstTasks);

            btnAdd = new Button
            {
                Text = "➕ Add Task",
                Location = new Point(20, 310),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 150, 200),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnComplete = new Button
            {
                Text = "✅ Complete",
                Location = new Point(130, 310),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 160, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
            btnComplete.Click += BtnComplete_Click;
            this.Controls.Add(btnComplete);

            btnDelete = new Button
            {
                Text = "🗑 Delete",
                Location = new Point(240, 310),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(160, 60, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(350, 310),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
            btnRefresh.Click += (s, e) => LoadTasks();
            this.Controls.Add(btnRefresh);

            Button btnClose = new Button
            {
                Text = "✕ Close",
                Location = new Point(380, 310),
                Size = new Size(80, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                FlatAppearance = { BorderSize = 0 }
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadTasks()
        {
            using (var db = new TaskDbContext())
            {
                var tasks = db.GetAllTasks();
                lstTasks.Items.Clear();

                if (tasks.Count == 0)
                {
                    lstTasks.Items.Add("No tasks found. Add one to get started!");
                    lstTasks.Enabled = false;
                    btnComplete.Enabled = false;
                    btnDelete.Enabled = false;
                    lblStatus.Text = "No tasks available";
                }
                else
                {
                    lstTasks.Enabled = true;
                    btnComplete.Enabled = true;
                    btnDelete.Enabled = true;

                    int completeCount = 0;
                    foreach (var task in tasks)
                    {
                        string display = task.ToString();
                        lstTasks.Items.Add(display);
                        if (task.IsCompleted) completeCount++;
                    }

                    lblStatus.Text = $"Tasks: {tasks.Count} total, {completeCount} completed, {tasks.Count - completeCount} pending";
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Add New Task";
                dialog.Size = new Size(400, 200);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.BackColor = Color.FromArgb(30, 30, 46);

                Label lblTaskName = new Label
                {
                    Text = "Task Title:",
                    ForeColor = Color.White,
                    Location = new Point(20, 20),
                    AutoSize = true
                };
                dialog.Controls.Add(lblTaskName);

                TextBox txtTitle = new TextBox
                {
                    Location = new Point(20, 45),
                    Size = new Size(340, 25),
                    BackColor = Color.FromArgb(40, 40, 60),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                dialog.Controls.Add(txtTitle);

                Label lblDesc = new Label
                {
                    Text = "Description (optional):",
                    ForeColor = Color.White,
                    Location = new Point(20, 75),
                    AutoSize = true
                };
                dialog.Controls.Add(lblDesc);

                TextBox txtDesc = new TextBox
                {
                    Location = new Point(20, 100),
                    Size = new Size(340, 25),
                    BackColor = Color.FromArgb(40, 40, 60),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                dialog.Controls.Add(txtDesc);

                Button btnSave = new Button
                {
                    Text = "💾 Save Task",
                    Location = new Point(20, 135),
                    Size = new Size(160, 30),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(0, 150, 200),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    FlatAppearance = { BorderSize = 0 }
                };
                btnSave.Click += (s2, e2) => {
                    if (string.IsNullOrWhiteSpace(txtTitle.Text))
                    {
                        MessageBox.Show("Please enter a task title.", "Missing Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    using (var db = new TaskDbContext())
                    {
                        db.AddTask(new TaskItem
                        {
                            Title = txtTitle.Text.Trim(),
                            Description = txtDesc.Text.Trim()
                        });
                    }

                    MessageBox.Show($"Task '{txtTitle.Text}' added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dialog.Close();
                    LoadTasks();
                };
                dialog.Controls.Add(btnSave);

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(200, 135),
                    Size = new Size(100, 30),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(80, 80, 100),
                    ForeColor = Color.White,
                    FlatAppearance = { BorderSize = 0 }
                };
                btnCancel.Click += (s2, e2) => dialog.Close();
                dialog.Controls.Add(btnCancel);

                dialog.ShowDialog();
            }
        }

        private void BtnComplete_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a task to complete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var db = new TaskDbContext())
            {
                var tasks = db.GetAllTasks();
                if (lstTasks.SelectedIndex < tasks.Count)
                {
                    var task = tasks[lstTasks.SelectedIndex];
                    if (task.IsCompleted)
                    {
                        MessageBox.Show("This task is already completed!", "Already Done",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    db.MarkTaskAsCompleted(task.TaskID);
                    MessageBox.Show($"Task '{task.Title}' marked as completed! 🎉", "Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTasks();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a task to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var db = new TaskDbContext())
            {
                var tasks = db.GetAllTasks();
                if (lstTasks.SelectedIndex < tasks.Count)
                {
                    var task = tasks[lstTasks.SelectedIndex];
                    var result = MessageBox.Show($"Are you sure you want to delete '{task.Title}'?",
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        db.DeleteTask(task.TaskID);
                        MessageBox.Show($"Task '{task.Title}' deleted.", "Deleted",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadTasks();
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