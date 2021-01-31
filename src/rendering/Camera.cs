using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;

namespace perlin_noise_visualization.src.rendering {


    class Camera {

        public Vector3 position = new Vector3(0,1,0), eye = new Vector3(0,0,0), up = new Vector3(0,1,0);
        float speed = 15;
        float xRotation, yRotation;

        public Scene scene;
        Vector2 lastPosition;
        public static Vector2 currentMousePosition;
        public static Matrix4 proj, world, view;

        bool canRotate = false;

        public Camera(Scene scene) {
            this.scene = scene;
            scene.UpdateFrame += Update;

            scene.MouseMove += MouseMove;
            scene.MouseDown += MouseDown;
            scene.MouseUp   += MouseUp;
        }

        public void Update(FrameEventArgs e) {
            xRotation = Clamp(xRotation, -89.9f, 89.9f);
            eye.X = (float)Math.Cos(MathHelper.DegreesToRadians(xRotation)) * (float)Math.Cos(MathHelper.DegreesToRadians(yRotation));
            eye.Y = (float)Math.Sin(MathHelper.DegreesToRadians(xRotation));
            eye.Z = (float)Math.Cos(MathHelper.DegreesToRadians(xRotation)) * (float)Math.Sin(MathHelper.DegreesToRadians(yRotation));

            eye = Vector3.Normalize(eye);

            if (scene.IsKeyDown(Keys.W)) position += eye * speed;
            else if (scene.IsKeyDown(Keys.S)) position -= eye * speed;


            Vector3 right = Vector3.Normalize(Vector3.Cross(eye, up));

            if (scene.IsKeyDown(Keys.A)) position -= right * speed;
            if (scene.IsKeyDown(Keys.D)) position += right * speed;


            if (scene.IsKeyDown(Keys.Q)) NoiseMap.seed += .01f;
            if (scene.IsKeyDown(Keys.E)) NoiseMap.seed -= .01f;

            float correction;
            float virtualRatio = 1f;
            float deviceRatio = -scene.ClientSize.X/scene.ClientSize.Y;

            if (virtualRatio < deviceRatio) correction = -(scene.ClientSize.Y*2)/100;
            else correction = (scene.ClientSize.X*2)/100;

            float left = -scene.ClientSize.X/correction;
            float _right = scene.ClientSize.X/correction;

            float top = scene.ClientSize.Y/correction;
            float bottom = -scene.ClientSize.Y/correction;

            view = Matrix4.LookAt(position, position+eye, up); 
            world = Matrix4.Identity;
            Matrix4.CreateOrthographicOffCenter(left, _right, bottom, top, 0.1f, 2000f, out proj);
        }

        void MouseMove(MouseMoveEventArgs e) {

            if (canRotate)
            {
                xRotation += (lastPosition.Y - e.Y) * .5f;
                yRotation -= (lastPosition.X - e.X) * .5f;
            }
            lastPosition = new Vector2(e.X, e.Y);

            Vector2 screenPosition = new Vector2((e.X/scene.ClientSize.X)*2, (e.Y/scene.ClientSize.Y)*2);
            /* 
                (line 78)
                Minus 0.5 means that we can get the center position to be (0,0), then multiply by 2 to
                get the mouse between -1 and 1 on the X and Y axis. The Y axis needs to be inverted hense
                the "-" sign. This is only if you aren't using any Ortho projections to the screen.
                (line 67)
                The reason for multiplying the screenPosition by 2 is because the ClientSize X and Y are only half the width and height of the screen size,
                resulting in bounds from 0 to 0.5 on the X and Y axis.
            */
            Vector2 exactMouseScreenPosition = new Vector2((screenPosition.X-.5f)*2, -(screenPosition.Y-.5f)*2);

            currentMousePosition = screenPosition;
        }

        void MouseDown(MouseButtonEventArgs e) {
            if (e.Button == MouseButton.Right) canRotate = true;
        }
        void MouseUp(MouseButtonEventArgs e) {
            if (e.Button == MouseButton.Right) canRotate = false;
        }

        float Clamp(float value, float min, float max) {

            if (value > max) value = max;
            if (value < min) value = min;
            return value;
        }
    }
}