using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ChessSharp;
using ChessSharp.Pieces;
using System;

namespace Chess;

public class Menu : DockPanel
{
    public Menu(MainWindow mainWindow)
    {
        Width = mainWindow.Width;
        Height = mainWindow.Height - Width;
        Background = Brushes.Sienna;
        LastChildFill = false;
        PointerPressed += (_, pointerPressedEventArgs) =>
        {
            if (pointerPressedEventArgs.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                mainWindow.BeginMoveDrag(pointerPressedEventArgs);
            }
        };
        StackPanel leftPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        leftPanel.Children.Add(new StackPanel
        {
            Width = 15
        });
        leftPanel.Children.Add(_stateDisplay);
        StackPanel rightPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        rightPanel.Children.Add(NewControlButton(Chess.Resources.DiceButton, (_, _) =>
        {
            mainWindow.ChessBoard.GenerateMove();
        }));
        rightPanel.Children.Add(NewControlButton(Chess.Resources.DangerButton, (_, _) =>
        {
            mainWindow.ChessBoard.ShowDangerousSquares = !mainWindow.ChessBoard.ShowDangerousSquares;
        }));
        rightPanel.Children.Add(new StackPanel
        {
            Width = 30
        });
        rightPanel.Children.Add(NewControlButton(Chess.Resources.WhiteQueen, (button, _) =>
        {
            if (button is Button && ((Button)button).Content is Image image)
            {
                switch (WhitePawnPromotion)
                {
                    case PawnPromotion.Knight:
                        WhitePawnPromotion = PawnPromotion.Bishop;
                        image.Source = Chess.Resources.WhiteBishop;
                        break;
                    case PawnPromotion.Bishop:
                        WhitePawnPromotion = PawnPromotion.Rook;
                        image.Source = Chess.Resources.WhiteRook;
                        break;
                    case PawnPromotion.Rook:
                        WhitePawnPromotion = PawnPromotion.Queen;
                        image.Source = Chess.Resources.WhiteQueen;
                        break;
                    case PawnPromotion.Queen:
                        WhitePawnPromotion = PawnPromotion.Knight;
                        image.Source = Chess.Resources.WhiteKnight;
                        break;
                }
            }
        }));
        rightPanel.Children.Add(NewControlButton(Chess.Resources.BlackQueen, (button, _) =>
        {
            if (button is Button && ((Button)button).Content is Image image)
            {
                switch (BlackPawnPromotion)
                {
                    case PawnPromotion.Knight:
                        BlackPawnPromotion = PawnPromotion.Bishop;
                        image.Source = Chess.Resources.BlackBishop;
                        break;
                    case PawnPromotion.Bishop:
                        BlackPawnPromotion = PawnPromotion.Rook;
                        image.Source = Chess.Resources.BlackRook;
                        break;
                    case PawnPromotion.Rook:
                        BlackPawnPromotion = PawnPromotion.Queen;
                        image.Source = Chess.Resources.BlackQueen;
                        break;
                    case PawnPromotion.Queen:
                        BlackPawnPromotion = PawnPromotion.Knight;
                        image.Source = Chess.Resources.BlackKnight;
                        break;
                }
            }
        }));
        rightPanel.Children.Add(new StackPanel
        {
            Width = 30
        });
        rightPanel.Children.Add(NewControlButton(Chess.Resources.RestartButton, (_, _) =>
        {
            mainWindow.ChessBoard.Restart();
        }));
        rightPanel.Children.Add(NewControlButton(Chess.Resources.MinimizeButton, (_, _) =>
        {
            mainWindow.WindowState = WindowState.Minimized;
        }));
        rightPanel.Children.Add(NewControlButton(Chess.Resources.QuitButton, (_, _) =>
        {
            mainWindow.Close();
        }));
        rightPanel.Children.Add(new StackPanel
        {
            Width = 15
        });
        SetDock(leftPanel, Dock.Left);
        SetDock(rightPanel, Dock.Right);
        Children.Add(leftPanel);
        Children.Add(rightPanel);
    }

    public PawnPromotion WhitePawnPromotion { get; set; } = PawnPromotion.Queen;
    public PawnPromotion BlackPawnPromotion { get; set; } = PawnPromotion.Queen;

    public void UpdateStateDisplay(GameState gameState, Player player)
    {
        switch (gameState)
        {
            case GameState.NotCompleted:
                _stateDisplay.Content = player == Player.White ? "White Turn" : "Black Turn";
                break;
            case GameState.WhiteInCheck:
                _stateDisplay.Content = "White in Check";
                break;
            case GameState.BlackInCheck:
                _stateDisplay.Content = "Black in Check";
                break;
            case GameState.Draw:
                _stateDisplay.Content = "Draw";
                break;
            case GameState.Stalemate:
                _stateDisplay.Content = "Stalemate";
                break;
            case GameState.WhiteWinner:
                _stateDisplay.Content = "White Winner";
                break;
            case GameState.BlackWinner:
                _stateDisplay.Content = "Black Winner";
                break;
        }
    }

    private static Button NewControlButton(Bitmap bitmap, EventHandler<RoutedEventArgs> eventHandler)
    {
        Button button = new Button
        {
            Width = 45,
            Height = 45,
            Background = Brushes.Transparent,
            Content = new Image
            {
                Source = bitmap,
                Width = 45,
                Height = 45,
                Cursor = new Cursor(StandardCursorType.Hand)
            }
        };
        button.PointerEntered += (button, _) =>
        {
            if (button is Button)
            {
                ((Button)button).Opacity = 0.75;
            }
        };
        button.PointerExited += (button, _) =>
        {
            if (button is Button)
            {
                ((Button)button).Opacity = 1;
            }
        };
        button.Click += eventHandler;
        return button;
    }

    private Label _stateDisplay = new Label
    {
        FontSize = 36,
        FontWeight = FontWeight.Bold,
        Foreground = Brushes.White,
        VerticalAlignment = VerticalAlignment.Center
    };
}
