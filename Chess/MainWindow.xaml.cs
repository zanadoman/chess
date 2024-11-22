using System.Windows;
using System.Windows.Controls;

namespace Chess;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowStyle = WindowStyle.None;
        ResizeMode = ResizeMode.NoResize;
        Width = 480;
        Height = 560;
        Title = "Chess";
        Content = new StackPanel()
        {
            Children = { Menu, ChessBoard }
        };
    }

    public static Menu Menu => _menu == null
        ? _menu = new Menu()
        : _menu;

    public static ChessBoard ChessBoard => _chessBoard == null
        ? _chessBoard = new ChessBoard()
        : _chessBoard;

    private static Menu? _menu = null;
    private static ChessBoard? _chessBoard = null;
}