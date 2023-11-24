using System.Collections.Generic;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace FixInverseWheel.Patches;

public static class PatchPlayerControllerBHelpers
{
    public static double ConditionalInvertScrollAmount(double scrollAmount)
    {
        return FixInverseWheel.Instance.IsInvertScrollDirection.Value ? -scrollAmount : scrollAmount;
    }
}
[HarmonyPatch(typeof(PlayerControllerB))]
public class PatchPlayerControllerB
{
    [HarmonyPatch("SwitchItem_performed")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> InverseControlSwitchItemPerformed(
        IEnumerable<CodeInstruction> instructions)
    {
        var callVirtCount = 0;
        var writeAfterX = -1;

        foreach (var instruction in instructions)
        {
            if (writeAfterX >= 0)
            {
                if (writeAfterX == 0)
                {
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(PatchPlayerControllerBHelpers), nameof(PatchPlayerControllerBHelpers.ConditionalInvertScrollAmount)));
                };
                writeAfterX--;
            }

            if (instruction.opcode == OpCodes.Callvirt && callVirtCount < 4)
            {
                callVirtCount++;
                if (callVirtCount == 4) writeAfterX = 2;
            }

            yield return instruction;
        }
    }
}