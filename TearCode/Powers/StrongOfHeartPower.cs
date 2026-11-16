using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Powers;

public class StrongOfHeartPower : TearPower
{
    private class Data
    {
        public bool TriggeredThisTurn;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        if (cardPlay.Card is not IFonicHymn)
            return;

        var data = GetInternalData<Data>();

        if (data.TriggeredThisTurn)
            return;

        data.TriggeredThisTurn = true;

        Flash();

        await PlayerCmd.GainEnergy(
            (int)Amount,
            Owner.Player);
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (participants.Contains(Owner))
        {
            GetInternalData<Data>().TriggeredThisTurn = false;
        }

        return Task.CompletedTask;
    }
}