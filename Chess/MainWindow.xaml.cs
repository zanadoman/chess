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
        Width = 480;
        Height = 540;
        Title = "Chess";
        Background = Brushes.SaddleBrown;
        Icon = Chess.Resources.WhiteKing;
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