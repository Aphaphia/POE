using System;

namespace CyberSecurityGUI.Models
{
    public class ActivityLogEntry
    {
        public int LogID { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}