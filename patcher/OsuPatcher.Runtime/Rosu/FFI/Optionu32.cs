using System;
using System.Runtime.InteropServices;

namespace OsuPatcher.Runtime.Rosu.FFI
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Optionu32
    {
        ///Element that is maybe valid.
        uint t;
        ///Byte where `1` means element `t` is valid.
        byte is_some;
    }

    public partial struct Optionu32
    {
        public static Optionu32 FromNullable(uint? nullable)
        {
            var result = new Optionu32();
            if (nullable.HasValue)
            {
                result.is_some = 1;
                result.t = nullable.Value;
            }

            return result;
        }

        public uint? ToNullable()
        {
            return this.is_some == 1 ? this.t : (uint?)null;
        }
    }
}
