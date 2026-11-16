using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Powers;

public class YuliasBlessingPower : TearPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner || amount <= 0m)
            return;

        if (!ElementalFononHelper.IsElementalFonon(power))
            return;

        await CardPileCmd.Draw(
            choiceContext,
            Amount,
            Owner.Player);
    }
}