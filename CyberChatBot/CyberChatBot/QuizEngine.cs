using System;
using System.Collections.Generic;

namespace CyberChatBot
{
    /// <summary>
    /// The type of quiz question, used by the UI to decide whether to
    /// render four options (MultipleChoice) or two (TrueFalse).
    /// </summary>
    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse
    }

    /// <summary>
    /// Represents a single quiz question, its possible answers, the
    /// correct option, and an explanation shown after the user answers.
    /// </summary>
    public class QuizQuestion
    {
        public string Question { get; set; }
        public QuestionType Type { get; set; }
        public List<string> Options { get; set; }
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; }
    }

    /// <summary>
    /// Drives the cybersecurity mini-game: holds the question bank,
    /// tracks the user's progress through the quiz, and calculates
    /// the final score and feedback message.
    /// </summary>
    public class QuizEngine
    {
        private readonly List<QuizQuestion> _questions;
        private int _currentIndex;
        private int _score;

        public QuizEngine()
        {
            _questions = BuildQuestionBank();
            _currentIndex = 0;
            _score = 0;
        }

        public int TotalQuestions => _questions.Count;
        public int CurrentQuestionNumber => _currentIndex + 1;
        public int Score => _score;
        public bool IsFinished => _currentIndex >= _questions.Count;

        /// <summary>
        /// Resets the quiz back to the first question with a score of zero.
        /// </summary>
        public void Reset()
        {
            _currentIndex = 0;
            _score = 0;
        }

        /// <summary>
        /// Returns the current question without advancing the quiz.
        /// </summary>
        public QuizQuestion GetCurrentQuestion()
        {
            return IsFinished ? null : _questions[_currentIndex];
        }

        /// <summary>
        /// Submits the user's chosen option index for the current question,
        /// advances to the next question, and returns whether it was correct.
        /// </summary>
        public bool SubmitAnswer(int chosenIndex)
        {
            QuizQuestion current = GetCurrentQuestion();
            if (current == null) return false;

            bool isCorrect = chosenIndex == current.CorrectIndex;
            if (isCorrect) _score++;

            _currentIndex++;
            return isCorrect;
        }

        /// <summary>
        /// Returns a friendly feedback message based on the final score percentage.
        /// </summary>
        public string GetFinalFeedback()
        {
            double percentage = (double)_score / _questions.Count * 100;

            if (percentage >= 80)
                return $"Great job! You scored {_score}/{_questions.Count} — you're a cybersecurity pro!";
            if (percentage >= 50)
                return $"Good effort! You scored {_score}/{_questions.Count}. Keep learning to stay even safer online!";

            return $"You scored {_score}/{_questions.Count}. Keep learning to stay safe online — review the topics and try again!";
        }

        /// <summary>
        /// Builds the bank of 12 cybersecurity questions covering phishing,
        /// password safety, safe browsing, social engineering, and more,
        /// mixing multiple-choice and true/false formats.
        /// </summary>
        private List<QuizQuestion> BuildQuestionBank()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    CorrectIndex = 2,
                    Explanation = "Reporting phishing emails helps prevent scams and protects others too."
                },
                new QuizQuestion
                {
                    Question = "Using the same password across multiple accounts is safe as long as it's a strong password.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "False — if one account is breached, reused passwords put all your other accounts at risk too."
                },
                new QuizQuestion
                {
                    Question = "Which of these is a sign of a phishing email?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "Urgent language demanding immediate action", "A personalised greeting with your full name", "An email from a known colleague", "A short, plain-text message" },
                    CorrectIndex = 0,
                    Explanation = "Urgency and pressure tactics are classic phishing red flags designed to stop you thinking it through."
                },
                new QuizQuestion
                {
                    Question = "Two-factor authentication (2FA) adds an extra layer of security beyond your password.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "True — even if your password is stolen, 2FA can stop attackers from accessing your account."
                },
                new QuizQuestion
                {
                    Question = "What does the padlock icon in your browser's address bar indicate?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "The site is owned by the government", "The connection is encrypted (HTTPS)", "The site has no ads", "The site is virus-free" },
                    CorrectIndex = 1,
                    Explanation = "The padlock means the connection is encrypted via HTTPS, protecting data sent between you and the site."
                },
                new QuizQuestion
                {
                    Question = "It's safe to use public Wi-Fi for online banking without a VPN.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "False — public Wi-Fi can be intercepted by attackers; a VPN helps keep your data private on these networks."
                },
                new QuizQuestion
                {
                    Question = "What is 'social engineering' in a cybersecurity context?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "Writing secure code", "Exploiting human psychology to gain information or access", "Engineering network hardware", "Designing social media platforms" },
                    CorrectIndex = 1,
                    Explanation = "Social engineering targets human trust and behaviour rather than technical vulnerabilities."
                },
                new QuizQuestion
                {
                    Question = "You should never share your one-time PIN (OTP) with anyone, even someone claiming to be from your bank.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "True — legitimate banks will never ask for your OTP. Sharing it is a common scam tactic."
                },
                new QuizQuestion
                {
                    Question = "What is ransomware?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "Software that speeds up your PC", "Malware that locks your files and demands payment", "A type of antivirus", "A password manager" },
                    CorrectIndex = 1,
                    Explanation = "Ransomware locks or encrypts your files and demands payment to release them — regular backups are the best defence."
                },
                new QuizQuestion
                {
                    Question = "A strong password should ideally be at least 12 characters long.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "True — longer passwords are significantly harder to crack through brute-force attacks."
                },
                new QuizQuestion
                {
                    Question = "Which of these is the safest way to verify a suspicious request from your bank?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "Reply directly to the email", "Click the link in the message", "Call the bank using the number on your card or official website", "Ignore it and hope it's nothing" },
                    CorrectIndex = 2,
                    Explanation = "Always verify through an official, independently-sourced contact channel — never the one provided in the suspicious message."
                },
                new QuizQuestion
                {
                    Question = "Antivirus software alone is enough to guarantee complete protection from all cyber threats.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "False — antivirus is one layer of defence, but safe habits, 2FA, backups, and awareness are all needed too."
                }
            };
        }
    }
}
