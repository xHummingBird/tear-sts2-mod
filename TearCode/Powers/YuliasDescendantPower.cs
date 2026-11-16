using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Powers;

public class YuliasDescendantPower : TearPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyKeywordsInCombat(
        CardModel card,
        ISet<CardKeyword> keywords)
    {
        if (card.Owner != Owner.Player)
        {
            return false;
        }

        if (card is not IFonicHymn)
        {
            return false;
        }

        return keywords.Add(CardKeyword.Retain);
    }

    public override Task AfterApplied(
        Creature? applier,
        CardModel? cardSource)
    {
        InvokeKeywordsChangedOnFonicHymns(Owner.Player);

        return Task.CompletedTask;
    }

    public override Task AfterRemoved(
        Creature oldOwner)
    {
        InvokeKeywordsChangedOnFonicHymns(oldOwner.Player);

        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStartLate(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
        {
            return Task.CompletedTask;
        }

        OverlimitManager.GainOverlimit(
            Owner.Player,
            Amount);

        return Task.CompletedTask;
    }

    private static void InvokeKeywordsChangedOnFonicHymns(
        Player player)
    {
        foreach (var card in player.PlayerCombatState.AllCards
                     .Where(card => card is IFonicHymn))
        {
            card.InvokeKeywordsChanged();
        }
    }
}