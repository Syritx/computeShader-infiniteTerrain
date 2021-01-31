using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace perlin_noise_visualization.src.rendering {
    class Scene : GameWindow {

        NoiseMap cube;
        Camera camera;

        public static Vector2 ScreenSize;

        float elapsed = 0;

        public Scene(GameWindowSettings GWS, NativeWindowSettings NWS) : base(GWS, NWS) {
            Run();
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            cube.Render(camera);
            SwapBuffers();

            ScreenSize = new Vector2(ClientSize.X*2, ClientSize.X*2);
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
        }

        protected override void OnLoad() {
            base.OnLoad();
            camera = new Camera(this);
            cube = new NoiseMap("src/shaders/glsl/vertexShader.glsl", "src/shaders/glsl/fragmentShader.glsl");

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ProgramPointSize);
            GL.ClearColor(0.1f,0.1f,0.1f,1.0f);
        }
    }
}