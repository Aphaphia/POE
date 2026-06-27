using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CyberSecurityGUI.Database;
using CyberSecurityGUI.Models;

namespace CyberSecurityGUI.Controllers
{
    public class ChatbotController
    {
        private readonly TaskDbContext _dbContext;
        private string _userName = "User";
        private bool _awaitingTaskReminder = false;
        private TaskItem _pendingTask = null;

        // ===== NLP: Intent Detection Keywords =====
        // Maps user intents to the keywords that trigger them
        private readonly Dictionary<string, List<string>> _intentKeywords = new Dictionary<string, List<string>>
        {
            { "add_task", new List<string> { "add task", "create task", "new task", "add to-do", "create to-do" } },
            { "view_tasks", new List<string> { "view tasks", "show tasks", "list tasks", "my tasks", "all tasks" } },
            { "complete_task", new List<string> { "complete task", "mark done", "finish task", "task done" } },
            { "delete_task", new List<string> { "delete task", "remove task", "cancel task" } },
            { "set_reminder", new List<string> { "set reminder", "add reminder", "remind me", "reminder for" } },
            { "start_quiz", new List<string> { "start quiz", "take quiz", "play quiz", "cyber quiz" } },
            { "show_log", new List<string> { "show log", "activity log", "what have you done", "recent actions" } },
            { "help", new List<string> { "help", "what can i do", "commands" } }
        };

        // ===== Cybersecurity Topics =====
        private readonly Dictionary<string, string> _topicResponses = new Dictionary<string, string>
        {
            { "password", "Use strong passwords with at least 12 characters, including uppercase, lowercase, numbers, and symbols." },
            { "phishing", "Phishing attacks trick you into revealing sensitive information. Always verify sender addresses." },
            { "2fa", "Two-factor authentication adds an extra layer of security. Use an authenticator app when possible." },
            { "ransomware", "Ransomware encrypts your files. Back up your data regularly and never pay the ransom." },
            { "vpn", "A VPN encrypts your internet traffic. Use a reputable VPN on public Wi-Fi networks." },
            { "malware", "Malware includes viruses, trojans, and spyware. Keep your antivirus software updated." },
            { "social engineering", "Social engineering manipulates people into revealing information. Always verify identities." },
            { "popia", "POPIA is South Africa's data privacy law. You have the right to know how your data is used." },
            { "encryption", "Encryption converts data into a coded format. Use end-to-end encrypted apps for sensitive communication." }
        };

        public ChatbotController(string userName = "User")
        {
            _userName = userName;
            _dbContext = new TaskDbContext();

            // Log initialization
            _dbContext.LogActivity("System Started", "Chatbot initialized");
        }

        public string UserName => _userName;

        // ===== Main Processing Method =====
        public string ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type something. I didn't receive any input.";

            string lowerInput = input.Trim().ToLower();

            // Check for help command
            if (IsIntent(lowerInput, "help"))
            {
                return GetHelpMessage();
            }

            // Check for quiz command
            if (IsIntent(lowerInput, "start_quiz"))
            {
                _dbContext.LogActivity("Quiz Started", "User requested to start the cybersecurity quiz");
                return "QUIZ_START";  // Special command for the UI
            }

            // Check for log command
            if (IsIntent(lowerInput, "show_log"))
            {
                return GetActivityLog();
            }

            // ===== Task Detection =====
            string taskIntent = DetectTaskIntent(lowerInput);

            if (taskIntent == "add_task")
            {
                string taskTitle = ExtractTaskTitle(lowerInput);
                if (!string.IsNullOrEmpty(taskTitle))
                {
                    _pendingTask = new TaskItem
                    {
                        Title = taskTitle,
                        Description = $"Task: {taskTitle}",
                        IsCompleted = false
                    };
                    _awaitingTaskReminder = true;
                    _dbContext.LogActivity("Task Created", $"New task: '{taskTitle}'");
                    return $"Task added: '{taskTitle}'. Would you like to set a reminder for this task? (Reply 'yes' or 'no')";
                }
                return "I need more details. What task would you like to add? (e.g., 'Add task - Review privacy settings')";
            }

            if (taskIntent == "view_tasks")
            {
                return GetTasksDisplay();
            }

            if (taskIntent == "complete_task")
            {
                int taskId = ExtractTaskId(lowerInput);
                if (taskId > 0)
                {
                    _dbContext.MarkTaskAsCompleted(taskId);
                    _dbContext.LogActivity("Task Completed", $"Task #{taskId} marked as completed");
                    return $"Task #{taskId} has been marked as completed! Great job staying secure!";
                }
                return "Please specify which task to complete (e.g., 'complete task 2')";
            }

            if (taskIntent == "delete_task")
            {
                int taskId = ExtractTaskId(lowerInput);
                if (taskId > 0)
                {
                    _dbContext.DeleteTask(taskId);
                    _dbContext.LogActivity("Task Deleted", $"Task #{taskId} deleted");
                    return $"Task #{taskId} has been deleted.";
                }
                return "Please specify which task to delete (e.g., 'delete task 2')";
            }

            // ===== Handle Reminder Response =====
            if (_awaitingTaskReminder && _pendingTask != null)
            {
                if (lowerInput.Contains("yes") || lowerInput.Contains("set"))
                {
                    _awaitingTaskReminder = false;
                    return "REMINDER_SET|" + _pendingTask.Title;  // Special command
                }
                else if (lowerInput.Contains("no"))
                {
                    _dbContext.AddTask(_pendingTask);
                    _awaitingTaskReminder = false;
                    string title = _pendingTask.Title;
                    _pendingTask = null;
                    return $"Task '{title}' saved without a reminder. Stay safe!";
                }
            }

            // ===== Check for Cybersecurity Topic =====
            foreach (var topic in _topicResponses)
            {
                if (lowerInput.Contains(topic.Key))
                {
                    _dbContext.LogActivity("Topic Query", $"User asked about: {topic.Key}");
                    return topic.Value + "\n\nIs there anything specific about this topic you'd like to know more about?";
                }
            }

            // ===== General Cybersecurity Question =====
            if (lowerInput.Contains("cyber") || lowerInput.Contains("security") ||
                lowerInput.Contains("safe") || lowerInput.Contains("protect"))
            {
                return "Great question! Cybersecurity is all about protecting your digital life. Here are the basics:\n" +
                       "• Use strong, unique passwords\n" +
                       "• Enable 2FA on all important accounts\n" +
                       "• Be cautious of phishing emails\n" +
                       "• Keep your software updated\n\n" +
                       "What specific topic would you like to learn about?";
            }

            // ===== Fallback Responses =====
            string[] fallbacks = {
                "I didn't quite catch that. Try saying 'help' to see what I can do!",
                "Hmm, I'm not sure about that. I can help with tasks, quizzes, or cybersecurity topics!",
                "That's interesting! Could you rephrase that? I'm here to help with cybersecurity!",
                "I'm still learning! Feel free to ask about cybersecurity topics or try 'help' for options."
            };

            Random rand = new Random();
            return fallbacks[rand.Next(fallbacks.Length)];
        }

        // ===== NLP: Intent Detection Methods =====

        // Checks if the user input matches a specific intent
        private bool IsIntent(string input, string intent)
        {
            if (!_intentKeywords.ContainsKey(intent)) return false;
            foreach (var keyword in _intentKeywords[intent])
            {
                if (input.Contains(keyword)) return true;
            }
            return false;
        }

        // Detects which task-related intent the user has
        private string DetectTaskIntent(string input)
        {
            if (IsIntent(input, "add_task")) return "add_task";
            if (IsIntent(input, "view_tasks")) return "view_tasks";
            if (IsIntent(input, "complete_task")) return "complete_task";
            if (IsIntent(input, "delete_task")) return "delete_task";
            if (IsIntent(input, "set_reminder")) return "set_reminder";
            return "";
        }

        // Extracts the task title from user input
        private string ExtractTaskTitle(string input)
        {
            // Remove common prefixes
            string cleaned = input;
            string[] prefixes = { "add task", "create task", "new task", "add to-do", "create to-do" };
            foreach (var prefix in prefixes)
            {
                if (cleaned.ToLower().Contains(prefix))
                {
                    cleaned = cleaned.Substring(cleaned.ToLower().IndexOf(prefix) + prefix.Length);
                }
            }

            // Remove dash, colon, or period
            cleaned = cleaned.TrimStart('-', ' ', ':', '.');

            // If title is too short or generic
            if (string.IsNullOrWhiteSpace(cleaned) || cleaned.Length < 3)
                return "";

            // Capitalize first letter
            return char.ToUpper(cleaned[0]) + cleaned.Substring(1).Trim();
        }

        // Extracts a task ID number from user input
        private int ExtractTaskId(string input)
        {
            var match = Regex.Match(input, @"\d+");
            if (match.Success)
                return int.Parse(match.Value);
            return -1;
        }

        // ===== Response Generators =====

        private string GetHelpMessage()
        {
            return @"I'm your Cybersecurity Assistant! Here's what I can help you with:

📋 **Tasks**:
  • 'Add task - Review privacy settings' - Create a new task
  • 'View tasks' - See all your tasks
  • 'Complete task 2' - Mark a task as done
  • 'Delete task 1' - Remove a task

🎮 **Quizzes**:
  • 'Start quiz' - Take a cybersecurity quiz

📊 **Activity Log**:
  • 'Show log' - View recent actions

💡 **Cybersecurity Topics**:
  • Ask about: passwords, phishing, 2FA, ransomware, VPNs, malware, social engineering, POPIA, encryption

What would you like to do today?";
        }

        private string GetTasksDisplay()
        {
            var tasks = _dbContext.GetIncompleteTasks();
            if (tasks.Count == 0)
                return "You have no incomplete tasks. Great job staying organized! 🎉";

            string display = "📋 **Your Cybersecurity Tasks:**\n\n";
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                display += $"{i + 1}. {task.Title}";
                if (task.ReminderDate.HasValue)
                {
                    display += $" ⏰ Reminder: {task.ReminderDate.Value:yyyy-MM-dd HH:mm}";
                }
                display += "\n";
            }
            display += "\nTip: Type 'complete task X' when you've finished a task!";
            return display;
        }

        private string GetActivityLog()
        {
            var entries = _dbContext.GetRecentActivityLog(10);
            if (entries.Count == 0)
                return "No recent activities recorded.";

            string display = "📊 **Recent Activity Log:**\n\n";
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                display += $"{i + 1}. [{entry.Timestamp:yyyy-MM-dd HH:mm}] {entry.ActionType}: {entry.Description}\n";
            }
            return display;
        }

        // ===== Reminder Handling =====
        public string SetReminderDate(string taskTitle, DateTime date)
        {
            if (_pendingTask == null)
                return "No pending task found.";

            _pendingTask.ReminderDate = date;
            _dbContext.AddTask(_pendingTask);
            string title = _pendingTask.Title;
            _dbContext.LogActivity("Reminder Set", $"Reminder for '{title}' on {date:yyyy-MM-dd HH:mm}");
            _pendingTask = null;
            return $"✅ Reminder set for '{title}' on {date:yyyy-MM-dd HH:mm}. I'll help you stay on track!";
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
