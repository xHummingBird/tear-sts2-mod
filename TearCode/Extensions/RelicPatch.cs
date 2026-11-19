using HarmonyLib;
using Tear.TearCode.Cards.Ancient;
using Tear.TearCode.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Tear.TearCode.Extensions;

[HarmonyPatch(typeof(TouchOfOrobas), "GetUpgradedStarterRelic")]
internal static class TearTouchOfOrobasPatch
{
    private static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic is Rod)
        {
            __result = ModelDb.Relic<BlueCrystalRod>().ToMutable();
        }
    }
}

[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
internal static class TearArchaicToothTranscendencePatch
{
    [HarmonyPostfix]
    private static void Postfix(ref Dictionary<ModelId, CardModel> __result)
    {
        __result[ModelDb.Card<Cards.Basic.Nightmare>().Id] = ModelDb.Card<MyxolidiaOfFonons>();
    }
}

[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.SetupForPlayer))]
public static class TearDustyTomeSetupPatch
{
    [HarmonyPostfix]
    public static void Postfix(DustyTome __instance, Player player)
    {
        if (player.Character is not Character.Tear)
            return;

        __instance.AncientCard = ModelDb.Card<Resurrection>().Id;
    }
}


[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.AfterObtained))]
public static class TearDustyTomePatch
{
    [HarmonyPrefix]
    public static void Prefix(DustyTome __instance)
    {
        if (__instance.Owner?.Character is not Character.Tear)
            return;
        
        __instance.AncientCard = ModelDb.Card<Resurrection>().Id;
    }
}