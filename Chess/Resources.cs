using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Chess;

internal static class Resources
{
    public static BitmapImage DiceButton => _diceButton ?? (_diceButton = LoadBitmapImage("DiceButton.png"));
    public static BitmapImage RestartButton => _restartButton ?? (_restartButton = LoadBitmapImage("RestartButton.png"));
    public static BitmapImage MinimizeButton => _minimizeButton ?? (_minimizeButton = LoadBitmapImage("MinimizeButton.png"));
    public static BitmapImage QuitButton => _quitButton ?? (_quitButton = LoadBitmapImage("QuitButton.png"));
    public static BitmapImage WhitePawn => _whitePawn ?? (_whitePawn = LoadBitmapImage("WhitePawn.png"));
    public static BitmapImage WhiteKnight => _whiteKnight ?? (_whiteKnight = LoadBitmapImage("WhiteKnight.png"));
    public static BitmapImage WhiteBishop => _whiteBishop ?? (_whiteBishop = LoadBitmapImage("WhiteBishop.png"));
    public static BitmapImage WhiteKing => _whiteKing ?? (_whiteKing = LoadBitmapImage("WhiteKing.png"));
    public static BitmapImage WhiteRook => _whiteRook ?? (_whiteRook = LoadBitmapImage("WhiteRook.png"));
    public static BitmapImage WhiteQueen => _whiteQueen ?? (_whiteQueen = LoadBitmapImage("WhiteQueen.png"));
    public static BitmapImage BlackPawn => _blackPawn ?? (_blackPawn = LoadBitmapImage("BlackPawn.png"));
    public static BitmapImage BlackKnight => _blackKnight ?? (_blackKnight = LoadBitmapImage("BlackKnight.png"));
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

    private static BitmapImage? _diceButton = null;
    private static BitmapImage? _restartButton = null;
    private static BitmapImage? _minimizeButton = null;
    private static BitmapImage? _quitButton = null;
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
