using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Mechanics;

public static class ElementalFononHelper
{
    public static bool IsElementalFonon(PowerModel power)
    {
        return power is FireFononPower or
            WaterFononPower or
            WindFononPower or
            EarthFononPower or
            LightFononPower or
            DarkFononPower;
    }
}