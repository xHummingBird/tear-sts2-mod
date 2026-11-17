using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Powers;

public class FonicMasteryPower : TearPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal damage,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer != Owner)
            return 1m;

        if (cardSource is not IArtes)
            return 1m;

        return 1m + (Amount / 100m);
    }
}