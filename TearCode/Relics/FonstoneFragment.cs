using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Relics;

public class FonstoneFragment : TearRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return;

        if (Owner.PlayerCombatState.TurnNumber > 1)
            return;

        Flash();

        var choiceContext =
            new ThrowingPlayerChoiceContext();

        var roll =
            Owner.RunState.Rng.CombatCardGeneration.NextInt(6);

        switch (roll)
        {
            case 0:
                await PowerCmd.Apply<FireFononPower>(
                    choiceContext,
                    Owner.Creature,
                    1m,
                    Owner.Creature, null);
                break;

            case 1:
                await PowerCmd.Apply<WaterFononPower>(
                    choiceContext,
                    Owner.Creature,
                    1m,
                    Owner.Creature,
                    null);
                break;

            case 2:
                await PowerCmd.Apply<WindFononPower>(
                    choiceContext,
                    Owner.Creature,
                    1m,
                    Owner.Creature,
                    null);
                break;

            case 3:
                await PowerCmd.Apply<EarthFononPower>(
                    choiceContext,
                    Owner.Creature,
                    1m,
                    Owner.Creature,
                    null);
                break;

            case 4:
                await PowerCmd.Apply<LightFononPower>(
                    choiceContext,
                    Owner.Creature,
                    1m,
                    Owner.Creature,
                    null);
                break;

            default:
                await PowerCmd.Apply<DarkFononPower>(
                    choiceContext,
                    Owner.Creature,
                    1m,
                    Owner.Creature,
                    null);
                break;
        }
    }
}