using Gtk;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    /// <summary>
    /// A Gtk interface for dialog boxes
    /// </summary>
    class DialogBox
    {
        public MessageDialog dialog = null;

        private Button okbtn = null;

        public DialogBox(string message, string icon = "gtk-dialog-warning")
        {
            Builder builder = Tools.ReadGlade("DialogBox");

            // The ok button
            okbtn = (Button)builder.GetObject("okbtn");
            okbtn.Clicked += OnOkClicked;

            // The message in the box
            dialog = (MessageDialog)builder.GetObject("dialog");          
            dialog.Text = message;
            dialog.Image = new Image(icon,  IconSize.Dialog);
            dialog.KeepAbove = true;
            dialog.ShowAll();

            builder.Dispose();
        }

        /// <summary>
        /// Handles the ok button clicked event
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        private void OnOkClicked(object sender, EventArgs e)
        {
            dialog.Destroy();
        }
    }
}
