using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Relics;

public class FonicRod : TearRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner.Creature)
            return;

        if (amount <= 0m)
            return;

        if (power is not (
            FireFononPower or
            WaterFononPower or
            WindFononPower or
            EarthFononPower or
            LightFononPower or
            DarkFononPower))
        {
            return;
        }

        Flash();

        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature.CombatState.HittableEnemies,
            5m,
            ValueProp.Unpowered,
            Owner.Creature);
    }
}