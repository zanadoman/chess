using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;

namespace Chess;

public partial class App : Application
{
    public static Random Random { get; } = new Random();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime i)
        {
            i.MainWindow = new MainWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }
}
