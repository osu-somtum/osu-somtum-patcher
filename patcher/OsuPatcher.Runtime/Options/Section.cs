using System.Reflection;
using System.Reflection.Emit;
using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Helpers;

namespace OsuPatcher.Runtime.Options
{
    /// <summary>
    /// Represents a section within a category in the options menu.
    /// </summary>
    internal class Section : Element
    {
        private static readonly ConstructorInfo BaseSection = ILPatch.FindConstructorBySignature(Patterns.Section_Constructor);

        public Section(string title) 
            : base(CreateSectionInstance(title))
        {
        }

        private static object CreateSectionInstance(string title) => 
            BaseSection.Invoke(new object[] { title, null });
    }
}
