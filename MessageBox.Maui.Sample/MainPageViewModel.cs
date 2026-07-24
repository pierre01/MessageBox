using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Delange.MessageBox;

namespace MessageBox.Maui.Sample;

public partial class MainPageViewModel : ObservableObject
{
    private readonly IMessageDialogService _dialogs;

    [ObservableProperty]
    public partial string DialogResult { get; set; } = "No dialog shown yet";

    [ObservableProperty]
    public partial string LastScenario { get; set; } = "Choose a scenario above to begin.";

    public MainPageViewModel(IMessageDialogService dialogs)
    {
        _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
    }

    [RelayCommand]
    private Task ErrorAsync() =>
        ShowAndRecordAsync("Three-choice error dialog", new(
            "Press Yes to override data, No to discard your changes, or Cancel to return.")
        {
            Title = "Emergency data decision",
            Buttons = MessageDialogButtons.YesNoCancel,
            Icon = MessageDialogIcon.Error,
            DefaultResult = MessageDialogResult.Yes
        });

    [RelayCommand]
    private Task WarningAsync() =>
        ShowAndRecordAsync("Confirmation warning", new("Closing will discard your changes.")
        {
            Title = "Warning: closing",
            Buttons = MessageDialogButtons.OkCancel,
            Icon = MessageDialogIcon.Warning
        });

    [RelayCommand]
    private Task QuestionAsync() =>
        ShowAndRecordAsync("Yes/No question", new("Can I call you Jim?")
        {
            Title = "Confirm how I address you",
            Buttons = MessageDialogButtons.YesNo,
            Icon = MessageDialogIcon.Question,
            DefaultResult = MessageDialogResult.No
        });

    [RelayCommand]
    private Task InformationAsync() =>
        ShowAndRecordAsync("Information alert", new(
            "The project has been closed and your database restored.")
        {
            Title = "Complete",
            Icon = MessageDialogIcon.Information
        });

    [RelayCommand]
    private Task SimpleAsync() =>
        ShowAndRecordAsync("Simple alert", new(
            "The application is closing. This might take a few seconds."));

    [RelayCommand]
    private async Task ComplexPathAsync()
    {
        int path = await RunComplexPathAsync();
        await ShowAndRecordAsync(
            "Complex workflow",
            new($"You reached exit path {path}.") { Title = "Path complete" });
    }

    private async Task<int> RunComplexPathAsync()
    {
        MessageDialogResult result = await ShowAsync(
            "Please answer.",
            "MessageBox 1",
            MessageDialogButtons.YesNo);

        if (result == MessageDialogResult.Yes)
        {
            await ShowAsync("Continue to the next decision.", "MessageBox 2");
            result = await ShowAsync(
                "Are you sure you want to continue?",
                "For sure?",
                MessageDialogButtons.YesNo);

            if (result == MessageDialogResult.Yes)
            {
                result = await ShowAsync(
                    "Choose OK or Cancel.",
                    "MessageBox 6",
                    MessageDialogButtons.OkCancel);
                return result == MessageDialogResult.Ok ? 1 : 2;
            }

            await ShowAsync("This branch ends here.", "MessageBox 7");
            return 3;
        }

        result = await ShowAsync(
            "Choose OK to continue or Cancel to exit.",
            "MessageBox 3",
            MessageDialogButtons.OkCancel);

        if (result == MessageDialogResult.Cancel)
        {
            return 9;
        }

        result = await ShowAsync(
            "Choose another branch.",
            "MessageBox 5",
            MessageDialogButtons.OkCancel);

        if (result == MessageDialogResult.Cancel)
        {
            result = await ShowAsync(
                "Choose Yes or No.",
                "MessageBox 8",
                MessageDialogButtons.YesNo);
            return result == MessageDialogResult.Yes ? 4 : 5;
        }

        result = await ShowAsync(
            "Choose the final exit.",
            "MessageBox 9",
            MessageDialogButtons.YesNoCancel);

        return result switch
        {
            MessageDialogResult.Yes => 6,
            MessageDialogResult.No => 7,
            _ => 8
        };
    }

    private async Task ShowAndRecordAsync(string scenario, MessageDialogRequest request)
    {
        LastScenario = scenario;
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
