using OpenGlTIPE.Rendering.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLFW;

namespace OpenGlTIPE.SimLoop
{
    abstract class SimulationModel
    {

        protected int InitialWindowWidth { get; set; }
        protected int InitialWindowHeight { get; set; }
        protected string InitialWindowTitle { get; set; }

        public SimulationModel(int initialWindowWidth, int initialWindowHeight, string initialWindowTitle)
        {
            InitialWindowWidth = initialWindowWidth;
            InitialWindowHeight = initialWindowHeight;
            InitialWindowTitle = initialWindowTitle;
        }

        public void Run()
        {
            Initialize();

            DisplayManager.CreateWindow(InitialWindowWidth, InitialWindowHeight, InitialWindowTitle);
            LoadContent();

            while (!Glfw.WindowShouldClose(DisplayManager.Window))
            {
                SimTime.DeltaTime = (float)Glfw.Time - SimTime.TotalElapsedSeconds;
                SimTime.TotalElapsedSeconds = (float)Glfw.Time;

                Update();

                Glfw.PollEvents(); // make sure windows knows that it hasen't crashed
                
                Render();
            }
            DisplayManager.CloseWindow();
        }

        protected abstract void Initialize();
        protected abstract void LoadContent();
        protected abstract void Update();
        protected abstract void Render();


    }
}
