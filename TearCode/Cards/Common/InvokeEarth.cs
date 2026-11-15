using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Common;

public class InvokeEarth() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<EarthFononPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<EnergyNextTurnPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Energy.BaseValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<EarthFononPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}