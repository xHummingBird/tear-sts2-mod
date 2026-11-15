using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Tear.TearCode.Extensions;

public static class TearStaticHoverTips
{
    public static readonly IHoverTip Overlimit = new HoverTip(
        new LocString(
            "static_hover_tips",
            "TEAR_OVERLIMIT.title"
        ),
        new LocString(
            "static_hover_tips",
            "TEAR_OVERLIMIT.description"
        )
    );

    public static readonly IHoverTip Arte = new HoverTip(
        new LocString(
            "static_hover_tips",
            "TEAR_ARTE.title"
        ),
        new LocString(
            "static_hover_tips",
            "TEAR_ARTE.description"
        )
    );

    public static readonly IHoverTip FonicHymn = new HoverTip(
        new LocString(
            "static_hover_tips",
            "TEAR_FONIC_HYMN.title"
        ),
        new LocString(
            "static_hover_tips",
            "TEAR_FONIC_HYMN.description"
        )
    );
}