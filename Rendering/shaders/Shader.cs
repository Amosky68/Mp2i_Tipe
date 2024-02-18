using static OpenGlTIPE.OpenGL.GL;
using GLFW;
using OpenGlTIPE.Rendering.Display;
using System.Diagnostics;



namespace OpenGlTIPE.Rendering.shaders
{
    class Shader
    {
        string vertexCode;
        string fragmentCode;


        public uint ProgramID { get;  set; }

        public Shader(string vertexCode, string fragmentCode) {
            this.vertexCode = vertexCode;
            this.fragmentCode = fragmentCode;
        }

        public void Load()
        {
            uint vs, fs;

            vs = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vs, vertexCode);
            glCompileShader(vs);

            int[] status = glGetShaderiv(vs, GL_COMPILE_STATUS, 1);
            if (status[0] == 0)
            {
                string error = glGetShaderInfoLog(vs);
                Debug.WriteLine("ERROR compiling shader : " + error);
            }

            fs = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fs, fragmentCode);
            glCompileShader(fs);

            status = glGetShaderiv(vs, GL_COMPILE_STATUS, 1);
            if (status[0] == 0)
            {
                string error = glGetShaderInfoLog(vs);
                Debug.WriteLine("ERROR compiling shader : " + error);
            }

            ProgramID = glCreateProgram();
            glAttachShader(ProgramID, vs);
            glAttachShader(ProgramID, fs);

            glLinkProgram(ProgramID);

            glDetachShader(ProgramID, vs);
            glDetachShader(ProgramID, fs);
            glDeleteShader(vs);
            glDeleteShader(fs);
        }


        public void Use()
        {
            glUseProgram(ProgramID);
        }

    }
}
