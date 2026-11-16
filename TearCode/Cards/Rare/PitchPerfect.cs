using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Rare;

public class PitchPerfect() : TearCard(
    2,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PitchPerfectPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<PitchPerfectPower>(
            choiceContext,
            Owner.Creature,
            IsUpgraded ? 2m : 1m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
    }
}