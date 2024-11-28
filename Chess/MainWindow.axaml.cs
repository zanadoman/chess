using Avalonia.Controls;
using Avalonia.Media;

namespace Chess;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // WindowStyle = WindowStyle.None;
        CanResize = false;
        Width = 700;
        Height = 770;
        Title = "Chess";
        Icon = new WindowIcon(Chess.Resources.WhiteKing);
        Background = new SolidColorBrush(Color.FromRgb(107, 50, 10));
        Content = new StackPanel
        {
            Children = { Menu, ChessBoard }
        };
    }

    public Menu Menu => _menu ?? (_menu = new Menu(this));
    public ChessBoard ChessBoard => _chessBoard ?? (_chessBoard = new ChessBoard(this));

    private Menu? _menu = null;
    private ChessBoard? _chessBoard = null;
}
