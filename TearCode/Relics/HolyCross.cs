using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace Tear.TearCode.Relics;

public class HolyCross : TearRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    public override async Task AfterCombatEnd(
        CombatRoom room)
    {
        Flash();

        await CreatureCmd.Heal(
            Owner.Creature,
            5m);
    }
}