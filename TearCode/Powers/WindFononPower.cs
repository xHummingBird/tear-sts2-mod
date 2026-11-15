using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Tear.TearCode.Powers;

public class WindFononPower : TearPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    public override async Task AfterApplied(
        Creature? applier,
        CardModel? cardSource)
    {
        await PowerCmd.Remove<FireFononPower>(Owner);
        await PowerCmd.Remove<WaterFononPower>(Owner);
        await PowerCmd.Remove<LightFononPower>(Owner);
        await PowerCmd.Remove<DarkFononPower>(Owner);
        await PowerCmd.Remove<EarthFononPower>(Owner);
    }
}