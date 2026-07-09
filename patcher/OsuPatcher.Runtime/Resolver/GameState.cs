using System;
using System.Reflection;

namespace OsuPatcher.Runtime.Resolver
{
    /// <summary>
    /// Detects when osu! is watching a replay (or spectating) rather than the local player
    /// actively playing. Used to hide the realtime pp counter, which is only meaningful for your
    /// own live play.
    ///
    /// osu! marks this on the Player instance: a private replay-watcher field is non-null only
    /// while a replay is loaded/being watched, and null during normal play (including rx/ap).
    /// The two mod flags in the miss guard are Relax/Autopilot — NOT a replay indicator — so we
    /// can't reuse those.
    ///
    /// The field name below is obfuscated and therefore build-specific. It was identified by
    /// dumping the Player instance's fields during a normal play vs. a replay and taking the one
    /// that is non-null only when watching. If an osu! update breaks this, re-derive it the same
    /// way (see git history for the temporary GameState diagnostic).
    /// </summary>
    internal static class GameState
    {
        // Player instance field holding the replay-watcher object (non-null only when watching).
        private const string ReplayFieldName = "#=zOJE2kiFjrF5gNRgwIw==";

        private static Type _cachedType;
        private static FieldInfo _replayField;

        /// <summary>True while watching a replay / spectating; false during the local player's live play.</summary>
        public static bool IsWatchingReplay(object playerInstance)
        {
            if (playerInstance == null)
                return false;

            var type = playerInstance.GetType();
            if (!ReferenceEquals(type, _cachedType))
            {
                _cachedType = type;
                _replayField = type.GetField(ReplayFieldName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            }

            if (_replayField == null)
                return false; // can't tell — default to showing the counter

            try { return _replayField.GetValue(playerInstance) != null; }
            catch { return false; }
        }
    }
}
