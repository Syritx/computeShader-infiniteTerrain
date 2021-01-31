using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Text;
using System.IO;
using perlin_noise_visualization.src.rendering;

namespace perlin_noise_visualization.src.shaders {

    abstract class Shader {

        public int VertexArrayObject, VertexBufferObject, IndexBufferArray, Program;
        int VertexShader, FragmentShader;

        public Shader(string VertexShaderPath, string FragmentShaderPath) {

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(VertexShader, GetShaderSource(VertexShaderPath));
            GL.ShaderSource(FragmentShader, GetShaderSource(FragmentShaderPath));
            GL.CompileShader(VertexShader);
            GL.CompileShader(FragmentShader);

            string info;
            GL.GetShaderInfoLog(FragmentShader,out info);
            System.Console.WriteLine(info);

            GL.GetShaderInfoLog(VertexShader,out info);
            System.Console.WriteLine(info);

            Program = GL.CreateProgram();
            GL.AttachShader(Program, VertexShader);
            GL.AttachShader(Program, FragmentShader);
            GL.LinkProgram(Program);
        }

        public void Use() {
            GL.UseProgram(Program);
        }

        public virtual void Render(Camera camera) {}

        public void SetUniformVector3(string name, Vector3 a) {
            
        }

        string GetShaderSource(string path) {
            string src = null;

            using (StreamReader r = new StreamReader(path, Encoding.UTF8)) {
                src = r.ReadToEnd();
            }
            return src;
        }
    }
}