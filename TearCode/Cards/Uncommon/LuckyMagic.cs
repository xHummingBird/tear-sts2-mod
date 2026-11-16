using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Cards.Uncommon;

public class LuckyMagic() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cards = await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.IntValue,
            Owner);

        var card = cards.FirstOrDefault();

        if (card is IArtes)
        {
            card.SetToFreeThisTurn();
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}