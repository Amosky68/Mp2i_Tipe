using OpenGlTIPE;
using OpenGlTIPE.SimLoop;

namespace MainProgram
{
    class Program
    {
        public static void Main(string[] args)
        {
            SimulationModel game = new InstancingTest(1200, 800, "Le vrai programme de la mort", false);
            game.Run();
        }
    }
}