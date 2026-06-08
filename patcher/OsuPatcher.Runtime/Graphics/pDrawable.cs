using System;
using System.Collections.Generic;

namespace OsuPatcher.Runtime.Graphics
{
    internal partial class pDrawable : IDisposable, IComparable<pDrawable>
    {
        public object Instance { get; set; }

        public virtual void Dispose()
        {
        }

        public int CompareTo(pDrawable other)
            => 0;
    }
}