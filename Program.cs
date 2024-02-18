using OpenGlTIPE.SimLoop;

namespace MainProgram
{
    class Program
    {
        public static void Main(string[] args)
        {
            SimulationModel game = new TestSim(800, 600, "test !");
            game.Run();
        }
    }
}