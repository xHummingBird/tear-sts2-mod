using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Common;

public class InvokeWater() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WaterFononPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        await PowerCmd.Apply<WaterFononPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}