using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using ChessSharp;
using ChessSharp.Pieces;
using ChessSharp.SquareData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess;

public class ChessBoard : Grid
{
    public ChessBoard(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        Width = _mainWindow.Width;
        Height = Width;
        for (int i = 0; i < 10; i++)
        {
            ColumnDefinitions.Add(new ColumnDefinition());
            RowDefinitions.Add(new RowDefinition());
        }
        foreach (File file in Enum.GetValues(typeof(File)))
        {
            Label label = NewIdentifierLabel(Enum.GetName(typeof(File), file));
            SetColumn(label, 1 + (int)file);
            SetRow(label, 0);
            Children.Add(label);
        }
        foreach (File file in Enum.GetValues(typeof(File)))
        {
            Label label = NewIdentifierLabel(Enum.GetName(typeof(File), file));
            SetColumn(label, 1 + (int)file);
            SetRow(label, 9);
            Children.Add(label);
        }
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {
            Label label = NewIdentifierLabel((1 + (int)rank).ToString());
            SetColumn(label, 0);
            SetRow(label, 1 + Rank.Eighth - rank);
            Children.Add(label);
        }
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {
            Label label = NewIdentifierLabel((1 + (int)rank).ToString());
            SetColumn(label, 10);
            SetRow(label, 1 + Rank.Eighth - rank);
            Children.Add(label);
        }
        foreach (File file in Enum.GetValues(typeof(File)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                Square square = new Square(file, rank);
                Button button = new Button
                {
                    Background = ((int)file + (int)rank) % 2 == 0 ? Brushes.Sienna : Brushes.Wheat,
                    Content = new Image
                    {
                        Width = Width / 11,
                        Height = Height / 11
                    },
                    CornerRadius = new CornerRadius(0),
                    BorderBrush = Brushes.Black
                };
                button.PointerEntered += OnPointerEntered;
                button.PointerExited += OnPointerExited;
                button.Click += OnClick;
                _squares.Add(square, button);
                UpdateSquare(square);
                SetColumn(button, 1 + (int)file);
                SetRow(button, 1 + Rank.Eighth - rank);
                Children.Add(button);
            }
        }
        _mainWindow.Menu.UpdateStateDisplay(_gameBoard.GameState, _gameBoard.WhoseTurn());
    }

    public bool ShowDangerousSquares
    {
        get => _showDangerousSquares;

        set
        {
            _showDangerousSquares = value;
            ClearIndicators();
            if (_source != null)
            {
                ShowValidMoves(_source);
            }
        }
    }

    public void GenerateMove()
    {
        List<Move> moves = new List<Move>();
        foreach (Square source in _squares.Keys.Where(k => _gameBoard[k] != null && _gameBoard[k].Owner == _gameBoard.WhoseTurn()))
        {
            foreach (Square destination in _squares.Keys.Where(k => _gameBoard[k] == null || _gameBoard[k].Owner != _gameBoard.WhoseTurn()))
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
        IOrderedEnumerable<Move>? filteredMoves = null;
        List<Square> dangerousSquares = GetDangerousSquares(_gameBoard.WhoseTurn());
        if (moves.Any(m => dangerousSquares.Contains(m.Source) && 1 < GetPieceValue(_gameBoard[m.Source])))
        {
            filteredMoves = moves
                .Where(m => dangerousSquares.Contains(m.Source) && 1 < GetPieceValue(_gameBoard[m.Source]))
                .OrderByDescending(m => !dangerousSquares.Contains(m.Destination))
                .ThenByDescending(m => GetPieceValue(_gameBoard[m.Source]))
                .ThenByDescending(m => GetPieceValue(_gameBoard[m.Destination]));
        }
        else if (moves.Any(m => GetPieceValue(_gameBoard[m.Source]) <= GetPieceValue(_gameBoard[m.Destination])))
        {
            filteredMoves = moves
                .Where(m => GetPieceValue(_gameBoard[m.Source]) <= GetPieceValue(_gameBoard[m.Destination]))
                .OrderByDescending(m => GetPieceValue(_gameBoard[m.Destination]))
                .ThenBy(m => GetPieceValue(_gameBoard[m.Source]));
        }
        else if (moves.Any(m => !dangerousSquares.Contains(m.Destination)))
        {
            filteredMoves = moves
                .Where(m => !dangerousSquares.Contains(m.Destination))
                .OrderByDescending(m => GetPieceValue(_gameBoard[m.Destination]));
        }
        else
        {
            filteredMoves = moves.OrderBy(m => GetPieceValue(_gameBoard[m.Source]));
        }
        if (_squares.Keys.Sum(k => _gameBoard[k] == null || _gameBoard[k].Owner == _gameBoard.WhoseTurn() ? 0 : GetPieceValue(_gameBoard[k])) <= 10)
        {
            Square king = _squares.Keys.Where(k => _gameBoard[k] is King king && king.Owner != _gameBoard.WhoseTurn()).First();
            filteredMoves
                .ThenBy(f => Math.Sqrt(Math.Pow(king.File - f.Destination.File, 2) + Math.Pow(king.Rank - f.Destination.Rank, 2)))
                .ThenByDescending(f => GetPieceValue(_gameBoard[f.Source]))
                .ThenByDescending(f => Math.Sqrt(Math.Pow(king.File - f.Source.File, 2) + Math.Pow(king.Rank - f.Source.Rank, 2)));
        }
        MakeMove(filteredMoves.ThenBy(f => App.Random.Next()).First());
        if (_source != null)
        {
            ClearIndicators();
            _squares[_source].BorderThickness = new Thickness(0);
            _source = null;
        }
        else if (_showDangerousSquares)
        {
            ClearIndicators();
        }
    }

    public void Restart()
    {
        ClearIndicators();
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
        _mainWindow.Menu.UpdateStateDisplay(_gameBoard.GameState, _gameBoard.WhoseTurn());
    }

    private void OnPointerEntered(object? button, PointerEventArgs _)
    {
        KeyValuePair<Square, Button> square = _squares.Where(s => s.Value == button).First();
        if ((_gameBoard[square.Key] != null && _gameBoard[square.Key].Owner == _gameBoard.WhoseTurn()) ||
            (_source != null && _gameBoard.IsValidMove(new Move(_source, square.Key, _gameBoard.WhoseTurn(), PawnPromotion.Queen))))
        {
            square.Value.BorderThickness = new Thickness(2);
            square.Value.Cursor = new Cursor(StandardCursorType.Hand);
        }
    }

    private void OnPointerExited(object? button, PointerEventArgs _)
    {
        if (button is Button)
        {
            ((Button)button).Cursor = new Cursor(StandardCursorType.Arrow);
            if (_source == null || button != _squares[_source])
            {
                ((Button)button).BorderThickness = new Thickness(0);
            }
        }
    }

    private void OnClick(object? button, RoutedEventArgs _)
    {
        KeyValuePair<Square, Button> square = _squares.Where(s => s.Value == button).First();
        if (_gameBoard[square.Key] != null && _gameBoard[square.Key].Owner == _gameBoard.WhoseTurn())
        {
            if (_source != null)
            {
                ClearIndicators();
                _squares[_source].BorderThickness = new Thickness(0);
            }
            _source = square.Key;
            square.Value.BorderThickness = new Thickness(2);
            ShowValidMoves(_source);
        }
        else if (_source != null)
        {
            Move move = new Move(_source, square.Key, _gameBoard.WhoseTurn(), _gameBoard.WhoseTurn() == Player.White
                ? _mainWindow.Menu.WhitePawnPromotion
                : _mainWindow.Menu.BlackPawnPromotion
            );
            if (_gameBoard.IsValidMove(move))
            {
                MakeMove(move);
                square.Value.BorderThickness = new Thickness(0);
                ClearIndicators();
                _squares[_source].BorderThickness = new Thickness(0);
                _source = null;
            }
        }
    }

    private void ShowValidMoves(Square source)
    {
        List<Square>? dangerousSquares = _showDangerousSquares ? GetDangerousSquares(_gameBoard.WhoseTurn()) : null;
        foreach (KeyValuePair<Square, Button> destination in _squares)
        {
            if (destination.Key.Equals(source))
            {
                destination.Value.Background = Brushes.MediumPurple;
            }
            else if (_gameBoard.IsValidMove(new Move(source, destination.Key, _gameBoard.WhoseTurn(), PawnPromotion.Queen)))
            {
                if (dangerousSquares != null && dangerousSquares.Contains(destination.Key))
                {
                    destination.Value.Background = ((int)destination.Key.File + (int)destination.Key.Rank) % 2 == 0
                        ? Brushes.Olive
                        : Brushes.DarkKhaki;
                }
                else
                {
                    destination.Value.Background = ((int)destination.Key.File + (int)destination.Key.Rank) % 2 == 0
                        ? Brushes.ForestGreen
                        : Brushes.LimeGreen;
                }
            }
        }
    }

    private void ClearIndicators()
    {
        List<Square>? dangerousSquares = _showDangerousSquares ? GetDangerousSquares(_gameBoard.WhoseTurn()) : null;
        foreach (KeyValuePair<Square, Button> square in _squares)
        {
            if (dangerousSquares != null && dangerousSquares.Contains(square.Key))
            {
                square.Value.Background = _gameBoard[square.Key] != null && _gameBoard[square.Key].Owner == _gameBoard.WhoseTurn()
                    ? ((int)square.Key.File + (int)square.Key.Rank) % 2 == 0 ? Brushes.DarkRed : Brushes.Crimson
                    : ((int)square.Key.File + (int)square.Key.Rank) % 2 == 0 ? Brushes.OrangeRed : Brushes.Orange;
            }
            else
            {
                square.Value.Background = ((int)square.Key.File + (int)square.Key.Rank) % 2 == 0 ? Brushes.Sienna : Brushes.Wheat;
            }
        }
    }

    private void MakeMove(Move move)
    {
        try
        {
            _gameBoard.MakeMove(move, true);
        }
        catch
        {
            return;
        }
        UpdateSquare(move.Source);
        UpdateSquare(move.Destination);
        if (_gameBoard[move.Destination] is King)
        {
            if (move.Source.File - move.Destination.File == 2)
            {
                UpdateSquare(new Square(File.A, move.Source.Rank));
                UpdateSquare(new Square(File.D, move.Source.Rank));
            }
            else if (move.Destination.File - move.Source.File == 2)
            {
                UpdateSquare(new Square(File.F, move.Source.Rank));
                UpdateSquare(new Square(File.H, move.Source.Rank));
            }
        }
        _mainWindow.Menu.UpdateStateDisplay(_gameBoard.GameState, _gameBoard.WhoseTurn());
    }

    private void UpdateSquare(Square square)
    {
        if (_squares[square].Content is Image image)
        {
            if (_gameBoard[square] is Pawn pawn)
            {
                image.Source = pawn.Owner == Player.White
                    ? Chess.Resources.WhitePawn
                    : Chess.Resources.BlackPawn;
            }
            else if (_gameBoard[square] is Knight knight)
            {
                image.Source = knight.Owner == Player.White
                    ? Chess.Resources.WhiteKnight
                    : Chess.Resources.BlackKnight;
            }
            else if (_gameBoard[square] is Bishop bishop)
            {
                image.Source = bishop.Owner == Player.White
                    ? Chess.Resources.WhiteBishop
                    : Chess.Resources.BlackBishop;
            }
            else if (_gameBoard[square] is King king)
            {
                image.Source = king.Owner == Player.White
                    ? Chess.Resources.WhiteKing
                    : Chess.Resources.BlackKing;
            }
            else if (_gameBoard[square] is Rook rook)
            {
                image.Source = rook.Owner == Player.White
                    ? Chess.Resources.WhiteRook
                    : Chess.Resources.BlackRook;
            }
            else if (_gameBoard[square] is Queen queen)
            {
                image.Source = queen.Owner == Player.White
                    ? Chess.Resources.WhiteQueen
                    : Chess.Resources.BlackQueen;
            }
            else
            {
                image.Source = null;
            }
        }
    }

    private List<Square> GetDangerousSquares(Player player)
    {
        List<Square> dangerousSquares = new List<Square>();
        foreach (Square source in _squares.Keys.Where(k => _gameBoard[k] != null && _gameBoard[k].Owner != player))
        {
            if (_gameBoard[source] is Pawn)
            {
                Rank rank = player == Player.White ? source.Rank - 1 : source.Rank + 1;
                if (source.File + 1 <= File.H)
                {
                    dangerousSquares.Add(new Square(source.File + 1, rank));
                }
                if (File.A <= source.File - 1)
                {
                    dangerousSquares.Add(new Square(source.File - 1, rank));
                }
                continue;
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
                continue;
            }
            if (_gameBoard[source] is King)
            {
                for (int file = (int)source.File - 1; file <= (int)source.File + 1; file++)
                {
                    for (int rank = (int)source.Rank - 1; rank <= (int)source.Rank + 1; rank++)
                    {
                        if (file < (int)File.A || (int)File.H < file)
                        {
                            continue;
                        }
                        if (rank < (int)Rank.First || (int)Rank.Eighth < rank)
                        {
                            continue;
                        }
                        if ((File)file == source.File && (Rank)rank == source.Rank)
                        {
                            continue;
                        }
                        dangerousSquares.Add(new Square((File)file, (Rank)rank));
                    }
                }
                continue;
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
        }
        return dangerousSquares;
    }

    private static int GetPieceValue(Piece piece)
    {
        if (piece is Pawn)
        {
            return 1;
        }
        if (piece is Knight)
        {
            return 3;
        }
        if (piece is Bishop)
        {
            return 3;
        }
        if (piece is King)
        {
            return 0;
        }
        if (piece is Rook)
        {
            return 5;
        }
        if (piece is Queen)
        {
            return 9;
        }
        return -1;
    }

    private static Label NewIdentifierLabel(string? content)
    {
        return new Label
        {
            FontSize = 36,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.BurlyWood,
            Content = content,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    private MainWindow _mainWindow;
    private GameBoard _gameBoard = new GameBoard();
    private Dictionary<Square, Button> _squares = new Dictionary<Square, Button>();
    private Square? _source = null;
    private bool _showDangerousSquares = false;
}
