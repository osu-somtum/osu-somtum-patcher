using OsuPatcher.Runtime.Helpers;
using System;

namespace OsuPatcher.Runtime.Wrappers
{
    public struct Color : IEquatable<Color>
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static readonly Color White = new Color(255, 255, 255);

        public override string ToString()
        {
            return $"{{R:{R} G:{G} B:{B} A:{A}}}";
        }

        public override bool Equals(object obj)
        {
            return obj is Color && Equals((Color)obj);
        }

        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Color a, Color b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return ((R << 24) | (G << 16) | (B << 8) | A);
        }

        internal object ToXnaColor()
            => Xna.CreateColor(R, G, B, A);
    }
}
