namespace OsuPatcher.Runtime.Graphics.Sprites
{

    internal enum Fields
    {
        /// <summary>
        ///   The gamefield resolution (512x384).  Used for hitobjects and anything which needs to align with gameplay elements.
        ///   This is scaled in proportion to the native resolution and aligned accordingly.
        /// </summary>
        Gamefield = 1,
        GamefieldWide = 2,

        /// <summary>
        ///   Gamefield "container" resolution.  This is where storyboard and background events sits, and is the same
        ///   scaling/positioning as Standard when in play mode.  It differs in editor design mode where this field
        ///   will be shrunk to allow for editing.
        /// </summary>
        Storyboard = 3,
        StoryboardCentre = 4,

        /// <summary>
        ///   Native screen resolution.
        /// </summary>
        Native = 5,

        TopLeft,
        TopCentre,
        TopRight,

        CentreLeft,
        Centre,
        CentreRight,

        BottomLeft,
        BottomCentre,
        BottomRight,

        StandardGamefieldScale,

        /// <summary>
        ///   Native screen resolution with 1024x768-native sprite scaling.
        /// </summary>
        NativeStandardScale,

        /// <summary>
        ///   Native screen resolution aligned from the right-hand side of the screen, where an X position of 0 is translated to Standard(WindowManager.Width).
        /// </summary>
        NativeRight,
        NativeBottomRight,

        NativeBottomCentre,
    };
}
