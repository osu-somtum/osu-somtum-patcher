using System.Reflection;
using System.Reflection.Emit;
using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Helpers;

namespace OsuPatcher.Runtime.Options
{
    /// <summary>
    /// Represents a textbox UI element.
    /// </summary>
    internal class TextBox : Element
    {
        private static ConstructorInfo _bindableCons;

        private static readonly ConstructorInfo BaseTextBox = ILPatch.FindConstructorBySignature(Patterns.TextBox_Constructor);

        public TextBox(string title, string initial, bool passwordBox = false)
            : base(CreateTextBoxInstance(title, initial, passwordBox))
        {
        }

        private static object CreateTextBoxInstance(string title, string initial, bool passwordBox = false)
        {
            if (_bindableCons == null)
            {
                _bindableCons = BaseTextBox
                    .GetParameters()[1]
                    .ParameterType
                    .GetConstructor(new[]
                    {
                        typeof(string)
                    });
            }
            
            object str = _bindableCons.Invoke(new object[] { initial });

            return BaseTextBox.Invoke(new object[]
            {
                title,
                str,
                passwordBox
            });
        }
    }
}
