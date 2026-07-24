using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Delange.MessageBox.Maui;

/// <summary>
/// Presents dialogs from the currently active MAUI page on the UI thread.
/// </summary>
public sealed class MauiDialogPresenter : IMauiDialogPresenter
{
    public Task ShowAlertAsync(string title, string message, string dismissText) =>
        MainThread.InvokeOnMainThreadAsync(
            () => GetCurrentPage().DisplayAlertAsync(title, message, dismissText));

    public Task<bool> ShowConfirmationAsync(
        string title,
        string message,
        string acceptText,
        string cancelText) =>
        MainThread.InvokeOnMainThreadAsync(
            () => GetCurrentPage().DisplayAlertAsync(title, message, acceptText, cancelText));

    public Task<string?> ShowActionSheetAsync(
        string title,
        string cancelText,
        params string[] choices) =>
        MainThread.InvokeOnMainThreadAsync(
            async () => (string?)await GetCurrentPage()
                .DisplayActionSheetAsync(title, cancelText, destruction: null, choices));

    private static Page GetCurrentPage()
    {
        Page? page = Shell.Current?.CurrentPage ??
                     Application.Current?.Windows
                         .FirstOrDefault(window => window.Page is not null)?.Page;

        return page ?? throw new InvalidOperationException(
            "No active MAUI page is available to present a dialog.");
    }
}
