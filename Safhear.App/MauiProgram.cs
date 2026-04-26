using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace Safhear;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMaui();

        builder.Services.AddSingleton(AudioManager.Current);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
