namespace Delange.MessageBox;

/// <summary>
/// Displays a message dialog without coupling callers to a UI framework.
/// </summary>
public interface IMessageDialogService
{
    Task<MessageDialogResult> ShowAsync(
        MessageDialogRequest request,
        CancellationToken cancellationToken = default);
}
