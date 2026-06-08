using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Helpers;

namespace OsuPatcher.Runtime.Patches
{
    /// <summary>
    /// Patches the game transition time to speed up screen transitions.
    /// </summary>
    [HarmonyPatch]
    internal class PatchTransition
    {
        [HarmonyTargetMethod]
        private static MethodBase Target() => ILPatch.FindMethodBySignature(Patterns.PatchTransition_Target);

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            codes.RemoveAt(127);
            codes.Insert(127, new CodeInstruction(OpCodes.Call,
                typeof(PatchTransition)
                .GetMethod(nameof(TransitionTime), BindingFlags.Public | BindingFlags.Static)));

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Returns the transition time based on configuration.
        /// </summary>
        /// <returns>200 if faster transition is enabled, otherwise 100.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float TransitionTime()
        {
            if (!Options.Options.Config.TransitionTime)
                return 100;

            return 200;
        }
    }
}
