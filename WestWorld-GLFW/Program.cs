

using OpenGLTutorial.GameLoop;

namespace OpenGLTutorial
{
    class Program
    {
        public static void Main(string[] args)
        {
            Game game = new TestGame(1200, 600, "Test game!");
            game.Run();
        }
    }
}