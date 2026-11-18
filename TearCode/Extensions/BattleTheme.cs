using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace Tear.TearCode.Extensions;

public static class BattleTheme
{
    private readonly record struct MusicTrack(
        string Path,
        float VolumeDb);

    private static AudioStreamPlayer? _battlePlayer;
    private static AudioStreamPlayer? _fanfarePlayer;
    private static AudioStreamPlayer? _gameOverPlayer;

    private static float _battleVolumeDb;
    private static float _fanfareVolumeDb;
    private static float _gameOverVolumeDb;

    // ============================================================
    // VICTORY / GAME OVER
    // ============================================================

    private static readonly MusicTrack VictoryFanfare =
        new(
            "res://Tear/music/1-06 Victory!.mp3",
            -14f);

    private static readonly MusicTrack VictoryFanfareShort =
        new(
            "res://Tear/music/1-06 Victory!.mp3",
            -14f);

    private static readonly MusicTrack GameOverTheme =
        new(
            "res://Tear/music/3-16 Game over.mp3",
            -14f);

    // ============================================================
    // NORMAL
    // Define each track as:
    // new("res://Tear/music/File.mp3", volumeDb)
    // ============================================================

    private static readonly MusicTrack[] Act1Normal =
    [
        new(
            "res://Tear/music/1-05 The arrow was shot.mp3",
            -14f),
    ];

    private static readonly MusicTrack[] Act2Normal =
    [
        new(
            "res://Tear/music/1-05 The arrow was shot.mp3",
            -14f),

        new(
            "res://Tear/music/2-22 The edge of a decision.mp3",
            -14f),
    ];

    private static readonly MusicTrack[] Act3Normal =
    [
        new(
            "res://Tear/music/1-05 The arrow was shot.mp3",
            -14f),

        new(
            "res://Tear/music/2-22 The edge of a decision.mp3",
            -14f),

        new(
            "res://Tear/music/3-28 Everlasting fight.mp3",
            -14f),
        
        new(
            "res://Tear/music/4-02 Never surrender.mp3",
            -14f)
    ];

    // ============================================================
    // ELITE
    // ============================================================

    private static readonly MusicTrack[] Act1Elite =
    [
        new(
            "res://Tear/music/2-22 The edge of a decision.mp3",
            -14f),
    ];

    private static readonly MusicTrack[] Act2Elite =
    [
        new(
            "res://Tear/music/1-14 Fang which wants blood.mp3",
            -14f),
    ];

    private static readonly MusicTrack[] Act3Elite =
    [
        new(
            "res://Tear/music/1-14 Fang which wants blood.mp3",
            -14f),

        new(
            "res://Tear/music/1-18 Awkward justice.mp3",
            -14f),
    ];

    // ============================================================
    // BOSS
    // ============================================================

    private static readonly MusicTrack[] Act1Boss =
    [
        new(
            "res://Tear/music/1-14 Fang which wants blood.mp3",
            -14f),
    ];

    private static readonly MusicTrack[] Act2Boss =
    [
        new(
            "res://Tear/music/1-18 Awkward justice.mp3",
            -14f),
    ];

    private static readonly MusicTrack[] Act3Boss =
    [
        new(
            "res://Tear/music/4-18 meaning of birth.mp3",
            -14f),

        new(
            "res://Tear/music/3-29 Eternal mind.mp3",
            -14f),
    ];

    // ============================================================
    // VANILLA MUSIC CONTROL
    // ============================================================

    public static void StopVanillaMusicOnly()
    {
        var controller =
            NRunMusicController.Instance;

        if (controller == null)
            return;

        var proxyField =
            AccessTools.Field(
                typeof(NRunMusicController),
                "_proxy");

        if (proxyField == null)
            return;

        var proxy =
            proxyField.GetValue(controller) as Node;

        proxy?.Call("stop_music");
    }

    public static void ResumeVanillaMusic()
    {
        var controller =
            NRunMusicController.Instance;

        if (controller == null)
            return;

        var proxyField =
            AccessTools.Field(
                typeof(NRunMusicController),
                "_proxy");

        var currentTrackField =
            AccessTools.Field(
                typeof(NRunMusicController),
                "_currentTrack");

        if (proxyField == null ||
            currentTrackField == null)
        {
            return;
        }

        var proxy =
            proxyField.GetValue(controller) as Node;

        string? currentTrack =
            currentTrackField.GetValue(controller)
                as string;

        proxy?.Call("stop_music");

        if (proxy == null ||
            string.IsNullOrEmpty(currentTrack))
        {
            return;
        }

        proxy.Call(
            "update_music",
            currentTrack);

        proxy.Call(
            "update_global_parameter",
            "Progress",
            7);
    }

    // ============================================================
    // VOLUME
    // ============================================================

    private static float ScaleDbByBgmSetting(
        float baseVolumeDb)
    {
        float bgmSetting =
            SaveManager.Instance?
                .SettingsSave?
                .VolumeBgm
            ?? 1f;

        if (bgmSetting <= 0f)
            return -80f;

        float linearVolume =
            Mathf.DbToLinear(baseVolumeDb);

        linearVolume *= bgmSetting;

        return Mathf.LinearToDb(
            Math.Max(
                linearVolume,
                0.0001f));
    }

    public static void RefreshCustomMusicVolumes()
    {
        SetPlayerVolume(
            _battlePlayer,
            _battleVolumeDb);

        SetPlayerVolume(
            _fanfarePlayer,
            _fanfareVolumeDb);

        SetPlayerVolume(
            _gameOverPlayer,
            _gameOverVolumeDb);
    }

    private static void SetPlayerVolume(
        AudioStreamPlayer? player,
        float baseVolumeDb)
    {
        if (player == null)
            return;

        if (!GodotObject.IsInstanceValid(player))
            return;

        player.VolumeDb =
            ScaleDbByBgmSetting(
                baseVolumeDb);
    }

    // ============================================================
    // BATTLE MUSIC
    // ============================================================

    public static void Play(
        ulong netId,
        int actIndex,
        RoomType roomType)
    {
        if (!TearConfig.EnableCustomBattleMusic)
            return;

        if (!IsLocalPlayer(netId))
            return;

        StopBattleMusic();
        StopFanfare();
        StopGameOverMusic();

        StopVanillaMusicOnly();

        MusicTrack[] pool =
            GetPool(
                actIndex,
                roomType);

        if (pool.Length == 0)
        {
            ResumeVanillaMusic();
            return;
        }

        MusicTrack track =
            pool[
                Random.Shared.Next(
                    pool.Length)];

        AudioStream? stream =
            GD.Load<AudioStream>(
                track.Path);

        if (stream == null)
        {
            GD.PrintErr(
                $"Failed to load Tear battle music: " +
                $"{track.Path}");

            ResumeVanillaMusic();
            return;
        }

        if (stream is AudioStreamMP3 mp3)
        {
            mp3.Loop = true;
        }

        _battleVolumeDb =
            track.VolumeDb;

        var player =
            new AudioStreamPlayer
            {
                Name = "TearBattleMusic",
                Stream = stream,
                Bus = "Master",
                VolumeDb =
                    ScaleDbByBgmSetting(
                        track.VolumeDb)
            };

        _battlePlayer =
            player;

        player.Finished += () =>
        {
            if (ReferenceEquals(
                    _battlePlayer,
                    player))
            {
                _battlePlayer = null;
            }

            if (GodotObject.IsInstanceValid(player))
            {
                player.QueueFree();
            }
        };

        Node? parent =
            NRun.Instance;

        if (parent == null)
            parent = NGame.Instance;

        if (parent == null)
        {
            _battlePlayer = null;

            player.QueueFree();

            ResumeVanillaMusic();
            return;
        }

        parent.AddChild(player);
        player.Play();

        GD.Print(
            $"Playing looping Tear music: " +
            $"{track.Path} at {track.VolumeDb} dB");
    }

    private static void StopBattleMusic()
    {
        StopPlayer(
            ref _battlePlayer);

        _battleVolumeDb = 0f;
    }

    // ============================================================
    // VICTORY FANFARE
    // ============================================================

    public static void PlayVictoryFanfare(
        ulong netId,
        bool isBoss)
    {
        if (!TearConfig.EnableCustomVictoryMusic)
            return;

        if (!IsLocalPlayer(netId))
            return;

        StopBattleMusic();
        StopFanfare();
        StopGameOverMusic();

        StopVanillaMusicOnly();

        MusicTrack track =
            isBoss
                ? VictoryFanfare
                : VictoryFanfareShort;

        AudioStream? stream =
            GD.Load<AudioStream>(
                track.Path);

        if (stream == null)
        {
            GD.PrintErr(
                $"Failed to load Tear victory fanfare: " +
                $"{track.Path}");

            return;
        }

        _fanfareVolumeDb =
            track.VolumeDb;

        var player =
            new AudioStreamPlayer
            {
                Name = "TearVictoryFanfare",
                Stream = stream,
                Bus = "Master",
                VolumeDb =
                    ScaleDbByBgmSetting(
                        track.VolumeDb)
            };

        _fanfarePlayer =
            player;

        player.Finished += () =>
        {
            if (ReferenceEquals(
                    _fanfarePlayer,
                    player))
            {
                _fanfarePlayer = null;
            }

            if (GodotObject.IsInstanceValid(player))
            {
                player.QueueFree();
            }
        };

        Node? parent =
            NRun.Instance;

        if (parent == null)
            parent = NGame.Instance;

        if (parent == null)
        {
            _fanfarePlayer = null;

            player.QueueFree();
            return;
        }

        parent.AddChild(player);
        player.Play();

        GD.Print(
            $"Playing Tear victory fanfare: " +
            $"{track.Path} at {track.VolumeDb} dB");
    }

    private static void StopFanfare()
    {
        StopPlayer(
            ref _fanfarePlayer);

        _fanfareVolumeDb = 0f;
    }

    // ============================================================
    // GAME OVER
    // ============================================================

    public static void PlayGameOverTheme()
    {
        if (!TearConfig.EnableCustomGameOverMusic)
            return;

        StopBattleMusic();
        StopFanfare();
        StopGameOverMusic();

        StopVanillaMusicOnly();

        MusicTrack track =
            GameOverTheme;

        AudioStream? stream =
            GD.Load<AudioStream>(
                track.Path);

        if (stream == null)
        {
            GD.PrintErr(
                $"Failed to load Tear game over music: " +
                $"{track.Path}");

            return;
        }

        _gameOverVolumeDb =
            track.VolumeDb;

        var player =
            new AudioStreamPlayer
            {
                Name = "TearGameOverMusic",
                Stream = stream,
                Bus = "Master",
                VolumeDb =
                    ScaleDbByBgmSetting(
                        track.VolumeDb)
            };

        _gameOverPlayer =
            player;

        player.Finished += () =>
        {
            if (ReferenceEquals(
                    _gameOverPlayer,
                    player))
            {
                _gameOverPlayer = null;
            }

            if (GodotObject.IsInstanceValid(player))
            {
                player.QueueFree();
            }
        };

        Node? parent =
            NRun.Instance;

        if (parent == null)
            parent = NGame.Instance;

        if (parent == null)
        {
            _gameOverPlayer = null;

            player.QueueFree();
            return;
        }

        parent.AddChild(player);
        player.Play();

        GD.Print(
            $"Playing Tear game over theme: " +
            $"{track.Path} at {track.VolumeDb} dB");
    }

    private static void StopGameOverMusic()
    {
        StopPlayer(
            ref _gameOverPlayer);

        _gameOverVolumeDb = 0f;
    }

    // ============================================================
    // CLEANUP
    // ============================================================

    public static void Stop()
    {
        StopBattleMusic();
        StopFanfare();
        StopGameOverMusic();

        ResumeVanillaMusic();

        GD.Print(
            "Stopped all Tear music");
    }

    private static void StopPlayer(
        ref AudioStreamPlayer? playerField)
    {
        AudioStreamPlayer? player =
            playerField;

        /*
         * Clear the field before stopping the node so any deferred
         * Finished callback cannot treat it as the active player.
         */
        playerField = null;

        if (player == null)
            return;

        if (!GodotObject.IsInstanceValid(player))
            return;

        player.Stop();
        player.QueueFree();
    }

    // ============================================================
    // POOL SELECTION
    // ============================================================

    private static MusicTrack[] GetPool(
        int actIndex,
        RoomType roomType)
    {
        if (roomType == RoomType.Boss)
        {
            return actIndex switch
            {
                1 => Act1Boss,
                2 => Act2Boss,
                _ => Act3Boss
            };
        }

        if (roomType == RoomType.Elite)
        {
            return actIndex switch
            {
                1 => Act1Elite,
                2 => Act2Elite,
                _ => Act3Elite
            };
        }

        return actIndex switch
        {
            1 => Act1Normal,
            2 => Act2Normal,
            _ => Act3Normal
        };
    }

    private static bool IsLocalPlayer(
        ulong netId)
    {
        return RunManager.Instance != null
            && RunManager.Instance.NetService != null
            && netId ==
               RunManager.Instance.NetService.NetId;
    }
}

// ================================================================
// PROCEED CLEANUP
// ================================================================

[HarmonyPatch(
    typeof(NProceedButton),
    "OnRelease")]
public static class StopTearMusicOnProceedPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        BattleTheme.Stop();
    }
}

// ================================================================
// MAIN MENU RESTORE
// ================================================================

[HarmonyPatch(
    typeof(NMainMenu),
    "_Ready")]
public static class RestoreTearMusicOnMainMenuPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        BattleTheme.Stop();
    }
}

// ================================================================
// BGM SLIDER SYNC
// ================================================================

[HarmonyPatch(
    typeof(NAudioManager),
    nameof(NAudioManager.SetBgmVol))]
public static class RefreshTearMusicVolumePatch
{
    [HarmonyPostfix]
    public static void Postfix(
        float __0)
    {
        BattleTheme.RefreshCustomMusicVolumes();
    }
}