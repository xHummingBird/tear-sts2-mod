using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Cards.Rare;

public class Finale() : TearCard(
    3,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy)
{
    private bool HasPlayedFonicHymnThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished.Any(
            entry =>
                entry.HappenedThisTurn(CombatState) &&
                entry.CardPlay.Card.Owner == Owner &&
                entry.CardPlay.Card is IFonicHymn);

    protected override bool ShouldGlowGoldInternal =>
        HasPlayedFonicHymnThisTurn;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(25m, ValueProp.Move),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null &&
            Owner?.Character is Character.Tear tear)
        {
            AudioHelper.PlayRandomAttack();
            tear.PlayAnimation(ownerCreature, "attack_tear");

            SfxCmd.Play("res://Tear/sounds/se/swing_1.mp3");
            SfxCmd.Play("res://Tear/sounds/se/swing_2.mp3");

            await Task.Delay(167);

            SfxCmd.Play("res://Tear/sounds/se/swing_3.mp3");

            tear.PlayVfxOnTarget(
                play.Target,
                "res://Tear/scenes/hit.tscn",
                "hit");
        }

        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this)
            return Task.CompletedTask;

        if (HasPlayedFonicHymnThisTurn)
            EnergyCost.SetThisTurn(0);

        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;

        if (cardPlay.Card is IFonicHymn)
            EnergyCost.SetThisTurn(0);

        return Task.CompletedTask;
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}