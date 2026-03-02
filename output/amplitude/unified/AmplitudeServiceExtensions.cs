using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

namespace Amplitude.Maui
{
    public static class AmplitudeServiceExtensions
    {
        public static MauiAppBuilder UseAmplitude(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IAmplitudeService, AmplitudeService>();
            return builder;
        }
    }
}
