using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Tear.TearCode.Cards.Uncommon;
using Tear.TearCode.Extensions;

namespace Tear.TearCode.Powers;

public class HolySongStrengthPower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel =>
        ModelDb.Card<HolySong>();
    
    public string? CustomPackedIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
            .BigPowerImagePath();

    public string? CustomBigIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
            .BigPowerImagePath();

    public string? CustomBigBetaIconPath =>
        null;
}