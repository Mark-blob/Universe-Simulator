using OpenTK.Mathematics;

namespace UniverseParticleSimulator
{
    public class SceneExplorer
    {
        Camera camera;

        public Vector3 direction;
        public Vector2 deltaCameraAngles;

        float cameraSpeed = 1;
        float cameraRotateSpeed = 0.1f;
        float maxCameraAngle;

        float CameraYRadian => camera.rotationAngles.Y * cameraRotateSpeed;

        public SceneExplorer(Camera camera)
        {
            this.camera = camera;
            maxCameraAngle = (2 / cameraRotateSpeed) * MathF.PI;
        }

        public void MoveCamera()
        {
            camera.position += RotateVectorByAngle(direction, CameraYRadian) * cameraSpeed;

            direction = Vector3.Zero;
        }

        public void RotateCamera(Vector2 angles)
        {
            camera.rotationAngles += angles;
            camera.rotationAngles.Y %= maxCameraAngle;
            //Console.WriteLine();

            Quaternion rotateX = Quaternion.FromAxisAngle(Vector3.UnitX, camera.rotationAngles.X * cameraRotateSpeed);
            Quaternion rotateY = Quaternion.FromAxisAngle(Vector3.UnitY, camera.rotationAngles.Y * cameraRotateSpeed);
            Quaternion combined = (rotateY * rotateX).Normalized();

            camera.LookDirection = Vector3.Transform(Vector3.UnitZ, combined);
            camera.UpDirection = Vector3.Transform(Vector3.UnitY, combined);
            camera.RightDirection = Vector3.Transform(Vector3.UnitX, combined);

            deltaCameraAngles = Vector2.Zero;
        }
        public static Vector3 RotateVectorByAngle(Vector3 vector, float angle)
        {
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            float x = vector.X * cos + vector.Z * sin;
            float z = -vector.X * sin + vector.Z * cos;
            return new Vector3(x, vector.Y, z);
        }
    }
}
