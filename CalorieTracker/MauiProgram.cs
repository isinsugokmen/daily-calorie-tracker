using CalorieTracker.Data;

namespace CalorieTracker;

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

        // SQLite database path
        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "calories.db");

        // Database service
        builder.Services.AddSingleton(
            new CalorieDatabase(dbPath));

        // MainPage (constructor injection için ZORUNLU)
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}