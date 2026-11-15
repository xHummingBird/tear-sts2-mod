using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Tear.TearCode.Character;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Cards.Rare;

public class CastArte() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var artePool = ModelDb.CardPool<TearCardPool>()
            .GetUnlockedCards(
                Owner.UnlockState,
                Owner.RunState.CardMultiplayerConstraint)
            .Where(card => card is IArtes);

        var generatedArte = CardFactory.GetForCombat(
                Owner,
                artePool,
                1,
                Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();

        if (generatedArte == null)
            return;

        if (IsUpgraded && generatedArte.IsUpgradable)
        {
            CardCmd.Upgrade(
                generatedArte,
                CardPreviewStyle.None);
        }

        generatedArte.SetToFreeThisTurn();

        await CardPileCmd.AddGeneratedCardsToCombat(
            [generatedArte],
            PileType.Hand,
            Owner);
    }
}