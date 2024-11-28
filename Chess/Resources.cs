using System;
using System.IO;
using Avalonia.Media.Imaging;

namespace Chess;

internal static class Resources
{
    public static Bitmap DiceButton => _diceButton ?? (_diceButton = LoadBitmap("DiceButton.png"));
    public static Bitmap DangerButton => _dangerButton ?? (_dangerButton = LoadBitmap("DangerButton.png"));
    public static Bitmap RestartButton => _restartButton ?? (_restartButton = LoadBitmap("RestartButton.png"));
    public static Bitmap MinimizeButton => _minimizeButton ?? (_minimizeButton = LoadBitmap("MinimizeButton.png"));
    public static Bitmap QuitButton => _quitButton ?? (_quitButton = LoadBitmap("QuitButton.png"));
    public static Bitmap WhitePawn => _whitePawn ?? (_whitePawn = LoadBitmap("WhitePawn.png"));
    public static Bitmap WhiteKnight => _whiteKnight ?? (_whiteKnight = LoadBitmap("WhiteKnight.png"));
    public static Bitmap WhiteBishop => _whiteBishop ?? (_whiteBishop = LoadBitmap("WhiteBishop.png"));
    public static Bitmap WhiteKing => _whiteKing ?? (_whiteKing = LoadBitmap("WhiteKing.png"));
    public static Bitmap WhiteRook => _whiteRook ?? (_whiteRook = LoadBitmap("WhiteRook.png"));
    public static Bitmap WhiteQueen => _whiteQueen ?? (_whiteQueen = LoadBitmap("WhiteQueen.png"));
    public static Bitmap BlackPawn => _blackPawn ?? (_blackPawn = LoadBitmap("BlackPawn.png"));
    public static Bitmap BlackKnight => _blackKnight ?? (_blackKnight = LoadBitmap("BlackKnight.png"));
    public static Bitmap BlackBishop => _blackBishop ?? (_blackBishop = LoadBitmap("BlackBishop.png"));
    public static Bitmap BlackKing => _blackKing ?? (_blackKing = LoadBitmap("BlackKing.png"));
    public static Bitmap BlackRook => _blackRook ?? (_blackRook = LoadBitmap("BlackRook.png"));
    public static Bitmap BlackQueen => _blackQueen ?? (_blackQueen = LoadBitmap("BlackQueen.png"));

    private static Bitmap LoadBitmap(string file)
    {
        return new Bitmap(Path.Combine(Environment.CurrentDirectory, "res", file));
    }

    private static Bitmap? _diceButton = null;
    private static Bitmap? _dangerButton = null;
    private static Bitmap? _restartButton = null;
    private static Bitmap? _minimizeButton = null;
    private static Bitmap? _quitButton = null;
    private static Bitmap? _whitePawn = null;
    private static Bitmap? _whiteKnight = null;
    private static Bitmap? _whiteBishop = null;
    private static Bitmap? _whiteKing = null;
    private static Bitmap? _whiteRook = null;
    private static Bitmap? _whiteQueen = null;
    private static Bitmap? _blackPawn = null;
    private static Bitmap? _blackKnight = null;
    private static Bitmap? _blackBishop = null;
    private static Bitmap? _blackKing = null;
    private static Bitmap? _blackRook = null;
    private static Bitmap? _blackQueen = null;
}
