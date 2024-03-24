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
                                    layout (location = 0) in vec2 aVertsPosition;
                                    layout (location = 1) in vec3 aColor;
                                    layout (location = 2) in mat4 aMatrix;



                                    //layout (location = 6) in vec2 aShapePos;


                                    out vec4 vertexColor;
    
                                    uniform mat4 projection;

                                    mat4 aMat4 = mat4(1.0, 0.0, 0.0, 0.0,  // 1. column
                                                      0.0, 1.0, 0.0, 0.0,  // 2. column
                                                      0.0, 0.0, 1.0, 0.0,  // 3. column
                                                      0.0, 0.0, 0.0, 1.0); // 4. column



                                    void main() 
                                    {
                                        int coll = 3;
                                        int line = 3;
                                        float divider = 1.0;
                                        mat4 testMatrix = aMatrix;

                                        vec3 collTst = vec3(testMatrix[coll][0],testMatrix[coll][1],testMatrix[coll][2]);
                                        vec3 lineTst = vec3(testMatrix[0][line],testMatrix[1][line],testMatrix[2][line]) * divider;
                                        //vec3 collone = c4.xyz;

                                        vertexColor = vec4(aColor.xyz, 1.0);
                                        gl_Position = projection * aMatrix * vec4(aVertsPosition.xy, 0, 1.0);
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
                 1f, 0f, 1f,
                 0f, 1f, 0f,
                 0f, 1f, 1f,
                 1f, 0f, 1f,
            };


            vao = glGenVertexArray();
            glBindVertexArray(vao);
            uint InstanceVBo;

            // verts VBO :
            
            InstanceVBo = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, InstanceVBo);
            fixed (float* v = &cubeVertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * cubeVertices.Length, v, GL_STATIC_DRAW);
            }
            glVertexAttribPointer(0, 2, GL_FLOAT, false, sizeof(float) * 2, (void*)0);
            glEnableVertexAttribArray(0);

            // Color VBO :
            InstanceVBo = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, InstanceVBo);
            fixed (float* c = &cubeColors[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * cubeColors.Length, c, GL_STATIC_DRAW);
            }
            glVertexAttribPointer(1, 3, GL_FLOAT, false, sizeof(float) * 3, (void*)0);
            glEnableVertexAttribArray(1);





            objectsPos = new List<Vector2>();
            int cubeSize = 10;
            for (int x = -cubeSize / 2; x < cubeSize / 2; x++)
            {
                for (int y = -cubeSize / 2; y < cubeSize / 2; y++)
                {
                    objectsPos.Add(new Vector2(x, y));
                }
            }

            float[] instancesMats = new float[objectsPos.Count * 16];


            /*
            // positions 
            InstanceVBo = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, InstanceVBo);
            float[] posArry = new float[objectsPos.Count * 2];
            for (int s = 0; s < objectsPos.Count; s++) { posArry[s * 2] = objectsPos[s].X; posArry[s * 2 + 1] = objectsPos[s].Y; }


            fixed (float* c = &posArry[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * cubeColors.Length, c, GL_STATIC_DRAW);
            }
            glVertexAttribPointer(6, 2, GL_FLOAT, false, sizeof(float) * 2, (void*)0);
            glVertexAttribDivisor(6, 1);
            glEnableVertexAttribArray(6);
            */
            
            int i = 0;
            foreach(Vector2 pos in objectsPos)
            {
                Matrix4x4 trans = Matrix4x4.CreateTranslation(pos.X, pos.Y, 0); //CreateTranslation(pos.X, pos.Y, 0);
                Debug.WriteLine("trans : " + trans);
                float[] matvalues = Shader.GetMatrix4x4Values(trans);
                for (int j = 0;  j < 16; j++) {
                    instancesMats[i*16+j] = matvalues[j];
                }
                i++;
            }
            Debug.WriteLine("[{0}]", string.Join(", ", instancesMats));




            glBindVertexArray(vao);

            // Bind the Model Buffer
            uint MatrixBuffer = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, MatrixBuffer);
            fixed (float* v = &instancesMats[0])  {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * instancesMats.Length, v, GL_STATIC_DRAW);
            }

            glVertexAttribPointer(2, 4, GL_FLOAT, false, sizeof(float) * 16, (void*)(0 * sizeof(float)));
            glVertexAttribPointer(3, 4, GL_FLOAT, false, sizeof(float) * 16, (void*)(4 * sizeof(float)));
            glVertexAttribPointer(4, 4, GL_FLOAT, false, sizeof(float) * 16, (void*)(8 * sizeof(float)));
            glVertexAttribPointer(5, 4, GL_FLOAT, false, sizeof(float) * 16, (void*)(12 * sizeof(float)));

            glVertexAttribDivisor(2, 1);
            glVertexAttribDivisor(3, 1);
            glVertexAttribDivisor(4, 1);
            glVertexAttribDivisor(5, 1);

            glEnableVertexAttribArray(2);
            glEnableVertexAttribArray(3);
            glEnableVertexAttribArray(4);
            glEnableVertexAttribArray(5);


            //glEnableVertexAttribArray(2-3-4-5);


            /*
            for (uint bI = 0; bI < 4; bI++)
            {
                uint localId = 2 + bI;
                glEnableVertexAttribArray(localId);
                glVertexAttribPointer(localId, 4, GL_FLOAT, false, sizeof(float) * 16, (void*)(i * 4 * sizeof(float)));
                glVertexAttribDivisor(localId, 1);
            }

            fixed (float* v = &instancesMats[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * instancesMats.Length, v, GL_STATIC_DRAW);
            }*/



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
            camera.zoom = 100.1f;
            shader.SetMatrix4x4("projection", camera.GetProjectionMatrix());



            glBindVertexArray(vao);
            //Debug.WriteLine("fps : " + 1 / SimTime.DeltaTime);


            // Instancing objects


            double t1 = Glfw.Time;

            glDrawArraysInstanced(GL_TRIANGLES, 0, 6, 50);

            Glfw.SwapBuffers(DisplayManager.Window);
            //Debug.WriteLine("Image drawn in " + (Glfw.Time - t1) + " seconds (" + 1f / (Glfw.Time - t1) + " calls/secs)");
            glBindVertexArray(0);

        }
    } 
}
