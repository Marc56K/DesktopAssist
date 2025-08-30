namespace AssistApp
{
    public partial class MainForm : Form
    {
        private SpellChecker _spellChecker;
        private Logger _logger;
        private Dictionary<Hotkey, string> _systemPromptsByHotkey = new Dictionary<Hotkey, string>();

        public MainForm()
        {
            InitializeComponent();
            _logger = new Logger(this.HandleLogMessage);
            _spellChecker = new SpellChecker(_logger);
            AddHotkey(0, Properties.Settings.Default.P0_HotKey, Properties.Settings.Default.P0_SystemPrompt);
            AddHotkey(1, Properties.Settings.Default.P1_HotKey, Properties.Settings.Default.P1_SystemPrompt);
            _spellChecker.PullModel();
        }

        private void AddHotkey(int id, string hotkey, string systemPrompt)
        {
            _systemPromptsByHotkey.Add(new Hotkey(_logger, this.Handle, id, hotkey, HandleHotkeyPressed), systemPrompt);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var hotkey in _systemPromptsByHotkey.Keys)
            {
                hotkey.Dispose();
            }
        }

        private void HandleLogMessage(string message)
        {
            var logFunc = () =>
            {
                OutputTextBox.AppendText(message.Replace("\n", Environment.NewLine));
                OutputTextBox.AppendText(Environment.NewLine);
            };

            if (OutputTextBox.InvokeRequired)
                OutputTextBox.Invoke(new Action(logFunc));
            else
                logFunc();
        }

        private void HandleHotkeyPressed(Hotkey hotkey)
        {
            Task.Delay(Properties.Settings.Default.CopyPasteDelay).ContinueWith(_ =>
            {
                if (!this.IsDisposed && !this.Disposing)
                {
                    this.Invoke(new Action<Hotkey>(UpdateSelectedText), hotkey);
                }
            });
        }

        protected override void WndProc(ref Message m)
        {
            foreach (var hotkey in _systemPromptsByHotkey.Keys)
            {
                hotkey.HandleWindowMessage(ref m);
            }            
            base.WndProc(ref m);
        }

        private void UpdateSelectedText(Hotkey hotkey)
        {
            _logger.Info(DateTime.Now.ToString());
            try
            {
                // Copy current selection
                SendKeys.SendWait("^c");
                Thread.Sleep(200);

                string selectedText = Clipboard.GetText();
                _logger.Info("INPUT:\n" + selectedText);

                if (!string.IsNullOrEmpty(selectedText))
                {
                    // Modify text (example: uppercase)
                    string systemPrompt = _systemPromptsByHotkey[hotkey];
                    string modifiedText = _spellChecker.FixSpelling(systemPrompt, selectedText);
                    _logger.Info("OUTPUT:\n" + modifiedText);

                    // Put modified text in clipboard
                    Clipboard.SetText(modifiedText);

                    // Paste back into the app
                    SendKeys.SendWait("^v");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }
    }
}
