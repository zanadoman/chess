using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Chess;

internal class Resources
{
    public static Image RestartIcon => LoadIcon("Restart.png");
    public static Image MinimizeIcon => LoadIcon("Minimize.png");
    public static Image QuitIcon => LoadIcon("Quit.png");

    public static Image WhitePawn => LoadPiece("WhitePawn.png");
    public static Image WhiteKnight => LoadPiece("WhiteKnight.png");
    public static Image WhiteBishop => LoadPiece("WhiteBishop.png");
    public static Image WhiteKing => LoadPiece("WhiteKing.png");
    public static Image WhiteRook => LoadPiece("WhiteRook.png");
    public static Image WhiteQueen => LoadPiece("WhiteQueen.png");

    public static Image BlackPawn => LoadPiece("BlackPawn.png");
    public static Image BlackKnight => LoadPiece("BlackKnight.png");
    public static Image BlackBishop => LoadPiece("BlackBishop.png");
    public static Image BlackKing => LoadPiece("BlackKing.png");
    public static Image BlackRook => LoadPiece("BlackRook.png");
    public static Image BlackQueen => LoadPiece("BlackQueen.png");

    private static Image LoadIcon(string icon)
    {
        return new Image()
        {
            Source = BitmapImage(icon),
            Width = 40,
            Height = 40
        };
    }

    private static Image LoadPiece(string piece)
    {
        return new Image()
        {
            Source = BitmapImage(piece),
            Width = 55,
            Height = 55
        };
    }

    private static BitmapImage BitmapImage(string file)
    {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(Path.Combine(Environment.CurrentDirectory, "res", file));
        bitmapImage.EndInit();
        return bitmapImage;
    }
}
