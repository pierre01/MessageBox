namespace Delange.MessageBox.Maui;

/// <summary>
/// Adapts the MAUI page APIs. Public to support replacement and focused unit testing.
/// </summary>
public interface IMauiDialogPresenter
{
    Task ShowAlertAsync(string title, string message, string dismissText);

    Task<bool> ShowConfirmationAsync(
        string title,
        string message,
        string acceptText,
        string cancelText);

    Task<string?> ShowActionSheetAsync(
        string title,
        string cancelText,
        params string[] choices);
}
