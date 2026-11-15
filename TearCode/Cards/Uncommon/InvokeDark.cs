using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Uncommon;

public class InvokeDark() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<DarkFononPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            CombatState.HittableEnemies,
            DynamicVars.Vulnerable.IntValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<DarkFononPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}