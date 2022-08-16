using Microsoft.AspNetCore.Components.WebView.Maui;
using ManewryMorskieRazor;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace ManewryMorskie.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            #region config
            Assembly a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("ManewryMorskie.MAUI.appsettings.json");
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
            builder.Configuration.AddConfiguration(config);
            #endregion

            builder.Services.AddManewryMorskieGame();

            return builder.Build();
        }
    }
}