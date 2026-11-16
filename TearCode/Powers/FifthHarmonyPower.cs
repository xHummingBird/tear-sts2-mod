using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Powers;

public class FifthHarmonyPower : TearPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner)
            return;

        if (card is not IFonicHymn)
            return;

        Flash();

        await CardPileCmd.Draw(
            choiceContext,
            Amount,
            card.Owner);
    }
}