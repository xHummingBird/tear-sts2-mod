using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Character;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Powers;

public class SeventhFonistPower : TearPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner.Player)
            return;

        var artePool = ModelDb.CardPool<TearCardPool>()
            .GetUnlockedCards(
                player.UnlockState,
                player.RunState.CardMultiplayerConstraint)
            .Where(card => card is IArtes)
            .ToList();

        if (artePool.Count == 0)
            return;

        var generatedArtes =
            new CardModel[(int)Amount];

        var rng =
            player.RunState.Rng.CombatCardGeneration;

        for (var i = 0; i < generatedArtes.Length; i++)
        {
            var arte = CardFactory.GetDistinctForCombat(
                    player,
                    artePool,
                    1,
                    rng)
                .First();

            CardCmd.ApplyKeyword(
                arte,
                CardKeyword.Ethereal);

            generatedArtes[i] = arte;
        }

        Flash();

        await CardPileCmd.AddGeneratedCardsToCombat(
            generatedArtes,
            PileType.Hand,
            player);
    }
}