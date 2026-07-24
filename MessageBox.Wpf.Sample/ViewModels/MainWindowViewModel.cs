using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Delange.MessageBox;

namespace MessageBox.Wpf.Sample.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IMessageDialogService _dialogs;

    [ObservableProperty]
    private string _dialogResult = string.Empty;

    public MainWindowViewModel(IMessageDialogService dialogs)
    {
        _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));

        Button1Command = new AsyncRelayCommand(Button1ClickAsync);
        Button2Command = new AsyncRelayCommand(Button2ClickAsync);
        Button3Command = new AsyncRelayCommand(Button3ClickAsync);
        Button4Command = new AsyncRelayCommand(Button4ClickAsync);
        Button5Command = new AsyncRelayCommand(Button5ClickAsync);
        ComplexPathCommand = new AsyncRelayCommand(ExecuteComplexPathAsync);
    }

    public IAsyncRelayCommand Button1Command { get; }

    public IAsyncRelayCommand Button2Command { get; }

    public IAsyncRelayCommand Button3Command { get; }

    public IAsyncRelayCommand Button4Command { get; }

    public IAsyncRelayCommand Button5Command { get; }

    public IAsyncRelayCommand ComplexPathCommand { get; }

    private async Task ExecuteComplexPathAsync()
    {
        int path = await ComplexPathWithMessageBoxesAsync();
        await _dialogs.ShowAsync(new($"The Path returned {path}"));
    }

    /// <summary>
    /// A logical path involving multiple decisions triggered by message dialogs.
    /// </summary>
    public async Task<int> ComplexPathWithMessageBoxesAsync()
    {
        MessageDialogResult result = await ShowAsync(
            "Please Answer",
            "MessageBox 1",
            MessageDialogButtons.YesNo);

        if (result == MessageDialogResult.Yes)
        {
            await ShowAsync("Please Answer", "MessageBox 2");
            result = await ShowAsync("Let's go", "For Sure", MessageDialogButtons.YesNo);

            if (result == MessageDialogResult.Yes)
            {
                result = await ShowAsync(
                    "Let's go",
                    "MessageBox 6",
                    MessageDialogButtons.OkCancel);
                return result == MessageDialogResult.Ok ? 1 : 2;
            }

            await ShowAsync("Let's go", "MessageBox 7");
            return 3;
        }

        result = await ShowAsync(
            "Let's go",
            "MessageBox 3",
            MessageDialogButtons.OkCancel);

        if (result == MessageDialogResult.Cancel)
        {
            return 9;
        }

        result = await ShowAsync(
            "Let's go",
            "MessageBox 5",
            MessageDialogButtons.OkCancel);

        if (result == MessageDialogResult.Cancel)
        {
            result = await ShowAsync(
                "Let's go",
                "MessageBox 8",
                MessageDialogButtons.YesNo);
            return result == MessageDialogResult.Yes ? 4 : 5;
        }

        result = await ShowAsync(
            "Let's go",
            "MessageBox 9",
            MessageDialogButtons.YesNoCancel);

        return result switch
        {
            MessageDialogResult.Yes => 6,
            MessageDialogResult.No => 7,
            _ => 8
        };
    }

    private async Task Button5ClickAsync() =>
        await ShowAndRecordAsync(new(
            "The application is closing, this might take a few seconds"));

    private async Task Button4ClickAsync() =>
        await ShowAndRecordAsync(new(
            "The project has been closed and your Database restored")
        {
            Title = "Closing",
            Icon = MessageDialogIcon.Information
        });

    private async Task Button3ClickAsync() =>
        await ShowAndRecordAsync(new("Hi James, Can I call you Jim?")
        {
            Title = "Confirm how I call you",
            Buttons = MessageDialogButtons.YesNo,
            Icon = MessageDialogIcon.Question,
            DefaultResult = MessageDialogResult.No
        });

    private async Task Button2ClickAsync() =>
        await ShowAndRecordAsync(new("Closing will discard your changes")
        {
            Title = "Warning: closing",
            Buttons = MessageDialogButtons.OkCancel,
            Icon = MessageDialogIcon.Warning
        });

    private async Task Button1ClickAsync() =>
        await ShowAndRecordAsync(new(
            $"Press Yes to override data, No to discard your changes...{Environment.NewLine}" +
            "(Press Cancel to go back to input screen)")
        {
            Title = "Emergency caption of the dialog",
            Buttons = MessageDialogButtons.YesNoCancel,
            Icon = MessageDialogIcon.Error,
            DefaultResult = MessageDialogResult.Yes
        });

    private async Task ShowAndRecordAsync(MessageDialogRequest request)
    {
        MessageDialogResult result = await _dialogs.ShowAsync(request);
        DialogResult = result.ToString();
    }

    private Task<MessageDialogResult> ShowAsync(
        string message,
        string title,
        MessageDialogButtons buttons = MessageDialogButtons.Ok) =>
        _dialogs.ShowAsync(new(message)
        {
            Title = title,
            Buttons = buttons
        });
}
