using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Cards.Basic;

public class NocturnalLight() : TearCard(0, CardType.Attack,
    CardRarity.Basic, TargetType.AnyEnemy), IArtes
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.Arte
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Tear tear)
        {
            SfxCmd.Play("res://Tear/sounds/nocturnal_light.wav", 0.7f);
            tear.PlayAnimation(ownerCreature, "attack_tear");
            SfxCmd.Play("res://Tear/sounds/se/swing_1.mp3");
            SfxCmd.Play("res://Tear/sounds/se/swing_2.mp3");
            await Task.Delay(167);
            SfxCmd.Play("res://Tear/sounds/se/swing_3.mp3");
            tear.PlayVfxOnTarget(play.Target, "res://Tear/scenes/hit.tscn", "hit");
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}