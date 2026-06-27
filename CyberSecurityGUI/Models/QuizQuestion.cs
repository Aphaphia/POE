using System.Collections.Generic;

namespace CyberSecurityGUI.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}