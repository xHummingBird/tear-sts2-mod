using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Common;

public class Charge() : TearCard(
    0,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self), IArtes
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override bool ShouldGlowGoldInternal =>
        Owner.HasPower<EarthFononPower>() ||
        Owner.HasPower<DarkFononPower>() ||
        Owner.HasPower<OverlimitPower>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
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
        var hasEarthFonon = ownerCreature.HasPower<EarthFononPower>() || ownerCreature.HasPower<OverlimitPower>() || ownerCreature.HasPower<DarkFononPower>();

        if (ownerCreature != null && Owner?.Character is Character.Tear tear)
        {
            tear.PlayAnimation(ownerCreature, "cast_1");
            SfxCmd.Play("res://Tear/sounds/se/cast_1.mp3");

            await Task.Delay(500);

            tear.PlayAnimation(ownerCreature, "cast_2");
            SfxCmd.Play("res://Tear/sounds/se/cast_2.mp3");
            SfxCmd.Play("res://Tear/sounds/se/revitalise.mp3");

            if (hasEarthFonon)
                SfxCmd.Play("res://Tear/sounds/tribute.wav", 0.7f);
            else
                SfxCmd.Play("res://Tear/sounds/charge.wav", 0.7f);
        }

        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);

        if (hasEarthFonon)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
            await PowerCmd.Remove<EarthFononPower>(ownerCreature);
            await PowerCmd.Remove<DarkFononPower>(ownerCreature);
        }
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}