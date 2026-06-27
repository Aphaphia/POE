using System;
using System.Drawing;
using System.Windows.Forms;
using CyberSecurityGUI.Controllers;
using CyberSecurityGUI.Forms;

namespace CyberSecurityGUI
{
    public partial class MainForm : Form
    {
        private ChatbotController _chatbot;
        private FlowLayoutPanel chatFlowPanel;
        private TextBox txtUserInput;
        private Button btnSend;
        private Button btnTasks;
        private Button btnQuiz;
        private Button btnLog;
        private Label lblTitle;

        public MainForm()
        {
            InitializeComponent();
            Load += MainForm_Load;
        }

        private void InitializeComponent()
        {
            this.Text = "🛡️ CyberSecurity Awareness Bot";
            this.Size = new Size(850, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.MinimumSize = new Size(700, 500);

            // Title Label
            lblTitle = new Label
            {
                Text = "🛡️ Cybersecurity Awareness Assistant 🛡️",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 255, 200),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(20, 20, 35)
            };
            this.Controls.Add(lblTitle);

            // Button Panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(20, 20, 35)
            };
            this.Controls.Add(buttonPanel);

            // Tasks Button
            btnTasks = new Button
            {
                Text = "📋 Tasks",
                Size = new Size(100, 32),
                Location = new Point(10, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 100),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 }
            };
            btnTasks.Click += BtnTasks_Click;
            buttonPanel.Controls.Add(btnTasks);

            // Quiz Button
            btnQuiz = new Button
            {
                Text = "🎮 Quiz",
                Size = new Size(100, 32),
                Location = new Point(120, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 100, 60),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 }
            };
            btnQuiz.Click += BtnQuiz_Click;
            buttonPanel.Controls.Add(btnQuiz);

            // Log Button
            btnLog = new Button
            {
                Text = "📊 Activity Log",
                Size = new Size(120, 32),
                Location = new Point(230, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 80, 60),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 }
            };
            btnLog.Click += BtnLog_Click;
            buttonPanel.Controls.Add(btnLog);

            // Help Text
            Label lblHelp = new Label
            {
                Text = "Type 'help' for commands",
                ForeColor = Color.FromArgb(150, 150, 180),
                Font = new Font("Segoe UI", 9),
                Location = new Point(360, 12),
                AutoSize = true
            };
            buttonPanel.Controls.Add(lblHelp);

            // Chat Display Panel
            chatFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 46),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(10, 10, 10, 10)
            };
            this.Controls.Add(chatFlowPanel);

            // Input Panel
            Panel inputPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(20, 20, 35)
            };
            this.Controls.Add(inputPanel);

            // Text Input
            txtUserInput = new TextBox
            {
                Location = new Point(10, 12),
                Size = new Size(700, 30),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtUserInput.KeyPress += TxtUserInput_KeyPress;
            inputPanel.Controls.Add(txtUserInput);

            // Send Button
            btnSend = new Button
            {
                Text = "Send ✉",
                Location = new Point(720, 12),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 150, 200),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
            btnSend.Click += BtnSend_Click;
            inputPanel.Controls.Add(btnSend);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _chatbot = new ChatbotController("User");

            // Welcome messages
            AddMessage("🤖 CyberBot",
                "Welcome to the Cybersecurity Awareness Assistant! I'm here to help you stay safe online.", true);
            AddMessage("🤖 CyberBot",
                "You can:\n• Ask cybersecurity questions\n• Add/view tasks\n• Take a quiz\n• Type 'help' to see all commands", true);
            AddMessage("🤖 CyberBot",
                "What would you like to do today?", true);

            txtUserInput.Focus();
        }

        // ===== UI Helper Methods =====

        private void AddMessage(string sender, string message, bool isBot)
        {
            // Create panel for each message
            Panel messagePanel = new Panel
            {
                Width = chatFlowPanel.Width - 30,
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 5),
                Padding = new Padding(5, 5, 5, 5)
            };

            // Sender label
            Label lblSender = new Label
            {
                Text = sender,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = isBot ? Color.FromArgb(0, 255, 200) : Color.FromArgb(255, 200, 0),
                AutoSize = true,
                Dock = DockStyle.Top
            };
            messagePanel.Controls.Add(lblSender);

            // Message text
            RichTextBox rtbMessage = new RichTextBox
            {
                Text = message,
                Font = new Font("Segoe UI", 10),
                BackColor = isBot ? Color.FromArgb(40, 40, 60) : Color.FromArgb(30, 30, 50),
                ForeColor = Color.White,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                AutoSize = false,
                Width = messagePanel.Width - 10,
                Height = 20
            };

            // Calculate height based on text
            Size textSize = TextRenderer.MeasureText(message, rtbMessage.Font,
                new Size(rtbMessage.Width - 10, int.MaxValue), TextFormatFlags.WordBreak);
            rtbMessage.Height = textSize.Height + 10;

            messagePanel.Controls.Add(rtbMessage);
            messagePanel.Height = rtbMessage.Height + 30;

            // Add to chat panel
            chatFlowPanel.Controls.Add(messagePanel);

            // Scroll to bottom
            chatFlowPanel.ScrollControlIntoView(messagePanel);
            Application.DoEvents();
        }

        private void ProcessUserInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            // Show user message
            AddMessage("👤 You", input, false);

            // Process with chatbot
            string response = _chatbot.ProcessInput(input);

            // Check for special commands
            if (response == "QUIZ_START")
            {
                QuizForm quizForm = new QuizForm(_chatbot.UserName);
                quizForm.ShowDialog();
                AddMessage("🤖 CyberBot", "Quiz completed! How did you do?", true);
            }
            else if (response.StartsWith("REMINDER_SET|"))
            {
                string taskTitle = response.Substring("REMINDER_SET|".Length);
                // Show reminder date picker dialog
                using (var dialog = new Form())
                {
                    dialog.Text = "Set Reminder";
                    dialog.Size = new Size(300, 150);
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    dialog.BackColor = Color.FromArgb(30, 30, 46);

                    DateTimePicker dtp = new DateTimePicker
                    {
                        Location = new Point(20, 20),
                        Size = new Size(240, 25),
                        Format = DateTimePickerFormat.Custom,
                        CustomFormat = "yyyy-MM-dd HH:mm"
                    };
                    dialog.Controls.Add(dtp);

                    Button btnSet = new Button
                    {
                        Text = "Set Reminder",
                        Location = new Point(20, 60),
                        Size = new Size(100, 30),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(0, 150, 200),
                        ForeColor = Color.White
                    };
                    btnSet.Click += (s, e2) => {
                        string result = _chatbot.SetReminderDate(taskTitle, dtp.Value);
                        AddMessage("🤖 CyberBot", result, true);
                        dialog.Close();
                    };
                    dialog.Controls.Add(btnSet);

                    Button btnCancel = new Button
                    {
                        Text = "Skip",
                        Location = new Point(140, 60),
                        Size = new Size(100, 30),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(80, 80, 100),
                        ForeColor = Color.White
                    };
                    btnCancel.Click += (s, e2) => {
                        string result = _chatbot.SetReminderDate(taskTitle, DateTime.Now);
                        AddMessage("🤖 CyberBot", $"Task '{taskTitle}' saved without a reminder.", true);
                        dialog.Close();
                    };
                    dialog.Controls.Add(btnCancel);

                    dialog.ShowDialog();
                }
            }
            else
            {
                AddMessage("🤖 CyberBot", response, true);
            }

            txtUserInput.Clear();
            txtUserInput.Focus();
        }

        // ===== Event Handlers =====

        private void BtnSend_Click(object sender, EventArgs e)
        {
            ProcessUserInput(txtUserInput.Text);
        }

        private void TxtUserInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcessUserInput(txtUserInput.Text);
            }
        }

        private void BtnTasks_Click(object sender, EventArgs e)
        {
            TaskManagerForm taskForm = new TaskManagerForm(_chatbot);
            taskForm.ShowDialog();
            // Refresh chat with updated tasks
            AddMessage("🤖 CyberBot", "Tasks updated! Type 'view tasks' to see your list.", true);
        }

        private void BtnQuiz_Click(object sender, EventArgs e)
        {
            QuizForm quizForm = new QuizForm(_chatbot.UserName);
            quizForm.ShowDialog();
            AddMessage("🤖 CyberBot", "Thanks for playing the cybersecurity quiz! Every bit of knowledge helps keep you safe online.", true);
        }

        private void BtnLog_Click(object sender, EventArgs e)
        {
            ActivityLogForm logForm = new ActivityLogForm();
            logForm.ShowDialog();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _chatbot?.Dispose();
            base.OnFormClosing(e);
        }
    }
}