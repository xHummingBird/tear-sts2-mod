using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Uncommon;

public class Judgment() : TearCard(
    2,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.RandomEnemy), IFonicHymn
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new HitCountVar(5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.FonicHymn,
        HoverTipFactory.FromPower<FireFononPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null)
            return;

        if (Owner.Character is Character.Tear tear)
        {
            SfxCmd.Play("res://Tear/sounds/hymns/fifth_hymn.mp3", 0.7f);
            
            tear.PlayAnimation(
                ownerCreature,
                "cast_1");

            SfxCmd.Play(
                "res://Tear/sounds/se/cast_1.mp3");

            await Task.Delay(500);

            tear.PlayAnimation(
                ownerCreature,
                "cast_2");

            SfxCmd.Play(
                "res://Tear/sounds/se/cast_2.mp3");
            SfxCmd.Play("res://Tear/sounds/judgment.wav", 0.7f);
        }

        await DamageCmd.Attack(
                DynamicVars.Damage.BaseValue)
            .WithHitCount(
                DynamicVars.HitCount.IntValue)
            .FromCard(this, play)
            .TargetingRandomOpponents(CombatState)
            .WithNoAttackerAnim()
            .WithWaitBeforeHit(0.67f, 0.67f)
            .WithHitVfxNode(target =>
            {
                SfxCmd.Play("res://Tear/sfx/from_killia/charged_thunder (2).wav");
                if (Owner.Character is Character.Tear tear)
                {
                    tear.PlayVfxOnTarget(
                        target,
                        "res://Tear/scenes/vfx.tscn",
                        "judgment");
                }

                return null;
            })
            .WithHitFx(
                null,
                "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);

        await PowerCmd.Apply<FireFononPower>(
            choiceContext,
            ownerCreature,
            1m,
            ownerCreature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HitCount.UpgradeValueBy(1m);
    }
}