using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CyberChatBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// Handles all UI events, chat bubble rendering, ASCII art display, voice greeting playback, and wires user input into ChatbotEngine.
    /// </summary>
    public partial class MainWindow : Window
    {
        // ─────────────────────────────────────────────
        //  Fields
        // ─────────────────────────────────────────────

        private readonly ChatbotEngine _engine = new ChatbotEngine();
        private string _userName = "";

        // Update this path if your WAV file moves
        private readonly string _wavPath = @"C:\Users\mngun\source\repos\CyberChatBot\CyberChatBot\Voice message.wav";

        // ASCII art banner matching the Cybersecurity Awareness ChatBot logo
        private readonly string _asciiArt =
" ██████╗██╗   ██╗██████╗ ███████╗██████╗ ███████╗███████╗ ██████╗██╗   ██╗██████╗ ██╗████████╗██╗   ██╗     █████╗ ██╗    ██╗ █████╗ ██████╗ ███████╗███╗   ██╗███████╗███████╗███████╗    ██████╗  ██████╗ ████████╗ \n" +
"██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔════╝██╔════╝██╔════╝██║   ██║██╔══██╗██║╚══██╔══╝╚██╗ ██╔╝    ██╔══██╗██║    ██║██╔══██╗██╔══██╗██╔════╝████╗  ██║██╔════╝██╔════╝██╔════╝    ██╔══██╗██╔═══██╗╚══██╔══╝ \n" +
"██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝███████╗█████╗  ██║     ██║   ██║██████╔╝██║   ██║    ╚████╔╝     ███████║██║ █╗ ██║███████║██████╔╝█████╗  ██╔██╗ ██║█████╗  ███████╗███████╗    ██████╔╝██║   ██║   ██║   \n" +
"██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗╚════██║██╔══╝  ██║     ██║   ██║██╔══██╗██║   ██║     ╚██╔╝      ██╔══██║██║███╗██║██╔══██║██╔══██╗██╔══╝  ██║╚██╗██║██╔══╝  ╚════██║╚════██║    ██╔══██╗██║   ██║   ██║   \n" +
"╚██████╗   ██║   ██████╔╝███████╗██║  ██║███████║███████╗╚██████╗╚██████╔╝██║  ██║██║   ██║      ██║       ██║  ██║╚███╔███╔╝██║  ██║██║  ██║███████╗██║ ╚████║███████╗███████║███████║    ██████╔╝╚██████╔╝   ██║  \n\n" +
 "╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝╚══════╝╚══════╝ ╚═════╝ ╚═════╝ ╚═╝  ╚═╝╚═╝   ╚═╝      ╚═╝       ╚═╝  ╚═╝ ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═══╝╚══════╝╚══════╝╚══════╝    ╚═════╝  ╚═════╝    ╚═╝   ";

        // ─────────────────────────────────────────────
        //  Constructor
        // ─────────────────────────────────────────────

        /// <summary>
        /// Initialises the window components and plays the voice greeting on startup.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            PlayVoiceGreeting();
        }

        // ─────────────────────────────────────────────
        //  Voice Greeting
        // ─────────────────────────────────────────────

        /// <summary>
        /// Plays the WAV voice greeting asynchronously when the application launches.
        /// Silently skips playback if the file is missing or unplayable.
        /// </summary>
        public void PlayVoiceGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer(_wavPath);
                player.Play();
            }
            catch
            {
                // Silently skip — no crash if file is missing
            }
        }

        // ─────────────────────────────────────────────
        //  Name Entry
        // ─────────────────────────────────────────────

        /// <summary>
        /// Handles the Enter key press on the name input field, triggering the same action as clicking the Start Chat button.
        /// </summary>
        public void NameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                StartChat_Click(sender, e);
        }

        /// <summary>
        /// Validates the user's name, stores it in the chatbot's memory,
        /// switches the UI from the name panel to the chat panel,
        /// and displays the ASCII art banner followed by a personalised welcome message.
        /// </summary>
        public void StartChat_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                NameError.Visibility = Visibility.Visible;
                return;
            }

            _userName = name;
            _engine.Remember("name", name);

            // Switch from name entry panel to chat panel
            NamePanel.Visibility = Visibility.Collapsed;
            ChatContainer.Visibility = Visibility.Visible;

            // Display ASCII art banner as the first message
            AddAsciiMessage(_asciiArt);

            // Personalised welcome message
            AddBotMessage(
                "Hi " + _userName + "! Welcome to the Cybersecurity Awareness Bot." +
                "I'm here to help you stay safe online. You can ask me about:\n\n" +
                "Passwords\n" + "Phishing\n" + "Scams\n" + "Privacy\n" + "Malware\n" + "Safe Browsing\n" + "Two-Factor Authentication\n" + "Social Engineering\n" + "What would you like to know?");

            UserInput.Focus();
            StatusText.Text = "Chatting as " + _userName;
        }

        // ─────────────────────────────────────────────
        //  Message Sending
        // ─────────────────────────────────────────────

        /// <summary>
        /// Handles the Enter key press in the message input box,
        /// sending the message if the input is not empty.
        /// </summary>
        public void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(UserInput.Text))
                SendMessage();
        }

        /// <summary>
        /// Handles the Send button click event.
        /// </summary>
        public void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        /// <summary>
        /// Reads the user's input, displays it as a user bubble,
        /// retrieves the bot's response from ChatbotEngine,
        /// displays the response as a bot bubble,
        /// and updates the status bar with any remembered topic.
        /// </summary>
        public void SendMessage()
        {
            string userText = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userText)) return;

            AddUserMessage(userText);
            UserInput.Clear();

            // ── Exit command — checked BEFORE GetResponse
            // This stops the fallback message firing alongside the goodbye
            if (userText.ToLower() == "bye" || userText.ToLower() == "thank you,bye" || userText.ToLower() == "exit")
            {
                AddBotMessage("Goodbye, " + _userName + "! Stay safe online.");
                SendButton.IsEnabled = false;
                UserInput.IsEnabled = false;
                StatusText.Text = "Session ended. Close the window to exit.";
                return;
            }

            string response = _engine.GetResponse(userText);

            // Special sentinel returned by the engine when NLP detects a quiz request
            if (response == "QUIZ_START")
            {
                OpenQuizPanel();
            }
            else
            {
                AddBotMessage(response);
            }

            // Update status bar if a favourite topic has been remembered
            string favTopic = _engine.Recall("favourite_topic");
            if (favTopic != null)
                StatusText.Text = "Remembered: You're interested in " + favTopic;

            ScrollToBottom();
        }

        // ─────────────────────────────────────────────
        //  Chat Bubble Rendering
        // ─────────────────────────────────────────────

        /// <summary>
        /// Adds a right-aligned chat bubble displaying the user's message.
        /// </summary>
        /// <param name="text">The message text typed by the user.</param>
        public void AddUserMessage(string text)
        {
            UIElement bubble = CreateBubble(
                text,
                new SolidColorBrush(Color.FromRgb(0, 92, 128)),
                Brushes.White,
                isUser: true,
                label: _userName);

            ChatPanel.Children.Add(bubble);
            ScrollToBottom();
        }

        /// <summary>
        /// Adds a left-aligned chat bubble displaying the bot's response.
        /// </summary>
        /// <param name="text">The response text generated by ChatbotEngine.</param>
        public void AddBotMessage(string text)
        {
            UIElement bubble = CreateBubble(
                text,
                new SolidColorBrush(Color.FromRgb(22, 27, 34)),
                new SolidColorBrush(Color.FromRgb(230, 237, 243)),
                isUser: false,
                label: "CyberBot");

            ChatPanel.Children.Add(bubble);
            ScrollToBottom();
        }

        /// <summary>
        /// Adds a special ASCII art bubble using Courier New font so the block characters render correctly and the logo displays as intended.
        /// </summary>
        /// <param name="art">The ASCII art string to display.</param>
        public void AddAsciiMessage(string art)
        {
            TextBlock artBlock = new TextBlock
            {
                Text = art,
                FontFamily = new FontFamily("Courier New"),
                FontSize = 5.5,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 212, 255)),
                TextWrapping = TextWrapping.NoWrap,
                Margin = new Thickness(8, 10, 8, 6),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            ChatPanel.Children.Add(artBlock);
            ScrollToBottom();
        }

        /// <summary>
        /// Creates a styled chat bubble containing a sender label, message text,
        /// and timestamp. Bubbles are right-aligned for the user and left-aligned
        /// for the bot, with distinct corner rounding to indicate direction.
        /// </summary>
        /// <param name="text">The message content to display inside the bubble.</param>
        /// <param name="background">The background brush for the bubble.</param>
        /// <param name="foreground">The text colour brush for the message.</param>
        /// <param name="isUser">True if this bubble belongs to the user; false for the bot.</param>
        /// <param name="label">The sender name shown above the bubble.</param>
        /// <returns>A UIElement ready to be added to the chat panel.</returns>
        public UIElement CreateBubble(string text, Brush background, Brush foreground, bool isUser, string label)
        {
            // Sender name label
            TextBlock nameLabel = new TextBlock
            {
                Text = label,
                Foreground = isUser
                    ? new SolidColorBrush(Color.FromRgb(0, 180, 230))
                    : new SolidColorBrush(Color.FromRgb(63, 185, 80)),
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(isUser ? 0 : 4, 0, isUser ? 4 : 0, 3),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left
            };

            // Message text block
            TextBlock messageText = new TextBlock
            {
                Text = text,
                Foreground = foreground,
                FontSize = 15,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 22
            };

            // Rounded bubble border — corners indicate message direction
            Border bubble = new Border
            {
                Background = background,
                CornerRadius = isUser
                    ? new CornerRadius(12, 2, 12, 12)
                    : new CornerRadius(2, 12, 12, 12),
                Padding = new Thickness(14, 10, 14, 10),
                BorderBrush = new SolidColorBrush(Color.FromRgb(48, 54, 61)),
                BorderThickness = new Thickness(1),
                Child = messageText
            };

            // Timestamp shown below the bubble
            TextBlock timestamp = new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                Foreground = new SolidColorBrush(Color.FromRgb(100, 110, 120)),
                FontSize = 15,
                Margin = new Thickness(4, 2, 4, 0),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left
            };

            // Stack: label → bubble → timestamp
            StackPanel innerStack = new StackPanel
            {
                MaxWidth = 580,
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left
            };
            innerStack.Children.Add(nameLabel);
            innerStack.Children.Add(bubble);
            innerStack.Children.Add(timestamp);

            // Outer container with spacing
            Border container = new Border
            {
                Margin = new Thickness(8, 4, 8, 4),
                Child = innerStack
            };

            return container;
        }

        // ─────────────────────────────────────────────
        //  Utility
        // ─────────────────────────────────────────────

        /// <summary>
        /// Scrolls the chat scroll viewer to the bottom so the latest message is visible.
        /// </summary>
        public void ScrollToBottom()
        {
            ChatScrollViewer.UpdateLayout();
            ChatScrollViewer.ScrollToBottom();
        }

        /// <summary>
        /// Clears all messages from the chat panel and displays a fresh prompt.
        /// The user's name and memory are preserved between clears.
        /// </summary>
        public void ClearChat_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Children.Clear();
            StatusText.Text = "Chat cleared — ready for new questions!";

            if (!string.IsNullOrEmpty(_userName))
                AddBotMessage("Chat cleared! What would you like to know next, " + _userName + "?");
        }

        // ─────────────────────────────────────────────
        //  Part 3 — Quiz Panel
        // ─────────────────────────────────────────────

        private int? _quizSelectedIndex = null;

        /// <summary>
        /// Opens the quiz overlay and loads the first question.
        /// Triggered either by the Quiz button or by NLP-recognised chat input.
        /// </summary>
        public void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            OpenQuizPanel();
        }

        /// <summary>
        /// Resets the quiz engine, shows the overlay, and renders question one.
        /// </summary>
        public void OpenQuizPanel()
        {
            _engine.Quiz.Reset();
            QuizOverlay.Visibility = Visibility.Visible;
            RenderCurrentQuestion();
        }

        /// <summary>
        /// Renders the current quiz question and its answer options as buttons.
        /// </summary>
        public void RenderCurrentQuestion()
        {
            var question = _engine.Quiz.GetCurrentQuestion();

            if (question == null)
            {
                ShowQuizResults();
                return;
            }

            _quizSelectedIndex = null;
            QuizFeedbackText.Visibility = Visibility.Collapsed;
            QuizNextButton.Visibility = Visibility.Collapsed;

            QuizProgressText.Text = $"Question {_engine.Quiz.CurrentQuestionNumber}/{_engine.Quiz.TotalQuestions}";
            QuizScoreText.Text = $"Score: {_engine.Quiz.Score}";
            QuizQuestionText.Text = question.Question;

            QuizOptionsPanel.Children.Clear();

            for (int i = 0; i < question.Options.Count; i++)
            {
                int optionIndex = i; // capture for closure
                Button optionButton = new Button
                {
                    Content = question.Options[i],
                    Height = 46,
                    Margin = new Thickness(0, 0, 0, 8),
                    Background = new SolidColorBrush(Color.FromRgb(33, 38, 45)),
                    Foreground = Brushes.White,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(48, 54, 61)),
                    BorderThickness = new Thickness(1),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Padding = new Thickness(14, 0, 0, 0),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    FontSize = 15
                };

                optionButton.Click += (s, e) => OnQuizOptionSelected(optionIndex, optionButton, question.CorrectIndex);
                QuizOptionsPanel.Children.Add(optionButton);
            }
        }

        /// <summary>
        /// Handles the user selecting an answer: locks the options,
        /// shows correct/incorrect feedback with the explanation, and reveals Next.
        /// </summary>
        private void OnQuizOptionSelected(int chosenIndex, Button clickedButton, int correctIndex)
        {
            if (_quizSelectedIndex.HasValue) return; // already answered
            _quizSelectedIndex = chosenIndex;

            var question = _engine.Quiz.GetCurrentQuestion();
            bool isCorrect = _engine.Quiz.SubmitAnswer(chosenIndex);

            // Disable and colour all option buttons to reveal the correct answer
            for (int i = 0; i < QuizOptionsPanel.Children.Count; i++)
            {
                if (QuizOptionsPanel.Children[i] is Button btn)
                {
                    btn.IsEnabled = false;

                    if (i == correctIndex)
                        btn.Background = new SolidColorBrush(Color.FromRgb(35, 134, 54)); // green
                    else if (i == chosenIndex)
                        btn.Background = new SolidColorBrush(Color.FromRgb(218, 54, 51)); // red
                }
            }

            QuizFeedbackText.Text = (isCorrect ? "✅ Correct! " : "❌ Not quite. ") + question.Explanation;
            QuizFeedbackText.Foreground = isCorrect
                ? new SolidColorBrush(Color.FromRgb(63, 185, 80))
                : new SolidColorBrush(Color.FromRgb(248, 81, 73));
            QuizFeedbackText.Visibility = Visibility.Visible;

            QuizScoreText.Text = $"Score: {_engine.Quiz.Score}";
            QuizNextButton.Visibility = Visibility.Visible;
            QuizNextButton.Content = _engine.Quiz.IsFinished ? "See Results 🏁" : "Next ➤";
        }

        /// <summary>
        /// Advances to the next question, or shows final results if the quiz is finished.
        /// </summary>
        public void QuizNextButton_Click(object sender, RoutedEventArgs e)
        {
            RenderCurrentQuestion();
        }

        /// <summary>
        /// Displays the final score and feedback inside the quiz panel,
        /// and posts a summary message into the main chat for the activity log.
        /// </summary>
        private void ShowQuizResults()
        {
            string feedback = _engine.FinishQuiz();

            QuizProgressText.Text = "Quiz Complete!";
            QuizQuestionText.Text = feedback;
            QuizOptionsPanel.Children.Clear();
            QuizFeedbackText.Visibility = Visibility.Collapsed;
            QuizScoreText.Text = $"Final Score: {_engine.Quiz.Score}/{_engine.Quiz.TotalQuestions}";
            QuizNextButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Closes the quiz overlay and returns focus to the chat input.
        /// </summary>
        public void QuizCloseButton_Click(object sender, RoutedEventArgs e)
        {
            QuizOverlay.Visibility = Visibility.Collapsed;

            if (_engine.Quiz.IsFinished)
                AddBotMessage(_engine.Quiz.GetFinalFeedback());

            UserInput.Focus();
        }

        // ─────────────────────────────────────────────
        //  Part 3 — Tasks Panel
        // ─────────────────────────────────────────────

        /// <summary>
        /// Opens the tasks overlay and renders all current tasks with complete/delete buttons.
        /// </summary>
        public void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            RenderTasksList();
            TasksOverlay.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Rebuilds the task list UI from the database, including a quick
        /// "add task" row and complete/delete actions per task.
        /// </summary>
        private void RenderTasksList()
        {
            TasksListPanel.Children.Clear();

            var tasks = _engine.Tasks.GetAllTasks();

            if (tasks.Count == 0)
            {
                TasksListPanel.Children.Add(new TextBlock
                {
                    Text = "No tasks yet. Try typing \"add a task to enable two-factor authentication\" in the chat.",
                    Foreground = new SolidColorBrush(Color.FromRgb(139, 148, 158)),
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 10)
                });
                return;
            }

            foreach (var task in tasks)
            {
                Border row = BuildTaskRow(task);
                TasksListPanel.Children.Add(row);
            }
        }

        /// <summary>
        /// Builds a single row UI element for a task, showing its title,
        /// description, reminder date, and complete/delete buttons.
        /// </summary>
        private Border BuildTaskRow(CyberTask task)
        {
            StackPanel textStack = new StackPanel();

            TextBlock titleText = new TextBlock
            {
                Text = (task.IsCompleted ? "✅ " : "🔲 ") + task.Title,
                FontSize = 17,
                FontWeight = FontWeights.SemiBold,
                Foreground = task.IsCompleted
                    ? new SolidColorBrush(Color.FromRgb(139, 148, 158))
                    : Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };
            textStack.Children.Add(titleText);

            if (!string.IsNullOrWhiteSpace(task.Description))
            {
                textStack.Children.Add(new TextBlock
                {
                    Text = task.Description,
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromRgb(139, 148, 158)),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 2, 0, 0)
                });
            }

            if (task.ReminderDate.HasValue)
            {
                textStack.Children.Add(new TextBlock
                {
                    Text = "⏰ Reminder: " + task.ReminderDate.Value.ToString("dd MMM yyyy"),
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromRgb(88, 166, 255)),
                    Margin = new Thickness(0, 2, 0, 0)
                });
            }

            Grid rowGrid = new Grid();
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Grid.SetColumn(textStack, 0);
            rowGrid.Children.Add(textStack);

            if (!task.IsCompleted)
            {
                Button completeBtn = new Button
                {
                    Content = "Complete",
                    Width = 90,
                    Height = 34,
                    Margin = new Thickness(8, 0, 6, 0),
                    Background = new SolidColorBrush(Color.FromRgb(35, 134, 54)),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    VerticalAlignment = VerticalAlignment.Top
                };
                completeBtn.Click += (s, e) =>
                {
                    _engine.Tasks.MarkCompleted(task.Id);
                    _engine.Activity.Log($"Task #{task.Id} marked as completed.");
                    RenderTasksList();
                };
                Grid.SetColumn(completeBtn, 1);
                rowGrid.Children.Add(completeBtn);
            }

            Button deleteBtn = new Button
            {
                Content = "Delete",
                Width = 80,
                Height = 34,
                Background = new SolidColorBrush(Color.FromRgb(33, 38, 45)),
                Foreground = new SolidColorBrush(Color.FromRgb(248, 81, 73)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(48, 54, 61)),
                BorderThickness = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Top
            };
            deleteBtn.Click += (s, e) =>
            {
                _engine.Tasks.DeleteTask(task.Id);
                _engine.Activity.Log($"Task #{task.Id} deleted.");
                RenderTasksList();
            };
            Grid.SetColumn(deleteBtn, 2);
            rowGrid.Children.Add(deleteBtn);

            return new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(13, 17, 23)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14, 12, 14, 12),
                Margin = new Thickness(0, 0, 0, 10),
                Child = rowGrid
            };
        }

        /// <summary>
        /// Closes the tasks overlay and returns focus to the chat input.
        /// </summary>
        public void TasksCloseButton_Click(object sender, RoutedEventArgs e)
        {
            TasksOverlay.Visibility = Visibility.Collapsed;
            UserInput.Focus();
        }
    }
}
