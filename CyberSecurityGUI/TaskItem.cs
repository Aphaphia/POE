using System;

namespace CyberSecurityGUI.Models
{
    public class TaskItem
    {
        public int TaskID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            string status = IsCompleted ? "✅ Completed" : "⏳ Pending";
            string reminder = ReminderDate.HasValue ?
                $" (Reminder: {ReminderDate.Value:yyyy-MM-dd HH:mm})" : "";
            return $"{Title}{reminder} - {status}";
        }
    }
}