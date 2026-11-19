using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Cards.Rare;

public class HarmoniseScore() : TearCard(
    2,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null)
            return;

        var hitCount =
            1 + Owner.PlayerCombatState.AllCards.Count(
                card => card is IFonicHymn);

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
            SfxCmd.Play("res://Tear/sounds/harmonise_score.wav", 0.7f);
            foreach (var enemy in CombatState.HittableEnemies)
            {
                tear.PlayVfxOnTarget(
                    enemy,
                    "res://Tear/scenes/vfx.tscn",
                    "harmonise_score");
            }
            await Task.Delay(267);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hitCount)
            .FromCard(this, play)
            .TargetingAllOpponents(CombatState)
            .WithHitVfxNode(target =>
            {
                if (Owner.Character is Character.Tear tear)
                {
                    tear.PlayVfxOnTarget(
                        target,
                        "res://Tear/scenes/hit.tscn",
                        "hit");
                }
                return null;
            })
            .WithHitFx(
                null,
                "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}