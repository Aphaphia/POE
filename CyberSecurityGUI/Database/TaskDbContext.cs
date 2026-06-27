using System;
using System.Collections.Generic;
using MySqlConnector;
using CyberSecurityGUI.Models;

namespace CyberSecurityGUI.Database
{
    public class TaskDbContext : IDisposable
    {
        // Store the connection string - UPDATE WITH YOUR PASSWORD
        private readonly string _connectionString;
        private MySqlConnection? _connection;

        // Constructor - sets up the connection details
        public TaskDbContext(string connectionString = null)
        {
            // IMPORTANT: Replace "AmoAtli@03" with your actual MySQL password
            _connectionString = connectionString ??
                "Server=localhost;Database=CyberSecurityChatbot;User ID=root;Password=AmoAtli@03;";
        }

        // Gets an open connection to the database
        private MySqlConnection GetConnection()
        {
            if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
            {
                _connection = new MySqlConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }

        // ============ TASK OPERATIONS ============

        // Adds a new task to the database
        public void AddTask(TaskItem task)
        {
            using var connection = GetConnection();
            string query = @"INSERT INTO Tasks (Title, Description, ReminderDate, IsCompleted) 
                            VALUES (@Title, @Description, @ReminderDate, @IsCompleted)";
            using var cmd = new MySqlCommand(query, connection);

            // Parameters prevent SQL injection attacks
            cmd.Parameters.AddWithValue("@Title", task.Title);
            cmd.Parameters.AddWithValue("@Description", task.Description);
            cmd.Parameters.AddWithValue("@ReminderDate", task.ReminderDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);

            cmd.ExecuteNonQuery();  // Executes the INSERT command

            // Log this action
            LogActivity("Task Added", $"Task: '{task.Title}'");
        }

        // Gets all tasks from the database
        public List<TaskItem> GetAllTasks()
        {
            var tasks = new List<TaskItem>();
            using var connection = GetConnection();
            string query = "SELECT * FROM Tasks ORDER BY CreatedAt DESC";
            using var cmd = new MySqlCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            // Loop through each row in the result
            while (reader.Read())
            {
                tasks.Add(new TaskItem
                {
                    TaskID = reader.GetInt32("TaskID"),
                    Title = reader.GetString("Title"),
                    Description = reader.GetString("Description"),
                    ReminderDate = reader.IsDBNull("ReminderDate") ? null : reader.GetDateTime("ReminderDate"),
                    IsCompleted = reader.GetBoolean("IsCompleted"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }
            return tasks;
        }

        // Gets only tasks that are NOT completed
        public List<TaskItem> GetIncompleteTasks()
        {
            var tasks = new List<TaskItem>();
            using var connection = GetConnection();
            string query = "SELECT * FROM Tasks WHERE IsCompleted = FALSE ORDER BY ReminderDate ASC";
            using var cmd = new MySqlCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                tasks.Add(new TaskItem
                {
                    TaskID = reader.GetInt32("TaskID"),
                    Title = reader.GetString("Title"),
                    Description = reader.GetString("Description"),
                    ReminderDate = reader.IsDBNull("ReminderDate") ? null : reader.GetDateTime("ReminderDate"),
                    IsCompleted = reader.GetBoolean("IsCompleted"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }
            return tasks;
        }

        // Updates an existing task
        public void UpdateTask(TaskItem task)
        {
            using var connection = GetConnection();
            string query = @"UPDATE Tasks SET Title = @Title, Description = @Description, 
                            ReminderDate = @ReminderDate, IsCompleted = @IsCompleted 
                            WHERE TaskID = @TaskID";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@TaskID", task.TaskID);
            cmd.Parameters.AddWithValue("@Title", task.Title);
            cmd.Parameters.AddWithValue("@Description", task.Description);
            cmd.Parameters.AddWithValue("@ReminderDate", task.ReminderDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);
            cmd.ExecuteNonQuery();

            LogActivity("Task Updated", $"Task: '{task.Title}' - Status: {(task.IsCompleted ? "Completed" : "Active")}");
        }

        // Deletes a task
        public void DeleteTask(int taskID)
        {
            using var connection = GetConnection();
            string query = "DELETE FROM Tasks WHERE TaskID = @TaskID";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@TaskID", taskID);

            string title = GetTaskTitle(taskID);  // Get title before deleting
            cmd.ExecuteNonQuery();

            LogActivity("Task Deleted", $"Task: '{title}'");
        }

        // Marks a task as completed
        public void MarkTaskAsCompleted(int taskID)
        {
            using var connection = GetConnection();
            string query = "UPDATE Tasks SET IsCompleted = TRUE WHERE TaskID = @TaskID";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@TaskID", taskID);

            string title = GetTaskTitle(taskID);
            cmd.ExecuteNonQuery();

            LogActivity("Task Completed", $"Task: '{title}' marked as completed");
        }

        // Helper method to get a task's title by ID
        private string GetTaskTitle(int taskID)
        {
            using var connection = GetConnection();
            string query = "SELECT Title FROM Tasks WHERE TaskID = @TaskID";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@TaskID", taskID);
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? "Unknown Task";
        }

        // ============ ACTIVITY LOG OPERATIONS ============

        // Logs an action to the database
        public void LogActivity(string actionType, string description)
        {
            using var connection = GetConnection();
            string query = "INSERT INTO ActivityLog (ActionType, Description) VALUES (@ActionType, @Description)";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ActionType", actionType);
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.ExecuteNonQuery();
        }

        // Gets the most recent log entries
        public List<ActivityLogEntry> GetRecentActivityLog(int count = 10)
        {
            var entries = new List<ActivityLogEntry>();
            using var connection = GetConnection();
            string query = "SELECT * FROM ActivityLog ORDER BY Timestamp DESC LIMIT @Count";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Count", count);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                entries.Add(new ActivityLogEntry
                {
                    LogID = reader.GetInt32("LogID"),
                    ActionType = reader.GetString("ActionType"),
                    Description = reader.GetString("Description"),
                    Timestamp = reader.GetDateTime("Timestamp")
                });
            }
            return entries;
        }

        // Saves quiz results
        public void SaveQuizResult(string userName, int score, int total)
        {
            using var connection = GetConnection();
            string query = "INSERT INTO QuizResults (UserName, Score, TotalQuestions) VALUES (@UserName, @Score, @Total)";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserName", userName);
            cmd.Parameters.AddWithValue("@Score", score);
            cmd.Parameters.AddWithValue("@Total", total);
            cmd.ExecuteNonQuery();

            LogActivity("Quiz Completed", $"User: {userName}, Score: {score}/{total}");
        }

        // Clean up resources
        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}