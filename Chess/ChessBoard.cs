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
        Width = 480;
        Height = 480;
        for (int i = 0; i < 8; i++)
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
                    ? new SolidColorBrush(Color.FromRgb(184, 139, 74))
                    : new SolidColorBrush(Color.FromRgb(227, 193, 111));
                button.BorderBrush = Brushes.White;
                button.BorderThickness = new Thickness(0);
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
    }

    private void OnMouseEnter(object button, MouseEventArgs _)
    {
        if (_selectedSquare == null)
        {
            return;
        }
        Square square = _squares.Where(s => s.Value == button).Select(s => s.Key).First();
        Move move = new Move(_selectedSquare, square, _gameBoard.WhoseTurn());
        if (_gameBoard.IsValidMove(move))
        {
            ((Button)button).BorderThickness = new Thickness(3);
        }
    }

    private void OnMouseLeave(object button, MouseEventArgs _)
    {
        if (_selectedSquare != null && button != _squares[_selectedSquare])
        {
            ((Button)button).BorderThickness = new Thickness(0);
        }
    }

    private void OnClick(object button, RoutedEventArgs _)
    {
        Square square = _squares.Where(s => s.Value == button).Select(s => s.Key).First();
        if (_selectedSquare == null)
        {
            Piece piece = _gameBoard.Board[(int)square.Rank, (int)square.File];
            if (piece != null && piece.Owner == _gameBoard.WhoseTurn())
            {
                ((Button)button).BorderThickness = new Thickness(3);
                _selectedSquare = square;
            }
        }
        else
        {
            Move move = new Move(_selectedSquare, square, _gameBoard.WhoseTurn());
            if (_gameBoard.IsValidMove(move))
            {
                _gameBoard.MakeMove(move, false);
                UpdateSquare(_selectedSquare);
                UpdateSquare(square);
                if (_gameBoard[square] is King && 1 < Math.Abs((int)square.File - (int)_selectedSquare.File))
                {
                    UpdateSquare(new Square(File.A, _selectedSquare.Rank));
                    UpdateSquare(new Square(File.D, _selectedSquare.Rank));
                    UpdateSquare(new Square(File.F, _selectedSquare.Rank));
                    UpdateSquare(new Square(File.H, _selectedSquare.Rank));
                }
                MainWindow.Menu.GameState = (_gameBoard.GameState, _gameBoard.WhoseTurn());
            }
            _squares[_selectedSquare].BorderThickness = new Thickness(0);
            ((Button)button).BorderThickness = new Thickness(0);
            _selectedSquare = null;
        }
    }

    private void UpdateSquare(Square square)
    {
        if (_gameBoard[square] is Pawn pawn)
        {
            _squares[square].Content = pawn.Owner == Player.White
                ? Chess.Resources.WhitePawn
                : Chess.Resources.BlackPawn;
        }
        else if (_gameBoard[square] is Knight knight)
        {
            _squares[square].Content = knight.Owner == Player.White
                ? Chess.Resources.WhiteKnight
                : Chess.Resources.BlackKnight;
        }
        else if (_gameBoard[square] is Bishop bishop)
        {
            _squares[square].Content = bishop.Owner == Player.White
                ? Chess.Resources.WhiteBishop
                : Chess.Resources.BlackBishop;
        }
        else if (_gameBoard[square] is King king)
        {
            _squares[square].Content = king.Owner == Player.White
                ? Chess.Resources.WhiteKing
                : Chess.Resources.BlackKing;
        }
        else if (_gameBoard[square] is Rook rook)
        {
            _squares[square].Content = rook.Owner == Player.White
                ? Chess.Resources.WhiteRook
                : Chess.Resources.BlackRook;
        }
        else if (_gameBoard[square] is Queen queen)
        {
            _squares[square].Content = queen.Owner == Player.White
                ? Chess.Resources.WhiteQueen
                : Chess.Resources.BlackQueen;
        }
        else
        {
            _squares[square].Content = null;
        }
    }

    private GameBoard _gameBoard = new GameBoard();
    private Dictionary<Square, Button> _squares = new Dictionary<Square, Button>();
    private Square? _selectedSquare = null;
}
