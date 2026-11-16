using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Powers;

public class FononAbsorptionPower : TearPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player.Creature != Owner)
            return;

        var hasFonon =
            Owner.HasPower<FireFononPower>() ||
            Owner.HasPower<WaterFononPower>() ||
            Owner.HasPower<WindFononPower>() ||
            Owner.HasPower<EarthFononPower>() ||
            Owner.HasPower<LightFononPower>() ||
            Owner.HasPower<DarkFononPower>();

        if (hasFonon)
            return;

        var gainLight =
            player.RunState.Rng.CombatCardSelection.NextBool();

        if (gainLight)
        {
            await PowerCmd.Apply<LightFononPower>(
                choiceContext,
                Owner,
                1m,
                Owner,
                null);
        }
        else
        {
            await PowerCmd.Apply<DarkFononPower>(
                choiceContext,
                Owner,
                1m,
                Owner,
                null);
        }
    }

    public override Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner &&
            cardPlay.Card is IArtes)
        {
            OverlimitManager.GainOverlimit(
                cardPlay.Card.Owner,
                Amount);
        }

        return Task.CompletedTask;
    }
}