using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace UniverseParticleSimulator
{
    public class Camera
    {
        public Vector3 position;
        public Vector2 rotationAngles;

        private Vector3 lookDirection, upDirection, rightDirection;
        public Vector3 LookDirection { get { return lookDirection; } set { lookDirection = value.Normalized(); } }
        public Vector3 UpDirection { get { return upDirection; } set { upDirection = value.Normalized(); } }
        public Vector3 RightDirection { get { return rightDirection; } set { rightDirection = value.Normalized(); } }

        public float focalLength, aspectRatio;
        public float FieldOfView
        {
            get => 2f * MathF.Atan(1f / focalLength) * (180f / MathF.PI);
            set
            {
                float fovRadians = value * (MathF.PI / 180f);
                focalLength = 1f / MathF.Tan(fovRadians / 2f);
            }
        }
        public Surface Screen { get; private set; }

        public Vector3 ImageCenter => position + focalLength * LookDirection;
        public Vector3 ImageCornerTL => ImageCenter + UpDirection - aspectRatio * RightDirection;
        public Vector3 ImageCornerTR => ImageCenter + UpDirection + aspectRatio * RightDirection;
        public Vector3 ImageCornerBL => ImageCenter - UpDirection - aspectRatio * RightDirection;
        public Vector3 ScreenHorizontal => ImageCornerTR - ImageCornerTL;
        public Vector3 ScreenVertical => ImageCornerBL - ImageCornerTL;


        public Camera(Vector3 position, Surface screen)
        {
            this.position = position;
            this.Screen = screen;
            LookDirection = Vector3.UnitZ;
            UpDirection = Vector3.UnitY;
            RightDirection = Vector3.UnitX;
            FieldOfView = 90;
            aspectRatio = 1.6f;
        }

        public Vector3 PixelCoordToWorld(int x, int y)
        {
            float a = (x + 0.5f) / Screen.width;
            float b = (y + 0.5f) / Screen.height;

            return ImageCornerTL + a * ScreenHorizontal + b * ScreenVertical;
        }

        public Matrix4 GetViewMatrix()
        {
            Vector3 f = LookDirection.Normalized();
            Vector3 r = RightDirection.Normalized();
            Vector3 u = UpDirection.Normalized();

            return new Matrix4(
                r.X, u.X, -f.X, 0,
                r.Y, u.Y, -f.Y, 0,
                r.Z, u.Z, -f.Z, 0,
                -Vector3.Dot(r, position),
                -Vector3.Dot(u, position),
                 Vector3.Dot(f, position),
                1
            );
        }

        public Matrix4 GetProjectionMatrix()
        {
            float fovRadians = 2f * MathF.Atan(1f / focalLength);
            float near = 0.1f;
            float far = 1000f;
            return Matrix4.CreatePerspectiveFieldOfView(
                fovRadians,
                aspectRatio,
                near,
                far
            );
        }
    }
}
