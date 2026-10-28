using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Tear.TearCode.Character;
using Tear.TearCode.Extensions;
using Godot;

namespace Tear.TearCode.Relics;

[Pool(typeof(TearRelicPool))]
public abstract class TearRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();

    protected override string PackedIconOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();

    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}