using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Cards.Ancient;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Relics;

namespace Tear.TearCode.Powers;

public class OverlimitPower : TearPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    private IEnumerable<MysticArte> GetMysticArtesInHand()
    {
        var hand =
            PileType.Hand.GetPile(Owner.Player);

        return hand.Cards
            .OfType<MysticArte>();
    }

    public override async Task AfterApplied(
        Creature? applier,
        CardModel? cardSource)
    {
        var existingCard =
            Owner.Player.PlayerCombatState?
                .AllCards
                .OfType<MysticArte>()
                .FirstOrDefault();

        if (existingCard == null)
        {
            var mysticArte =
                CombatState.CreateCard<MysticArte>(
                    Owner.Player);

            if (Owner.Player.GetRelic<BlueCrystalRod>() != null)
            {
                CardCmd.Upgrade(mysticArte);
            }

            await CardPileCmd.AddGeneratedCardToCombat(
                mysticArte,
                PileType.Hand,
                Owner.Player);
        }
        else
        {
            if (Owner.Player.GetRelic<BlueCrystalRod>() != null &&
                existingCard.IsUpgradable)
            {
                CardCmd.Upgrade(existingCard);
            }

            if (existingCard.Pile?.Type != PileType.Hand)
            {
                await CardPileCmd.Add(
                    existingCard,
                    PileType.Hand);
            }
        }

        var tear =
            Owner.Player.Character as Character.Tear;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Side)
            return;

        if (!participants.Contains(Owner))
            return;

        var player =
            Owner.Player;

        var playerState =
            player.PlayerCombatState;

        if (playerState == null)
            return;

        var mysticArte =
            playerState.AllCards
                .OfType<MysticArte>()
                .FirstOrDefault();

        if (mysticArte == null)
        {
            mysticArte =
                CombatState.CreateCard<MysticArte>(
                    player);

            if (player.GetRelic<BlueCrystalRod>() != null)
            {
                CardCmd.Upgrade(mysticArte);
            }

            await CardPileCmd.AddGeneratedCardToCombat(
                mysticArte,
                PileType.Hand,
                player);

            return;
        }

        if (player.GetRelic<BlueCrystalRod>() != null &&
            mysticArte.IsUpgradable)
        {
            CardCmd.Upgrade(mysticArte);
        }

        if (mysticArte.Pile?.Type != PileType.Hand)
        {
            await CardPileCmd.Add(
                mysticArte,
                PileType.Hand);
        }
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
            return;

        if (!participants.Contains(Owner))
            return;

        if (Owner.GetPowerAmount<OverlimitPower>() <= 1)
        {
            foreach (var mysticArte in
                     GetMysticArtesInHand().ToList())
            {
                await CardCmd.Exhaust(
                    choiceContext,
                    mysticArte);
            }

            OverlimitManager.SetOverlimit(
                Owner.Player,
                0);
        }

        await PowerCmd.Decrement(this);
    }
}