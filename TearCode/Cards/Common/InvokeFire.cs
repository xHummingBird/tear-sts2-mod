using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Common;

public class InvokeFire() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VigorPower>(3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VigorPower>(),
        HoverTipFactory.FromPower<FireFononPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<VigorPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["VigorPower"].IntValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<FireFononPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VigorPower"].UpgradeValueBy(2m);
    }
}