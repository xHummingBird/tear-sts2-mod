using System.Collections.Generic;
using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Cards.Common;

public class PowerCharge() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        foreach (var card in PileType.Hand
                     .GetPile(Owner)
                     .Cards
                     .Where(card =>
                         card is IArtes &&
                         card.IsUpgradable))
        {
            CardCmd.Upgrade(card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}