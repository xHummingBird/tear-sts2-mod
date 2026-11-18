using BaseLib.Extensions;
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

public class HolyLance() : TearCard(
    0,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AllEnemies), IArtes
{
    protected override bool HasEnergyCostX => true;

    private bool HasEarthFonon =>
        IsMutable &&
        Owner != null &&
        (
            Owner.HasPower<EarthFononPower>() ||
            Owner.HasPower<DarkFononPower>() ||
            Owner.HasPower<OverlimitPower>()
        );

    protected override bool ShouldGlowGoldInternal =>
        HasEarthFonon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.Arte
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null)
            return;

        var hasEarthFonon =
            Owner.HasPower<EarthFononPower>() ||
            Owner.HasPower<DarkFononPower>() ||
            Owner.HasPower<OverlimitPower>();
        
        var enemies = CombatState.HittableEnemies.ToList();
        

        var hitCount = ResolveEnergyXValue();

        if (hasEarthFonon)
            hitCount *= 2;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        
        if (Owner.Character is Character.Tear tear)
        {
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

            if (hasEarthFonon)
            {
                SfxCmd.Play(
                    "res://Tear/sounds/cluster_raid.wav",
                    0.7f);
                SfxCmd.Play("res://Tear/sounds/se/cluster_raid_1.mp3");
                SfxCmd.Play("res://Tear/sounds/se/cluster_raid_2.mp3");
                if (enemies.Count > 0)
                {
                    var middleEnemy = enemies[enemies.Count / 2];

                    tear.PlayVfxBehindTarget(
                        middleEnemy,
                        "res://Tear/scenes/cluster_raid.tscn",
                        "effect");
                }
                await Task.Delay(500);
                SfxCmd.Play("res://Tear/sounds/se/cluster_raid_3.mp3");
                await Task.Delay(267);
                SfxCmd.Play("res://Tear/sounds/se/cluster_raid_4.mp3");
                await Task.Delay(33);

            }
            else
            {
                SfxCmd.Play(
                    "res://Tear/sounds/holy_lance.wav",
                    0.7f);
                SfxCmd.Play("res://Tear/sounds/se/holy_lance_1.mp3");
                if (enemies.Count > 0)
                {
                    var middleEnemy = enemies[enemies.Count / 2];

                    tear.PlayVfxBehindTarget(
                        middleEnemy,
                        "res://Tear/scenes/holy_lance.tscn",
                        "effect");
                }
                await Task.Delay(600);
                SfxCmd.Play("res://Tear/sounds/se/holy_lance_2.mp3");
                await Task.Delay(33);
            }
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hitCount)
            .FromCard(this, play)
            .TargetingAllOpponents(CombatState)
            .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);

        if (hasEarthFonon)
        {
            await PowerCmd.Remove<EarthFononPower>(
                ownerCreature);

            await PowerCmd.Remove<DarkFononPower>(
                ownerCreature);
        }
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}