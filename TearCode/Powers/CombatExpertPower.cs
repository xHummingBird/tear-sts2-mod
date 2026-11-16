using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Powers;

public class CombatExpertPower : TearPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        if (cardPlay.Card is not IArtes)
            return;

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner,
            Amount,
            Owner,
            cardPlay.Card);
    }
}