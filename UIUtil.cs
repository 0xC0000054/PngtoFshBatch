using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows.Forms;

namespace PngtoFshBatchtxt
{
    internal static class UIUtil
    {
        private static readonly bool TaskDialogSupported = CheckTaskDialogSupported();

        private static bool CheckTaskDialogSupported()
        {
            OperatingSystem os = Environment.OSVersion;

            return (os.Platform == PlatformID.Win32NT && os.Version.Major >= 6);
        }

        private static void dialog_Opened(object sender, EventArgs e)
        {
            // The TaskDialog wrapper API has a bug, so we must reset the icon in the Opened event for it to be shown.
            TaskDialog taskDialog = (TaskDialog)sender;
            taskDialog.Icon = taskDialog.Icon;
            taskDialog.InstructionText = taskDialog.InstructionText;
        }

        private static DialogResult ErrorTaskDialog(IWin32Window owner, string message, string caption, TaskDialogStandardIcon icon)
        {
            DialogResult result = DialogResult.None;

            using (TaskDialog dialog = new TaskDialog())
            {
                dialog.Opened += new EventHandler(dialog_Opened);
                dialog.Cancelable = true;

                if (owner != null)
                {
                    dialog.OwnerWindowHandle = owner.Handle;
                    dialog.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                }

                dialog.StandardButtons = TaskDialogStandardButtons.Ok;
                dialog.Icon = icon;
                dialog.InstructionText = message;
                dialog.Caption = caption;

                dialog.Show();

                result = DialogResult.OK;
            }

            return result;
        }

        public static DialogResult ShowErrorMessage(IWin32Window owner, string message, string caption)
        {
            if (TaskDialogSupported)
            {
                return ErrorTaskDialog(owner, message, caption, TaskDialogStandardIcon.Error);
            }
            else
            {
                return MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
            }
        }

        public static DialogResult ShowWarningMessage(IWin32Window owner, string message, string caption)
        {
            if (TaskDialogSupported)
            {
                return ErrorTaskDialog(owner, message, caption, TaskDialogStandardIcon.Warning);
            }
            else
            {
                return MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, 0);
            }
        }
    }
}
