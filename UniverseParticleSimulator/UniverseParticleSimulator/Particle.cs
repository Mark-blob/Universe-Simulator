using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace UniverseParticleSimulator
{
    public class Particle
    {
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ParticleStruct
    {
        public Vector4 position;
        public Vector4 velocity;

        public ParticleStruct(Vector3 position, Vector3 velocity, float mass, float color = 0f)
        {
            this.position = new Vector4(position, mass);
            this.velocity = new Vector4(velocity, color);
        }
    }
}