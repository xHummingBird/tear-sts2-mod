using BaseLib.Extensions;
using BaseLib.Utils;
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

namespace Tear.TearCode.Cards.Uncommon;

public class NocturnalAqua() : TearCard(0, CardType.Attack,
    CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(12m),
        new ExtraDamageVar(6m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(
                static (CardModel card, Creature? _) =>
                    card.CombatState?.HittableEnemies.Count == 1
                        ? 1m
                        : 0m)
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && Owner?.Character is Character.Tear tear)
        {
            SfxCmd.Play("res://Tear/sounds/noctur", 0.7f);
            tear.PlayAnimation(ownerCreature, "attack_tear");
            SfxCmd.Play("res://Tear/sounds/se/swing_1.mp3");
            SfxCmd.Play("res://Tear/sounds/se/swing_2.mp3");
            foreach (var enemy in CombatState.HittableEnemies)
                tear.PlayVfxOnTarget(enemy, "res://Tear/scenes/vfx.tscn", "nocturnal_aqua");
            SfxCmd.Play("res://Tear/sfx/water_2.wav");
            await Task.Delay(167);
            SfxCmd.Play("res://Tear/sounds/se/swing_3.mp3");
            await Task.Delay(100);
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);
        await PowerCmd.Apply<WaterFononPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
        DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}