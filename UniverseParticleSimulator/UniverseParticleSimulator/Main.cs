using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;
//using OpenTK.Graphics.OpenGL;

namespace UniverseParticleSimulator
{
    class AppWindow : GameWindow
    {
        //member vars
        public Simulation simulation;
        public ParticleGPUBuffer buffer;
        public Surface screen;
        public Camera camera;

        public int programID;
        public int cProgramID;

        List<Shader> shaders = new();

        public AppWindow(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                Title = title,
                Size = new Vector2i(width, height),
                WindowBorder = WindowBorder.Resizable,
                StartVisible = false,
                StartFocused = true,
                WindowState = WindowState.Normal,
                API = ContextAPI.OpenGL,
                Profile = ContextProfile.Core,
                APIVersion = new Version(4, 5)
            })
        {
            CenterWindow();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            IsVisible = true;
            base.OnLoad();
            screen = new Surface(ClientSize.X, ClientSize.Y);
            simulation = new Simulation(screen);
            buffer = new ParticleGPUBuffer(simulation);
            simulation.Start();
            camera = simulation.Camera;
            GL.ClearColor(0, 0, 0, 1);

            //init shaders
            Shader cShader = new Shader("screen_cs", ShaderType.ComputeShader);
            Shader vShader = new Shader("screen_vs", ShaderType.VertexShader);
            Shader fShader = new Shader("screen_fs", ShaderType.FragmentShader);
            shaders.Add(cShader);
            shaders.Add(vShader);
            shaders.Add(fShader);

            foreach (Shader shader in shaders)
            {
                shader.Init();
            }

            //settings
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ProgramPointSize);

            //particles
            buffer.UploadParticles();

            // Program: a set of shaders to be used together in a pipeline
            cProgramID = GL.CreateProgram();
            programID = GL.CreateProgram();

            foreach (Shader shader in shaders)
            {
                switch (shader.ShaderType)
                {
                    case ShaderType.ComputeShader: GL.AttachShader(cProgramID, shader.ShaderLoc); break;
                    default: GL.AttachShader(programID, shader.ShaderLoc); break;
                }
            }

            LinkPrograms();

            // send all the following draw calls through this pipeline
            GL.UseProgram(cProgramID);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);

            GL.UseProgram(programID);
            // tell the VAO which part of the VBO data should go to each shader input
            //int location = GL.GetAttribLocation(programID, "vPosition");
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            simulation.Tick(KeyboardState, (float)args.Time);

            //camera
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();
            int viewLoc = GL.GetUniformLocation(programID, "view");
            int projLoc = GL.GetUniformLocation(programID, "projection");
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);

            //compute shader
            GL.UseProgram(cProgramID);
            buffer.UpdateParticles(args, programID, cProgramID);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //
            GL.DrawArrays( PrimitiveType.Points, 0, simulation.ParticleAmount);

            //
            SwapBuffers();
            base.OnRenderFrame(args);
        }

        private void LinkPrograms()
        {
            GL.LinkProgram(cProgramID);
            GL.LinkProgram(programID);
            
            int status;
            GL.GetProgram(cProgramID, GetProgramParameterName.LinkStatus, out status);
            if (status != (int)All.True)
            {
                string log = GL.GetProgramInfoLog(cProgramID);
                throw new Exception($"Error occurred whilst linking compute program ({cProgramID}):\n{log}");
            }
            GL.GetProgram(programID, GetProgramParameterName.LinkStatus, out status);
            if (status != (int)All.True)
            {
                string log = GL.GetProgramInfoLog(programID);
                throw new Exception($"Error occurred whilst linking program ({programID}):\n{log}");
            }

            // the program contains the compiled shaders, we can delete the source
            foreach (Shader shader in shaders)
            {
                switch (shader.ShaderType)
                {
                    case ShaderType.ComputeShader: GL.DetachShader(cProgramID, shader.ShaderLoc); break;
                    default: GL.DetachShader(programID, shader.ShaderLoc); break;
                }

                GL.DeleteShader(shader.ShaderLoc);
            }
        }
    }
}
