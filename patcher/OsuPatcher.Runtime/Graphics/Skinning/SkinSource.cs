using System;

namespace OsuPatcher.Runtime.Graphics.Skinning
{
    [Flags]
    public enum SkinSource
    {
        None = 0,
        Osu = 1,
        Skin = 2,
        Beatmap = 4,
        Temporal = 8,
        ExceptBeatmap = Osu | Skin,
        All = Osu | Skin | Beatmap
    }
}
