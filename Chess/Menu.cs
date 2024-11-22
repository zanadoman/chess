using ChessSharp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Chess;

public class Menu : StackPanel
{
    public Menu()
    {
        Height = 80;
        Background = Brushes.SaddleBrown;
        MouseDown += (_, mouse) =>
        {
            if (mouse.ChangedButton == MouseButton.Left)
            {
                Application.Current.MainWindow.DragMove();
            }
        };
        Children.Add(_label);
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

    private (GameState, Player) _gameState;
    private Label _label = new Label();
}
