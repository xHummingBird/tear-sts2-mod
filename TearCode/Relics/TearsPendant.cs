using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Relics;

public class TearsPendant : TearRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal damage,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer != Owner.Creature)
            return 1m;

        if (cardSource is not IArtes &&
            cardSource is not IFonicHymn)
        {
            return 1m;
        }

        return 1.25m;
    }
}