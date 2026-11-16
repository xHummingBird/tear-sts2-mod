using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Cards.Common;

public class MagicGuard() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var discardedCard = (await CardSelectCmd.FromHandForDiscard(
                context: choiceContext,
                player: Owner,
                prefs: new CardSelectorPrefs(
                    CardSelectorPrefs.DiscardSelectionPrompt,
                    1),
                filter: null,
                source: this))
            .FirstOrDefault();

        if (discardedCard == null)
            return;

        var isArte = discardedCard is IArtes;

        await CardCmd.Discard(
            choiceContext,
            discardedCard);

        await CommonActions.CardBlock(this, play);

        if (isArte)
        {
            await CommonActions.CardBlock(this, play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}