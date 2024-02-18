using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGlTIPE.OpenGL.GL;
using GLFW;
using OpenGlTIPE.Rendering.Display;
using OpenGlTIPE.Rendering.shaders;

namespace OpenGlTIPE.SimLoop
{
    internal class TestSim : SimulationModel
    {

        uint vao;
        uint vbo;

        Shader shader;

        public TestSim(int initialWindowWidth, int initialWindowHeight, string initialWindowTitle) : base(initialWindowWidth, initialWindowHeight, initialWindowTitle)
        {
        }

        protected override void Initialize()
        {
        }

        protected unsafe override void LoadContent()
        {


            string vertexShader = @"#version 330 core
                                    layout (location = 0) in vec2 aPosition;
                                    layout (location = 1) in vec3 aColor;
                                    out vec4 vertexColor;
    
                                    void main() 
                                    {
                                        vertexColor = vec4(aColor.rgb, 1.0);
                                        gl_Position = vec4(aPosition.xy, 0, 1.0);
                                    }";

            string fragmentShader = @"#version 330 core
                                    out vec4 FragColor;
                                    in vec4 vertexColor;

                                    void main() 
                                    {
                                        FragColor = vertexColor;
                                    }";


            shader = new Shader(vertexShader, fragmentShader);
            shader.Load();

            vao = glGenVertexArray();
            vbo = glGenBuffer();

            glBindVertexArray(vao);
            glBindBuffer(GL_ARRAY_BUFFER, vbo);

            // x,y,r,g,b
            float[] vertices =
            {
                -0.5f, 0.5f, 1f, 0f, 0f, // top left
                0.5f, 0.5f, 0f, 1f, 0f,// top right
                -0.5f, -0.5f, 0f, 0f, 1f, // bottom left

                0.5f, 0.5f, 0f, 1f, 0f,// top right
                0.5f, -0.5f, 0f, 1f, 1f, // bottom right
                -0.5f, -0.5f, 1f, 0f, 1f, // bottom left
            };

            fixed(float* v = &vertices[0]) { // get the address of the first element as a pointer
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
            }
            // format of the data

            // index of buffer : 0 | first List of 2 elements | of type float | not normalised |
            // the next paire is 5 away | offset is 0 
            glVertexAttribPointer(0, 2, GL_FLOAT, false, sizeof(float) * 5, (void*)0);
            glEnableVertexAttribArray(0);

            // index of buffer : 1 | first List of 3 elements | of type float | not normalised |
            // the next paire is 5 away | offset is 2 floats
            glVertexAttribPointer(1, 3, GL_FLOAT, false, sizeof(float) * 5, (void*)(sizeof(float) * 2));
            glEnableVertexAttribArray(1);

            glBindBuffer(GL_ARRAY_BUFFER, 0);
            glBindVertexArray(0);

        }

        protected override void Update()
        {
        }
        protected override void Render()
        {
            glClearColor(0,0,0,1);
            glClear(GL_COLOR_BUFFER_BIT);

            shader.Use();

            glBindVertexArray(vao);
            glDrawArrays(GL_TRIANGLES, 0, 6);
            glBindVertexArray(0);

            Glfw.SwapBuffers(DisplayManager.Window);
        }

    }
}
