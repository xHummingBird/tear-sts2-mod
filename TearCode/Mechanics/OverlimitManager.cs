using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Powers;
using Tear.TearCode.Relics;

namespace Tear.TearCode.Mechanics;

public static class OverlimitManager
{
    public const int DefaultMaxOverlimit = 100;

    // =====================================================================
    // READ
    // =====================================================================

    private static readonly Dictionary<Player, OverlimitUIData>
        UIData = [];

    public static OverlimitUIData GetDataForUI(
        Player player)
    {
        if (!UIData.TryGetValue(
                player,
                out OverlimitUIData? data))
        {
            data = new OverlimitUIData();

            UIData[player] = data;
        }

        return data;
    }
    
    public static int GetOverlimit(
        Player? player)
    {
        FonicRelicBase? relic =
            GetFonicRelic(player);

        return relic?.StoredOverlimit ?? 0;
    }

    public static int GetMaxOverlimit(
        Player? player)
    {
        FonicRelicBase? relic =
            GetFonicRelic(player);

        return relic?.MaxOverlimit
               ?? DefaultMaxOverlimit;
    }

    public static bool IsFull(
        Player? player)
    {
        FonicRelicBase? relic =
            GetFonicRelic(player);

        if (relic == null)
            return false;

        return relic.StoredOverlimit >=
               relic.MaxOverlimit;
    }

    public static bool HasOverlimit(
        Player? player,
        int amount)
    {
        if (amount <= 0)
            return true;

        FonicRelicBase? relic =
            GetFonicRelic(player);

        if (relic == null)
            return false;

        return relic.StoredOverlimit >= amount;
    }

    // =====================================================================
    // SET
    // =====================================================================

    /// <summary>
    /// Sets Overlimit and returns the actual signed change.
    ///
    /// Examples:
    /// 40 to 60 returns 20.
    /// 60 to 40 returns -20.
    /// </summary>
    public static int SetOverlimit(
        Player? player,
        int amount)
    {
        FonicRelicBase? relic =
            GetFonicRelic(player);

        if (relic == null)
            return 0;

        int previousAmount =
            relic.StoredOverlimit;

        int newAmount =
            Math.Clamp(
                amount,
                0,
                relic.MaxOverlimit);

        if (previousAmount == newAmount)
            return 0;

        relic.StoredOverlimit =
            newAmount;

        relic.NotifyOverlimitChanged(
            previousAmount,
            newAmount);

        GetDataForUI(player)
            .NotifyOverlimitChanged(
                newAmount);

        return newAmount - previousAmount;
    }

    // =====================================================================
    // GAIN
    // =====================================================================

    /// <summary>
    /// Adds Overlimit and returns the amount actually gained.
    ///
    /// Adding 10 at 95/100 returns 5.
    /// </summary>
    public static int GainOverlimit(
        Player? player,
        int amount)
    {
        if (amount <= 0)
            return 0;

        FonicRelicBase? relic =
            GetFonicRelic(player);

        if (relic == null)
            return 0;

        int previousAmount =
            relic.StoredOverlimit;

        SetOverlimit(
            player,
            previousAmount + amount);

        return relic.StoredOverlimit -
               previousAmount;
    }

    // =====================================================================
    // SPEND
    // =====================================================================

    /// <summary>
    /// Attempts to spend the complete amount.
    /// Nothing is spent if there is not enough Overlimit.
    /// </summary>
    public static bool TrySpendOverlimit(
        Player? player,
        int amount)
    {
        if (amount <= 0)
            return true;

        FonicRelicBase? relic =
            GetFonicRelic(player);

        if (relic == null)
            return false;

        if (relic.StoredOverlimit < amount)
            return false;

        SetOverlimit(
            player,
            relic.StoredOverlimit - amount);

        return true;
    }

    /// <summary>
    /// Spends up to the requested amount and returns the
    /// amount actually spent.
    /// </summary>
    public static int SpendOverlimit(
        Player? player,
        int amount)
    {
        if (amount <= 0)
            return 0;

        FonicRelicBase? relic =
            GetFonicRelic(player);

        if (relic == null)
            return 0;

        int amountSpent =
            Math.Min(
                relic.StoredOverlimit,
                amount);

        if (amountSpent <= 0)
            return 0;

        SetOverlimit(
            player,
            relic.StoredOverlimit - amountSpent);

        return amountSpent;
    }

    // =====================================================================
    // READY CHECK
    // =====================================================================

    public static async Task CheckOverlimitReady(
        Creature owner,
        PlayerChoiceContext? choiceContext,
        Creature? source,
        CardModel? card)
    {
        if (owner == null)
            return;

        int overlimit =
            GetOverlimit(owner.Player);

        int maxOverlimit =
            GetMaxOverlimit(owner.Player);

        bool isReady =
            overlimit >= maxOverlimit;

        bool alreadyInOverlimit =
            owner.HasPower<OverlimitPower>();

        var powerAmount = 2;
        if (owner.Player.GetRelic<FonicCrest>() != null)
            powerAmount = 3;

        /*
         * OverlimitPower owns the Overlimit lifecycle.
         *
         * This manager only detects when the gauge is full
         * and applies 2 Overlimit.
         *
         * OverlimitPower is responsible for resetting the
         * stored gauge to 0.
         */
        if (isReady && !alreadyInOverlimit)
        {
            await PowerCmd.Apply<OverlimitPower>(
                choiceContext,
                owner,
                powerAmount,
                source,
                card);
        }
    }

    // =====================================================================
    // RESET
    // =====================================================================

    public static void ResetOverlimit(
        Player? player)
    {
        SetOverlimit(
            player,
            0);
    }

    // =====================================================================
    // RELIC LOOKUP
    // =====================================================================

    public static FonicRelicBase?
        GetFonicRelic(
            Player? player)
    {
        if (player == null)
            return null;

        return player.Relics
            .OfType<FonicRelicBase>()
            .FirstOrDefault();
    }
}

public sealed class OverlimitUIData
{
    public event Action<int>? OnOverlimitChanged;

    internal void NotifyOverlimitChanged(
        int amount)
    {
        OnOverlimitChanged?.Invoke(
            amount);
    }
}