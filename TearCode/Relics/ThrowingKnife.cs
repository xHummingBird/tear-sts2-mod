using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Relics;

public class ThrowingKnife : TearRelic
{
    private bool _usedThisCombat;

    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    public override Task BeforeCombatStart()
    {
        _usedThisCombat = false;

        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (_usedThisCombat)
            return false;

        if (card.Owner != Owner)
            return false;

        if (card is not IArtes)
            return false;

        if (card.Pile?.Type is not (PileType.Hand or PileType.Play))
            return false;

        modifiedCost = 0m;

        return modifiedCost != originalCost;
    }

    public override Task AfterCardPlayed(
        MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (_usedThisCombat)
            return Task.CompletedTask;

        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;

        if (cardPlay.Card is not IArtes)
            return Task.CompletedTask;

        _usedThisCombat = true;

        Flash();

        return Task.CompletedTask;
    }
}