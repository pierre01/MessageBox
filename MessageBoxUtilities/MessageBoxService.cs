using System.Windows;

namespace Delange.MessageBox.Wpf;

/// <summary>
/// Displays the shared message-dialog contract using the custom WPF dialog.
/// </summary>
public sealed class WpfMessageDialogService : IMessageDialogService
{
    public Task<MessageDialogResult> ShowAsync(
        MessageDialogRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        MessageBoxDialog dialog = new(request);

        if (Application.Current?.MainWindow is { } owner)
        {
            dialog.Owner = owner;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        dialog.ShowDialog();
        return Task.FromResult(dialog.Result);
    }
}
