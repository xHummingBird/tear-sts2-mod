using MegaCrit.Sts2.Core.Entities.Relics;

namespace Tear.TearCode.Relics;

public class BlueCrystalRod : FonicRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int OverlimitPerTurn => 7;
    protected override int OverlimitPerCard => 5;
    protected override int OverlimitPerArte => 8;

    protected override int OverlimitPerFonicHymn => 8;
}