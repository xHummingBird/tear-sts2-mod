using MegaCrit.Sts2.Core.Commands;

namespace Tear.TearCode.Extensions;

public static class AudioHelper
{
    public static float voiceVolume = 0.7f;
    
    private static readonly Random rng = new Random();
    
    private static readonly string[] AttackSfx =
    {
        "res://Tear/sounds/attack (1).wav",
        "res://Tear/sounds/attack (2).wav",
        "res://Tear/sounds/attack (3).wav"
    };
    
    private static readonly string[] DamagedSfx =
    {
        "res://Tear/sounds/damaged.wav"
    };
    
    private static readonly string[] HighDamagedSfx =
    {
        "res://Tear/sounds/damaged_high.wav"
    };
    
    private static readonly string[] CriticalDamagedSfx =
    {
        "res://Tear/sounds/damaged_critical.wav"
    };
    
    private static readonly string[] VictorySfx =
    {
        "res://Tear/sounds/victory (1).wav",
        "res://Tear/sounds/victory (2).wav",
        "res://Tear/sounds/victory (3).wav",
        "res://Tear/sounds/victory (4).wav",
        "res://Tear/sounds/victory (5).wav",
        "res://Tear/sounds/victory (6).wav",
    };

    private static readonly string[] GameoverSfx =
    {
        "res://Tear/sounds/",
    };
    
    private static readonly string[] PhraseSfx =
    {
        "res://Rydia/sounds/magic (1).wav",
        "res://Rydia/sounds/magic (2).wav",
        "res://Rydia/sounds/magic (3).wav",
        "res://Rydia/sounds/magic (4).wav",
    };
    
    public static void PlayRandomAttack()
    {
        PlayRandom(AttackSfx);
    }
    
    public static void PlayRandomPhrase()
    {
        PlayRandom(PhraseSfx);
    }
    
    public static void PlayRandomDamaged()
    {
        PlayRandom(DamagedSfx);
    }

    public static void PlayRandomDamagedHigh()
    {
        PlayRandom(HighDamagedSfx);
    }

    public static void PlayRandomGameover()
    {
        PlayRandom(GameoverSfx);
    }

    public static void PlayRandomDamagedCritical()
    {
        PlayRandom(CriticalDamagedSfx);
    }
    
    public static void PlayRandomVictory()
    {
        PlayRandom(VictorySfx);
    }
    

    public static void PlayRandom(string[] pool)
    {
        int index = rng.Next(pool.Length);
        SfxCmd.Play(pool[index], voiceVolume);
    }
}