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
using System.Windows.Media.Imaging;

namespace Chess;

public class ChessBoard : Grid
{
    public ChessBoard()
    {
        Width = Application.Current.MainWindow.Width;
        Height = Application.Current.MainWindow.Width;
        foreach (Rank _ in Enum.GetValues(typeof(Rank)))
        {
            RowDefinitions.Add(new RowDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());
        }
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {
            foreach (File file in Enum.GetValues(typeof(File)))
            {
                Button button = new Button();
                button.Background = ((int)rank + (int)file) % 2 == 0
                    ? Brushes.Sienna
                    : Brushes.Wheat;
                button.BorderBrush = Brushes.White;
                button.BorderThickness = new Thickness(0);
                button.Content = new Image
                {
                    Width = 55,
                    Height = 55
                };
                button.MouseEnter += OnMouseEnter;
                button.MouseLeave += OnMouseLeave;
                button.Click += OnClick;
                SetRow(button, (int)Rank.Eighth - (int)rank);
                SetColumn(button, (int)file);
                Children.Add(button);
                Square square = new Square(file, rank);
                _squares.Add(square, button);
                UpdateSquare(square);
            }
        }
        MainWindow.Menu.UpdateStateDisplay(_gameBoard.GameState, _gameBoard.WhoseTurn());
    }

    public void Restart()
    {
        _gameBoard = new GameBoard();
        if (_selectedSquare != null)
        {
            _squares[_selectedSquare].BorderThickness = new Thickness(0);
            _selectedSquare = null;
        }
        ClearValidMoves();
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {
            foreach (File file in Enum.GetValues(typeof(File)))
            {
                UpdateSquare(new Square(file, rank));
            }
        }
    }

    private void OnMouseEnter(object button, MouseEventArgs _)
    {
        Square square = _squares.Where(s => s.Value == button).Select(s => s.Key).First();
        if (_selectedSquare == null)
        {
            if (_gameBoard[square] != null && _gameBoard[square].Owner == _gameBoard.WhoseTurn())
            {
                ((Button)button).BorderThickness = new Thickness(3);
            }
        }
        else
        {
            Move move = new Move(_selectedSquare, square, _gameBoard.WhoseTurn(), MainWindow.Menu.PawnPromotion);
            if (_gameBoard.IsValidMove(move))
            {
                ((Button)button).BorderThickness = new Thickness(3);
            }
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
        Square square = _squares.Where(s => s.Value == button).Select(s => s.Key).First();
        if (_selectedSquare == null)
        {
            if (_gameBoard[square] != null && _gameBoard[square].Owner == _gameBoard.WhoseTurn())
            {
                ((Button)button).BorderThickness = new Thickness(3);
                _selectedSquare = square;
                ShowValidMoves(_selectedSquare);
            }
        }
        else if (_gameBoard[square] != null && _gameBoard[square].Owner == _gameBoard.WhoseTurn())
        {
            ClearValidMoves();
            _squares[_selectedSquare].BorderThickness = new Thickness(0);
            ((Button)button).BorderThickness = new Thickness(3);
            _selectedSquare = square;
            ShowValidMoves(_selectedSquare);
        }
        else
        {
            Move move = new Move(_selectedSquare, square, _gameBoard.WhoseTurn(), MainWindow.Menu.PawnPromotion);
            if (_gameBoard.IsValidMove(move))
            {
                _gameBoard.MakeMove(move, false);
                UpdateSquare(_selectedSquare);
                UpdateSquare(square);
                if (_gameBoard[square] is King)
                {
                    if ((int)_selectedSquare.File - (int)square.File == 2)
                    {
                        UpdateSquare(new Square(File.A, _selectedSquare.Rank));
                        UpdateSquare(new Square(File.D, _selectedSquare.Rank));
                    }
                    else if ((int)square.File - (int)_selectedSquare.File == 2)
                    {
                        UpdateSquare(new Square(File.F, _selectedSquare.Rank));
                        UpdateSquare(new Square(File.H, _selectedSquare.Rank));
                    }
                }
                MainWindow.Menu.UpdateStateDisplay(_gameBoard.GameState, _gameBoard.WhoseTurn());
                ClearValidMoves();
            }
            _squares[_selectedSquare].BorderThickness = new Thickness(0);
            ((Button)button).BorderThickness = new Thickness(0);
            _selectedSquare = null;
        }
    }

    private void UpdateSquare(Square square)
    {
        BitmapImage? bitmapImage = null;
        if (_gameBoard[square] is Pawn pawn)
        {
            bitmapImage = pawn.Owner == Player.White
                ? Chess.Resources.WhitePawn
                : Chess.Resources.BlackPawn;
        }
        else if (_gameBoard[square] is Knight knight)
        {
            bitmapImage = knight.Owner == Player.White
                ? Chess.Resources.WhiteKnight
                : Chess.Resources.BlackKnight;
        }
        else if (_gameBoard[square] is Bishop bishop)
        {
            bitmapImage = bishop.Owner == Player.White
                ? Chess.Resources.WhiteBishop
                : Chess.Resources.BlackBishop;
        }
        else if (_gameBoard[square] is King king)
        {
            bitmapImage = king.Owner == Player.White
                ? Chess.Resources.WhiteKing
                : Chess.Resources.BlackKing;
        }
        else if (_gameBoard[square] is Rook rook)
        {
            bitmapImage = rook.Owner == Player.White
                ? Chess.Resources.WhiteRook
                : Chess.Resources.BlackRook;
        }
        else if (_gameBoard[square] is Queen queen)
        {
            bitmapImage = queen.Owner == Player.White
                ? Chess.Resources.WhiteQueen
                : Chess.Resources.BlackQueen;
        }
        if (_squares[square].Content is Image image)
        {
            image.Source = bitmapImage;
        }
    }

    private void ShowValidMoves(Square selectedSquare)
    {
        foreach (Square square in _squares.Keys)
        {
            Move move = new Move(selectedSquare, square, _gameBoard.WhoseTurn(), MainWindow.Menu.PawnPromotion);
            if (square.Equals(selectedSquare) || _gameBoard.IsValidMove(move))
            {
                _squares[square].Background = ((int)square.Rank + (int)square.File) % 2 == 0
                    ? Brushes.SlateGray
                    : Brushes.WhiteSmoke;
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

    private GameBoard _gameBoard = new GameBoard();
    private Dictionary<Square, Button> _squares = new Dictionary<Square, Button>();
    private Square? _selectedSquare = null;
}
