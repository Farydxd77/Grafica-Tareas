using System;

namespace Opentk_2222
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run(60.0, 60.0);
            }
        }
    }
}