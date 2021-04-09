using System;
using System.Threading.Tasks;
using System.Windows;
using Codeworx.Identity.Wpf.Test.Common;

namespace Codeworx.Identity.Wpf.Test
{
    public class WpfDialogHandler : IDialogHandler
    {
        public WpfDialogHandler()
        {
        }

        public Task ShowMessageBoxAsync(string caption, string text, MessageBoxKind messageBoxKind)
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, GetMessageBoxImage(messageBoxKind));
            return Task.CompletedTask;
        }

        private MessageBoxImage GetMessageBoxImage(MessageBoxKind messageBoxKind)
        {
            switch (messageBoxKind)
            {
                case MessageBoxKind.Info:
                    return MessageBoxImage.Information;
                case MessageBoxKind.Warning:
                    return MessageBoxImage.Warning;
                case MessageBoxKind.Error:
                    return MessageBoxImage.Error;
            }

            throw new NotSupportedException("This should not happen!!!");
        }
    }
}