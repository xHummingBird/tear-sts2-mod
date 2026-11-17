using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using Tear.TearCode.Character;
using Tear.TearCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards;

[Pool(typeof(TearCardPool))]
public abstract class TearCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    
    protected override bool IsPlayable
    {
        get
        {
            if (this is not IFonicHymn)
                return true;

            if (Owner.Creature.HasPower<OverlimitPower>() ||
                Owner.Creature.HasPower<CadencePower>())
            {
                return true;
            }

            return !CombatManager.Instance.History.CardPlaysFinished.Any(
                entry =>
                    entry.HappenedThisTurn(CombatState) &&
                    entry.CardPlay.Card.Owner == Owner &&
                    entry.CardPlay.Card is IFonicHymn);
        }
    }
    
    protected override void AddExtraArgsToDescription(
        LocString description)
    {
        base.AddExtraArgsToDescription(description);

        description.Add(
            "FireFonon",
            "[img]res://Tear/images/charui/fire_fonon_icon.png[/img]");

        description.Add(
            "WindFonon",
            "[img]res://Tear/images/charui/wind_fonon_icon.png[/img]");

        description.Add(
            "EarthFonon",
            "[img]res://Tear/images/charui/earth_fonon_icon.png[/img]");

        description.Add(
            "WaterFonon",
            "[img]res://Tear/images/charui/water_fonon_icon.png[/img]");

        description.Add(
            "LightFonon",
            "[img]res://Tear/images/charui/light_fonon_icon.png[/img]");

        description.Add(
            "DarkFonon",
            "[img]res://Tear/images/charui/dark_fonon_icon.png[/img]");
    }
}