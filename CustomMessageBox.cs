using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ABT.Test.Lib {
    public partial class CustomMessageBox : Form {
        public CustomMessageBox() {
            InitializeComponent();
        }

        public static void Show(String Title, String Message, Icon OptionalIcon = null) {
            CustomMessageBox cms = new CustomMessageBox {
                Text = Title,
                Icon = (OptionalIcon is null ? SystemIcons.Information : OptionalIcon),

            };
            cms.richTextBox.Text = Message;
            cms.ShowDialog();
        }

        private void ButtonClipboard_Click(Object sender, EventArgs e) { Clipboard.SetText(richTextBox.Text); }

        private void RichTextBox_LinkClicked(Object sender, LinkClickedEventArgs e) {
            Process.Start(new ProcessStartInfo(e.LinkText) { UseShellExecute = true });
        }
    }
}
