using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CyberChatBot
{
    /// <summary>
    /// The intent the NLP layer believes the user is expressing.
    /// </summary>
    public enum Intent
    {
        None,
        AddTask,
        SetReminder,
        ViewTasks,
        CompleteTask,
        DeleteTask,
        StartQuiz,
        ShowActivityLog,
        ShowFullActivityLog
    }

    /// <summary>
    /// The result of interpreting a user message: the detected intent plus
    /// any extracted details (task title, reminder timeframe, etc.).
    /// </summary>
    public class NlpResult
    {
        public Intent Intent { get; set; } = Intent.None;
        public string ExtractedTitle { get; set; }
        public int? ReminderDays { get; set; }
        public DateTime? ReminderDate { get; set; }
        public int? ReferencedTaskId { get; set; }
    }

    /// <summary>
    /// Simulates basic NLP using keyword detection and simple string
    /// manipulation (string.Contains, regular expressions) rather than a
    /// real language model, as permitted by the brief. Recognises varied
    /// phrasing for tasks, reminders, the quiz, and the activity log so the
    /// chatbot feels responsive without requiring exact command syntax.
    /// </summary>
    public class NlpHelper
    {
        // Phrases that indicate the user wants to add a new task
        private readonly string[] _addTaskPhrases =
        {
            "add a task", "add task", "create a task", "new task",
            "add a reminder to", "remind me to", "i need to", "i want to add"
        };

        private readonly string[] _viewTaskPhrases =
        {
            "show tasks", "view tasks", "show my tasks", "view my tasks",
            "list tasks", "my tasks", "what tasks", "show task list"
        };

        private readonly string[] _completeTaskPhrases =
        {
            "mark task", "complete task", "finish task", "mark as done", "task done", "completed task"
        };

        private readonly string[] _deleteTaskPhrases =
        {
            "delete task", "remove task", "delete the task", "cancel task"
        };

        private readonly string[] _quizPhrases =
        {
            "start quiz", "take quiz", "play quiz", "quiz me", "start the quiz",
            "mini game", "minigame", "play game", "test my knowledge"
        };

        private readonly string[] _logPhrases =
        {
            "show activity log", "activity log", "show log", "what have you done",
            "show history", "recent actions", "show my activity"
        };

        private readonly string[] _showMorePhrases =
        {
            "show more", "show full log", "full history", "show everything", "see all"
        };

        /// <summary>
        /// Analyses the raw user input and returns the detected intent along
        /// with any extracted details, using lowercase Contains() matching
        /// so slightly different phrasing is still recognised.
        /// </summary>
        public NlpResult Interpret(string rawInput)
        {
            string input = rawInput.ToLower().Trim();
            var result = new NlpResult();

            // ── Activity log (checked early since "what have you done" is distinctive) ──
            if (_showMorePhrases.Any(p => input.Contains(p)))
            {
                result.Intent = Intent.ShowFullActivityLog;
                return result;
            }

            if (_logPhrases.Any(p => input.Contains(p)))
            {
                result.Intent = Intent.ShowActivityLog;
                return result;
            }

            // ── Quiz ──
            if (_quizPhrases.Any(p => input.Contains(p)))
            {
                result.Intent = Intent.StartQuiz;
                return result;
            }

            // ── Task management ──
            if (_completeTaskPhrases.Any(p => input.Contains(p)))
            {
                result.Intent = Intent.CompleteTask;
                result.ReferencedTaskId = ExtractTaskId(input);
                return result;
            }

            if (_deleteTaskPhrases.Any(p => input.Contains(p)))
            {
                result.Intent = Intent.DeleteTask;
                result.ReferencedTaskId = ExtractTaskId(input);
                return result;
            }

            if (_viewTaskPhrases.Any(p => input.Contains(p)))
            {
                result.Intent = Intent.ViewTasks;
                return result;
            }

            // ── Reminder for an existing task, e.g. "remind me in 3 days" ──
            if (input.Contains("remind me in") || (input.Contains("remind") && ContainsDayNumber(input)))
            {
                result.Intent = Intent.SetReminder;
                result.ReminderDays = ExtractDayNumber(input);
                return result;
            }

            if (input.Contains("remind me tomorrow") || input.Contains("tomorrow"))
            {
                if (input.Contains("remind"))
                {
                    result.Intent = Intent.SetReminder;
                    result.ReminderDate = DateTime.Now.AddDays(1).Date;
                    return result;
                }
            }

            // ── Add task (checked after reminder-only phrases to avoid clashing) ──
            if (_addTaskPhrases.Any(p => input.Contains(p)))
            {
                result.Intent = Intent.AddTask;
                result.ExtractedTitle = ExtractTaskTitle(input);

                // Allow combined "add a task to X" with same-message reminder, e.g. "in 7 days"
                if (ContainsDayNumber(input))
                    result.ReminderDays = ExtractDayNumber(input);

                return result;
            }

            result.Intent = Intent.None;
            return result;
        }

        /// <summary>
        /// Pulls a clean task title out of phrases like "add a task to enable 2FA"
        /// or "remind me to update my password tomorrow".
        /// </summary>
        private string ExtractTaskTitle(string input)
        {
            string title = input;

            string[] prefixesToStrip =
            {
                "add a task to ", "add a task - ", "add a task ", "add task to ", "add task - ", "add task ",
                "create a task to ", "create a task ", "new task to ", "new task ",
                "add a reminder to ", "remind me to ", "i need to ", "i want to add "
            };

            foreach (var prefix in prefixesToStrip)
            {
                int idx = title.IndexOf(prefix, StringComparison.Ordinal);
                if (idx >= 0)
                {
                    title = title.Substring(idx + prefix.Length);
                    break;
                }
            }

            // Strip trailing timeframe words so they don't pollute the title
            string[] trailingNoise = { "tomorrow", "today", "in 1 day", "in a week" };
            foreach (var noise in trailingNoise)
                title = Regex.Replace(title, Regex.Escape(noise) + "$", "", RegexOptions.IgnoreCase).Trim();

            title = Regex.Replace(title, @"in\s+\d+\s+days?$", "", RegexOptions.IgnoreCase).Trim();
            title = title.Trim(' ', '.', '!', '?');

            if (string.IsNullOrWhiteSpace(title))
                return "New cybersecurity task";

            // Capitalise first letter for a tidy display
            return char.ToUpper(title[0]) + title.Substring(1);
        }

        /// <summary>
        /// Checks whether the input contains a "in N day(s)" style phrase.
        /// </summary>
        private bool ContainsDayNumber(string input)
        {
            return Regex.IsMatch(input, @"in\s+\d+\s+days?");
        }

        /// <summary>
        /// Extracts the number of days from a phrase like "remind me in 3 days".
        /// </summary>
        private int? ExtractDayNumber(string input)
        {
            var match = Regex.Match(input, @"in\s+(\d+)\s+days?");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int days))
                return days;

            return null;
        }

        /// <summary>
        /// Extracts a task ID number if the user references one directly,
        /// e.g. "complete task 3". Returns null if no number is present.
        /// </summary>
        private int? ExtractTaskId(string input)
        {
            var match = Regex.Match(input, @"\d+");
            if (match.Success && int.TryParse(match.Value, out int id))
                return id;

            return null;
        }
    }
}
