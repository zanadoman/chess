using ChessSharp;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Chess;

public class Menu : DockPanel
{
    public Menu()
    {
        Width = Application.Current.MainWindow.Width - 20;
        Height = Application.Current.MainWindow.Height - Application.Current.MainWindow.Width;
        Background = Brushes.Transparent;
        LastChildFill = false;
        MouseDown += (_, mouse) =>
        {
            if (mouse.ChangedButton == MouseButton.Left)
            {
                Application.Current.MainWindow.DragMove();
            }
        };

        StackPanel controlButtons = new StackPanel()
        {
            Orientation = Orientation.Horizontal
        };
        controlButtons.Children.Add(NewButton(Chess.Resources.RestartIcon, (_, _) =>
        {
            MainWindow.ChessBoard.Restart();
        }));
        controlButtons.Children.Add(NewButton(Chess.Resources.MinimizeIcon, (_, _) =>
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }));
        controlButtons.Children.Add(NewButton(Chess.Resources.QuitIcon, (_, _) =>
        {
            Application.Current.Shutdown();
        }));

        SetDock(_label, Dock.Left);
        SetDock(controlButtons, Dock.Right);
        Children.Add(_label);
        Children.Add(controlButtons);
    }

    public (GameState, Player) GameState
    {
        get => _gameState;
        set
        {
            switch (value.Item1)
            {
                case ChessSharp.GameState.NotCompleted:
                    _label.Content = value.Item2 == Player.White
                        ? "White Turn."
                        : "Black Turn.";
                    break;
                case ChessSharp.GameState.WhiteInCheck:
                    _label.Content = "White in Check.";
                    break;
                case ChessSharp.GameState.BlackInCheck:
                    _label.Content = "Black in Check.";
                    break;
                case ChessSharp.GameState.Draw:
                    _label.Content = "Draw.";
                    break;
                case ChessSharp.GameState.Stalemate:
                    _label.Content = "Stalemate.";
                    break;
                case ChessSharp.GameState.WhiteWinner:
                    _label.Content = "White Winner.";
                    break;
                case ChessSharp.GameState.BlackWinner:
                    _label.Content = "Black Winner.";
                    break;
            }
            _gameState = value;
        }
    }

    private Button NewButton(Image image, RoutedEventHandler routedEventHandler)
    {
        Button button = new Button()
        {
            Width = 40,
            Height = 40,
            Content = image,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0)
        };
        button.Click += routedEventHandler;
        return button;
    }

    private (GameState, Player) _gameState;
    private Label _label = new Label()
    {
        FontSize = 36,
        Foreground = Brushes.WhiteSmoke,
        VerticalAlignment = VerticalAlignment.Center
    };
}
