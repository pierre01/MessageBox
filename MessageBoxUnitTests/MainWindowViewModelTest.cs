using CommunityToolkit.Mvvm.Input;
using Delange.MessageBox;
using MessageBox.Wpf.Sample.ViewModels;

namespace MessageBoxUnitTests;

public sealed class MainWindowViewModelTest
{
    public static TheoryData<MessageDialogResult[], int> ComplexPaths => new()
    {
        { [MessageDialogResult.Yes, MessageDialogResult.Ok, MessageDialogResult.Yes, MessageDialogResult.Ok], 1 },
        { [MessageDialogResult.Yes, MessageDialogResult.Ok, MessageDialogResult.Yes, MessageDialogResult.Cancel], 2 },
        { [MessageDialogResult.Yes, MessageDialogResult.Ok, MessageDialogResult.No, MessageDialogResult.Ok], 3 },
        { [MessageDialogResult.No, MessageDialogResult.Ok, MessageDialogResult.Cancel, MessageDialogResult.Yes], 4 },
        { [MessageDialogResult.No, MessageDialogResult.Ok, MessageDialogResult.Cancel, MessageDialogResult.No], 5 },
        { [MessageDialogResult.No, MessageDialogResult.Ok, MessageDialogResult.Ok, MessageDialogResult.Yes], 6 },
        { [MessageDialogResult.No, MessageDialogResult.Ok, MessageDialogResult.Ok, MessageDialogResult.No], 7 },
        { [MessageDialogResult.No, MessageDialogResult.Ok, MessageDialogResult.Ok, MessageDialogResult.Cancel], 8 },
        { [MessageDialogResult.No, MessageDialogResult.Cancel], 9 }
    };

    [Theory]
    [MemberData(nameof(ComplexPaths))]
    public async Task ComplexPathReturnsExpectedExit(
        MessageDialogResult[] responses,
        int expected)
    {
        FakeMessageDialogService dialogs = new(responses);
        MainWindowViewModel viewModel = new(dialogs);

        int result = await viewModel.ComplexPathWithMessageBoxesAsync();

        Assert.Equal(expected, result);
        Assert.Empty(dialogs.Responses);
    }

    [Fact]
    public async Task ComplexPathCommandDisplaysReturnedPath()
    {
        FakeMessageDialogService dialogs = new(
            MessageDialogResult.Yes,
            MessageDialogResult.Ok,
            MessageDialogResult.Yes,
            MessageDialogResult.Ok,
            MessageDialogResult.Ok);
        MainWindowViewModel viewModel = new(dialogs);

        await viewModel.ComplexPathCommand.ExecuteAsync(null);

        Assert.Equal("The Path returned 1", dialogs.LastRequest?.Message);
    }

    [Theory]
    [InlineData(1, MessageDialogButtons.YesNoCancel, MessageDialogIcon.Error)]
    [InlineData(2, MessageDialogButtons.OkCancel, MessageDialogIcon.Warning)]
    [InlineData(3, MessageDialogButtons.YesNo, MessageDialogIcon.Question)]
    [InlineData(4, MessageDialogButtons.Ok, MessageDialogIcon.Information)]
    [InlineData(5, MessageDialogButtons.Ok, MessageDialogIcon.None)]
    public async Task ButtonCommandsUseExpectedDialogConfiguration(
        int commandNumber,
        MessageDialogButtons expectedButtons,
        MessageDialogIcon expectedIcon)
    {
        FakeMessageDialogService dialogs = new(MessageDialogResult.Ok);
        MainWindowViewModel viewModel = new(dialogs);

        IAsyncRelayCommand command = commandNumber switch
        {
            1 => viewModel.Button1Command,
            2 => viewModel.Button2Command,
            3 => viewModel.Button3Command,
            4 => viewModel.Button4Command,
            5 => viewModel.Button5Command,
            _ => throw new ArgumentOutOfRangeException(nameof(commandNumber))
        };

        await command.ExecuteAsync(null);

        Assert.Equal(expectedButtons, dialogs.LastRequest?.Buttons);
        Assert.Equal(expectedIcon, dialogs.LastRequest?.Icon);
        Assert.Equal(MessageDialogResult.Ok.ToString(), viewModel.DialogResult);
    }

    private sealed class FakeMessageDialogService : IMessageDialogService
    {
        public FakeMessageDialogService(params MessageDialogResult[] responses)
        {
            Responses = new Queue<MessageDialogResult>(responses);
        }

        public Queue<MessageDialogResult> Responses { get; }

        public MessageDialogRequest? LastRequest { get; private set; }

        public Task<MessageDialogResult> ShowAsync(
            MessageDialogRequest request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LastRequest = request;
            return Task.FromResult(Responses.Dequeue());
        }
    }
}
