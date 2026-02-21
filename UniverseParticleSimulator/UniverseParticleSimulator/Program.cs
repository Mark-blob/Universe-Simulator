namespace UniverseParticleSimulator
{
    class Program
    {
        static void Main()
        {
            using AppWindow window = new AppWindow(960, 720, "Sim");
            window.Run();
        }
    }
}