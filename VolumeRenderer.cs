using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using static UniverseParticleSimulator.ParticleBatcher;

namespace UniverseParticleSimulator
{
    public class VolumeRenderer
    {
        int densityTex;
        int particleCount;

        public void UploadParticles(ParticleS[] particles)
        {
            particleCount = particles.Length;

            int particleSSBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, particleSSBO);
            GL.BufferData
            (
                BufferTarget.ShaderStorageBuffer,
                particles.Length * Marshal.SizeOf<Particle>(),
                particles,
                BufferUsageHint.DynamicDraw
            );

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, particleSSBO);
        }

        public void BindDensityTexture(int voxelGridSize)
        {
            //GL.UseProgram();

            densityTex = GL.GenTexture();

            //Bind densityTex to GL
            GL.BindTexture(TextureTarget.Texture3D, densityTex);

            //Define the 3d texture
            GL.TexImage3D(
                TextureTarget.Texture3D,
                0,
                PixelInternalFormat.R32f,
                voxelGridSize,
                voxelGridSize,
                voxelGridSize,
                0,
                PixelFormat.Red,
                PixelType.Float,
                IntPtr.Zero
            );

            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            //Bind texture as writable image. Is used in compute shader to atomically add particles 
            //On the density texture
            GL.BindImageTexture
            (
                0,
                densityTex,
                0,
                true,
                0,
                TextureAccess.WriteOnly,
                SizedInternalFormat.R32f
            );
        }

        public void ClearDensityTexture()
        {
            float zero = 0;
            GL.ClearTexImage(densityTex, 0, PixelFormat.Red, PixelType.Float, ref zero);
        }

        public void DispatchComputeShaders()
        {
            int groupCount = (particleCount + 255) / 256;

            GL.DispatchCompute(groupCount, 1, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
        }
    }
}
