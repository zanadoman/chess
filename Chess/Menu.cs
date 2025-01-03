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
        Width = Application.Current.MainWindow.Width;
        Height = Application.Current.MainWindow.Height - Width;
        Background = Brushes.Sienna;
        LastChildFill = false;
        MouseDown += (_, mouseButtonEventArgs) =>
        {
            if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
            {
                Application.Current.MainWindow.DragMove();
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
            MainWindow.ChessBoard.GenerateMove();
        }));
        rightPanel.Children.Add(NewControlButton(Chess.Resources.DangerButton, (_, _) =>
        {
            MainWindow.ChessBoard.ShowDangerousSquares = !MainWindow.ChessBoard.ShowDangerousSquares;
        }));
        rightPanel.Children.Add(new StackPanel
        {
            Width = 30
        });
        rightPanel.Children.Add(NewControlButton(Chess.Resources.WhiteQueen, (sender, _) =>
        {
            switch (WhitePawnPromotion)
            {
                case PawnPromotion.Knight:
                    WhitePawnPromotion = PawnPromotion.Bishop;
                    ((Image)((Button)sender).Content).Source = Chess.Resources.WhiteBishop;
                    break;
                case PawnPromotion.Bishop:
                    WhitePawnPromotion = PawnPromotion.Rook;
                    ((Image)((Button)sender).Content).Source = Chess.Resources.WhiteRook;
                    break;
                case PawnPromotion.Rook:
                    WhitePawnPromotion = PawnPromotion.Queen;
                    ((Image)((Button)sender).Content).Source = Chess.Resources.WhiteQueen;
                    break;
                case PawnPromotion.Queen:
                    WhitePawnPromotion = PawnPromotion.Knight;
                    ((Image)((Button)sender).Content).Source = Chess.Resources.WhiteKnight;
                    break;
            }
        }));
        rightPanel.Children.Add(NewControlButton(Chess.Resources.BlackQueen, (sender, _) =>
        {
            switch (BlackPawnPromotion)
            {
                case PawnPromotion.Knight:
                    BlackPawnPromotion = PawnPromotion.Bishop;
                    ((Image)((Button)sender).Content).Source = Chess.Resources.BlackBishop;
                    break;
                case PawnPromotion.Bishop:
                    BlackPawnPromotion = PawnPromotion.Rook;
                    ((Image)((Button)sender).Content).Source = Chess.Resources.BlackRook;
                    break;
                case PawnPromotion.Rook:
                    BlackPawnPromotion = PawnPromotion.Queen;
                    ((Image)((Button)sender).Content).Source = Chess.Resources.BlackQueen;
                    break;
                case PawnPromotion.Queen:
                    BlackPawnPromotion = PawnPromotion.Knight;
                    ((Image)((Button)sender).Content).Source = Chess.Resources.BlackKnight;
                    break;
            }
        }));
        rightPanel.Children.Add(new StackPanel
        {
            Width = 30
        });
        rightPanel.Children.Add(NewControlButton(Chess.Resources.RestartButton, (_, _) =>
        {
            MainWindow.ChessBoard.Restart();
        }));
        rightPanel.Children.Add(NewControlButton(Chess.Resources.MinimizeButton, (_, _) =>
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }));
        rightPanel.Children.Add(NewControlButton(Chess.Resources.QuitButton, (_, _) =>
        {
            Application.Current.MainWindow.Close();
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

    private static Button NewControlButton(BitmapImage bitmapImage, RoutedEventHandler routedEventHandler)
    {
        Button button = new Button
        {
            Width = 45,
            Height = 45,
            Background = Brushes.Transparent,
            Content = new Image
            {
                Source = bitmapImage,
                Width = 45,
                Height = 45
            },
            BorderThickness = new Thickness(0)
        };
        button.Click += routedEventHandler;
        return button;
    }

    private Label _stateDisplay = new Label
    {
        FontSize = 30,
        FontWeight = FontWeights.Bold,
        Foreground = Brushes.White,
        VerticalAlignment = VerticalAlignment.Center
    };
}
