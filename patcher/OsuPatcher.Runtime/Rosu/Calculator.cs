using OsuPatcher.Runtime.Rosu.FFI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OsuPatcher.Runtime.Rosu
{
    public sealed class Calculator
    {
        private const string NativeLibraryFileName = "akatsuki_pp_ffi.dll";

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetDllDirectory(string pathName);

        [DllImport("akatsuki_pp_ffi", CallingConvention = CallingConvention.Cdecl)]
        private static extern CalculateResult calculate_akatsuki_from_bytes(
            Sliceu8 beatmap_bytes,
            uint mode,
            uint mods,
            uint max_combo,
            float accuracy,
            uint miss_count,
            Optionu32 passed_objects);

        private static MethodInfo _getBeatmapStream;
        private readonly object _beatmap;
        private readonly object _mods;

        private byte[] _cachedBeatmap;
        private GCHandle _pinnedHandle;
        private Sliceu8 _beatmapSlice;
        private bool _disposed;

        static Calculator()
        {
            LoadNativeLibraryFromAssemblyDirectory();
        }

        public Calculator(object beatmap, MethodInfo getBeatmapStream, object mods)
        {
            _beatmap = beatmap;
            _getBeatmapStream = getBeatmapStream;
            _mods = mods;
            InitializeBeatmapData();
        }

        private static void LoadNativeLibraryFromAssemblyDirectory()
        {
            string assemblyDirectory = Path.GetDirectoryName(typeof(Calculator).Assembly.Location);
            if (string.IsNullOrEmpty(assemblyDirectory))
                return;

            string nativePath = Path.Combine(assemblyDirectory, NativeLibraryFileName);
            if (!File.Exists(nativePath))
                return;

            SetDllDirectory(assemblyDirectory);

            if (LoadLibrary(nativePath) == IntPtr.Zero)
                throw new InvalidOperationException(
                    $"failed to load {nativePath}: Win32 error {Marshal.GetLastWin32Error()}");
        }

        private void InitializeBeatmapData()
        {
            if (_beatmap == null || _getBeatmapStream == null)
                return;

            using (Stream stream = (Stream)_getBeatmapStream.Invoke(_beatmap, null))
            {
                if (stream == null)
                    return;

                if (stream.CanSeek)
                {
                    _cachedBeatmap = new byte[stream.Length];
                    stream.Read(_cachedBeatmap, 0, (int)stream.Length);
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        _cachedBeatmap = memoryStream.ToArray();
                    }
                }
            }

            if (_cachedBeatmap != null && _cachedBeatmap.Length != 0)
            {
                _pinnedHandle = GCHandle.Alloc(_cachedBeatmap, GCHandleType.Pinned);
                _beatmapSlice = new Sliceu8(_pinnedHandle.AddrOfPinnedObject(), (ulong)((long)_cachedBeatmap.Length));
            }
        }

        public double CalculateScore(object score, float accuracy, int legacyScore, int maxCombo, int playMode)
        {
            if (_disposed)
                throw new ObjectDisposedException("Calculator");

            if (_cachedBeatmap == null || _cachedBeatmap.Length == 0)
                return 0.0;

            var ushortFields = score.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.FieldType == typeof(ushort)).ToArray();

            if (ushortFields.Length <= 5)
                return 0.0;

            ushort count300 = (ushort)ushortFields[0].GetValue(score);
            ushort count100 = (ushort)ushortFields[1].GetValue(score);
            ushort count50 = (ushort)ushortFields[2].GetValue(score);
            ushort countMiss = (ushort)ushortFields[5].GetValue(score);
            uint passedObjectCount = (uint)(count300 + count100 + count50 + countMiss);

            Optionu32 passedObjects = Optionu32.FromNullable(new uint?(passedObjectCount));
            uint mods = (uint)Convert.ToInt32(_mods);

            return calculate_akatsuki_from_bytes(
                _beatmapSlice,
                (uint)playMode,
                mods,
                (uint)maxCombo,
                accuracy,
                (uint)Convert.ToInt32(countMiss),
                passedObjects).pp;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            if (_pinnedHandle.IsAllocated)
                _pinnedHandle.Free();

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~Calculator()
        {
            Dispose();
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct CalculateResult
    {
        public double pp;
        public double stars;
    }
}
