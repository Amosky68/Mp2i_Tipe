using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using static OpenGlTIPE.OpenGL.GL;
using System.Drawing;

namespace OpenGlTIPE.Rendering.Display
{
    static class DisplayManager
    {
        public static Window Window { get; set; }
        public static Vector2 WindowSize { get; set; }


        public static void CreateWindow(int width, int height, string title, bool vsync = true)
        {
            WindowSize = new Vector2(width, height);
            Glfw.Init();
            // Opengl 3.3
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);

            Glfw.WindowHint(Hint.Focused, true);
            Glfw.WindowHint(Hint.Resizable, false);

            //create the window
            Window = Glfw.CreateWindow(width, height, title, Monitor.None, Window.None);

            if (Window == Window.None)
            {
                // something has gone wrong
                return;
            }

            Rectangle screen = Glfw.PrimaryMonitor.WorkArea;
            int x = (screen.Width - width) / 2;
            int y = (screen.Height - height) / 2;
            Glfw.SetWindowPosition(Window, x, y);

            Glfw.MakeContextCurrent(Window);
            Import(Glfw.GetProcAddress);

            glViewport(0,0,width,height);
            int vs = vsync ? 1 : 0;
            Glfw.SwapInterval(vs); //Vsync is off, 1 to turn it on
        }
        public static void CloseWindow() 
        {
            Glfw.Terminate();
        }
    }
}
