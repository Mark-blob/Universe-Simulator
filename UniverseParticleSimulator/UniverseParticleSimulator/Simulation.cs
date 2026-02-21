using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace UniverseParticleSimulator
{
    internal class Simulation
    {
        public Surface screen;

        public SceneExplorer sceneExplorer;
        const float mouseSens = 12f;
        const float moveSpeedC = 6f;
        float moveSpeed;
        bool shiftDown = false;

        public List<ParticleStruct> particles;
        public Vector3[] particlePositions; //
        public Camera Camera { get; private set; }

        public int ParticleAmount { get; private set; }

        public Simulation(Surface screen)
        {
            this.screen = screen;
        }

        public void Start()
        {
            AddCamera(new Vector3(0, 2.5f, -8), screen);
            sceneExplorer = new SceneExplorer(Camera);

            ParticleAmount = 1000000;
            particles = new List<ParticleStruct>();

            Random rng = new Random();

            for (int i = 0; i < ParticleAmount; i++)
            {
                Vector3 pos = new Vector3(0, 0, 50);

                //random velocity
                float angleXY = (float)(rng.NextDouble() * 2 * Math.PI);   //horizontal angle
                float angleZ = (float)(rng.NextDouble() * Math.PI / 4 - Math.PI / 8); //vertical spread

                float speed = (float)(rng.NextDouble() * 5 + 0.01f); //random speed

                Vector3 vel = new Vector3(
                    speed * (float)Math.Cos(angleXY) * (float)Math.Cos(angleZ),
                    speed * (float)Math.Sin(angleZ),
                    speed * (float)Math.Sin(angleXY) * (float)Math.Cos(angleZ)
                );

                particles.Add(new ParticleStruct(pos, vel, 10));
            }
        }

        public void Tick(KeyboardState keyboard, float dt)
        {
            sceneExplorer.MoveCamera();
            sceneExplorer.RotateCamera(sceneExplorer.deltaCameraAngles);

            HandleInput(keyboard, dt);
        }

        public void HandleInput(KeyboardState keyboard, float dt)
        {
            shiftDown = keyboard.IsKeyDown(Keys.LeftShift);
            if (shiftDown) { moveSpeed = 2 * moveSpeedC; } else { moveSpeed = moveSpeedC; }

            if (keyboard.IsKeyDown(Keys.W)) sceneExplorer.direction += Vector3.UnitZ * moveSpeed * dt;
            if (keyboard.IsKeyDown(Keys.S)) sceneExplorer.direction -= Vector3.UnitZ * moveSpeed * dt;
            if (keyboard.IsKeyDown(Keys.D)) sceneExplorer.direction += Vector3.UnitX * moveSpeed * dt;
            if (keyboard.IsKeyDown(Keys.A)) sceneExplorer.direction -= Vector3.UnitX * moveSpeed * dt;

            if (keyboard.IsKeyDown(Keys.Space)) sceneExplorer.direction += 1f * Vector3.UnitY * moveSpeed * dt;
            if (keyboard.IsKeyDown(Keys.C)) sceneExplorer.direction -= 1f * Vector3.UnitY * moveSpeed * dt;

            if (keyboard.IsKeyDown(Keys.Up)) sceneExplorer.deltaCameraAngles -= mouseSens * Vector2.UnitX * dt;
            if (keyboard.IsKeyDown(Keys.Down)) sceneExplorer.deltaCameraAngles += mouseSens * Vector2.UnitX * dt;
            if (keyboard.IsKeyDown(Keys.Right)) sceneExplorer.deltaCameraAngles += mouseSens * Vector2.UnitY * dt;
            if (keyboard.IsKeyDown(Keys.Left)) sceneExplorer.deltaCameraAngles -= mouseSens * Vector2.UnitY * dt;
        }

        public void AddCamera(Vector3 position, Surface screen)
        {
            if (Camera == null)
            {
                Camera = new Camera(position, screen);
            }
        }
    }
}
