using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using Tear.TearCode.Cards;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Relics;

public abstract class FonicRelicBase : TearRelic
{
    [SavedProperty]
    public int StoredOverlimit { get; internal set; }

    public virtual int MaxOverlimit => 70;
    
    public decimal StartingCombatHp { get; private set; }

    protected virtual int OverlimitPerTurn => 5;

    protected virtual int OverlimitPerCard => 3;

    protected virtual int OverlimitPerArte => 6;

    protected virtual int OverlimitPerFonicHymn => 9;

    public override bool ShowCounter => false;

    public override Task BeforeCombatStart()
    {
        StartingCombatHp = Owner.Creature.CurrentHp;
        
        OverlimitManager.ResetOverlimit(Owner);

        Status = RelicStatus.Normal;

        var room = Owner.RunState.CurrentRoom;

        BattleTheme.Play(
            RunManager.Instance.NetService.NetId,
            Owner.RunState.CurrentActIndex + 1,
            room.RoomType);

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStartLate(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!CombatManager.Instance.IsInProgress)
            return;

        if (Owner?.Creature == null)
            return;

        if (side != Owner.Creature.Side)
            return;

        if (Owner.Creature.IsDead)
            return;

        int amount = CalculateTurnStartGain();

        amount = ModifyTurnStartOverlimitGain(amount);

        GainOverlimit(amount);

        await OverlimitManager.CheckOverlimitReady(
            Owner.Creature,
            null,
            Owner.Creature,
            null);
    }

    public override async Task AfterCombatVictory(
        CombatRoom room)
    {
        Creature? creature = Owner?.Creature;

        if (creature == null)
            return;

        if (!LocalContext.IsMe(creature))
            return;

        bool isBoss =
            room.RoomType == RoomType.Boss;

        BattleTheme.PlayVictoryFanfare(
            RunManager.Instance.NetService.NetId,
            isBoss);
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!CombatManager.Instance.IsInProgress)
            return;

        CardModel? card = cardPlay.Card;

        if (card == null)
            return;

        if (card.Owner != Owner)
            return;

        int amount;

        /*
         * Fonic Hymns generate the most Overlimit.
         * They take priority in case a card ever implements
         * both IFonicHymn and IArtes.
         */
        if (card is IFonicHymn)
        {
            amount = CalculateFonicHymnGain(card);

            amount = ModifyFonicHymnOverlimitGain(
                card,
                amount);
            if (Owner.Character is Character.Tear tear)
                tear.PlayAnimation(Owner.Creature, "idle_tear");
        }
        else if (card is IArtes)
        {
            amount = CalculateArteGain(card);

            amount = ModifyArteOverlimitGain(
                card,
                amount);
            if (Owner.Character is Character.Tear tear)
                tear.PlayAnimation(Owner.Creature, "idle_tear");
        }
        else
        {
            amount = CalculateCardGain(card);

            amount = ModifyCardOverlimitGain(
                card,
                amount);
        }

        amount = ModifyTotalCardOverlimitGain(
            card,
            amount);

        if (amount <= 0)
            return;

        GainOverlimit(amount);

        await OverlimitManager.CheckOverlimitReady(
            Owner.Creature,
            choiceContext,
            Owner.Creature,
            null);
    }

    protected virtual int CalculateTurnStartGain()
    {
        return OverlimitPerTurn;
    }

    protected virtual int CalculateCardGain(
        CardModel card)
    {
        return OverlimitPerCard;
    }

    protected virtual int CalculateArteGain(
        CardModel card)
    {
        return OverlimitPerArte;
    }

    protected virtual int CalculateFonicHymnGain(
        CardModel card)
    {
        return OverlimitPerFonicHymn;
    }

    // =====================================================================
    // OVERRIDE POINTS
    // =====================================================================

    protected virtual int ModifyTurnStartOverlimitGain(
        int amount)
    {
        return amount;
    }

    protected virtual int ModifyCardOverlimitGain(
        CardModel card,
        int amount)
    {
        return amount;
    }

    protected virtual int ModifyArteOverlimitGain(
        CardModel card,
        int amount)
    {
        return amount;
    }

    protected virtual int ModifyFonicHymnOverlimitGain(
        CardModel card,
        int amount)
    {
        return amount;
    }

    protected virtual int ModifyTotalCardOverlimitGain(
        CardModel card,
        int amount)
    {
        return amount;
    }

    /// <summary>
    /// Called whenever this relic's stored Overlimit changes.
    /// Derived Fonic relics can override this for UI or other effects.
    /// </summary>
    protected virtual void OnOverlimitChanged(
        int previousAmount,
        int newAmount)
    {
    }

    // =====================================================================
    // MANAGER COMMUNICATION
    // =====================================================================

    internal void NotifyOverlimitChanged(
        int previousAmount,
        int newAmount)
    {
        OnOverlimitChanged(
            previousAmount,
            newAmount);
    }

    // =====================================================================
    // HELPERS
    // =====================================================================

    private void GainOverlimit(
        int requestedAmount)
    {
        requestedAmount = Math.Max(
            0,
            requestedAmount);

        if (requestedAmount <= 0)
            return;

        OverlimitManager.GainOverlimit(
            Owner,
            requestedAmount);
    }
}