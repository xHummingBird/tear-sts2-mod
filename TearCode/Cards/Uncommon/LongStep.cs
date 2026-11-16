using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Cards.Uncommon;

public class LongStep() : TearCard(
    2,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private bool HasPlayedArteThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished.Any(
            entry =>
                entry.HappenedThisTurn(CombatState) &&
                entry.CardPlay.Card.Owner == Owner &&
                entry.CardPlay.Card is IArtes);

    protected override bool ShouldGlowGoldInternal =>
        HasPlayedArteThisTurn;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(10m, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this)
            return Task.CompletedTask;

        if (HasPlayedArteThisTurn)
            EnergyCost.SetThisTurn(0);

        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;

        if (cardPlay.Card is IArtes)
            EnergyCost.SetThisTurn(0);

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}