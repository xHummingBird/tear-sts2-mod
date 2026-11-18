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

public class SeveredFate() : TearCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy), IArtes
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.Arte,
    ];
    
    public override TargetType TargetType =>
        WindFonon
            ? TargetType.AllEnemies
            : TargetType.AnyEnemy;
    
    private bool WindFonon =>
        IsMutable &&
        Owner != null &&
        (
            Owner.HasPower<WindFononPower>() ||
            Owner.HasPower<LightFononPower>() ||
            Owner.HasPower<OverlimitPower>()
        );
    
    protected override bool ShouldGlowGoldInternal =>
        Owner.HasPower<WindFononPower>() ||
        Owner.HasPower<LightFononPower>() ||
        Owner.HasPower<OverlimitPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        
        var hasWindFonon =
            Owner.HasPower<WindFononPower>() ||
            Owner.HasPower<LightFononPower>() ||
            Owner.HasPower<OverlimitPower>();
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && Owner?.Character is Character.Tear tear)
        {
            if (hasWindFonon)
                SfxCmd.Play("res://Tear/sounds/fatal_circle.wav", 0.7f);
            else
                SfxCmd.Play("res://Tear/sounds/severed_fate.wav", 0.7f);
            tear.PlayAnimation(ownerCreature, "attack_tear");
            SfxCmd.Play("res://Tear/sounds/se/swing_1.mp3");
            SfxCmd.Play("res://Tear/sounds/se/swing_2.mp3");
            await Task.Delay(167);
            SfxCmd.Play("res://Tear/sounds/se/swing_3.mp3");
            tear.PlayVfxOnTarget(play.Target, "res://Tear/scenes/vfx.tscn", "severed_fate");
            await Task.Delay(333);
            tear.PlayVfxOnTarget(play.Target, "res://Tear/scenes/hit.tscn", "hit");
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);
        if (hasWindFonon)
        {
            await PowerCmd.Remove<WindFononPower>(ownerCreature);
            await PowerCmd.Remove<LightFononPower>(ownerCreature);
        }
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}