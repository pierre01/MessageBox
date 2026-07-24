using System.Windows;

namespace Delange.MessageBox;

public class MessageBoxService : IMessageBoxService
{
    /// <summary>
    /// Display a simple MessageBox with an OK button.
    /// </summary>
    public MessageBoxServiceResult Show(string messageBoxText) =>
        Show(messageBoxText, string.Empty);

    /// <summary>
    /// Display a simple MessageBox with an OK button.
    /// </summary>
    public MessageBoxServiceResult Show(string messageBoxText, string caption)
    {
        ShowDialog(owner: null, messageBoxText, caption);
        return MessageBoxServiceResult.OK;
    }

    /// <summary>
    /// Display a MessageBox and return the button that was clicked.
    /// </summary>
    public MessageBoxServiceResult Show(
        string messageBoxText,
        string caption,
        MessageBoxServiceButton button) =>
        ShowDialog(owner: null, messageBoxText, caption, button);

    /// <summary>
    /// Display a MessageBox and return the button that was clicked.
    /// </summary>
    public MessageBoxServiceResult Show(
        string messageBoxText,
        string caption,
        MessageBoxServiceButton button,
        MessageBoxServiceIcon icon) =>
        ShowDialog(owner: null, messageBoxText, caption, button, icon);

    /// <summary>
    /// Display a MessageBox and return the button that was clicked.
    /// </summary>
    public MessageBoxServiceResult Show(
        string messageBoxText,
        string caption,
        MessageBoxServiceButton button,
        MessageBoxServiceIcon icon,
        MessageBoxServiceResult defaultButton) =>
        ShowDialog(owner: null, messageBoxText, caption, button, icon, defaultButton);

    /// <summary>
    /// Display a MessageBox in front of the specified window.
    /// </summary>
    public MessageBoxServiceResult Show(Window owner, string messageBoxText) =>
        ShowDialog(owner, messageBoxText);

    /// <summary>
    /// Display a MessageBox in front of the specified window.
    /// </summary>
    public MessageBoxServiceResult Show(Window owner, string messageBoxText, string caption) =>
        ShowDialog(owner, messageBoxText, caption);

    /// <summary>
    /// Display a MessageBox in front of the specified window.
    /// </summary>
    public MessageBoxServiceResult Show(
        Window owner,
        string messageBoxText,
        string caption,
        MessageBoxServiceButton button) =>
        ShowDialog(owner, messageBoxText, caption, button);

    /// <summary>
    /// Display a MessageBox in front of the specified window.
    /// </summary>
    public MessageBoxServiceResult Show(
        Window owner,
        string messageBoxText,
        string caption,
        MessageBoxServiceButton button,
        MessageBoxServiceIcon icon) =>
        ShowDialog(owner, messageBoxText, caption, button, icon);

    /// <summary>
    /// Display a MessageBox in front of the specified window.
    /// </summary>
    public MessageBoxServiceResult Show(
        Window owner,
        string messageBoxText,
        string caption,
        MessageBoxServiceButton button,
        MessageBoxServiceIcon icon,
        MessageBoxServiceResult defaultButton) =>
        ShowDialog(owner, messageBoxText, caption, button, icon, defaultButton);

    private static MessageBoxServiceResult ShowDialog(
        Window? owner,
        string messageBoxText,
        string caption = "",
        MessageBoxServiceButton button = MessageBoxServiceButton.Ok,
        MessageBoxServiceIcon icon = MessageBoxServiceIcon.None,
        MessageBoxServiceResult defaultButton = MessageBoxServiceResult.OK)
    {
        MessageBoxDialog dialog = new(messageBoxText, caption, button, icon, defaultButton);

        if (owner is not null)
        {
            dialog.Owner = owner;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        dialog.ShowDialog();
        return dialog.Result;
    }
}
