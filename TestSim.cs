using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGlTIPE.OpenGL.GL;
using GLFW;
using OpenGlTIPE.Rendering.Display;
using OpenGlTIPE.Rendering.shaders;
using OpenGlTIPE.Rendering.Camera;
using System.Numerics;
using System.Diagnostics;

namespace OpenGlTIPE.SimLoop
{
    internal class TestSim : SimulationModel
    {

        uint vao;
        uint vbo;

        Shader shader;

        Camera2d camera;
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
    
                                    uniform mat4 projection;
                                    uniform mat4 model;

                                    void main() 
                                    {
                                        vertexColor = vec4(aColor.rgb, 1.0);
                                        gl_Position = projection * model * vec4(aPosition.xy, 0, 1.0);
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
            glBindVertexArray(vao);


            // x,y,r,g,b
            /*
            float[] vertices =
            {
                -50f, 50f, 1f, 0f, 0f, // top left
                50f, 50f, 0f, 1f, 0f,// top right
                -50f, -50f, 0f, 0f, 1f, // bottom left

                50f, 50f, 0f, 1f, 0f,// top right
                50f, -50f, 0f, 1f, 1f, // bottom right
                -50f, -50f, 1f, 0f, 1f, // bottom left
            }; */


            float[] vertices =
            {
                -50f, 50f, // top left
                50f, 50f , // top right
                -50f, -50f, // bottom left

                50f, 50f, // top right
                50f, -50f, // bottom right
                -50f, -50f, // bottom left
            };

            float[] colors =
            {
                 1f, 0f, 0f,
                 0f, 1f, 0f,
                 0f, 0f, 1f,
                 0f, 1f, 0f,
                 0f, 1f, 1f,
                 1f, 0f, 1f,
            };

            uint vertsId = glGenBuffer();
            uint colsId = glGenBuffer();


            glBindBuffer(GL_ARRAY_BUFFER, vertsId);
            fixed (float* v = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, vertices.Length * sizeof(float), v, GL_STATIC_DRAW);
            }
            // format of the data
            // index of buffer : 0 | first List of 2 elements | of type float | not normalised |
            // the next paire is 5 away | offset is 0 
            glVertexAttribPointer(0, 2, GL_FLOAT, false, sizeof(float) * 2, (void*)0);
            glEnableVertexAttribArray(0);



            glBindBuffer(GL_ARRAY_BUFFER, colsId);
            fixed (float* v = &colors[0])
            {
                glBufferData(GL_ARRAY_BUFFER, colors.Length * sizeof(float), v, GL_STATIC_DRAW);
            }
            // format of the data
            // index of buffer : 0 | first List of 2 elements | of type float | not normalised |
            // the next paire is 5 away | offset is 0 
            glVertexAttribPointer(1, 3, GL_FLOAT, false, sizeof(float) * 3, (void*)0);
            glEnableVertexAttribArray(1);


            Vector2 focusPos = new Vector2(100, 10);//DisplayManager.WindowSize;
            camera = new Camera2d(focusPos, 1f);

        }

        protected override void Update()
        {
            //Console.WriteLine(SimTime.DeltaTime);
        }
        protected override void Render()
        {
            glClearColor(0,0,0,1);
            glClear(GL_COLOR_BUFFER_BIT);

            shader.Use();
            camera.zoom = 2.5f;
            shader.SetMatrix4x4("projection", camera.GetProjectionMatrix());

            Vector2 position = new Vector2(100, 0);
            Vector2 scale = new Vector2(1,0.5f);
            float rotation = 3.1415f * (float)Math.Sin((float)SimTime.TotalElapsedSeconds * 0.2f);
            Debug.WriteLine(rotation);

            Matrix4x4 trans = Matrix4x4.CreateTranslation(position.X , position.Y, 0);
            Matrix4x4 sca = Matrix4x4.CreateScale(scale.X, scale.Y, 1);
            Matrix4x4 rot = Matrix4x4.CreateRotationZ(rotation);

            shader.SetMatrix4x4("model", sca * rot * trans);


            glBindVertexArray(vao);
            glDrawArrays(GL_TRIANGLES, 0, 6);

             position = new Vector2(150, 0);
             scale = new Vector2(0.7f, 0.5f);
             rotation = 3.1415f * (float)Math.Sin((float)SimTime.TotalElapsedSeconds * 0.3f);
            Debug.WriteLine(rotation);

             trans = Matrix4x4.CreateTranslation(position.X, position.Y, 0);
             sca = Matrix4x4.CreateScale(scale.X, scale.Y, 1);
             rot = Matrix4x4.CreateRotationZ(rotation);

            shader.SetMatrix4x4("model", sca * rot * trans);


            glBindVertexArray(vao);
            glDrawArrays(GL_TRIANGLES, 0, 6);
            glBindVertexArray(0);

            Glfw.SwapBuffers(DisplayManager.Window);
        }

    }
}
