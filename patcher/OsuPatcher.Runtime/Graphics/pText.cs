using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Graphics.Sprites;
using OsuPatcher.Runtime.Helpers;
using OsuPatcher.Runtime.Wrappers;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace OsuPatcher.Runtime.Graphics
{
    internal class pText : pSprite
    {
        private static readonly ConstructorInfo BaseText = ILPatch.FindConstructorBySignature(Patterns.Text_Constructor);

        private static readonly Dictionary<Type, MethodInfo> _textSetters = new Dictionary<Type, MethodInfo>();

        internal pText(string text, float textSize, float posX, float posY, float boundsX, float boundsY, float drawDepth,
            bool alwaysDraw, Color colour, bool shadow = true,
            Fields field = Fields.TopLeft, Origins origin = Origins.TopLeft, Clocks clock = Clocks.Game)
            : base(null, field, origin, clock, posX, posY, drawDepth, alwaysDraw, colour)
        {
            Instance = CreateTextInstance(text, textSize, posX, posY, boundsX, boundsY, drawDepth, alwaysDraw, colour, shadow, field, origin, clock);
        }

        protected pText(object instance)
            : base(null, Fields.TopLeft, Origins.TopLeft, Clocks.Game, 0, 0, 0, true, Color.White)
        {
            Instance = instance;
        }

        public virtual string Text
        {
            set
            {
                if (Instance == null) return;
                var t = Instance.GetType();

                if (!_textSetters.TryGetValue(t, out var setter))
                {
                    setter = ILPatch.FindMethodBySignature(Patterns.Text_Setter);

                    _textSetters[t] = setter;
                }

                setter?.Invoke(Instance, new object[] { value });
            }
        }

        private static object CreateTextInstance(string text, float textSize, float posX, float posY, float boundsX, float boundsY, float drawDepth,
            bool alwaysDraw, Color colour, bool shadow, Fields field, Origins origin, Clocks clock)
        {
            object startPosition = Xna.CreateVector2(posX, posY);
            object bounds = Xna.CreateVector2(boundsX, boundsY);
            object xnaColor = colour.ToXnaColor();
            var parameters = BaseText.GetParameters();

            return BaseText.Invoke(new object[]
            {
                text,
                textSize,
                startPosition,
                bounds,
                drawDepth,
                alwaysDraw,
                xnaColor,
                shadow,
                Enum.ToObject(parameters[3].ParameterType, field),
                Enum.ToObject(parameters[4].ParameterType, origin),
                Enum.ToObject(parameters[5].ParameterType, clock),
            });
        }
    }
}
