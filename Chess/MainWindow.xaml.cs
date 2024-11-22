using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            for (int rank = (int)ChessSharp.SquareData.Rank.First; rank <= (int)ChessSharp.SquareData.Rank.Eighth; rank++)
            {
                for (int file = (int)ChessSharp.SquareData.File.A; file <= (int)ChessSharp.SquareData.File.H; file++)
                {
                    Button button = new Button();
                    button.Background = (rank + file) % 2 == 0 ? new SolidColorBrush(Color.FromRgb(227, 193, 111))
                                                               : new SolidColorBrush(Color.FromRgb(184, 139, 74));
                    button.BorderThickness = new Thickness(0);
                    Grid.SetRow(button, rank);
                    Grid.SetColumn(button, file);
                    board.Children.Add(button);
                    _buttons.Add((rank, file), button);
                }
            }
            Content = board;

            UpdateBoard();
        }

        private void UpdateBoard()
        {
            for (int rank = (int)ChessSharp.SquareData.Rank.First; rank <= (int)ChessSharp.SquareData.Rank.Eighth; rank++)
            {
                for (int file = (int)ChessSharp.SquareData.File.A; file <= (int)ChessSharp.SquareData.File.H; file++)
                {
                    ChessSharp.Piece piece = _gameBoard.Board[rank, file];
                    if (piece is ChessSharp.Pieces.Pawn pawn)
                    {
                        _buttons[(rank, file)].Content = pawn.Owner == ChessSharp.Player.White
                            ? Chess.Resources.WhitePawn
                            : Chess.Resources.BlackPawn;
                    }
                    if (piece is ChessSharp.Pieces.Knight knight)
                    {
                        _buttons[(rank, file)].Content = knight.Owner == ChessSharp.Player.White
                            ? Chess.Resources.WhiteKnight
                            : Chess.Resources.BlackKnight;
                    }
                    if (piece is ChessSharp.Pieces.Bishop bishop)
                    {
                        _buttons[(rank, file)].Content = bishop.Owner == ChessSharp.Player.White
                            ? Chess.Resources.WhiteBishop
                            : Chess.Resources.BlackBishop;
                    }
                    if (piece is ChessSharp.Pieces.King king)
                    {
                        _buttons[(rank, file)].Content = king.Owner == ChessSharp.Player.White
                            ? Chess.Resources.WhiteKing
                            : Chess.Resources.BlackKing;
                    }
                    if (piece is ChessSharp.Pieces.Rook rook)
                    {
                        _buttons[(rank, file)].Content = rook.Owner == ChessSharp.Player.White
                            ? Chess.Resources.WhiteRook
                            : Chess.Resources.BlackRook;
                    }
                    if (piece is ChessSharp.Pieces.Queen queen)
                    {
                        _buttons[(rank, file)].Content = queen.Owner == ChessSharp.Player.White
                            ? Chess.Resources.WhiteQueen
                            : Chess.Resources.BlackQueen;
                    }
                }
            }
        }

        private ChessSharp.GameBoard _gameBoard = new ChessSharp.GameBoard();
        private Dictionary<(int, int), Button> _buttons = new Dictionary<(int, int), Button>();
    }
}