using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Uncommon;

public class HolySong() : TearCard(
    0,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self), IFonicHymn
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2m),
        new PowerVar<DexterityPower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.FonicHymn,
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<WindFononPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<HolySongStrengthPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Strength.BaseValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<HolySongDexterityPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Dexterity.BaseValue,
            Owner.Creature,
            this);
        
        await PowerCmd.Apply<WindFononPower>(choiceContext,
        Owner.Creature,
        1,
        Owner.Creature,
        this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Strength.UpgradeValueBy(1m);
        DynamicVars.Dexterity.UpgradeValueBy(1m);
    }
}