using OpenGlTIPE.SimLoop;

namespace MainProgram
{
    class Program
    {
        public static void Main(string[] args)
        {
            SimulationModel game = new TestSim(1200, 800, "test !");
            game.Run();
        }
    }
}