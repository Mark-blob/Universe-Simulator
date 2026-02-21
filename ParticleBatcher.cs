using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace UniverseParticleSimulator
{
    public class ParticleBatcher
    {


        [StructLayout(LayoutKind.Sequential)]
        public struct ParticleS
        {
            public Vector3 position;
            public Vector3 velocity;
        }
    }
}
