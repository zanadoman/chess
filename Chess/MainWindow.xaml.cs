using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Title = "Chess";
            Width = 800;
            Height = 800;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;

            Grid board = new Grid { Width = 800, Height = 800 };
            for (int i = 0; i < 8; i++)
            {
                board.RowDefinitions.Add(new RowDefinition());
                board.ColumnDefinitions.Add(new ColumnDefinition());
            }

            foreach (ChessSharp.SquareData.Rank rank in Enum.GetValues(typeof(ChessSharp.SquareData.Rank)))
            {
                foreach (ChessSharp.SquareData.File file in Enum.GetValues(typeof(ChessSharp.SquareData.File)))
                {
                    Button button = new Button();
                    button.Background = ((int)rank + (int)file) % 2 == 0 ? new SolidColorBrush(Color.FromRgb(227, 193, 111))
                                                                         : new SolidColorBrush(Color.FromRgb(184, 139, 74));
                    button.BorderThickness = new Thickness(0);
                    button.BorderBrush = Brushes.White;
                    Grid.SetRow(button, (int)rank);
                    Grid.SetColumn(button, (int)file);
                    board.Children.Add(button);
                    button.Click += OnClick;
                    button.MouseEnter += OnMouseEnter;
                    button.MouseLeave += OnMouseLeave;
                    ChessSharp.SquareData.Square square = new ChessSharp.SquareData.Square(file, rank);
                    _buttons.Add(square, button);
                    UpdateSquare(square);
                }
            }
            Content = board;
        }

        private void OnMouseEnter(object button, MouseEventArgs _)
        {
            if (_selectedSquare != null)
            {
                ChessSharp.SquareData.Square square = _buttons.Where(pair => pair.Value == button).Select(pair => pair.Key).First();
                if (_gameBoard.IsValidMove(new ChessSharp.Move(_selectedSquare, square, _gameBoard.WhoseTurn())))
                {
                    ((Button)button).BorderThickness = new Thickness(5);
                }
            }
        }

        private void OnMouseLeave(object button, MouseEventArgs _)
        {
            if (_selectedSquare != null && button != _buttons[_selectedSquare])
            {
                ((Button)button).BorderThickness = new Thickness(0);
            }
        }

        private void OnClick(object button, RoutedEventArgs _)
        {
            ChessSharp.SquareData.Square square = _buttons.Where(pair => pair.Value == button).Select(pair => pair.Key).First();
            if (_selectedSquare == null)
            {
                ChessSharp.Piece piece = _gameBoard.Board[(int)square.Rank, (int)square.File];
                if (piece != null && piece.Owner == _gameBoard.WhoseTurn())
                {
                    ((Button)button).BorderThickness = new Thickness(5);
                    _selectedSquare = square;
                }
            }
            else
            {
                ChessSharp.Move move = new ChessSharp.Move(_selectedSquare, square, _gameBoard.WhoseTurn());
                if (_gameBoard.IsValidMove(move))
                {
                    _gameBoard.MakeMove(move, false);
                    UpdateSquare(_selectedSquare);
                    UpdateSquare(square);
                }
                _buttons[_selectedSquare].BorderThickness = new Thickness(0);
                ((Button)button).BorderThickness = new Thickness(0);
                _selectedSquare = null;
            }
        }

        private ChessSharp.SquareData.Square? _selectedSquare = null;

        private void UpdateSquare(ChessSharp.SquareData.Square square)
        {
            ChessSharp.Piece piece = _gameBoard.Board[(int)square.Rank, (int)square.File];
            if (piece is ChessSharp.Pieces.Pawn pawn)
            {
                _buttons[square].Content = pawn.Owner == ChessSharp.Player.White
                    ? Chess.Resources.WhitePawn
                    : Chess.Resources.BlackPawn;
            }
            else if (piece is ChessSharp.Pieces.Knight knight)
            {
                _buttons[square].Content = knight.Owner == ChessSharp.Player.White
                    ? Chess.Resources.WhiteKnight
                    : Chess.Resources.BlackKnight;
            }
            else if (piece is ChessSharp.Pieces.Bishop bishop)
            {
                _buttons[square].Content = bishop.Owner == ChessSharp.Player.White
                    ? Chess.Resources.WhiteBishop
                    : Chess.Resources.BlackBishop;
            }
            else if (piece is ChessSharp.Pieces.King king)
            {
                _buttons[square].Content = king.Owner == ChessSharp.Player.White
                    ? Chess.Resources.WhiteKing
                    : Chess.Resources.BlackKing;
            }
            else if (piece is ChessSharp.Pieces.Rook rook)
            {
                _buttons[square].Content = rook.Owner == ChessSharp.Player.White
                    ? Chess.Resources.WhiteRook
                    : Chess.Resources.BlackRook;
            }
            else if (piece is ChessSharp.Pieces.Queen queen)
            {
                _buttons[square].Content = queen.Owner == ChessSharp.Player.White
                    ? Chess.Resources.WhiteQueen
                    : Chess.Resources.BlackQueen;
            }
            else
            {
                _buttons[square].Content = null;
            }
        }

        private ChessSharp.GameBoard _gameBoard = new ChessSharp.GameBoard();
        private Dictionary<ChessSharp.SquareData.Square, Button> _buttons =
            new Dictionary<ChessSharp.SquareData.Square, Button>();
    }
}