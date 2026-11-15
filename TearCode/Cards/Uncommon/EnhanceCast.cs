using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Uncommon;

public class EnhanceCast() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self), IArtes
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.Arte
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        
        var hasWindFonon =
            ownerCreature.HasPower<WindFononPower>() ||
            ownerCreature.HasPower<LightFononPower>() ||
            ownerCreature.HasPower<OverlimitPower>();

        if (ownerCreature != null && Owner?.Character is Character.Tear tear)
        {
            tear.PlayAnimation(ownerCreature, "cast_1");
            SfxCmd.Play("res://Tear/sounds/se/cast_1.mp3");
            await Task.Delay(500);
            tear.PlayAnimation(ownerCreature, "cast_2");
            SfxCmd.Play("res://Tear/sounds/se/cast_2.mp3");
            SfxCmd.Play("res://Tear/sounds/se/revitalise.mp3");
            if (hasWindFonon)
                SfxCmd.Play("res://Tear/sounds/witchcraft.wav", 0.7f);
            else
                SfxCmd.Play("res://Tear/sounds/enhance_cast.wav", 0.7f);
        }

        await PowerCmd.Apply<FreeAttackPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);

        if (hasWindFonon)
        {
            await PowerCmd.Apply<FreeSkillPower>(
                choiceContext,
                Owner.Creature,
                1m,
                Owner.Creature,
                this);
            await PowerCmd.Remove<WindFononPower>(ownerCreature);
            await PowerCmd.Remove<LightFononPower>(ownerCreature);
        }
    }
    
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}