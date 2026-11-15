using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Tear.TearCode.Extensions;

public static class TearAssets
{
    private static PackedScene? _tearScene;
    private static PackedScene? _vfxScene;
    private static PackedScene? _clusterRaidScene;
    private static PackedScene? _hitScene;
    private static PackedScene? _holyLanceScene;
    private static PackedScene? _innocentShineScene;

    private const string TearScenePath =
        "res://Tear/scenes/tear.tscn";

    private const string VfxPath =
        "res://Tear/scenes/vfx.tscn";

    private const string ClusterRaidPath =
        "res://Tear/scenes/cluster_raid.tscn";

    private const string HitPath =
        "res://Tear/scenes/hit.tscn";

    private const string HolyLancePath =
        "res://Tear/scenes/holy_lance.tscn";

    private const string InnocentShinePath =
        "res://Tear/scenes/innocent_shine.tscn";


    public static bool IsTearInRun(IRunState runState)
    {
        return runState?.Players?.Any(
            p => p?.Character is Tear.TearCode.Character.Tear
        ) ?? false;
    }

    public static PackedScene? TearScene
    {
        get
        {
            _tearScene = LoadOrReload(
                _tearScene,
                TearScenePath,
                "Tear scene");

            return _tearScene;
        }
    }

    public static PackedScene? VfxScene
    {
        get
        {
            _vfxScene = LoadOrReload(
                _vfxScene,
                VfxPath,
                "VFX");

            return _vfxScene;
        }
    }

    public static PackedScene? ClusterRaidScene
    {
        get
        {
            _clusterRaidScene = LoadOrReload(
                _clusterRaidScene,
                ClusterRaidPath,
                "Cluster Raid");

            return _clusterRaidScene;
        }
    }

    public static PackedScene? HitScene
    {
        get
        {
            _hitScene = LoadOrReload(
                _hitScene,
                HitPath,
                "Hit");

            return _hitScene;
        }
    }

    public static PackedScene? HolyLanceScene
    {
        get
        {
            _holyLanceScene = LoadOrReload(
                _holyLanceScene,
                HolyLancePath,
                "Holy Lance");

            return _holyLanceScene;
        }
    }

    public static PackedScene? InnocentShineScene
    {
        get
        {
            _innocentShineScene = LoadOrReload(
                _innocentShineScene,
                InnocentShinePath,
                "Innocent Shine");

            return _innocentShineScene;
        }
    }

    private static PackedScene? LoadOrReload(
        PackedScene? cachedScene,
        string path,
        string label)
    {
        if (cachedScene != null &&
            GodotObject.IsInstanceValid(cachedScene))
        {
            return cachedScene;
        }

        GD.Print(
            $"TearAssets: Loading {label} from {path}");

        var scene =
            GD.Load<PackedScene>(path);

        if (scene == null)
        {
            GD.PrintErr(
                $"TearAssets: FAILED to load {label}: {path}");

            return null;
        }

        GD.Print(
            $"TearAssets: Loaded {label}");

        return scene;
    }

    public static void EnsurePreloaded()
    {
        _ = TearScene;
        _ = VfxScene;
        _ = ClusterRaidScene;
        _ = HitScene;
        _ = HolyLanceScene;
        _ = InnocentShineScene;

        GD.Print(
            "TearAssets: EnsurePreloaded finished");
    }
}

[HarmonyPatch(
    typeof(Hook),
    nameof(Hook.AfterActEntered))]
public static class TearAfterActEnteredPreloadPatch
{
    [HarmonyPrefix]
    public static void Prefix(
        IRunState runState)
    {
        if (!TearAssets.IsTearInRun(runState))
            return;

        GD.Print(
            "AfterActEntered: Tear detected -> preloading");

        TearAssets.EnsurePreloaded();
    }
}

[HarmonyPatch(
    typeof(Hook),
    nameof(Hook.AfterRoomEntered))]
public static class TearAfterRoomEnteredPreloadPatch
{
    [HarmonyPrefix]
    public static void Prefix(
        IRunState runState,
        AbstractRoom room)
    {
        if (!TearAssets.IsTearInRun(runState))
            return;

        GD.Print(
            $"AfterRoomEntered: Tear detected -> preloading. " +
            $"Room = {room.GetType().Name}");

        TearAssets.EnsurePreloaded();
    }
}