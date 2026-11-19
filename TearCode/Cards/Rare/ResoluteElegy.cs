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
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Rare;

public class ResoluteElegy() : TearCard(
    1,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new PowerVar<VulnerablePower>(1m),
        new PowerVar<WeakPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<DarkFononPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null)
            return;

        var enemies = CombatState.HittableEnemies.ToList();

        CenterCardCinematic.Start(
            RunManager.Instance.NetService.NetId);

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

            SfxCmd.Play(
                "res://Tear/sounds/resolute_elegy.wav",
                0.7f);

            if (enemies.Count > 0)
            {
                var middleEnemy =
                    enemies[enemies.Count / 2];

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

        await CommonActions.CardAttack(
            this,
            play.Target)
            .WithHitFx(
                null,
                "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            CombatState.HittableEnemies,
            DynamicVars.Vulnerable.BaseValue,
            ownerCreature,
            this);

        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            CombatState.HittableEnemies,
            DynamicVars.Weak.BaseValue,
            ownerCreature,
            this);

        await PowerCmd.Apply<DarkFononPower>(
            choiceContext,
            ownerCreature,
            1m,
            ownerCreature,
            this);

        CenterCardCinematic.End(
            RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
        DynamicVars.Weak.UpgradeValueBy(1m);
    }
}
