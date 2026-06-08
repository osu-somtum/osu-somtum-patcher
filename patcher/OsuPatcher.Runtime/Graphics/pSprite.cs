using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Graphics.Skinning;
using OsuPatcher.Runtime.Graphics.Sprites;
using OsuPatcher.Runtime.Helpers;
using OsuPatcher.Runtime.Wrappers;
using System;
using System.Reflection;

namespace OsuPatcher.Runtime.Graphics
{
    internal class pSprite : pDrawable
    {
        private static readonly ConstructorInfo BaseSprite = ILPatch.FindConstructorBySignature(Patterns.Sprite_Constructor);

        internal pSprite(object texture, Fields fieldType, Origins origin, Clocks clock, float posX, float posY, float drawDepth, bool alwaysDraw, Color colour, object tag = null)
        {
            Instance = CreateSpriteInstance(texture, fieldType, origin, clock, posX, posY, drawDepth, alwaysDraw, colour, tag);
        }

        private object CreateSpriteInstance(object texture, Fields fieldType, Origins origin, Clocks clock, float posX, float posY, float drawDepth, bool alwaysDraw, Color colour, object tag)
        {
            object startPosition = Xna.CreateVector2(posX, posY);
            object xnaColor = colour.ToXnaColor();
            var parameters = BaseSprite.GetParameters();

            return BaseSprite.Invoke(new object[]
            {
                texture,
                Enum.ToObject(parameters[1].ParameterType, fieldType),
                Enum.ToObject(parameters[2].ParameterType, origin),
                Enum.ToObject(parameters[3].ParameterType, clock),
                startPosition,
                drawDepth,
                alwaysDraw,
                xnaColor,
                tag
            });
        }
    }
}