using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace UniverseParticleSimulator
{
    internal class Shader
    {
        string fileName;
        public ShaderType ShaderType { get; private set; }
        public int ShaderLoc { get; private set; }

        public Shader(string fileName, ShaderType shaderType)
        {
            this.fileName = fileName;
            ShaderType = shaderType;
        }

        public void Init()
        {
            string shaderSource = File.ReadAllText($"../../../shaders/{fileName}.glsl");
            ShaderLoc = GL.CreateShader(ShaderType);
            GL.ShaderSource(ShaderLoc, shaderSource);
            GL.CompileShader(ShaderLoc);
            GL.GetShader(ShaderLoc, ShaderParameter.CompileStatus, out int status);
            if (status != (int)All.True)
            {
                string log = GL.GetShaderInfoLog(ShaderLoc);
                throw new Exception($"Error occurred whilst compiling shader ({ShaderLoc}):\n{log}");
            }
        }
    }
}
