using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CyberChatBot
{
    /// <summary>
    /// Represents a single cybersecurity task stored in the database.
    /// </summary>
    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Manages cybersecurity tasks using a MySQL database for persistent storage.
    /// On startup the class ensures the database and table exist, creating them
    /// automatically if they don't so the app works out-of-the-box.
    /// 
    /// HOW TO SET UP:
    ///   1. Install MySQL Community Server (https://dev.mysql.com/downloads/mysql/)
    ///      and make sure it is running on localhost:3306.
    ///   2. The default root password below is empty string — change it to match
    ///      yours, OR create a dedicated user (see comment on ConnectionString).
    ///   3. Build the project once; NuGet will pull in MySql.Data automatically.
    ///   4. Run the app — the database and table are created for you on first launch.
    /// </summary>
    public class TaskManager
    {
        // ──────────────────────────────────────────────────────────────
        //  Connection string
        //  Change "root" / "YourPasswordHere" to match your MySQL setup.
        //  If you created a dedicated user, replace them accordingly.
        // ──────────────────────────────────────────────────────────────
        private const string Server = "localhost";
        private const string Port = "3306";
        private const string Database = "cyberchatbot_db";
        private const string User = "root";
        private const string Password = "1234";   // ← put your MySQL root password here

        private string ConnectionString =>
            $"Server={Server};Port={Port};Database={Database};Uid={User};Pwd={Password};CharSet=utf8mb4;";

        // Used for the initial database-creation step (no database selected yet)
        private string RootConnectionString =>
            $"Server={Server};Port={Port};Uid={User};Pwd={Password};CharSet=utf8mb4;";

        // ──────────────────────────────────────────────────────────────
        //  Constructor — ensures DB and table exist
        // ──────────────────────────────────────────────────────────────

        public TaskManager()
        {
            InitialiseDatabase();
        }

        /// <summary>
        /// Creates the database and the Tasks table if they don't already exist.
        /// Called once on startup; safe to call repeatedly.
        /// </summary>
        private void InitialiseDatabase()
        {
            // Step 1 — create the database if needed (connect without selecting a DB)
            using (var conn = new MySqlConnection(RootConnectionString))
            {
                conn.Open();
                string createDb = $"CREATE DATABASE IF NOT EXISTS `{Database}` " +
                                  "CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
                using (var cmd = new MySqlCommand(createDb, conn))
                    cmd.ExecuteNonQuery();
            }

            // Step 2 — create the Tasks table if needed
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string createTable = @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id           INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Title        VARCHAR(255)  NOT NULL,
                        Description  TEXT,
                        ReminderDate DATE          NULL,
                        IsCompleted  TINYINT(1)    NOT NULL DEFAULT 0,
                        CreatedAt    DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                using (var cmd = new MySqlCommand(createTable, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        // ──────────────────────────────────────────────────────────────
        //  Public API  (same surface the engine and UI already call)
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Inserts a new task and returns the auto-generated ID.
        /// </summary>
        public int AddTask(string title, string description, DateTime? reminderDate = null)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                string sql = @"
                    INSERT INTO Tasks (Title, Description, ReminderDate)
                    VALUES (@title, @desc, @reminder);
                    SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title ?? "New Task");
                    cmd.Parameters.AddWithValue("@desc", description ?? "");
                    cmd.Parameters.AddWithValue("@reminder", reminderDate.HasValue
                        ? (object)reminderDate.Value.Date
                        : DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Returns all tasks ordered by creation date (newest last).
        /// </summary>
        public List<CyberTask> GetAllTasks()
        {
            var tasks = new List<CyberTask>();

            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                string sql = "SELECT Id, Title, Description, ReminderDate, IsCompleted " +
                             "FROM Tasks ORDER BY CreatedAt ASC;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new CyberTask
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                              ? ""
                                              : reader.GetString("Description"),
                            ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate"))
                                              ? (DateTime?)null
                                              : reader.GetDateTime("ReminderDate"),
                            IsCompleted = reader.GetBoolean("IsCompleted")
                        });
                    }
                }
            }

            return tasks;
        }

        /// <summary>
        /// Returns the most recently inserted incomplete task,
        /// used when the user says "remind me in X days" without specifying a task.
        /// </summary>
        public CyberTask GetMostRecentTask()
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                string sql = "SELECT Id, Title, Description, ReminderDate, IsCompleted " +
                             "FROM Tasks WHERE IsCompleted = 0 " +
                             "ORDER BY CreatedAt DESC LIMIT 1;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new CyberTask
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                              ? ""
                                              : reader.GetString("Description"),
                            ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate"))
                                              ? (DateTime?)null
                                              : reader.GetDateTime("ReminderDate"),
                            IsCompleted = reader.GetBoolean("IsCompleted")
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Marks the task with the given ID as completed.
        /// Returns true if a row was updated, false if the ID wasn't found.
        /// </summary>
        public bool MarkCompleted(int taskId)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                string sql = "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @id;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Deletes the task with the given ID.
        /// Returns true if a row was deleted, false if the ID wasn't found.
        /// </summary>
        public bool DeleteTask(int taskId)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                string sql = "DELETE FROM Tasks WHERE Id = @id;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Sets or updates the reminder date for the task with the given ID.
        /// </summary>
        public void SetReminder(int taskId, DateTime reminderDate)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                string sql = "UPDATE Tasks SET ReminderDate = @date WHERE Id = @id;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@date", reminderDate.Date);
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}