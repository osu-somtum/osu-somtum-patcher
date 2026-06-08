using OsuPatcher.Runtime.Graphics;
using OsuPatcher.Runtime.Rosu;
using System;
using System.Reflection;

namespace OsuPatcher.Runtime.Play
{
    internal class Performance
    {
        private readonly Calculator _ppCalculator;
        private readonly pSpriteText[] _ppSpriteTexts;
        private int currentPP;
        private int previousPP;
        private int lastLegacyScore;

        public Performance(object beatmap, MethodInfo beatmapStream, uint mods, pSpriteText[] spriteTexts)
            : this(beatmap, beatmapStream, mods)
        {
            _ppSpriteTexts = spriteTexts;
        }

        public Performance(object beatmap, MethodInfo beatmapStream, uint mods)
        {
            _ppCalculator = new Calculator(beatmap, beatmapStream, mods);
        }

        public void UpdatePerformance(object score, float accuracy, int legacyScore, int maxCombo, int playMode)
        {
            if (lastLegacyScore != legacyScore)
            {
                double pp = _ppCalculator.CalculateScore(score, accuracy, legacyScore, maxCombo, playMode);

                if (double.IsNaN(pp) || double.IsInfinity(pp) || pp < 0.0)
                    pp = 0.0;

                currentPP = (int)Math.Round(pp);
                lastLegacyScore = legacyScore;
            }

            if (previousPP != currentPP && _ppSpriteTexts != null)
                foreach (var spriteText in _ppSpriteTexts)
                    spriteText.Text = currentPP.ToString();

            previousPP = currentPP;
        }
    }
}
