using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberChatBot
{
    public class ActivityEntry
    {
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm}] {Description}";
        }
    }

    /// <summary>
    /// Records every significant action the chatbot takes (tasks added, reminders set, quiz attempts, NLP-recognised commands) 
    ///so the user can review a recent activity summary on request.
    /// </summary>
    public class ActivityLog
    {
        private readonly List<ActivityEntry> _entries = new List<ActivityEntry>();

        /// <summary>
        /// Adds a new entry to the log with the current timestamp.
        /// </summary>
        public void Log(string description)
        {
            _entries.Add(new ActivityEntry
            {
                Timestamp = DateTime.Now,
                Description = description
            });
        }

        /// <summary>
        /// Returns the most recent entries (default 10), newest first.
        /// </summary>
        public List<ActivityEntry> GetRecent(int count = 10)
        {
            return _entries
                .OrderByDescending(e => e.Timestamp)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Returns every entry ever logged, newest first (used by "show more").
        /// </summary>
        public List<ActivityEntry> GetAll()
        {
            return _entries.OrderByDescending(e => e.Timestamp).ToList();
        }

        /// <summary>
        /// Formats the most recent entries as a numbered, chatbot-friendly summary.
        /// </summary>
        public string GetFormattedSummary(int count = 10)
        {
            var recent = GetRecent(count);

            if (recent.Count == 0)
                return "There's no activity logged yet — start adding tasks or taking the quiz!";

            var sb = new StringBuilder();
            sb.AppendLine("Here's a summary of recent actions:");

            for (int i = 0; i < recent.Count; i++)
                sb.AppendLine($"{i + 1}. {recent[i].Description}");

            if (_entries.Count > count)
                sb.Append($"\n({_entries.Count - count} more action(s) in full history — ask to \"show full log\" to see everything.)");

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Formats the entire history as a numbered list (used for "show more").
        /// </summary>
        public string GetFullFormattedSummary()
        {
            var all = GetAll();

            if (all.Count == 0)
                return "There's no activity logged yet — start adding tasks or taking the quiz!";

            var sb = new StringBuilder();
            sb.AppendLine($"Full activity history ({all.Count} actions):");

            for (int i = 0; i < all.Count; i++)
                sb.AppendLine($"{i + 1}. {all[i]}");

            return sb.ToString().TrimEnd();
        }
    }
}

