using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace OsuPatcher.UI
{
    internal static class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
            {
                var name = new AssemblyName(args.Name).Name + ".dll";
                var asm  = Assembly.GetExecutingAssembly();
                // Embedded as "OsuPatcher.UI.<name>" (default manifest resource name)
                foreach (var resource in asm.GetManifestResourceNames())
                {
                    if (!resource.EndsWith(name, StringComparison.OrdinalIgnoreCase)) continue;
                    using (var stream = asm.GetManifestResourceStream(resource))
                    {
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        return Assembly.Load(bytes);
                    }
                }
                return null;
            };
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
