using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Character;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Powers;

public class LingeringMelodyPower : TearPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        var player = Owner.Player;

        var playedHymn = CombatManager.Instance.History
            .CardPlaysFinished.Any(
                entry =>
                    entry.HappenedThisTurn(Owner.CombatState) &&
                    entry.CardPlay.Card.Owner == player &&
                    entry.CardPlay.Card is IFonicHymn);

        if (playedHymn)
            return;

        var hymnPool = ModelDb.CardPool<TearCardPool>()
            .GetUnlockedCards(
                player.UnlockState,
                player.RunState.CardMultiplayerConstraint)
            .Where(card => card is IFonicHymn);

        var hymn = CardFactory.GetForCombat(
                player,
                hymnPool,
                1,
                player.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();

        if (hymn == null)
            return;

        hymn.ExhaustOnNextPlay = true;

        await CardCmd.AutoPlay(
            choiceContext,
            hymn,
            null);
    }
}