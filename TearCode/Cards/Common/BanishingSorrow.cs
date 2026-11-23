using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Common;

public class BanishingSorrow() : TearCard(2, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy), IArtes
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new PowerVar<VulnerablePower>(1m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.Arte,
        HoverTipFactory.FromPower<VulnerablePower>()
    ];
    
    public override TargetType TargetType =>
        FireFonon
            ? TargetType.AllEnemies
            : TargetType.AnyEnemy;
    
    protected override bool ShouldGlowGoldInternal =>
        Owner.HasPower<FireFononPower>() ||
        Owner.HasPower<LightFononPower>() ||
        Owner.HasPower<OverlimitPower>();
    
    private bool FireFonon =>
        IsMutable &&
        Owner != null &&
        (
            Owner.HasPower<FireFononPower>() ||
            Owner.HasPower<LightFononPower>() ||
            Owner.HasPower<OverlimitPower>()
        );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        
        var hasFireFonon =
            ownerCreature.HasPower<FireFononPower>() ||
            ownerCreature.HasPower<LightFononPower>() ||
            ownerCreature.HasPower<OverlimitPower>();
        var target = play.Target;

        if (!hasFireFonon && target == null)
        {
            var enemies = CombatState.HittableEnemies.ToList();

            if (enemies.Count > 0)
                target = enemies[
                    Owner.RunState.Rng.CombatTargets.NextInt(enemies.Count)
                ];
        }
        
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && Owner?.Character is Character.Tear tear)
        {
            if (hasFireFonon)
                SfxCmd.Play("res://Tear/sounds/searing_sorrow.wav", 0.7f);
            else
                SfxCmd.Play("res://Tear/sounds/banishing_sorrow.wav", 0.7f);
            tear.PlayAnimation(ownerCreature, "attack_tear");
            SfxCmd.Play("res://Tear/sounds/se/swing_1.mp3");
            SfxCmd.Play("res://Tear/sounds/se/swing_2.mp3");
            if (hasFireFonon)
            {
                foreach (var enemy in CombatState.HittableEnemies)
                    tear.PlayVfxOnTarget(enemy, "res://Tear/scenes/vfx.tscn", "searing_sorrow");
                SfxCmd.Play("res://Tear/sfx/from_killia/fire_charge.wav");
                await Task.Delay(533);
                SfxCmd.Play("res://Tear/sfx/from_killia/fire_impact (1).wav");
                await CommonActions.CardAttack(this, play.Target)
                    .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
                    .Execute(choiceContext);
            }

            else
            {
                await Task.Delay(167);
                SfxCmd.Play("res://Tear/sounds/se/swing_3.mp3");
                tear.PlayVfxOnTarget(play.Target, "res://Tear/scenes/hit.tscn", "hit");
                await CommonActions.CardAttack(this, target)
                    .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
                    .Execute(choiceContext);
            }
        }
        else  await CommonActions.CardAttack(this, target)
            .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);
        
        if (hasFireFonon)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, CombatState.HittableEnemies,
                DynamicVars.Vulnerable.BaseValue, ownerCreature, this);
            await PowerCmd.Remove<FireFononPower>(ownerCreature);
            await PowerCmd.Remove<LightFononPower>(ownerCreature);
        }
        else await PowerCmd.Apply<VulnerablePower>(choiceContext, target, DynamicVars.Vulnerable.BaseValue, ownerCreature, this);
        
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}