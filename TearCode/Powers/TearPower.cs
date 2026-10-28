using BaseLib.Abstracts;
using BaseLib.Extensions;
using Tear.TearCode.Extensions;
using Godot;

namespace Tear.TearCode.Powers;

public abstract class TearPower : CustomPowerModel
{
    //Loads from Tear/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}