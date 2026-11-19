using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Character;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Relics;

public class UnicornHorn : TearRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return;

        if (Owner.PlayerCombatState.TurnNumber > 1)
            return;

        var fonicHymnPool =
            ModelDb.CardPool<TearCardPool>()
                .GetUnlockedCards(
                    Owner.UnlockState,
                    Owner.RunState.CardMultiplayerConstraint)
                .Where(card => card is IFonicHymn);

        var fonicHymn =
            CardFactory.GetForCombat(
                    Owner,
                    fonicHymnPool,
                    1,
                    Owner.RunState.Rng.CombatCardGeneration)
                .FirstOrDefault();

        if (fonicHymn == null)
            return;

        Flash();

        await CardPileCmd.AddGeneratedCardsToCombat(
            [fonicHymn],
            PileType.Hand,
            Owner);
    }
}