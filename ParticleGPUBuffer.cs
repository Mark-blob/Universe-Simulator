using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace UniverseParticleSimulator
{
    internal class ParticleGPUBuffer
    {
        Simulation simulation;
        int particleVAO;
        int particleVBO;

        public ParticleGPUBuffer(Simulation simulation)
        {
            this.simulation = simulation;
        }

        public void UploadParticles()
        {
            particleVAO = GL.GenVertexArray();
            particleVBO = GL.GenBuffer();

            GL.BindVertexArray(particleVAO);

            //
            int sizeInBytes = simulation.particles.Count * Marshal.SizeOf<ParticleStruct>();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, particleVBO);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, sizeInBytes, IntPtr.Zero, BufferUsageHint.DynamicCopy);

            GCHandle handle = GCHandle.Alloc(simulation.particles.ToArray(), GCHandleType.Pinned);
            try
            {
                GL.BufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, sizeInBytes, handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, particleVBO);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 32, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, particleVBO);
        }

        public void UpdateParticles(FrameEventArgs args, int programID, int cProgramID)
        {
            //send
            float deltaTime = (float)args.Time;
            SendUniformsToComputeShader(deltaTime, cProgramID);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, particleVBO);
            GL.DispatchCompute(simulation.ParticleAmount / 256 + 1, 1, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit | MemoryBarrierFlags.VertexAttribArrayBarrierBit);

            //vao
            GL.UseProgram(programID);
            GL.BindVertexArray(particleVAO);

            //vbo
            GL.BindBuffer(BufferTarget.ArrayBuffer, particleVBO);
        }

        private void SendUniformsToComputeShader(float deltaTime, int cProgramID)
        {
            int dtLocation = GL.GetUniformLocation(cProgramID, "dt");
            GL.Uniform1(dtLocation, deltaTime);
        }
    }
}
