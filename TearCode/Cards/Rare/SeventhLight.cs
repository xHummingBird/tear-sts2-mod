using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Rare;

public class SeventhLight() : TearCard(1, CardType.Attack,
    CardRarity.Rare, TargetType.AllEnemies), IArtes
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new HitCountVar(4)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.Arte
    ];
    
    protected override bool ShouldGlowGoldInternal =>
        Owner.HasPower<WindFononPower>() ||
        Owner.HasPower<LightFononPower>() ||
        Owner.HasPower<OverlimitPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        
        var hasWindFonon =
            ownerCreature.HasPower<WindFononPower>() ||
            ownerCreature.HasPower<LightFononPower>() ||
            ownerCreature.HasPower<OverlimitPower>();
        int currentHit = 4;
        if (hasWindFonon)
            currentHit = 7;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && Owner?.Character is Character.Tear tear)
        {
            if (hasWindFonon)
                SfxCmd.Play("res://Tear/sounds/seventh_raid.wav", 0.7f);
            else
                SfxCmd.Play("res://Tear/sounds/seventh_light.wav", 0.7f);
            tear.PlayAnimation(ownerCreature, "attack_tear");
            SfxCmd.Play("res://Tear/sounds/se/swing_1.mp3");
            SfxCmd.Play("res://Tear/sounds/se/swing_2.mp3");
           
            await Task.Delay(167);
            SfxCmd.Play("res://Tear/sounds/se/swing_3.mp3");
            foreach(var enemy in CombatState.HittableEnemies)
                tear.PlayVfxOnTarget(enemy, "res://Tear/scenes/vfx.tscn", "seventh_light");
        }
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(currentHit)
            .FromCard(this, play)
            .TargetingAllOpponents(CombatState)
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
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}