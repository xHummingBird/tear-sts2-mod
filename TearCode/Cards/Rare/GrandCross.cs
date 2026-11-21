using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Powers;
using Tear.TearCode.Relics;

namespace Tear.TearCode.Cards.Rare;

public class GrandCross() : TearCard(
    2,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(25m, ValueProp.Move)
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

        if (ownerCreature == null)
            return;
        
        var enemies = CombatState.HittableEnemies.ToList();
        
        if (Owner.Character is Character.Tear tear)
        {
            if (Owner.GetRelic<SpecialFlagA>() != null)
                SfxCmd.Play("res://Tear/sounds/hymns/sixth_hymn.mp3");
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
                "res://Tear/sounds/grand_cross.wav",
                0.7f);
            
            var middleEnemy =
                enemies[enemies.Count / 2];

            tear.PlayVfxBehindTarget(
                middleEnemy,
                "res://Tear/scenes/vfx.tscn",
                "grand_cross");
            
            SfxCmd.Play("res://Tear/sounds/se/cast_revitalise.mp3");
            SfxCmd.Play("res://Tear/sounds/se/innocent_shine_1.mp3");
            await Task.Delay(800);
            foreach (var enemy in CombatState.HittableEnemies)
            {
                tear.PlayVfxOnTarget(
                    enemy,
                    "res://Tear/scenes/vfx.tscn",
                    "grand_cross_hit");
            }
            SfxCmd.Play("res://Tear/sfx/from_tifa/chi_sfx.wav");
        }
        
        await DamageCmd.Attack(
                DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingAllOpponents(CombatState)
            .WithHitFx(
                null,
                "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);

        await PowerCmd.Apply<LightFononPower>(
            choiceContext,
            ownerCreature,
            1m,
            ownerCreature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7m);
    }
}