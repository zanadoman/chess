﻿using ChessSharp;
using ChessSharp.Pieces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        controlButtons.Children.Add(NewControlButton(Chess.Resources.WhiteQueen, (button, _) =>
        {
            switch (PawnPromotion)
            {
                case PawnPromotion.Knight:
                    PawnPromotion = PawnPromotion.Bishop;
                    ((Image)((Button)button).Content).Source = Chess.Resources.WhiteBishop;
                    break;
                case PawnPromotion.Bishop:
                    PawnPromotion = PawnPromotion.Rook;
                    ((Image)((Button)button).Content).Source = Chess.Resources.WhiteRook;
                    break;
                case PawnPromotion.Rook:
                    PawnPromotion = PawnPromotion.Queen;
                    ((Image)((Button)button).Content).Source = Chess.Resources.WhiteQueen;
                    break;
                case PawnPromotion.Queen:
                    PawnPromotion = PawnPromotion.Knight;
                    ((Image)((Button)button).Content).Source = Chess.Resources.WhiteKnight;
                    break;
            }
        }));
        controlButtons.Children.Add(NewControlButton(Chess.Resources.RestartButton, (_, _) =>
        {
            MainWindow.ChessBoard.Restart();
        }));
        controlButtons.Children.Add(NewControlButton(Chess.Resources.MinimizeButton, (_, _) =>
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }));
        controlButtons.Children.Add(NewControlButton(Chess.Resources.QuitButton, (_, _) =>
        {
            Application.Current.Shutdown();
        }));

        SetDock(_stateDisplay, Dock.Left);
        SetDock(controlButtons, Dock.Right);
        Children.Add(_stateDisplay);
        Children.Add(controlButtons);
    }

    public PawnPromotion PawnPromotion { get; set; } = PawnPromotion.Queen;

    public void UpdateStateDisplay(GameState gameState, Player player)
    {
        switch (gameState)
        {
            case GameState.NotCompleted:
                _stateDisplay.Content = player == Player.White
                    ? "White Turn."
                    : "Black Turn.";
                break;
            case GameState.WhiteInCheck:
                _stateDisplay.Content = "White in Check.";
                break;
            case GameState.BlackInCheck:
                _stateDisplay.Content = "Black in Check.";
                break;
            case GameState.Draw:
                _stateDisplay.Content = "Draw.";
                break;
            case GameState.Stalemate:
                _stateDisplay.Content = "Stalemate.";
                break;
            case GameState.WhiteWinner:
                _stateDisplay.Content = "White Winner.";
                break;
            case GameState.BlackWinner:
                _stateDisplay.Content = "Black Winner.";
                break;
        }
    }

    private Button NewControlButton(BitmapImage bitmapImage, RoutedEventHandler routedEventHandler)
    {
        Button button = new Button
        {
            Width = 40,
            Height = 40,
            Content = new Image
            {
                Source = bitmapImage,
                Width = 40,
                Height = 40
            },
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0)
        };
        button.Click += routedEventHandler;
        return button;
    }

    private Label _stateDisplay = new Label
    {
        FontSize = 36,
        Foreground = Brushes.White,
        VerticalAlignment = VerticalAlignment.Center
    };
}
