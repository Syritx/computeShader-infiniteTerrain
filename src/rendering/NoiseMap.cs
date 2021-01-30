using perlin_noise_visualization.src.shaders;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

using System;

namespace perlin_noise_visualization.src.rendering {

    class NoiseMap : Shader {

        int Resolution = 1200;
        int size = 50;
        float[] vertices;
        public static float seed = new Random().Next(0,100000);

        public NoiseMap(string v, string f) : base(v,f) {

            CreateVertices();

            VertexArrayObject = GL.GenVertexArray();
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VertexArrayObject);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
            
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
        }

        void CreateVertices() {
            int R = Resolution/2;
            List<float> vertices_list = new List<float>();

            for (int x = -R; x <= R; x++) {
                for (int y = -R; y <= R; y++) {
                    Vector2 toScreenCoords = new Vector2((float)x/(float)R, (float)y/(float)R);
                    //float multiplier = CreateNoiseLayer(3, 2, 0.5f, toScreenCoords.X+R, toScreenCoords.Y+R, seed);
                    vertices_list.Add(toScreenCoords.X*size);
                    vertices_list.Add(toScreenCoords.Y*size);
                    vertices_list.Add(1);
                    vertices_list.Add(1);
                    vertices_list.Add(1);
                }   
            }

            vertices = new float[vertices_list.Count];
            for (int i = 0; i < vertices.Length; i++) {
                vertices[i] = vertices_list[i];
            }
        }

        public override void Render(Camera camera) {
            base.Render(camera);
            GL.Enable(EnableCap.ProgramPointSize);

            Matrix4 world = Matrix4.Identity, view = Matrix4.LookAt(camera.position, camera.position+camera.eye, camera.up), projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), 1, .1f, 1000f);
            int seedLoc = GL.GetUniformLocation(Program, "s");
            Console.WriteLine(seedLoc);

            int m = GL.GetUniformLocation(Program, "model");
            int v = GL.GetUniformLocation(Program, "view");
            int p = GL.GetUniformLocation(Program, "projection");
            int c = GL.GetUniformLocation(Program, "cameraPosition");

            GL.UniformMatrix4(m, false, ref world);
            GL.UniformMatrix4(v, false, ref view);
            GL.UniformMatrix4(p, false, ref projection);
            GL.Uniform3(c, camera.position);
            GL.Uniform1(seedLoc, seed);

            Use();
            int pointSizeLocation = GL.GetUniformLocation(Program, "pointSize");
            GL.Uniform1(pointSizeLocation, 100);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.DrawArrays(PrimitiveType.Points, 0, vertices.Length);
        }
    }
}