using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;

namespace Tear.TearCode.Cards.Uncommon;

public class TrainedAssassin() : TearCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
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

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var isFirstCardThisTurn = IsFirstCardThisTurn;
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

        if (!isFirstCardThisTurn)
            return;

        var randomAttack = PileType.Hand
            .GetPile(Owner)
            .Cards
            .Where(card =>
                card.Type == CardType.Attack &&
                !card.Keywords.Contains(CardKeyword.Unplayable))
            .ToList()
            .StableShuffle(Owner.RunState.Rng.CombatCardSelection)
            .FirstOrDefault();

        if (randomAttack != null)
        {
            await CardCmd.AutoPlay(
                choiceContext,
                randomAttack,
                null);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}