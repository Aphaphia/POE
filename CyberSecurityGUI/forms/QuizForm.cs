using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CyberSecurityGUI.Data;
using CyberSecurityGUI.Models;
using CyberSecurityGUI.Database;

namespace CyberSecurityGUI.Forms
{
    public partial class QuizForm : Form
    {
        private List<QuizQuestion> _questions;
        private int _currentQuestionIndex = 0;
        private int _score = 0;
        private string _userName;
        private Label lblQuestion;
        private RadioButton[] radioOptions;
        private Button btnSubmit;
        private Button btnNext;
        private Label lblScore;
        private Label lblProgress;
        private Panel questionPanel;
        private RichTextBox rtbExplanation;
        private bool _answered = false;

        public QuizForm(string userName)
        {
            _userName = userName;
            _questions = QuizData.GetQuestions();
            InitializeComponent();
            LoadQuestion();
        }

        private void InitializeComponent()
        {
            this.Text = "🎮 Cybersecurity Quiz";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.MinimumSize = new Size(500, 400);

            // Progress Label
            lblProgress = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(0, 255, 200),
                Location = new Point(20, 15)
            };
            this.Controls.Add(lblProgress);

            // Score Label
            lblScore = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 0),
                Location = new Point(20, 45)
            };
            this.Controls.Add(lblScore);

            // Question Panel
            questionPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(540, 300),
                BackColor = Color.FromArgb(40, 40, 60)
            };
            this.Controls.Add(questionPanel);

            // Question Label
            lblQuestion = new Label
            {
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 10),
                Size = new Size(520, 50),
                AutoSize = false
            };
            questionPanel.Controls.Add(lblQuestion);

            // Radio buttons for options
            radioOptions = new RadioButton[4];
            for (int i = 0; i < 4; i++)
            {
                radioOptions[i] = new RadioButton
                {
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.White,
                    Location = new Point(20, 60 + (i * 45)),
                    Size = new Size(500, 30),
                    AutoSize = false,
                    Enabled = true
                };
                questionPanel.Controls.Add(radioOptions[i]);
            }

            // Explanation
            rtbExplanation = new RichTextBox
            {
                Location = new Point(20, 250),
                Size = new Size(500, 40),
                BackColor = Color.FromArgb(50, 50, 70),
                ForeColor = Color.FromArgb(200, 200, 200),
                Font = new Font("Segoe UI", 9),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Visible = false
            };
            questionPanel.Controls.Add(rtbExplanation);

            // Submit Button
            btnSubmit = new Button
            {
                Text = "Submit Answer",
                Location = new Point(20, 390),
                Size = new Size(120, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 150, 200),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
            btnSubmit.Click += BtnSubmit_Click;
            this.Controls.Add(btnSubmit);

            // Next Button
            btnNext = new Button
            {
                Text = "Next Question ➜",
                Location = new Point(160, 390),
                Size = new Size(130, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 160, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 },
                Enabled = false
            };
            btnNext.Click += BtnNext_Click;
            this.Controls.Add(btnNext);

            // Close Button
            Button btnClose = new Button
            {
                Text = "✕ Close",
                Location = new Point(460, 390),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 60, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadQuestion()
        {
            if (_currentQuestionIndex >= _questions.Count)
            {
                ShowResults();
                return;
            }

            var q = _questions[_currentQuestionIndex];

            lblProgress.Text = $"Question {_currentQuestionIndex + 1} of {_questions.Count}";
            lblScore.Text = $"Score: {_score}/{_currentQuestionIndex}";

            lblQuestion.Text = $"[{q.Category}] {q.Question}";

            for (int i = 0; i < radioOptions.Length; i++)
            {
                if (i < q.Options.Count)
                {
                    radioOptions[i].Text = q.Options[i];
                    radioOptions[i].Visible = true;
                    radioOptions[i].Checked = false;
                    radioOptions[i].Enabled = true;
                }
                else
                {
                    radioOptions[i].Visible = false;
                }
            }

            rtbExplanation.Visible = false;
            btnSubmit.Enabled = true;
            btnNext.Enabled = false;
            _answered = false;

            foreach (var rb in radioOptions)
            {
                rb.ForeColor = Color.White;
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (_answered) return;

            int selectedIndex = -1;
            for (int i = 0; i < radioOptions.Length; i++)
            {
                if (radioOptions[i].Checked)
                {
                    selectedIndex = i;
                    break;
                }
            }

            if (selectedIndex == -1)
            {
                MessageBox.Show("Please select an answer!", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var q = _questions[_currentQuestionIndex];
            bool isCorrect = (selectedIndex == q.CorrectAnswerIndex);

            if (isCorrect)
            {
                _score++;
                radioOptions[selectedIndex].ForeColor = Color.LightGreen;
            }
            else
            {
                radioOptions[selectedIndex].ForeColor = Color.LightCoral;
                radioOptions[q.CorrectAnswerIndex].ForeColor = Color.LightGreen;
            }

            rtbExplanation.Text = (isCorrect ? "✅ Correct! " : "❌ Incorrect. ") + q.Explanation;
            rtbExplanation.Visible = true;
            rtbExplanation.Height = 50;

            btnSubmit.Enabled = false;
            foreach (var rb in radioOptions)
            {
                rb.Enabled = false;
            }

            _answered = true;
            btnNext.Enabled = true;
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            _currentQuestionIndex++;
            LoadQuestion();
        }

        private void ShowResults()
        {
            using (var db = new TaskDbContext())
            {
                db.SaveQuizResult(_userName, _score, _questions.Count);
            }

            foreach (Control ctrl in this.Controls)
            {
                ctrl.Visible = false;
            }

            Label lblResult = new Label
            {
                Text = "🎉 Quiz Complete! 🎉",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 255, 200),
                Location = new Point(150, 80),
                AutoSize = true
            };
            this.Controls.Add(lblResult);

            Label lblScoreFinal = new Label
            {
                Text = $"Your Score: {_score} out of {_questions.Count}",
                Font = new Font("Segoe UI", 16),
                ForeColor = Color.White,
                Location = new Point(180, 150),
                AutoSize = true
            };
            this.Controls.Add(lblScoreFinal);

            double percentage = (double)_score / _questions.Count * 100;
            string feedback = "";
            if (percentage >= 90) feedback = "🌟 Excellent! You're a cybersecurity pro!";
            else if (percentage >= 70) feedback = "👏 Great job! Keep learning to stay safe!";
            else if (percentage >= 50) feedback = "📚 Good effort! Review the topics you missed.";
            else feedback = "💪 Keep going! Cybersecurity is important - review the basics!";

            Label lblFeedback = new Label
            {
                Text = feedback,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(80, 200),
                Size = new Size(440, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            this.Controls.Add(lblFeedback);

            Button btnClose = new Button
            {
                Text = "✕ Close",
                Location = new Point(250, 350),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 60, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }
    }
}