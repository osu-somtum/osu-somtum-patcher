using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Helpers;

namespace OsuPatcher.Runtime.Options
{
    /// <summary>
    /// Base class for UI elements in the options menu.
    /// </summary>
    internal class Element
    {
        internal object V { get; set; }
        protected Element(object v) => V = v;

        private static readonly MethodBase BaseSetChildren = ILPatch.FindMethodBySignature(Patterns.Element_SetChildren);

        public static Array CreateArray(params Element[] elements)
        {
            Array array = Array.CreateInstance(elements
                .First()
                .V.GetType()
                .BaseType,
                elements.Length);

            for (int i = 0; i < elements.Length; i++)
                array.SetValue(elements[i].V, i);
            
            return array;
        }

        public void SetChildren(Array children) 
            => BaseSetChildren.Invoke(V, new object[] { children });
    }
}
