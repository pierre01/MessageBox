namespace Delange.MessageBox;

/// <summary>
/// Platform-neutral description of a message dialog.
/// </summary>
public sealed record MessageDialogRequest
{
    public MessageDialogRequest(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Message = message;
    }

    public string Message { get; }

    public string Title { get; init; } = string.Empty;

    public MessageDialogButtons Buttons { get; init; } = MessageDialogButtons.Ok;

    public MessageDialogIcon Icon { get; init; } = MessageDialogIcon.None;

    public MessageDialogResult DefaultResult { get; init; } = MessageDialogResult.Ok;

    public string OkText { get; init; } = "OK";

    public string CancelText { get; init; } = "Cancel";

    public string YesText { get; init; } = "Yes";

    public string NoText { get; init; } = "No";
}
