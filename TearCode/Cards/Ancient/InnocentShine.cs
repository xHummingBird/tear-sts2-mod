using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Ancient;

public class InnocentShine() : TearCard(
    0,
    CardType.Attack,
    CardRarity.Ancient,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(30m),
        new ExtraDamageVar(2m),

        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(
                static (CardModel card, Creature? _) =>
                    CombatManager.Instance.History.CardPlaysFinished.Count(
                        entry =>
                            entry.CardPlay.Player == card.Owner &&
                            entry.CardPlay.Card is IArtes))
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
        
        var enemies = CombatState.HittableEnemies.ToList();
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (Owner.Character is Character.Tear tear)
        {
            SfxCmd.Play("res://Tear/sounds/innocent_shine_1.wav", 0.7f);
            tear.PlayAnimation(ownerCreature, "cast_1");

            SfxCmd.Play(
                "res://Tear/sounds/se/cast_1.mp3");

            await Task.Delay(500);

            tear.PlayAnimation(
                ownerCreature,
                "cast_2");

            SfxCmd.Play(
                "res://Tear/sounds/se/cast_2.mp3");
            if (enemies.Count > 0)
            {
                var middleEnemy = enemies[enemies.Count / 2];

                tear.PlayVfxOnTarget(
                    middleEnemy,
                    "res://Tear/scenes/innocent_shine.tscn",
                    "special");
            }
            SfxCmd.Play(
                "res://Tear/sounds/se/innocent_shine_1.mp3");
            await Task.Delay(1000);
            SfxCmd.Play("res://Tear/sounds/se/innocent_shine_2.mp3");
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(233);
            SfxCmd.Play("res://Tear/sounds/se/innocent_shine_3.mp3");
            SfxCmd.Play("res://Tear/sounds/innocent_shine_2.wav", 0.7f);
            tear.PlayCutIn("innocent_cutin");
            await Task.Delay(800);
            SfxCmd.Play("res://Tear/sounds/se/innocent_shine_4.mp3");
            await Task.Delay(33);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
            await Task.Delay(100);
            foreach (var enemy in enemies)
                TearExtensions.CombatHelpers.TearFakeHit(ownerCreature, enemy, null, null, null, null, false);
            SfxCmd.Play("res://Tear/sfx/tear_hit.wav");
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(6m);
        DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}