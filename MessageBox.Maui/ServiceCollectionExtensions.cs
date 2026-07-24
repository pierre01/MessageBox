using Delange.MessageBox;
using Microsoft.Extensions.DependencyInjection;

namespace Delange.MessageBox.Maui;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the native MAUI message dialog service.
    /// </summary>
    public static IServiceCollection AddMauiMessageDialogs(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IMauiDialogPresenter, MauiDialogPresenter>();
        services.AddSingleton<IMessageDialogService, MauiMessageDialogService>();
        return services;
    }
}
