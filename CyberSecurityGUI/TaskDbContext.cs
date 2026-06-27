using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using CyberSecurityGUI.Models;

namespace CyberSecurityGUI.Database
{
    public class TaskDbContext : IDisposable
    {
        
        private readonly string _connectionString;
        private MySqlConnection _connection;

        public TaskDbContext(string connectionString = null)
        {
            _connectionString = connectionString ??
                "Server=localhost;Database=CyberSecurityChatbot;User ID=root;Password=AmoAtli@03;";
        }

        private MySqlConnection GetConnection()
        {
            if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
            {
                _connection = new MySqlConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }

        public void AddTask(TaskItem task)
        {
            using (var connection = GetConnection())
            {
                string query = @"INSERT INTO Tasks (Title, Description, ReminderDate, IsCompleted) 
                                VALUES (@Title, @Description, @ReminderDate, @IsCompleted)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Title", task.Title);
                    cmd.Parameters.AddWithValue("@Description", task.Description);
                    cmd.Parameters.AddWithValue("@ReminderDate", task.ReminderDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);
                    cmd.ExecuteNonQuery();
                }
                LogActivity("Task Added", $"Task: '{task.Title}'");
            }
        }

        public List<TaskItem> GetAllTasks()
        {
            var tasks = new List<TaskItem>();
            using (var connection = GetConnection())
            {
                string query = "SELECT * FROM Tasks ORDER BY CreatedAt DESC";
                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            TaskID = reader.GetInt32("TaskID"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description"),
                            ReminderDate = reader.IsDBNull("ReminderDate") ? null : (DateTime?)reader.GetDateTime("ReminderDate"),
                            IsCompleted = reader.GetBoolean("IsCompleted"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        });
                    }
                }
            }
            return tasks;
        }

        public List<TaskItem> GetIncompleteTasks()
        {
            var tasks = new List<TaskItem>();
            using (var connection = GetConnection())
            {
                string query = "SELECT * FROM Tasks WHERE IsCompleted = FALSE ORDER BY ReminderDate ASC";
                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            TaskID = reader.GetInt32("TaskID"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description"),
                            ReminderDate = reader.IsDBNull("ReminderDate") ? null : (DateTime?)reader.GetDateTime("ReminderDate"),
                            IsCompleted = reader.GetBoolean("IsCompleted"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        });
                    }
                }
            }
            return tasks;
        }

        public void UpdateTask(TaskItem task)
        {
            using (var connection = GetConnection())
            {
                string query = @"UPDATE Tasks SET Title = @Title, Description = @Description, 
                                ReminderDate = @ReminderDate, IsCompleted = @IsCompleted 
                                WHERE TaskID = @TaskID";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TaskID", task.TaskID);
                    cmd.Parameters.AddWithValue("@Title", task.Title);
                    cmd.Parameters.AddWithValue("@Description", task.Description);
                    cmd.Parameters.AddWithValue("@ReminderDate", task.ReminderDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);
                    cmd.ExecuteNonQuery();
                }
                LogActivity("Task Updated", $"Task: '{task.Title}' - Status: {(task.IsCompleted ? "Completed" : "Active")}");
            }
        }

        public void DeleteTask(int taskID)
        {
            using (var connection = GetConnection())
            {
                string title = GetTaskTitle(taskID);
                string query = "DELETE FROM Tasks WHERE TaskID = @TaskID";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TaskID", taskID);
                    cmd.ExecuteNonQuery();
                }
                LogActivity("Task Deleted", $"Task: '{title}'");
            }
        }

        public void MarkTaskAsCompleted(int taskID)
        {
            using (var connection = GetConnection())
            {
                string title = GetTaskTitle(taskID);
                string query = "UPDATE Tasks SET IsCompleted = TRUE WHERE TaskID = @TaskID";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TaskID", taskID);
                    cmd.ExecuteNonQuery();
                }
                LogActivity("Task Completed", $"Task: '{title}' marked as completed");
            }
        }

        private string GetTaskTitle(int taskID)
        {
            using (var connection = GetConnection())
            {
                string query = "SELECT Title FROM Tasks WHERE TaskID = @TaskID";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    var result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "Unknown Task";
                }
            }
        }

        public void LogActivity(string actionType, string description)
        {
            using (var connection = GetConnection())
            {
                string query = "INSERT INTO ActivityLog (ActionType, Description) VALUES (@ActionType, @Description)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ActionType", actionType);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<ActivityLogEntry> GetRecentActivityLog(int count = 10)
        {
            var entries = new List<ActivityLogEntry>();
            using (var connection = GetConnection())
            {
                string query = "SELECT * FROM ActivityLog ORDER BY Timestamp DESC LIMIT @Count";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Count", count);
                    using (var reader = cmd.ExecuteReader())
                    {
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
                    }
                }
            }
            return entries;
        }

        public void SaveQuizResult(string userName, int score, int total)
        {
            using (var connection = GetConnection())
            {
                string query = "INSERT INTO QuizResults (UserName, Score, TotalQuestions) VALUES (@UserName, @Score, @Total)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.ExecuteNonQuery();
                }
                LogActivity("Quiz Completed", $"User: {userName}, Score: {score}/{total}");
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}