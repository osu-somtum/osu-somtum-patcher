using System;
using System.Reflection;
using System.Reflection.Emit;
using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Helpers;

namespace OsuPatcher.Runtime.Options
{
    /// <summary>
    /// Represents a checkbox UI element.
    /// </summary>
    internal class CheckBox : Element
    {
        private static ConstructorInfo _boolCons;

        private static readonly ConstructorInfo BaseCheckBox = ILPatch.FindConstructorBySignature(Patterns.CheckBox_Constructor);

        public CheckBox(string title, string tooltip, bool initial, EventHandler onChanged)
            : base(CreateCheckBoxInstance(title, tooltip, initial, onChanged))
        {
        }

        private static object CreateCheckBoxInstance(string title, string tooltip, bool initial, EventHandler onChanged)
        {
            if (_boolCons == null)
            {
                _boolCons = BaseCheckBox
                    .GetParameters()[2]
                    .ParameterType
                    .GetConstructor(new[] 
                    { 
                        typeof(bool) 
                    });
            }

            object bindableBool = _boolCons.Invoke(new object[] { initial });

            return BaseCheckBox.Invoke(new object[]
            {
                title,
                tooltip,
                bindableBool,
                onChanged
            });
        }
    }
}
