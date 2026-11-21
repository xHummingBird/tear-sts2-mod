using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;
using Tear.TearCode.Relics;

namespace Tear.TearCode.Cards.Uncommon;

public class FirstAid() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self), IArtes
{
    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(3m)
    ];
    
    protected override bool ShouldGlowGoldInternal =>
        Owner.HasPower<WaterFononPower>() ||
        Owner.HasPower<DarkFononPower>() ||
        Owner.HasPower<OverlimitPower>();

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

        var hasWaterFonon =
            ownerCreature.HasPower<WaterFononPower>() ||
            ownerCreature.HasPower<DarkFononPower>() ||
            ownerCreature.HasPower<OverlimitPower>();

        if (Owner.Character is Character.Tear tear)
        {
            tear.PlayAnimation(ownerCreature, "cast_1");
            SfxCmd.Play("res://Tear/sounds/se/cast_1.mp3");

            await Task.Delay(500);

            tear.PlayAnimation(ownerCreature, "cast_2");
            SfxCmd.Play("res://Tear/sounds/se/cast_2.mp3");
            SfxCmd.Play("res://Tear/sounds/se/revitalise.mp3");

            if (hasWaterFonon)
                SfxCmd.Play("res://Tear/sounds/meditation.wav", 0.7f);
            else
                SfxCmd.Play("res://Tear/sounds/first_aid.wav", 0.7f);
        }

        decimal healAmount = DynamicVars.Heal.BaseValue;

        if (hasWaterFonon)
            healAmount *= 2m;

        if (Owner.Character is Character.Tear)
        {
            var fonicRelic = Owner.Relics
                .OfType<FonicRelicBase>()
                .FirstOrDefault();

            if (fonicRelic != null)
            {
                healAmount = Math.Min(
                    healAmount,
                    Math.Max(
                        0m,
                        fonicRelic.StartingCombatHp -
                        ownerCreature.CurrentHp));
            }
        }

        if (healAmount > 0m)
        {
            // Replace this line with your confirmed healing command.
            await CreatureCmd.Heal(
                ownerCreature,
                healAmount);
        }
        
        
        if (hasWaterFonon)
        {
            await PowerCmd.Remove<WaterFononPower>(ownerCreature);
            await PowerCmd.Remove<DarkFononPower>(ownerCreature);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}