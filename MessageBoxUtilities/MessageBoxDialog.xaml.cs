using System.Windows;

namespace Delange.MessageBox.Wpf;

/// <summary>
/// Interaction logic for MessageBoxDialog.xaml.
/// </summary>
public partial class MessageBoxDialog : Window
{
    public MessageDialogResult Result { get; private set; } = MessageDialogResult.None;

    public MessageBoxDialog(MessageDialogRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        InitializeComponent();

        MessageText.Text = request.Message;
        Title = request.Title;
        ConfigureIcon(request.Icon);
        ConfigureButtons(request);
        ConfigureDefaultButton(request.DefaultResult);
    }

    private void ConfigureIcon(MessageDialogIcon icon)
    {
        switch (icon)
        {
            case MessageDialogIcon.Information:
                InformationIcon.Visibility = Visibility.Visible;
                break;
            case MessageDialogIcon.Warning:
                WarningIcon.Visibility = Visibility.Visible;
                break;
            case MessageDialogIcon.Error:
                CriticalIcon.Visibility = Visibility.Visible;
                break;
            case MessageDialogIcon.Question:
                QuestionIcon.Visibility = Visibility.Visible;
                break;
        }
    }

    private void ConfigureButtons(MessageDialogRequest request)
    {
        OkButton.Content = request.OkText;
        CancelButton.Content = request.CancelText;
        YesButton.Content = request.YesText;
        NoButton.Content = request.NoText;

        switch (request.Buttons)
        {
            case MessageDialogButtons.OkCancel:
                OkButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
                CancelButton.IsCancel = true;
                break;
            case MessageDialogButtons.YesNo:
                YesButton.Visibility = Visibility.Visible;
                NoButton.Visibility = Visibility.Visible;
                NoButton.IsCancel = true;
                break;
            case MessageDialogButtons.YesNoCancel:
                YesButton.Visibility = Visibility.Visible;
                NoButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
                CancelButton.IsCancel = true;
                break;
            case MessageDialogButtons.Ok:
                OkButton.Visibility = Visibility.Visible;
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(request),
                    request.Buttons,
                    "Unsupported button combination.");
        }
    }

    private void ConfigureDefaultButton(MessageDialogResult defaultResult)
    {
        OkButton.IsDefault = defaultResult == MessageDialogResult.Ok;
        CancelButton.IsDefault = defaultResult == MessageDialogResult.Cancel;
        YesButton.IsDefault = defaultResult == MessageDialogResult.Yes;
        NoButton.IsDefault = defaultResult == MessageDialogResult.No;
    }

    private void OKClicked(object sender, RoutedEventArgs e) => CloseWith(MessageDialogResult.Ok);

    private void CancelClicked(object sender, RoutedEventArgs e) =>
        CloseWith(MessageDialogResult.Cancel);

    private void YesClicked(object sender, RoutedEventArgs e) => CloseWith(MessageDialogResult.Yes);

    private void NoClicked(object sender, RoutedEventArgs e) => CloseWith(MessageDialogResult.No);

    private void CloseWith(MessageDialogResult result)
    {
        Result = result;
        DialogResult = true;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        if (Result == MessageDialogResult.None)
        {
            Result = CancelButton.Visibility == Visibility.Visible
                ? MessageDialogResult.Cancel
                : MessageDialogResult.Ok;
        }
    }
}
