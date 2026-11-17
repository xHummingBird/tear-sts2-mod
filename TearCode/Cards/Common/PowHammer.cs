using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Common;

public class PowHammer() : TearCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy), IArtes
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new PowerVar<WeakPower>(1m),
        new PowerVar<VulnerablePower>(1m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.Arte,
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];
    
    protected override bool ShouldGlowGoldInternal =>
        Owner.HasPower<WaterFononPower>() ||
        Owner.HasPower<DarkFononPower>() ||
        Owner.HasPower<OverlimitPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        
        var hasWaterFonon =
            ownerCreature.HasPower<WaterFononPower>() ||
            ownerCreature.HasPower<DarkFononPower>() ||
            ownerCreature.HasPower<OverlimitPower>();
        
        if (ownerCreature != null && Owner?.Character is Character.Tear tear)
        {
            if (hasWaterFonon)
                SfxCmd.Play("res://Tear/sounds/ice_hammer.wav", 0.7f);
            else
                SfxCmd.Play("res://Tear/sounds/piko_hammer.wav", 0.7f);
            tear.PlayAnimation(ownerCreature, "attack_tear");
            SfxCmd.Play("res://Tear/sounds/se/swing_1.mp3");
            SfxCmd.Play("res://Tear/sounds/se/swing_2.mp3");
            int currentHit = 0;
            if (hasWaterFonon)
                tear.PlayVfxOnTarget(play.Target, "res://Tear/scenes/vfx.tscn", "frozen_hammer");
            else
                tear.PlayVfxOnTarget(play.Target, "res://Tear/scenes/vfx.tscn", "pow_hammer");
            SfxCmd.Play("res://Tear/sfx/from_killia/punch_1.mp3");
            await Task.Delay(300);
            if (hasWaterFonon)
                SfxCmd.Play("res://Tear/sfx/from_killia/ice.wav");
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, DynamicVars.Weak.BaseValue, Owner.Creature, this);
        if (hasWaterFonon)
            await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
        
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Weak.UpgradeValueBy(1m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}