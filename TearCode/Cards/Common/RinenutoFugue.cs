using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Common;

public class RinenutoFugue() : TearCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new HitCountVar(3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<LightFononPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null &&
            Owner?.Character is Character.Tear tear)
        {
            SfxCmd.Play("res://Tear/sounds/riteneto_fugue.wav", 0.7f);

            tear.PlayAnimation(
                ownerCreature,
                "attack_tear");

            SfxCmd.Play("res://Tear/sounds/se/swing_1.mp3");
            SfxCmd.Play("res://Tear/sounds/se/swing_2.mp3");

            await Task.Delay(167);

            SfxCmd.Play("res://Tear/sounds/se/swing_3.mp3");
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.HitCount.IntValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitVfxNode(target =>
            {
                if (Owner.Character is Character.Tear tear)
                {
                    tear.PlayVfxOnTarget(
                        target,
                        "res://Tear/scenes/hit.tscn",
                        "hit");
                }

                return null;
            })
            .WithHitFx(
                null,
                "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);
        await PowerCmd.Apply<LightFononPower>(choiceContext, ownerCreature, 1, ownerCreature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}