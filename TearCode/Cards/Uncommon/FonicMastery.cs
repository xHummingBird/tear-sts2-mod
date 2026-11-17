using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Uncommon;

public class FonicMastery() : TearCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<FonicMasteryPower>(
            choiceContext,
            Owner.Creature,
            IsUpgraded ? 50m : 25m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
    }
}