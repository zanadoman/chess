using ChessSharp;
using ChessSharp.Pieces;
using ChessSharp.SquareData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Chess;

public class ChessBoard : Grid
{
    public ChessBoard()
    {
        Width = Application.Current.MainWindow.Width;
        Height = Width;
        foreach (Rank _ in Enum.GetValues(typeof(Rank)))
        {
            RowDefinitions.Add(new RowDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());
        }
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {
            foreach (File file in Enum.GetValues(typeof(File)))
            {
                Square square = new Square(file, rank);
                Button button = new Button
                {
                    Background = ((int)rank + (int)file) % 2 == 0
                        ? Brushes.Sienna
                        : Brushes.Wheat,
                    Content = new Image
                    {
                        Width = 70,
                        Height = 70
                    },
                    BorderThickness = new Thickness(0),
                    BorderBrush = Brushes.White
                };
                button.MouseEnter += OnMouseEnter;
                button.MouseLeave += OnMouseLeave;
                button.Click += OnClick;
                _squares.Add(square, button);
                UpdateSquare(square);
                SetRow(button, (int)Rank.Eighth - (int)rank);
                SetColumn(button, (int)file);
                Children.Add(button);
            }
        }
        MainWindow.Menu.UpdateStateDisplay(_gameBoard.GameState, _gameBoard.WhoseTurn());
    }

    public void Restart()
    {
        ClearValidMoves();
        if (_selectedSquare != null)
        {
            _squares[_selectedSquare].BorderThickness = new Thickness(0);
            _selectedSquare = null;
        }
        _gameBoard = new GameBoard();
        foreach (Square square in _squares.Keys)
        {
            UpdateSquare(square);
        }
        MainWindow.Menu.UpdateStateDisplay(_gameBoard.GameState, _gameBoard.WhoseTurn());
    }

    private void OnMouseEnter(object button, MouseEventArgs _)
    {
        KeyValuePair<Square, Button> square = _squares.Where(s => s.Value == button).First();
        if (_selectedSquare == null)
        {
            if (_gameBoard[square.Key] != null && _gameBoard[square.Key].Owner == _gameBoard.WhoseTurn())
            {
                square.Value.BorderThickness = new Thickness(3);
            }
        }
        else if (_gameBoard.IsValidMove(new Move(_selectedSquare, square.Key, _gameBoard.WhoseTurn(), PawnPromotion.Queen)))
        {
            square.Value.BorderThickness = new Thickness(3);
        }
    }

    private void OnMouseLeave(object button, MouseEventArgs _)
    {
        if (_selectedSquare == null || button != _squares[_selectedSquare])
        {
            ((Button)button).BorderThickness = new Thickness(0);
        }
    }

    private void OnClick(object button, RoutedEventArgs _)
    {
        KeyValuePair<Square, Button> square = _squares.Where(s => s.Value == button).First();
        if (_gameBoard[square.Key] != null && _gameBoard[square.Key].Owner == _gameBoard.WhoseTurn())
        {
            if (_selectedSquare != null)
            {
                ClearValidMoves();
                _squares[_selectedSquare].BorderThickness = new Thickness(0);
            }
            _selectedSquare = square.Key;
            square.Value.BorderThickness = new Thickness(3);
            ShowValidMoves(_selectedSquare);
        }
        else if (_selectedSquare != null)
        {
            Move move = new Move(_selectedSquare, square.Key, _gameBoard.WhoseTurn(), _gameBoard.WhoseTurn() == Player.White
                ? MainWindow.Menu.WhitePawnPromotion
                : MainWindow.Menu.BlackPawnPromotion
            );
            if (_gameBoard.IsValidMove(move))
            {
                _gameBoard.MakeMove(move, false);
                UpdateSquare(_selectedSquare);
                UpdateSquare(square.Key);
                if (_gameBoard[square.Key] is King)
                {
                    if ((int)_selectedSquare.File - (int)square.Key.File == 2)
                    {
                        UpdateSquare(new Square(File.A, _selectedSquare.Rank));
                        UpdateSquare(new Square(File.D, _selectedSquare.Rank));
                    }
                    else if ((int)square.Key.File - (int)_selectedSquare.File == 2)
                    {
                        UpdateSquare(new Square(File.F, _selectedSquare.Rank));
                        UpdateSquare(new Square(File.H, _selectedSquare.Rank));
                    }
                }
                _squares[_selectedSquare].BorderThickness = new Thickness(0);
                _selectedSquare = null;
                square.Value.BorderThickness = new Thickness(0);
                ClearValidMoves();
                MainWindow.Menu.UpdateStateDisplay(_gameBoard.GameState, _gameBoard.WhoseTurn());
            }
        }
    }

    private void ShowValidMoves(Square selectedSquare)
    {
        foreach (KeyValuePair<Square, Button> square in _squares)
        {
            if (square.Key.Equals(selectedSquare))
            {
                square.Value.Background = Brushes.MediumPurple;
            }
            else if (_gameBoard.IsValidMove(new Move(selectedSquare, square.Key, _gameBoard.WhoseTurn(), PawnPromotion.Queen)))
            {
                square.Value.Background = ((int)square.Key.Rank + (int)square.Key.File) % 2 == 0
                    ? Brushes.ForestGreen
                    : Brushes.LimeGreen;
            }
        }
    }

    private void ClearValidMoves()
    {
        foreach (KeyValuePair<Square, Button> square in _squares)
        {
            square.Value.Background = ((int)square.Key.Rank + (int)square.Key.File) % 2 == 0
                ? Brushes.Sienna
                : Brushes.Wheat;
        }
    }

    private void UpdateSquare(Square square)
    {
        if (_gameBoard[square] is Pawn pawn)
        {
            ((Image)_squares[square].Content).Source = pawn.Owner == Player.White
                ? Chess.Resources.WhitePawn
                : Chess.Resources.BlackPawn;
        }
        else if (_gameBoard[square] is Knight knight)
        {
            ((Image)_squares[square].Content).Source = knight.Owner == Player.White
                ? Chess.Resources.WhiteKnight
                : Chess.Resources.BlackKnight;
        }
        else if (_gameBoard[square] is Bishop bishop)
        {
            ((Image)_squares[square].Content).Source = bishop.Owner == Player.White
                ? Chess.Resources.WhiteBishop
                : Chess.Resources.BlackBishop;
        }
        else if (_gameBoard[square] is King king)
        {
            ((Image)_squares[square].Content).Source = king.Owner == Player.White
                ? Chess.Resources.WhiteKing
                : Chess.Resources.BlackKing;
        }
        else if (_gameBoard[square] is Rook rook)
        {
            ((Image)_squares[square].Content).Source = rook.Owner == Player.White
                ? Chess.Resources.WhiteRook
                : Chess.Resources.BlackRook;
        }
        else if (_gameBoard[square] is Queen queen)
        {
            ((Image)_squares[square].Content).Source = queen.Owner == Player.White
                ? Chess.Resources.WhiteQueen
                : Chess.Resources.BlackQueen;
        }
        else
        {
            ((Image)_squares[square].Content).Source = null;
        }
    }

    private GameBoard _gameBoard = new GameBoard();
    private Dictionary<Square, Button> _squares = new Dictionary<Square, Button>();
    private Square? _selectedSquare = null;
}
