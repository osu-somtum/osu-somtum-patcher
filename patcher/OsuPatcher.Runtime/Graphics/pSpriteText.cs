using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Graphics.Skinning;
using OsuPatcher.Runtime.Graphics.Sprites;
using OsuPatcher.Runtime.Helpers;
using OsuPatcher.Runtime.Wrappers;
using System;
using System.Linq;
using System.Reflection;

namespace OsuPatcher.Runtime.Graphics
{
    internal class pSpriteText : pText
    {
        private static readonly ConstructorInfo BaseSpriteText = ILPatch.FindConstructorBySignature(Patterns.SpriteText_Constructor);
        private static readonly MethodBase BaseRefreshTexture = ILPatch.FindMethodBySignature(Patterns.Text_RefreshTexture);

        private static FieldInfo _textConstantSpacingField;
        private static FieldInfo _scaleField;

        public pSpriteText(string text, string fontname, float spacingOverlap, Fields fieldType, Origins origin, Clocks clock,
                          float posX, float posY, float drawDepth, bool alwaysDraw, Color colour, bool precache = true, SkinSource source = SkinSource.All)
            : base(CreateSpriteTextInstance(text, fontname, spacingOverlap, fieldType, origin, clock, posX, posY, drawDepth, alwaysDraw, colour, precache, source))
        {
        }

        public bool TextConstantSpacing
        {
            set
            {
                if (_textConstantSpacingField == null)
                {
                    _textConstantSpacingField = Instance.GetType()
                        .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .Where(f => f.FieldType == typeof(bool) && !f.IsPublic)
                        .First();
                }
                _textConstantSpacingField.SetValue(Instance, value);
            }
        }

        public float Scale
        {
            set
            {
                if (_scaleField == null)
                {
                    Type currentType = Instance.GetType();
                    while (currentType != null && currentType != typeof(object))
                    {
                        var floatFields = currentType
                            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .Where(f => f.FieldType == typeof(float) && !f.IsPublic)
                            .ToList();

                        if (floatFields.Count >= 3)
                        {
                            _scaleField = floatFields[2];
                            break;
                        }

                        currentType = currentType.BaseType;
                    }
                }
                _scaleField.SetValue(Instance, value);
            }
        }

        private static object CreateSpriteTextInstance(string text, string fontname, float spacingOverlap, Fields fieldType, Origins origin, Clocks clock,
                                                       float posX, float posY, float drawDepth, bool alwaysDraw, Color colour, bool precache, SkinSource source)
        {
            object startPos = Xna.CreateVector2(posX, posY);
            object colourXna = colour.ToXnaColor();
            var parameters = BaseSpriteText.GetParameters();

            return BaseSpriteText.Invoke(new object[]
            {
                text,
                fontname,
                spacingOverlap,
                Enum.ToObject(parameters[3].ParameterType, fieldType),
                Enum.ToObject(parameters[4].ParameterType, origin),
                Enum.ToObject(parameters[5].ParameterType, clock),
                startPos,
                drawDepth,
                alwaysDraw,
                colourXna,
                precache,
                Enum.ToObject(parameters[11].ParameterType, source)
            });
        }

        public void RefreshTexture()
            => BaseRefreshTexture.Invoke(Instance, null);
    }
}
