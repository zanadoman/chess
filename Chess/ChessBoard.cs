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

    public void GenerateMove()
    {
        List<Move> moves = new List<Move>();
        foreach (Square source in _squares.Keys.Where(s => _gameBoard[s] != null && _gameBoard[s].Owner == _gameBoard.WhoseTurn()))
        {
            foreach (Square destination in _squares.Keys.Where(s => _gameBoard[s] == null || _gameBoard[s].Owner != _gameBoard.WhoseTurn()))
            {
                Move move = new Move(source, destination, _gameBoard.WhoseTurn(), PawnPromotion.Queen);
                if (_gameBoard.IsValidMove(move))
                {
                    moves.Add(move);
                }
            }
        }
        if (moves.Count == 0)
        {
            return;
        }
        List<Square> dangerousSquares = GetDangerousSquares(_gameBoard.WhoseTurn());
        if (moves.Any(m => dangerousSquares.Contains(m.Source) && 1 < GetPieceValue(_gameBoard[m.Source])))
        {
            moves = moves
                .Where(m => dangerousSquares.Contains(m.Source) && 1 < GetPieceValue(_gameBoard[m.Source]))
                .OrderByDescending(m => !dangerousSquares.Contains(m.Destination))
                .ThenByDescending(m => GetPieceValue(_gameBoard[m.Source]))
                .ThenByDescending(m => GetPieceValue(_gameBoard[m.Destination]))
                .ThenByDescending(m => _gameBoard.WhoseTurn() == Player.White
                    ? m.Source.Rank - m.Destination.Rank
                    : m.Destination.Rank - m.Source.Rank)
                .ThenBy(_ => App.Random.Next())
                .ToList();
        }
        else if (moves.Any(m => GetPieceValue(_gameBoard[m.Source]) <= GetPieceValue(_gameBoard[m.Destination])))
        {
            moves = moves
                .Where(m => GetPieceValue(_gameBoard[m.Source]) <= GetPieceValue(_gameBoard[m.Destination]))
                .OrderByDescending(m => GetPieceValue(_gameBoard[m.Destination]))
                .ThenBy(m => GetPieceValue(_gameBoard[m.Source]))
                .ThenBy(_ => App.Random.Next())
                .ToList();
        }
        else if (moves.Any(m => !dangerousSquares.Contains(m.Destination)))
        {
            moves = moves
                .Where(m => !dangerousSquares.Contains(m.Destination))
                .OrderByDescending(m => GetPieceValue(_gameBoard[m.Destination]))
                .ThenBy(_ => App.Random.Next())
                .ToList();
        }
        else
        {
            moves = moves
                .OrderBy(m => GetPieceValue(_gameBoard[m.Source]))
                .ThenBy(_ => App.Random.Next())
                .ToList();
        }
        MakeMove(moves.First());
        if (_source != null)
        {
            _squares[_source].BorderThickness = new Thickness(0);
            _source = null;
            ClearValidMoves();
        }
    }

    public void Restart()
    {
        ClearValidMoves();
        if (_source != null)
        {
            _squares[_source].BorderThickness = new Thickness(0);
            _source = null;
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
        if (_source == null)
        {
            if (_gameBoard[square.Key] != null && _gameBoard[square.Key].Owner == _gameBoard.WhoseTurn())
            {
                square.Value.BorderThickness = new Thickness(3);
            }
        }
        else if (_gameBoard.IsValidMove(new Move(_source, square.Key, _gameBoard.WhoseTurn(), PawnPromotion.Queen)))
        {
            square.Value.BorderThickness = new Thickness(3);
        }
    }

    private void OnMouseLeave(object button, MouseEventArgs _)
    {
        if (_source == null || button != _squares[_source])
        {
            ((Button)button).BorderThickness = new Thickness(0);
        }
    }

    private void OnClick(object button, RoutedEventArgs _)
    {
        KeyValuePair<Square, Button> destination = _squares.Where(s => s.Value == button).First();
        if (_gameBoard[destination.Key] != null && _gameBoard[destination.Key].Owner == _gameBoard.WhoseTurn())
        {
            if (_source != null)
            {
                ClearValidMoves();
                _squares[_source].BorderThickness = new Thickness(0);
            }
            _source = destination.Key;
            destination.Value.BorderThickness = new Thickness(3);
            ShowValidMoves(_source);
        }
        else if (_source != null)
        {
            Move move = new Move(_source, destination.Key, _gameBoard.WhoseTurn(), _gameBoard.WhoseTurn() == Player.White
                ? MainWindow.Menu.WhitePawnPromotion
                : MainWindow.Menu.BlackPawnPromotion
            );
            if (_gameBoard.IsValidMove(move))
            {
                MakeMove(move);
                _squares[_source].BorderThickness = new Thickness(0);
                _source = null;
                destination.Value.BorderThickness = new Thickness(0);
                ClearValidMoves();
            }
        }
    }

    private void ShowValidMoves(Square source)
    {
        foreach (KeyValuePair<Square, Button> destination in _squares)
        {
            if (destination.Key.Equals(source))
            {
                destination.Value.Background = Brushes.MediumPurple;
            }
            else if (_gameBoard.IsValidMove(new Move(source, destination.Key, _gameBoard.WhoseTurn(), PawnPromotion.Queen)))
            {
                destination.Value.Background = ((int)destination.Key.Rank + (int)destination.Key.File) % 2 == 0
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

    private void MakeMove(Move move)
    {
        _gameBoard.MakeMove(move, true);
        UpdateSquare(move.Source);
        UpdateSquare(move.Destination);
        if (_gameBoard[move.Destination] is King)
        {
            if ((int)move.Source.File - (int)move.Destination.File == 2)
            {
                UpdateSquare(new Square(File.A, move.Source.Rank));
                UpdateSquare(new Square(File.D, move.Source.Rank));
            }
            else if ((int)move.Destination.File - (int)move.Source.File == 2)
            {
                UpdateSquare(new Square(File.F, move.Source.Rank));
                UpdateSquare(new Square(File.H, move.Source.Rank));
            }
        }
        MainWindow.Menu.UpdateStateDisplay(_gameBoard.GameState, _gameBoard.WhoseTurn());
    }

    private List<Square> GetDangerousSquares(Player player)
    {
        List<Square> dangerousSquares = new List<Square>();
        foreach (Square source in _squares.Keys.Where(s => _gameBoard[s] != null && _gameBoard[s].Owner != player))
        {
            if (_gameBoard[source] is Pawn)
            {
                Rank rank = player == Player.White
                    ? source.Rank - 1
                    : source.Rank + 1;
                if (source.File + 1 <= File.H)
                {
                    dangerousSquares.Add(new Square(source.File + 1, rank));
                }
                if (File.A <= source.File - 1)
                {
                    dangerousSquares.Add(new Square(source.File - 1, rank));
                }
            }
            if (_gameBoard[source] is Knight)
            {
                if (source.File + 2 <= File.H)
                {
                    if (source.Rank + 1 <= Rank.Eighth)
                    {
                        dangerousSquares.Add(new Square(source.File + 2, source.Rank + 1));
                    }
                    if (Rank.First <= source.Rank - 1)
                    {
                        dangerousSquares.Add(new Square(source.File + 2, source.Rank - 1));
                    }
                }
                if (File.A <= source.File - 2)
                {
                    if (source.Rank + 1 <= Rank.Eighth)
                    {
                        dangerousSquares.Add(new Square(source.File - 2, source.Rank + 1));
                    }
                    if (Rank.First <= source.Rank - 1)
                    {
                        dangerousSquares.Add(new Square(source.File - 2, source.Rank - 1));
                    }
                }
                if (source.Rank + 2 <= Rank.Eighth)
                {
                    if (source.File + 1 <= File.H)
                    {
                        dangerousSquares.Add(new Square(source.File + 1, source.Rank + 2));
                    }
                    if (File.A <= source.File - 1)
                    {
                        dangerousSquares.Add(new Square(source.File - 1, source.Rank + 2));
                    }
                }
                if (Rank.First <= source.Rank - 2)
                {
                    if (source.File + 1 <= File.H)
                    {
                        dangerousSquares.Add(new Square(source.File + 1, source.Rank - 2));
                    }
                    if (File.A <= source.File - 1)
                    {
                        dangerousSquares.Add(new Square(source.File - 1, source.Rank - 2));
                    }
                }
            }
            if (_gameBoard[source] is Bishop || _gameBoard[source] is Queen)
            {
                for (int file = (int)source.File + 1, rank = (int)source.Rank + 1; file <= (int)File.H && rank <= (int)Rank.Eighth; file++, rank++)
                {
                    dangerousSquares.Add(new Square((File)file, (Rank)rank));
                    if (_gameBoard[dangerousSquares.Last()] != null)
                    {
                        break;
                    }
                }
                for (int file = (int)source.File - 1, rank = (int)source.Rank + 1; (int)File.A <= file && rank <= (int)Rank.Eighth; file--, rank++)
                {
                    dangerousSquares.Add(new Square((File)file, (Rank)rank));
                    if (_gameBoard[dangerousSquares.Last()] != null)
                    {
                        break;
                    }
                }
                for (int file = (int)source.File + 1, rank = (int)source.Rank - 1; file <= (int)File.H && (int)Rank.First <= rank; file++, rank--)
                {
                    dangerousSquares.Add(new Square((File)file, (Rank)rank));
                    if (_gameBoard[dangerousSquares.Last()] != null)
                    {
                        break;
                    }
                }
                for (int file = (int)source.File - 1, rank = (int)source.Rank - 1; (int)File.A <= file && (int)Rank.First <= rank; file--, rank--)
                {
                    dangerousSquares.Add(new Square((File)file, (Rank)rank));
                    if (_gameBoard[dangerousSquares.Last()] != null)
                    {
                        break;
                    }
                }
            }
            if (_gameBoard[source] is Rook || _gameBoard[source] is Queen)
            {
                for (int file = (int)source.File + 1; file <= (int)File.H; file++)
                {
                    dangerousSquares.Add(new Square((File)file, source.Rank));
                    if (_gameBoard[dangerousSquares.Last()] != null)
                    {
                        break;
                    }
                }
                for (int file = (int)source.File - 1; (int)File.A <= file; file--)
                {
                    dangerousSquares.Add(new Square((File)file, source.Rank));
                    if (_gameBoard[dangerousSquares.Last()] != null)
                    {
                        break;
                    }
                }
                for (int rank = (int)source.Rank + 1; rank <= (int)Rank.Eighth; rank++)
                {
                    dangerousSquares.Add(new Square(source.File, (Rank)rank));
                    if (_gameBoard[dangerousSquares.Last()] != null)
                    {
                        break;
                    }
                }
                for (int rank = (int)source.Rank - 1; (int)Rank.First <= rank; rank--)
                {
                    dangerousSquares.Add(new Square(source.File, (Rank)rank));
                    if (_gameBoard[dangerousSquares.Last()] != null)
                    {
                        break;
                    }
                }
            }
            if (_gameBoard[source] is King)
            {
                for (int file = (int)source.File - 1; file <= (int)source.File + 1; file++)
                {
                    for (int rank = (int)source.Rank - 1; rank <= (int)source.Rank + 1; rank++)
                    {
                        if (file < (int)File.A || (int)File.H < file || (File)file == source.File)
                        {
                            continue;
                        }
                        if (rank < (int)Rank.First || (int)Rank.Eighth < rank || (Rank)rank == source.Rank)
                        {
                            continue;
                        }
                        dangerousSquares.Add(new Square((File)file, (Rank)rank));
                    }
                }
            }
        }
        return dangerousSquares;
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

    private static int GetPieceValue(Piece piece)
    {
        if (piece is Pawn)
        {
            return 1;
        }
        if (piece is Knight)
        {
            return 2;
        }
        if (piece is Bishop)
        {
            return 2;
        }
        if (piece is King)
        {
            return 0;
        }
        if (piece is Rook)
        {
            return 3;
        }
        if (piece is Queen)
        {
            return 4;
        }
        return -1;
    }

    private GameBoard _gameBoard = new GameBoard();
    private Dictionary<Square, Button> _squares = new Dictionary<Square, Button>();
    private Square? _source = null;
}
