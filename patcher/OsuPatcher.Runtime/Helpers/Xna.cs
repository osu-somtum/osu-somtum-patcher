
using System;
using System.Reflection;

namespace OsuPatcher.Runtime.Helpers
{
    internal static class Xna
    {
        private static Type _vector2Type;
        private static ConstructorInfo _vector2Constructor;
        private static Type _colorType;

        static Xna()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (_vector2Type == null)
                    _vector2Type = assembly.GetType("Microsoft.Xna.Framework.Vector2");

                if (_colorType == null)
                    _colorType = assembly.GetType("Microsoft.Xna.Framework.Graphics.Color");

                if (_vector2Type != null && _colorType != null)
                    break;
            }

            if (_vector2Type == null)
                throw new InvalidOperationException("[Xna] Could not find Microsoft.Xna.Framework.Vector2 in loaded assemblies");

            _vector2Constructor = _vector2Type.GetConstructor(new[] { typeof(float), typeof(float) });

            if (_vector2Constructor == null)
                throw new InvalidOperationException("[Xna] Could not find Vector2(float, float) constructor");
        }

        /// <summary>
        /// Creates a Vector2 instance from the client's XNA Framework
        /// </summary>
        public static object CreateVector2(float x, float y)
        {
            return _vector2Constructor.Invoke(new object[] { x, y });
        }

        /// <summary>
        /// Gets the Vector2 type from the client's XNA Framework
        /// </summary>
        public static Type Vector2Type => _vector2Type;

        /// <summary>
        /// Gets the Color type from the client's XNA Framework
        /// </summary>
        public static Type ColorType => _colorType;

        /// <summary>
        /// Creates a Color from RGBA values
        /// </summary>
        public static object CreateColor(byte r, byte g, byte b, byte a = 255)
        {
            if (_colorType == null)
                throw new InvalidOperationException("[Xna] Color type not found");

            var constructor = _colorType.GetConstructor(new[] { typeof(byte), typeof(byte), typeof(byte), typeof(byte) });
            if (constructor != null)
                return constructor.Invoke(new object[] { r, g, b, a });

            throw new InvalidOperationException("[Xna] Could not find Color(byte, byte, byte, byte) constructor");
        }
    }
}
