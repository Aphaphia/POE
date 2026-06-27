using System.Collections.Generic;
using CyberSecurityGUI.Models;

namespace CyberSecurityGUI.Data
{
    public static class QuizData
    {
        public static List<QuizQuestion> GetQuestions()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Id = 1,
                    Question = "What is a strong password?",
                    Options = new List<string> { "Your birthdate", "A 12-character mix of letters, numbers, and symbols", "Your pet's name", "Password123" },
                    CorrectAnswerIndex = 1,
                    Explanation = "A strong password is at least 12 characters long and uses a mix of character types.",
                    Category = "Passwords"
                },
                new QuizQuestion
                {
                    Id = 2,
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report it as phishing", "Ignore it" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Legitimate companies never ask for passwords via email. Report phishing attempts!",
                    Category = "Phishing"
                },
                new QuizQuestion
                {
                    Id = 3,
                    Question = "What does 2FA stand for?",
                    Options = new List<string> { "Two-Factor Authentication", "Two-File Access", "Transfer File Authorization", "Temporary File Archive" },
                    CorrectAnswerIndex = 0,
                    Explanation = "2FA is Two-Factor Authentication - a second layer of security for your accounts.",
                    Category = "Authentication"
                },
                new QuizQuestion
                {
                    Id = 4,
                    Question = "What is ransomware?",
                    Options = new List<string> { "A type of virus that encrypts files", "A security software", "A password manager", "A web browser" },
                    CorrectAnswerIndex = 0,
                    Explanation = "Ransomware encrypts your files and demands payment for their release. Never pay the ransom!",
                    Category = "Malware"
                },
                new QuizQuestion
                {
                    Id = 5,
                    Question = "What should you do on public Wi-Fi?",
                    Options = new List<string> { "Use a VPN", "Do banking transactions", "Share personal info", "Use the same password for everything" },
                    CorrectAnswerIndex = 0,
                    Explanation = "A VPN encrypts your traffic on public Wi-Fi. Avoid sensitive activities on public networks.",
                    Category = "Wi-Fi Security"
                },
                new QuizQuestion
                {
                    Id = 6,
                    Question = "True or False: You should use the same password for all your accounts.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Using the same password for multiple accounts is dangerous. If one is breached, all your accounts are at risk.",
                    Category = "Passwords"
                },
                new QuizQuestion
                {
                    Id = 7,
                    Question = "What is a VPN?",
                    Options = new List<string> { "Virtual Private Network", "Very Private Network", "Virtual Protocol Network", "Verified Personal Network" },
                    CorrectAnswerIndex = 0,
                    Explanation = "A VPN creates an encrypted connection over the internet, protecting your privacy.",
                    Category = "VPN"
                },
                new QuizQuestion
                {
                    Id = 8,
                    Question = "What is social engineering?",
                    Options = new List<string> { "Building social networks", "Manipulating people to reveal info", "Engineering social media", "Creating virtual communities" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Social engineering uses psychological manipulation to trick people into revealing information.",
                    Category = "Social Engineering"
                },
                new QuizQuestion
                {
                    Id = 9,
                    Question = "What does HTTPS protect?",
                    Options = new List<string> { "Your entire computer", "Your connection to a website", "Your email account", "Your social media" },
                    CorrectAnswerIndex = 1,
                    Explanation = "HTTPS encrypts your connection to a website, protecting the data you send and receive.",
                    Category = "Safe Browsing"
                },
                new QuizQuestion
                {
                    Id = 10,
                    Question = "What is the 3-2-1 backup rule?",
                    Options = new List<string> { "3 copies, 2 different storage types, 1 offsite copy", "3 backups, 2 days, 1 week", "3 files, 2 folders, 1 drive", "3 devices, 2 accounts, 1 password" },
                    CorrectAnswerIndex = 0,
                    Explanation = "The 3-2-1 rule: 3 copies of your data, 2 different storage types, 1 offsite copy.",
                    Category = "Backup"
                },
                new QuizQuestion
                {
                    Id = 11,
                    Question = "How often should you update your software?",
                    Options = new List<string> { "Never", "Only when there's a problem", "As soon as updates are available", "Once a year" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Updates patch security vulnerabilities. Install them as soon as they're available!",
                    Category = "Updates"
                },
                new QuizQuestion
                {
                    Id = 12,
                    Question = "True or False: You should share your OTP with the bank if they call.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "NEVER share your OTP. Banks will never ask for it over the phone. This is a common scam!",
                    Category = "Scams"
                }
            };
        }
    }
}