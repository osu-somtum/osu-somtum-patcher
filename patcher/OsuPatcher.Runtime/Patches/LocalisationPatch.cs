using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Helpers;

namespace OsuPatcher.Runtime.Patches
{
    /// <summary>
    /// Patches the LocalisationManager to modify string retrieval.
    /// </summary>
    [HarmonyPatch]
    public class LocalisationManager
    {
        [HarmonyTargetMethod]
        private static MethodBase Target() => ILPatch.FindMethodBySignature(Patterns.LocalisationManager_Target);

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codes = new List<CodeInstruction>(instructions);
            var skip = il.DefineLabel();
            
            codes[0].labels.Add(skip);
            
            codes.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4, OsuConstants.TryLazer),
                new CodeInstruction(OpCodes.Ble_S, skip),
                new CodeInstruction(OpCodes.Ldstr, "Patcher"),
                new CodeInstruction(OpCodes.Ret)
            });
            
            return codes.AsEnumerable();
        }
    }
}
