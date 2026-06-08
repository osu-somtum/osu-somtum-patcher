using System.Reflection;
using System.Reflection.Emit;
using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Helpers;

namespace OsuPatcher.Runtime.Options
{
    /// <summary>
    /// Represents a category in the options menu.
    /// </summary>
    internal class Category : Element
    {
        private static readonly ConstructorInfo BaseCategoryConstructor = ILPatch.FindConstructorBySignature(Patterns.Category_Constructor);

        public Category(FontAwesome icon)
            : base(CreateCategoryInstance(icon))
        {
        }

        private static object CreateCategoryInstance(FontAwesome icon)
            => BaseCategoryConstructor.Invoke(new object[] { OsuConstants.TryLazer + 1, icon });
    }
}
