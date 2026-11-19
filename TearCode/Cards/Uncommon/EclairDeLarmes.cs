using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Uncommon;

public class EclairDeLarmes() : TearCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AllEnemies), IArtes
{
    private const string TurnsKey = "Turns";

    private bool HasFireFonon =>
        IsMutable &&
        Owner != null &&
        (
            Owner.HasPower<FireFononPower>() ||
            Owner.HasPower<LightFononPower>() ||
            Owner.HasPower<OverlimitPower>()
        );

    protected override bool ShouldGlowGoldInternal =>
        HasFireFonon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar(TurnsKey, 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.Arte,
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null)
            return;

        var hasFireFonon =
            ownerCreature.HasPower<FireFononPower>() ||
            ownerCreature.HasPower<LightFononPower>() ||
            ownerCreature.HasPower<OverlimitPower>();

        var enemies =
            CombatState.HittableEnemies.ToList();

        CenterCardCinematic.Start(
            RunManager.Instance.NetService.NetId);

        if (Owner.Character is Character.Tear tear)
        {
            tear.PlayAnimation(ownerCreature, "cast_1");
            SfxCmd.Play("res://Tear/sounds/se/cast_1.mp3");
            await Task.Delay(500);
            tear.PlayAnimation(ownerCreature, "cast_2");
            SfxCmd.Play("res://Tear/sounds/se/cast_2.mp3");
            if (enemies.Count > 0)
            {
                var middleEnemy =
                    enemies[enemies.Count / 2];

                if (hasFireFonon)
                {
                    SfxCmd.Play(
                        "res://Tear/sounds/flamme_rouge.wav",
                        0.7f);
                    tear.PlayVfxOnTarget(
                        middleEnemy,
                        "res://Tear/scenes/vfx.tscn",
                        "flamme_rouge");
                    SfxCmd.Play("res://Tear/sfx/from_killia/fire_impact (1).wav");
                    await Task.Delay(133);
                }
                else
                {
                    SfxCmd.Play(
                        "res://Tear/sounds/eclair.wav",
                        0.7f);
                    tear.PlayVfxBehindTarget(
                        middleEnemy,
                        "res://Tear/scenes/vfx.tscn",
                        "fatal_circle");

                    SfxCmd.Play(
                        "res://Tear/sounds/se/cluster_raid_1.mp3");

                    await Task.Delay(100);

                    SfxCmd.Play(
                        "res://Tear/sfx/from_tifa/chi_sfx_2.wav");

                    foreach (var enemy in enemies)
                    {
                        tear.PlayVfxOnTarget(
                            enemy,
                            "res://Tear/scenes/hit.tscn",
                            "hit");
                    }
                }
            }
        }
        
        if (hasFireFonon)
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, play).TargetingAllOpponents(base.CombatState)
                .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
                .WithHitVfxSpawnedAtBase()
                .BeforeDamage(async delegate
                {
                    var targets = base.CombatState.HittableEnemies;
                    foreach (var target in targets)
                    {
                        var vfx = NGroundFireVfx.Create(target, VfxColor.Red);
                        if (vfx != null)
                        {
                            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                            SfxCmd.Play("event:/sfx/characters/attack_fire");
                        }
                    }
                })
                .Execute(choiceContext);
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(
                    null,
                    "res://Tear/sfx/tear_hit.wav")
                .Execute(choiceContext);

        if (hasFireFonon)
        {
            await PowerCmd.Apply<DebilitatePower>(
                choiceContext,
                CombatState.HittableEnemies,
                DynamicVars[TurnsKey].BaseValue,
                ownerCreature,
                this);

            await PowerCmd.Remove<FireFononPower>(
                ownerCreature);

            await PowerCmd.Remove<LightFononPower>(
                ownerCreature);
        }

        CenterCardCinematic.End(
            RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars[TurnsKey].UpgradeValueBy(1m);
    }
}