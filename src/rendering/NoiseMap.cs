using perlin_noise_visualization.src.shaders;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

using System;

namespace perlin_noise_visualization.src.rendering {

    class NoiseMap : Shader {

        int Resolution = 1200;
        int size = 100;
        float[] vertices;
        uint[] indices;

        public static float seed = new Random().Next(0,100000);

        public NoiseMap(string v, string f) : base(v,f) {

            CreateVertices();

            VertexArrayObject = GL.GenVertexArray();
            VertexBufferObject = GL.GenBuffer();
            IndexBufferArray = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferArray);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferArray);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
            
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
        }

        void CreateVertices() {
            int R = Resolution/2;
            List<float> vertices_list = new List<float>();
            List<uint> inds = new List<uint>();

            for (int x = 0; x < Resolution; x++) {
                for (int y = 0; y < Resolution; y++) {

                    //float multiplier = CreateNoiseLayer(3, 2, 0.5f, toScreenCoords.X+R, toScreenCoords.Y+R, seed);
                    vertices_list.Add((float)(x-R)/R*size);
                    vertices_list.Add((float)(y-R)/R*size);
                    vertices_list.Add(1);
                    vertices_list.Add(1);
                    vertices_list.Add(1);

                    uint index = (uint)(y+x*Resolution);

                    if (x < Resolution-1 && y < Resolution-1) {
                        inds.Add(index);
                        inds.Add(index+(uint)Resolution+1);
                        inds.Add(index+(uint)Resolution);

                        inds.Add(index);
                        inds.Add(index+1);
                        inds.Add(index+(uint)Resolution+1);
                    }
                }   
            }

            vertices = new float[vertices_list.Count];
            for (int i = 0; i < vertices.Length; i++) {
                vertices[i] = vertices_list[i];
            }

            indices = new uint[inds.Count];
            int id = 0;
            foreach (uint a in inds) {
                indices[id] = a;
                id++;
            }
        }

        public override void Render(Camera camera) {
            base.Render(camera);
            GL.Enable(EnableCap.ProgramPointSize);

            Matrix4 world = Matrix4.Identity, view = Matrix4.LookAt(camera.position, camera.position+camera.eye, camera.up), projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), 1, .1f, 1000f);
            int seedLoc = GL.GetUniformLocation(Program, "s");

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
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}