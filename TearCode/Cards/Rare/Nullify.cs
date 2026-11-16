using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Tear.TearCode.Cards.Rare;

public class Nullify() : TearCard(
    1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        decimal currentBlock = Owner.Creature.Block;

        if (currentBlock <= 0m)
            return;

        var block = new BlockVar(
            currentBlock,
            ValueProp.Unpowered);
        
        await CreatureCmd.GainBlock(
            Owner.Creature,
            block,
            play);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}