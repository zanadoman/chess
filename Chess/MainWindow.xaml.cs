using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chess;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowStyle = WindowStyle.None;
        ResizeMode = ResizeMode.NoResize;
        Width = 640;
        Height = 700;
        Title = "Chess";
        Icon = Chess.Resources.WhiteKing;
        Background = Brushes.SaddleBrown;
        Content = new StackPanel
        {
            Children = { Menu, ChessBoard }
        };
    }

    public static Menu Menu => _menu ?? (_menu = new Menu());
    public static ChessBoard ChessBoard => _chessBoard ?? (_chessBoard = new ChessBoard());

    private static Menu? _menu = null;
    private static ChessBoard? _chessBoard = null;
}
