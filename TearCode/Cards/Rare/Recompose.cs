using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Character;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Cards.Rare;

public class Recompose() : TearCard(
    0,
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
        var selectedCard = (await CardSelectCmd.FromHand(
                context: choiceContext,
                player: Owner,
                prefs: new CardSelectorPrefs(
                    SelectionScreenPrompt,
                    1),
                filter: null,
                source: this))
            .FirstOrDefault();

        if (selectedCard == null)
            return;

        await CardCmd.Exhaust(
            choiceContext,
            selectedCard);

        var fonicHymnPool = ModelDb.CardPool<TearCardPool>()
            .GetUnlockedCards(
                Owner.UnlockState,
                Owner.RunState.CardMultiplayerConstraint)
            .Where(card => card is IFonicHymn);

        var generatedFonicHymn = CardFactory.GetForCombat(
                Owner,
                fonicHymnPool,
                1,
                Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();

        if (generatedFonicHymn == null)
            return;

        CardCmd.ApplyKeyword(
            generatedFonicHymn,
            CardKeyword.Retain);

        await CardPileCmd.AddGeneratedCardsToCombat(
            [generatedFonicHymn],
            PileType.Hand,
            Owner);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}