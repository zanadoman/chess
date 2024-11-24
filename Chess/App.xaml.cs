using System;
using System.Windows;

namespace Chess
{
    public partial class App : Application
    {
        public static Random Random { get; } = new Random();
    }
}
