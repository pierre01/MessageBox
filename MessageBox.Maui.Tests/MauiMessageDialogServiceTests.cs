using Delange.MessageBox;
using Delange.MessageBox.Maui;

namespace MessageBox.Maui.Tests;

public sealed class MauiMessageDialogServiceTests
{
    [Fact]
    public async Task OkDialogReturnsOk()
    {
        FakePresenter presenter = new();
        using MauiMessageDialogService service = new(presenter);

        MessageDialogResult result = await service.ShowAsync(new("Saved"));

        Assert.Equal(MessageDialogResult.Ok, result);
        Assert.Equal(("Saved", "OK"), (presenter.Message, presenter.DismissText));
    }

    [Theory]
    [InlineData(true, MessageDialogResult.Ok)]
    [InlineData(false, MessageDialogResult.Cancel)]
    public async Task OkCancelMapsNativeResponse(
        bool nativeResponse,
        MessageDialogResult expected)
    {
        FakePresenter presenter = new() { ConfirmationResult = nativeResponse };
        using MauiMessageDialogService service = new(presenter);

        MessageDialogResult result = await service.ShowAsync(new("Continue?")
        {
            Buttons = MessageDialogButtons.OkCancel
        });

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(true, MessageDialogResult.Yes)]
    [InlineData(false, MessageDialogResult.No)]
    public async Task YesNoMapsNativeResponse(
        bool nativeResponse,
        MessageDialogResult expected)
    {
        FakePresenter presenter = new() { ConfirmationResult = nativeResponse };
        using MauiMessageDialogService service = new(presenter);

        MessageDialogResult result = await service.ShowAsync(new("Continue?")
        {
            Buttons = MessageDialogButtons.YesNo
        });

        Assert.Equal(expected, result);
        Assert.Equal(("Yes", "No"), (presenter.AcceptText, presenter.CancelText));
    }

    [Theory]
    [InlineData("Yes", MessageDialogResult.Yes)]
    [InlineData("No", MessageDialogResult.No)]
    [InlineData("Cancel", MessageDialogResult.Cancel)]
    [InlineData(null, MessageDialogResult.Cancel)]
    public async Task YesNoCancelMapsActionSheetSelection(
        string? selection,
        MessageDialogResult expected)
    {
        FakePresenter presenter = new() { ActionSheetResult = selection };
        using MauiMessageDialogService service = new(presenter);

        MessageDialogResult result = await service.ShowAsync(new("Choose")
        {
            Title = "Question",
            Buttons = MessageDialogButtons.YesNoCancel
        });

        Assert.Equal(expected, result);
        Assert.Equal($"Question{Environment.NewLine}Choose", presenter.Title);
    }

    [Fact]
    public async Task CustomLabelsArePassedToPresenter()
    {
        FakePresenter presenter = new() { ConfirmationResult = true };
        using MauiMessageDialogService service = new(presenter);

        await service.ShowAsync(new("Delete?")
        {
            Title = "Confirm",
            Buttons = MessageDialogButtons.YesNo,
            YesText = "Delete",
            NoText = "Keep"
        });

        Assert.Equal("Confirm", presenter.Title);
        Assert.Equal(("Delete", "Keep"), (presenter.AcceptText, presenter.CancelText));
    }

    [Fact]
    public async Task CancelledRequestDoesNotShowDialog()
    {
        FakePresenter presenter = new();
        using MauiMessageDialogService service = new(presenter);
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(
            () => service.ShowAsync(new("Never shown"), cancellation.Token));

        Assert.Equal(0, presenter.CallCount);
    }

    [Fact]
    public async Task ConcurrentDialogsAreSerialized()
    {
        BlockingPresenter presenter = new();
        using MauiMessageDialogService service = new(presenter);

        Task<MessageDialogResult> first = service.ShowAsync(new("First"));
        await presenter.FirstCallStarted.Task;
        Task<MessageDialogResult> second = service.ShowAsync(new("Second"));

        Assert.Equal(1, presenter.CallCount);

        presenter.ReleaseFirstCall.SetResult();
        await Task.WhenAll(first, second);

        Assert.Equal(2, presenter.CallCount);
    }

    private class FakePresenter : IMauiDialogPresenter
    {
        public bool ConfirmationResult { get; init; }
        public string? ActionSheetResult { get; init; }
        public int CallCount { get; protected set; }
        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public string DismissText { get; private set; } = string.Empty;
        public string AcceptText { get; private set; } = string.Empty;
        public string CancelText { get; private set; } = string.Empty;

        public virtual Task ShowAlertAsync(string title, string message, string dismissText)
        {
            CallCount++;
            Title = title;
            Message = message;
            DismissText = dismissText;
            return Task.CompletedTask;
        }

        public Task<bool> ShowConfirmationAsync(
            string title,
            string message,
            string acceptText,
            string cancelText)
        {
            CallCount++;
            Title = title;
            Message = message;
            AcceptText = acceptText;
            CancelText = cancelText;
            return Task.FromResult(ConfirmationResult);
        }

        public Task<string?> ShowActionSheetAsync(
            string title,
            string cancelText,
            params string[] choices)
        {
            CallCount++;
            Title = title;
            CancelText = cancelText;
            return Task.FromResult(ActionSheetResult);
        }
    }

    private sealed class BlockingPresenter : FakePresenter
    {
        public TaskCompletionSource FirstCallStarted { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource ReleaseFirstCall { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override async Task ShowAlertAsync(
            string title,
            string message,
            string dismissText)
        {
            await base.ShowAlertAsync(title, message, dismissText);

            if (CallCount == 1)
            {
                FirstCallStarted.SetResult();
                await ReleaseFirstCall.Task;
            }
        }
    }
}
