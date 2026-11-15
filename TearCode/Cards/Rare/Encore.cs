using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Cards.Rare;

public class Encore() : TearCard(
    0,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var fonicHymn = PileType.Discard
            .GetPile(Owner)
            .Cards
            .Where(card => card is IFonicHymn)
            .ToList()
            .StableShuffle(Owner.RunState.Rng.CombatCardSelection)
            .FirstOrDefault();

        if (fonicHymn == null)
            return;

        if (IsUpgraded && fonicHymn.IsUpgradable)
        {
            CardCmd.Upgrade(
                fonicHymn,
                CardPreviewStyle.None);
        }

        fonicHymn.ExhaustOnNextPlay = true;

        await CardCmd.AutoPlay(
            choiceContext,
            fonicHymn,
            null);
    }
}