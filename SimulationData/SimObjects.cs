using OpenGlTIPE.Rendering.shaders;
using OpenGlTIPE.Rendering.shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace OpenGlTIPE.SimulationData
{
    class SimObjects
    {

        public Vector2 position;
        public Vector2 velocity;
        public float anglularVelocity;
        public float rotation;

        public string name;

        public VertexShapes shape;

        public SimObjects(Vector2 position, Vector2 velocity, float anglularVelocity, float rotation, string name, VertexShapes shape)
        {
            this.position = position;
            this.velocity = velocity;
            this.anglularVelocity = anglularVelocity;
            this.rotation = rotation;
            this.name = name;
            this.shape = shape;
            Debug.WriteLine("Init | name : " + name + " with position : " + position);
        }

        public void draw(Shader shaderProgram)
        {
            shape.SetPosition(position);
            shape.SetRotation(rotation);
            shape.draw(shaderProgram);
        }



        public static SimObjects[] GetRandomCubes(int amount, Vector4 PosRange, Vector2 scaleRange, float randomRotationRange)
        {

            float[] vertices =
            {
                -0.5f,  0.5f, // top left
                 0.5f,  0.5f , // top right
                -0.5f, -0.5f, // bottom left

                 0.5f,  0.5f, // top right
                 0.5f, -0.5f, // bottom right
                -0.5f, -0.5f, // bottom left
            };
            


            SimObjects[] returncubes = new SimObjects[amount];
            Random random = new Random();

            for (int i = 0; i < amount; i++)
            {
                Vector2 pos = new Vector2((float)random.NextDouble() * (PosRange.X - PosRange.Y) + PosRange.X, (float)random.NextDouble() * (PosRange.Z - PosRange.W) + PosRange.Z);
                float sca = (float)random.NextDouble() * (scaleRange.X - scaleRange.Y) + scaleRange.X;
                float rot = ((float)random.NextDouble() * 2 - 1f) * randomRotationRange;

                float r = (float)random.NextDouble();
                float g = (float)random.NextDouble();
                float b = (float)random.NextDouble();

                float[] colors =
                {
                 r, g, b,
                 r, g, b,
                 r, g, b,
                 r, g, b,
                 r, g, b,
                 r, g, b
                };      


                var shape = new VertexShapes(pos, new Vector2(sca, sca), rot, vertices, colors, false);
                returncubes[i] = new SimObjects(pos, Vector2.Zero, 0f, rot, i.ToString(), shape);
            }

            return returncubes;
        }
        

    }
}
