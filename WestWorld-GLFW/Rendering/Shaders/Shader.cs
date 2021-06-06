using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using static OpenGLTutorial.OpenGL.GL;

namespace OpenGLTutorial.Rendering.Shaders
{
    class Shader
    {
        string vertexCode;
        string fragmentCode;

        public uint ProgramID { get; set; }

        public Shader(string vertexCode, string fragmentCode)
        {
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
                // Failed to compile
                string error = glGetShaderInfoLog(vs);
                Debug.WriteLine("ERROR COMPILING VERTEX SHADER: " + error);
            }

            fs = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fs, fragmentCode);
            glCompileShader(fs);

            status = glGetShaderiv(fs, GL_COMPILE_STATUS, 1);
            if (status[0] == 0)
            {
                // Failed to compile
                string error = glGetShaderInfoLog(fs);
                Debug.WriteLine("ERROR COMPILING FRAGMENT SHADER: " + error);
            }

            ProgramID = glCreateProgram();
            glAttachShader(ProgramID, vs);
            glAttachShader(ProgramID, fs);

            glLinkProgram(ProgramID);

            // Delete shaders

            glDetachShader(ProgramID, vs);
            glDetachShader(ProgramID, fs);
            glDeleteShader(vs);
            glDeleteShader(fs);
        }

        public void SetInt(string name, int value)
        {
            int location = glGetUniformLocation(ProgramID, name);
            glUniform1i(location, value);
        }

        public void SetMat4(string name, System.Numerics.Matrix4x4 matrix)
        {
            int location = glGetUniformLocation(ProgramID, name);

            float[] mat =
{
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44,
            };
            glUniformMatrix4fv(location, 1, false, mat);
        }

        public void Use()
        {
            glUseProgram(ProgramID);
        }

        public void SetMatrix4x4(string uniformName, Matrix4x4 mat)
        {
            int location = glGetUniformLocation(ProgramID, uniformName);
            glUniformMatrix4fv(location, 1, false, GetMatrix4x4Values(mat));

        }

        private float[] GetMatrix4x4Values(Matrix4x4 m)
        {
            return new float[]
            {
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44
            };
        }
    }
}
