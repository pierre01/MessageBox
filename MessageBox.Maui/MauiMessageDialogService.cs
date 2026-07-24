using Delange.MessageBox;

namespace Delange.MessageBox.Maui;

/// <summary>
/// Serializes and maps message requests to platform-native MAUI dialogs.
/// </summary>
public sealed class MauiMessageDialogService : IMessageDialogService, IDisposable
{
    private readonly IMauiDialogPresenter _presenter;
    private readonly SemaphoreSlim _dialogLock = new(1, 1);

    public MauiMessageDialogService(IMauiDialogPresenter presenter)
    {
        _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
    }

    public async Task<MessageDialogResult> ShowAsync(
        MessageDialogRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateLabels(request);

        await _dialogLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await ShowCoreAsync(request).ConfigureAwait(false);
        }
        finally
        {
            _dialogLock.Release();
        }
    }

    public void Dispose() => _dialogLock.Dispose();

    private async Task<MessageDialogResult> ShowCoreAsync(MessageDialogRequest request)
    {
        switch (request.Buttons)
        {
            case MessageDialogButtons.Ok:
                await _presenter
                    .ShowAlertAsync(request.Title, request.Message, request.OkText)
                    .ConfigureAwait(false);
                return MessageDialogResult.Ok;

            case MessageDialogButtons.OkCancel:
                return await _presenter
                    .ShowConfirmationAsync(
                        request.Title,
                        request.Message,
                        request.OkText,
                        request.CancelText)
                    .ConfigureAwait(false)
                        ? MessageDialogResult.Ok
                        : MessageDialogResult.Cancel;

            case MessageDialogButtons.YesNo:
                return await _presenter
                    .ShowConfirmationAsync(
                        request.Title,
                        request.Message,
                        request.YesText,
                        request.NoText)
                    .ConfigureAwait(false)
                        ? MessageDialogResult.Yes
                        : MessageDialogResult.No;

            case MessageDialogButtons.YesNoCancel:
                string? selection = await _presenter
                    .ShowActionSheetAsync(
                        BuildActionSheetTitle(request),
                        request.CancelText,
                        request.YesText,
                        request.NoText)
                    .ConfigureAwait(false);

                if (selection == request.YesText)
                {
                    return MessageDialogResult.Yes;
                }

                return selection == request.NoText
                    ? MessageDialogResult.No
                    : MessageDialogResult.Cancel;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(request),
                    request.Buttons,
                    "Unsupported button combination.");
        }
    }

    private static string BuildActionSheetTitle(MessageDialogRequest request) =>
        string.IsNullOrWhiteSpace(request.Title)
            ? request.Message
            : $"{request.Title}{Environment.NewLine}{request.Message}";

    private static void ValidateLabels(MessageDialogRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.OkText);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CancelText);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.YesText);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.NoText);

        if (request.Buttons == MessageDialogButtons.YesNoCancel &&
            (request.YesText == request.NoText ||
             request.YesText == request.CancelText ||
             request.NoText == request.CancelText))
        {
            throw new ArgumentException(
                "Yes, No, and Cancel labels must be unique for a YesNoCancel dialog.",
                nameof(request));
        }
    }
}
