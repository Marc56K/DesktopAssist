using System.Runtime.InteropServices;

namespace AssistApp
{
    public partial class MainForm : Form
    {
        private SpellChecker spellChecker { get; } = new SpellChecker();

        public MainForm()
        {
            InitializeComponent();
            if (!RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, VK_X))
            {
                MessageBox.Show("Failed to register hotkey.");
            }
            updateTimer.Interval = 1000;
        }

        private void log(string message)
        {
            outputTextBox.AppendText(message.Replace("\n", Environment.NewLine));
            outputTextBox.AppendText(Environment.NewLine);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            updateTimer.Stop();
            UpdateSelectedText();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                updateTimer.Start();
            }

            base.WndProc(ref m); 
        }
        private void UpdateSelectedText()
        {
            log(DateTime.Now.ToString());
            try
            {
                // Save old clipboard
                IDataObject? oldData = Clipboard.GetDataObject();

                // Copy current selection
                SendKeys.SendWait("^c");
                Thread.Sleep(200);

                string selectedText = Clipboard.GetText();
                log("INPUT:\n" + selectedText);

                if (!string.IsNullOrEmpty(selectedText))
                {
                    // Modify text (example: uppercase)
                    string modifiedText = spellChecker.fixSpelling(selectedText);
                    log("OUTPUT:\n" + modifiedText);

                    // Put modified text in clipboard
                    Clipboard.SetText(modifiedText);

                    // Paste back into the app
                    SendKeys.SendWait("^v");
                }

                // Restore clipboard
                if (oldData != null)
                {
                    Thread.Sleep(500);
                    Clipboard.SetDataObject(oldData);
                }
            }
            catch (Exception ex)
            {
                log("ERROR: " + ex.ToString());
            }
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint VK_X = 0x58;
        private const int HOTKEY_ID = 9000;
        private const int WM_HOTKEY = 0x0312;
    }
}
