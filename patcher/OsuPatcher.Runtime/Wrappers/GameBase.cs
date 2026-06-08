using System.Reflection;
using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Helpers;

namespace OsuPatcher.Runtime.Wrappers
{
    /// <summary>
    /// Provides base game functionality wrappers.
    /// </summary>
    internal class GameBase
    {
        private static readonly MethodBase BaseBeginExit = ILPatch.FindMethodBySignature(Patterns.GameBase_BeginExit);

        /// <summary>
        /// Initiates the game exit sequence.
        /// </summary>
        /// <param name="forceConfirm">If set to <c>true</c>, forces confirmation dialog.</param>
        public static void BeginExit(
            bool forceConfirm = false)
         => BaseBeginExit.Invoke(null, new object[] { forceConfirm });
    }
}
