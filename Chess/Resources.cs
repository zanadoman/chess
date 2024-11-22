using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Chess
{
    internal class Resources
    {
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

        private static Image LoadPiece(string piece)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(Path.Combine(Environment.CurrentDirectory, "res", piece));
            bitmapImage.EndInit();
            return new Image()
            {
                Source = bitmapImage,
                Width = 90,
                Height = 90
            };
        }

        private static Image? _whitePawn = null;
        private static Image? _whiteKnight = null;
        private static Image? _whiteBishop = null;
        private static Image? _whiteKing = null;
        private static Image? _whiteRook = null;
        private static Image? _whiteQueen = null;
        private static Image? _blackPawn = null;
        private static Image? _blackKnight = null;
        private static Image? _blackBishop = null;
        private static Image? _blackKing = null;
        private static Image? _blackRook = null;
        private static Image? _blackQueen = null;
    }
}
