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
using OpenGlTIPE.Rendering.shapes;
using OpenGlTIPE.SimulationData;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using OpenGlTIPE.SimLoop;


namespace OpenGlTIPE
{

    struct TestObjects
    {

    }



    internal class InstancingTest : SimulationModel
    {
        uint vao;

        Shader shader;
        Camera2d camera;
        List<Vector2> objectsPos;


        public InstancingTest(int initialWindowWidth, int initialWindowHeight, string initialWindowTitle, bool vsync) : base(initialWindowWidth, initialWindowHeight, initialWindowTitle, vsync)
        {
        }

        protected override void Initialize()
        {
        }

        protected unsafe override void LoadContent() {

            string vertexShader = @"#version 330 core
                                    layout (location = 0) in vec2 aPosition;
                                    layout (location = 1) in vec3 aColor;
                                    layout (location = 2) in mat4 aModel;
                                    out vec4 vertexColor;
    
                                    uniform mat4 projection;


                                    void main() 
                                    {
                                        vertexColor = vec4(aColor.rgb, 1.0);
                                        gl_Position = projection * aModel * vec4(aPosition.xy + aOffset, 0, 1.0);
                                    }";

            string fragmentShader = @"#version 330 core
                                    out vec4 FragColor;
                                    in vec4 vertexColor;

                                    void main() 
                                    {
                                        FragColor = vertexColor;
                                    }";


            float[] cubeVertices =
            {
                -0.25f,  0.5f, // top left
                 0.5f,  0.5f , // top right
                -0.5f, -0.5f, // bottom left

                 0.5f,  0.5f, // top right
                 0.5f, -0.5f, // bottom right
                -0.5f, -0.5f, // bottom left
            };
            float[] cubeColors =
            {
                 1f, 0f, 0f,
                 0f, 1f, 0f,
                 0f, 0f, 1f,
                 0f, 1f, 0f,
                 0f, 1f, 1f,
                 1f, 0f, 1f,
            };


            vao = glGenVertexArray();
            glBindVertexArray(vao);

            // verts VBO :
            uint InstanceVBo = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, InstanceVBo);
            fixed (float* v = &cubeVertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * 2 * cubeVertices.Length, v, GL_STATIC_DRAW);
            }
            glVertexAttribPointer(0, 2, GL_FLOAT, false, sizeof(float) * 2, (void*)0);
            glEnableVertexAttribArray(0);


            // Color VBO :
            InstanceVBo = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, InstanceVBo);
            fixed (float* v = &cubeColors[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * 2 * cubeVertices.Length, v, GL_STATIC_DRAW);
            }
            glVertexAttribPointer(1, 3, GL_FLOAT, false, sizeof(float) * 3, (void*)0);
            glEnableVertexAttribArray(1);


            objectsPos = new List<Vector2>();
            int cubeSize = 10;
            for (int x = -cubeSize/2; x < cubeSize/2; x++){
                for (int y = -cubeSize / 2; y < cubeSize / 2; y++) {
                    objectsPos.Add(new Vector2(x, y));
                }
            }

            float[] instancesMats = new float[objectsPos.Count * 16];


            int i = 0;
            foreach(Vector2 pos in objectsPos)
            {
                Matrix4x4 trans = Matrix4x4.CreateTranslation(pos.X, pos.Y, 0);
                float[] matvalues = Shader.GetMatrix4x4Values(trans);
                for (int j = 0;  j < 16; j++) {
                    instancesMats[i*16+j] = matvalues[j];
                }
                i++;
            }




            // Bind the Model Buffer
            InstanceVBo = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, InstanceVBo);

            for (uint bI = 0; bI < 4; bI++)
            {
                uint localId = InstanceVBo + bI;
                glVertexAttribPointer(localId, 4, GL_FLOAT, false, sizeof(float) * 16, (void*)(i * 4));
                glVertexAttribDivisor(localId, 1);
                glEnableVertexAttribArray(localId);
            }

            fixed (float* v = &instancesMats[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * instancesMats.Length, v, GL_STATIC_DRAW);
            }




            Vector2 focusPos = new Vector2(0, 0);//DisplayManager.WindowSize;
            camera = new Camera2d(focusPos, 1f);


            shader = new Shader(vertexShader, fragmentShader);
            shader.Load();
        }

        protected override void Update()
        {

        }


        protected override void Render()
        {
            glClearColor(0, 0, 0, 1);
            glClear(GL_COLOR_BUFFER_BIT);

            shader.Use();
            camera.zoom = .1f;
            shader.SetMatrix4x4("projection", camera.GetProjectionMatrix());



            glBindVertexArray(vao);
            glDrawArrays(GL_TRIANGLES, 0, 6);
            glBindVertexArray(0); 
            Debug.WriteLine("fps : " + 1 / SimTime.DeltaTime);


            // Instancing objects


            double t1 = Glfw.Time;

            glDrawArraysInstanced(GL_TRIANGLES, 0, 6, 0);

            Glfw.SwapBuffers(DisplayManager.Window);
            Debug.WriteLine("Image drawn in " + (Glfw.Time - t1) + " seconds (" + 1f / (Glfw.Time - t1) + " calls/secs)");

        }
    } 
}
