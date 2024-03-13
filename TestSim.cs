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

namespace OpenGlTIPE.SimLoop
{
    internal class TestSim : SimulationModel
    {

        uint vao;

        Shader shader;

        Camera2d camera;

        List<SimObjects> objects;
        

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
            
            


            float[] vertices =
            {
                -0.5f,  0.5f, // top left
                 0.5f,  0.5f , // top right
                -0.5f, -0.5f, // bottom left

                 0.5f,  0.5f, // top right
                 0.5f, -0.5f, // bottom right
                -0.5f, -0.5f, // bottom left
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



            float s = 1800f;
            objects = SimObjects.GetRandomCubes(750, new Vector4(s,-s,+s,-s), new Vector2(25f,12f), 3.1415f).ToList();

            /*

            objects = new List<SimObjects>();
            VertexShapes shape1 = new VertexShapes(
                new Vector2(100, 100),
                new Vector2(100, 100),
                3.141562f/3f,
                vertices,
                colors,
                false);

            SimObjects object1 = new SimObjects(
                new Vector2(100, 100),
                new Vector2(0, 0),
                0f,
                3.141562f / 3f,
                "object1",
                shape1);



            VertexShapes shape2 = new VertexShapes(
                new Vector2(300, 300),
                new Vector2(200, 100),
                3.141562f / 6f,
                vertices,
                colors,
                false);

            SimObjects object2 = new SimObjects(
                new Vector2(300, 300),
                new Vector2(0, 0),
                0f,
                3.141562f / 6f,
                "object2",
                shape2);


            objects = new List<SimObjects>();
            objects.Add(object1);
            objects.Add(object2);
            */

            Vector2 focusPos = new Vector2(s, s);//DisplayManager.WindowSize;
            camera = new Camera2d(focusPos, 1f);

        }

        protected override void Update()
        {
            const float GravityConstant = 15000f;
            //const float threshold = 0.01f;


            foreach (SimObjects mainObj in objects)
            {

                Vector2 acc = Vector2.Zero;
                foreach (SimObjects pair in objects)
                {
                    float dist = Vector2.Distance(mainObj.position, pair.position);

                    if (dist == 0f) { continue; }

                    
                    acc += (pair.position - mainObj.position) * GravityConstant / (dist * dist * dist + 100);

                }

                //Debug.WriteLine(mainObj.name + " " + mainObj.position);
                mainObj.velocity += acc * SimTime.DeltaTime;
                mainObj.position += mainObj.velocity * SimTime.DeltaTime;
            }
        }
        protected override void Render()
        {
            glClearColor(0,0,0,1);
            glClear(GL_COLOR_BUFFER_BIT);

            shader.Use();
            camera.zoom = 0.1f;
            shader.SetMatrix4x4("projection", camera.GetProjectionMatrix());

            /*
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
            glBindVertexArray(0);*/
            objects[0].rotation = (float)Math.Sin((double)SimTime.TotalElapsedSeconds);
            Debug.WriteLine("fps : " + 1 / SimTime.DeltaTime);

            double t1 = Glfw.Time;
            foreach (SimObjects shape in objects)
            {
                shape.draw(shader);
            }
            Debug.WriteLine("Image drawn in " + (Glfw.Time - t1) + " seconds (" + 1f/(Glfw.Time - t1) + " calls/secs)");

            Glfw.SwapBuffers(DisplayManager.Window);
        }

    }
}
