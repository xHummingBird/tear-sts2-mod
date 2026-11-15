using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Tear.TearCode.Cards.Uncommon;

namespace Tear.TearCode.Powers;

public class HolySongStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel =>
        ModelDb.Card<HolySong>();
}