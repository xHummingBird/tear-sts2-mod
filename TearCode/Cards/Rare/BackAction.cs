using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Rare;

public class BackAction() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    private int PlayCountThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished.Count(
            entry =>
                entry.HappenedThisTurn(CombatState) &&
                entry.CardPlay.Player == Owner);

    private bool IsFirstCardThisTurn =>
        PlayCountThisTurn == 0;

    protected override bool ShouldGlowGoldInternal =>
        IsFirstCardThisTurn;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FreeAttackPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var isFirstCardThisTurn = IsFirstCardThisTurn;

        await CommonActions.CardBlock(this, play);

        if (isFirstCardThisTurn)
        {
            await PowerCmd.Apply<FreeAttackPower>(
                choiceContext,
                Owner.Creature,
                1m,
                Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}