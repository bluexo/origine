using Xenko.Engine;

namespace Orgine.macOS
{
    class OrgineApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
