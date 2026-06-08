using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace OsuPatcher.Runtime.Helpers
{
    internal sealed class ILPatch
    {
        private static readonly Module OsuModule = AppDomain.CurrentDomain.GetAssemblies()
            .SingleOrDefault(a => a.GetName().Name == "osu!")
            .GetModules()
            .SingleOrDefault();
        
        /// <summary>
        /// Cari method pake IL Opcodes (kalo match)
        /// </summary>
        internal static MethodInfo FindMethodBySignature(OpCode[] signature)
        {
            if (signature == null || signature.Length == 0 || OsuModule == null)
                return null;

            return OsuModule.GetTypes()
                .SelectMany(t => t.GetRuntimeMethods())
                .FirstOrDefault(m =>
                {
                    var b = m.GetMethodBody()?.GetILAsByteArray();
                    if (b == null) return false;

                    var opcodes = new ILReader(b).GetOpCodes();
                    int idx = 0;

                    foreach (var op in opcodes)
                    {
                        if (op == signature[idx])
                        {
                            idx++;
                            if (idx == signature.Length) return true;
                        }
                        else
                            idx = 0;
                    }

                    return false;
                });
        }

        /// <summary>
        /// Cari constructor pake IL Opcodes (kalo match)
        /// </summary>
        public static ConstructorInfo FindConstructorBySignature(OpCode[] signature)
        {
            if (signature == null || signature.Length == 0 || OsuModule == null)
                return null;

            return OsuModule.GetTypes()
                .SelectMany(t => t.GetConstructors(
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.Public |
                    BindingFlags.NonPublic))
                .FirstOrDefault(ctor =>
                {
                    var b = ctor.GetMethodBody()?.GetILAsByteArray();
                    if (b == null) return false;

                    var opcodes = new ILReader(b).GetOpCodes();
                    int idx = 0;

                    foreach (var op in opcodes)
                    {
                        if (op == signature[idx])
                        {
                            idx++;
                            if (idx == signature.Length) return true;
                        }
                        else
                            idx = 0;
                    }

                    return false;
                });
        }
    }
}
