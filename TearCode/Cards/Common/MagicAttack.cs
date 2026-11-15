using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Common;

public class MagicAttack() : TearCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("Overlimit",5m),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OverlimitPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Tear tear)
        {
            AudioHelper.PlayRandomAttack();
            tear.PlayAnimation(ownerCreature, "attack_tear");

            SfxCmd.Play("res://Tear/sounds/se/swing_1.mp3");
            SfxCmd.Play("res://Tear/sounds/se/swing_2.mp3");

            await Task.Delay(167);

            SfxCmd.Play("res://Tear/sounds/se/swing_3.mp3");
            tear.PlayVfxOnTarget(
                play.Target,
                "res://Tear/scenes/hit.tscn",
                "hit");
        }

        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);

        OverlimitManager.GainOverlimit(Owner, DynamicVars["Overlimit"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Overlimit"].UpgradeValueBy(3m);
    }
}