namespace AssistApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            outputTextBox = new TextBox();
            updateTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // outputTextBox
            // 
            outputTextBox.Dock = DockStyle.Fill;
            outputTextBox.Location = new Point(0, 0);
            outputTextBox.Multiline = true;
            outputTextBox.Name = "outputTextBox";
            outputTextBox.ScrollBars = ScrollBars.Vertical;
            outputTextBox.Size = new Size(800, 450);
            outputTextBox.TabIndex = 0;
            // 
            // updateTimer
            // 
            updateTimer.Tick += updateTimer_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(outputTextBox);
            Name = "MainForm";
            Text = "DesktopAssist - SpellChecker";
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox outputTextBox;
        private System.Windows.Forms.Timer updateTimer;
    }
}
