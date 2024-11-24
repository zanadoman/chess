using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Chess;

internal class Resources
{
    public static BitmapImage RestartIcon => _restartIcon ?? (_restartIcon = LoadBitmapImage("Restart.png"));
    public static BitmapImage MinimizeIcon => _minimizeIcon ?? (_minimizeIcon = LoadBitmapImage("Minimize.png"));
    public static BitmapImage QuitIcon => _quitIcon ?? (_quitIcon = LoadBitmapImage("Quit.png"));

    public static BitmapImage WhitePawn => _whitePawn ?? (_whitePawn = LoadBitmapImage("WhitePawn.png"));
    public static BitmapImage WhiteKnight => _whiteKing ?? (_whiteKnight = LoadBitmapImage("WhiteKnight.png"));
    public static BitmapImage WhiteBishop => _whiteBishop ?? (_whiteBishop = LoadBitmapImage("WhiteBishop.png"));
    public static BitmapImage WhiteKing => _whiteKing ?? (_whiteKing = LoadBitmapImage("WhiteKing.png"));
    public static BitmapImage WhiteRook => _whiteRook ?? (_whiteRook = LoadBitmapImage("WhiteRook.png"));
    public static BitmapImage WhiteQueen => _whiteQueen ?? (_whiteQueen = LoadBitmapImage("WhiteQueen.png"));

    public static BitmapImage BlackPawn => _blackPawn ?? (_blackPawn = LoadBitmapImage("BlackPawn.png"));
    public static BitmapImage BlackKnight => _blackKing ?? (_blackKnight = LoadBitmapImage("BlackKnight.png"));
    public static BitmapImage BlackBishop => _blackBishop ?? (_blackBishop = LoadBitmapImage("BlackBishop.png"));
    public static BitmapImage BlackKing => _blackKing ?? (_blackKing = LoadBitmapImage("BlackKing.png"));
    public static BitmapImage BlackRook => _blackRook ?? (_blackRook = LoadBitmapImage("BlackRook.png"));
    public static BitmapImage BlackQueen => _blackQueen ?? (_blackQueen = LoadBitmapImage("BlackQueen.png"));

    private static BitmapImage LoadBitmapImage(string image)
    {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(Path.Combine(Environment.CurrentDirectory, "res", image));
        bitmapImage.EndInit();
        return bitmapImage;
    }

    private static BitmapImage? _restartIcon = null;
    private static BitmapImage? _minimizeIcon = null;
    private static BitmapImage? _quitIcon = null;
    private static BitmapImage? _whitePawn = null;
    private static BitmapImage? _whiteKnight = null;
    private static BitmapImage? _whiteBishop = null;
    private static BitmapImage? _whiteKing = null;
    private static BitmapImage? _whiteRook = null;
    private static BitmapImage? _whiteQueen = null;
    private static BitmapImage? _blackPawn = null;
    private static BitmapImage? _blackKnight = null;
    private static BitmapImage? _blackBishop = null;
    private static BitmapImage? _blackKing = null;
    private static BitmapImage? _blackRook = null;
    private static BitmapImage? _blackQueen = null;
}
