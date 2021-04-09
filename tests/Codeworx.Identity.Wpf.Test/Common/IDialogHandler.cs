using System.Threading.Tasks;

namespace Codeworx.Identity.Wpf.Test.Common
{
    public interface IDialogHandler
    {
        Task ShowMessageBoxAsync(string caption, string text, MessageBoxKind messageBoxKind);
    }
}
