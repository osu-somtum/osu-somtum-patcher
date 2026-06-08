namespace OsuPatcher.Runtime.Graphics.Sprites
{
    internal enum Clocks
    {
        /// <summary>
        /// A simple clock type which is based off the game's internal clock.
        /// Sprites of this clock type will be thrown out and recycled after their transformations are exhausted unless set to AlwaysDraw.
        /// </summary>
        Game,
        /// <summary>
        /// A simple clock type which is bound to the active audio track.
        /// Sprites of this clocktype will be kept in history (to allow for seeking) unless ForwardPlayOptimisations is in use.
        /// </summary>
        Audio,
        /// <summary>
        /// When rewinding, sprites using AudioOnce will be removed completely after they have been "unplayed".
        /// This allows for some extra effects (like hit burst particles, kiai flashes etc.) which are generated on-the-fly
        /// to still play in rewind, but not duplicate when they are next generated after playing in a forward direction.
        /// </summary>
        AudioOnce
    };
}
