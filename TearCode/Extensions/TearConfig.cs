using BaseLib.Config;

namespace Tear.TearCode.Extensions;

public sealed class TearConfig : SimpleModConfig
{
    [ConfigSection("Audio")]
    public static bool EnableCustomBattleMusic { get; set; } = true;

    public static bool EnableCustomVictoryMusic { get; set; } = true;

    public static bool EnableCustomGameOverMusic { get; set; } = true;
}