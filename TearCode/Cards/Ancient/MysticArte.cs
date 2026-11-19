using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Tear.TearCode.Cards.Rare;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;

namespace Tear.TearCode.Cards.Ancient;

public class MysticArte() : TearCard(
    0,
    CardType.Skill,
    CardRarity.Ancient,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<InnocentShine>(),
        HoverTipFactory.FromCard<FortunesArc>()
    ];

    private bool HasPlayedFonicHymnThisTurn =>
        IsMutable &&
        Owner != null &&
        CombatState != null &&
        CombatManager.Instance.History.CardPlaysFinished.Any(
            entry =>
                entry.HappenedThisTurn(CombatState) &&
                entry.CardPlay.Player == Owner &&
                entry.CardPlay.Card is IFonicHymn);

    protected override bool ShouldGlowGoldInternal =>
        HasPlayedFonicHymnThisTurn;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var innocentShine =
            CombatState.CreateCard<InnocentShine>(Owner);

        if (IsUpgraded)
        {
            CardCmd.Upgrade(innocentShine);
        }

        CardModel selectedCard;

        if (HasPlayedFonicHymnThisTurn)
        {
            var fortunesArc =
                CombatState.CreateCard<FortunesArc>(Owner);

            if (IsUpgraded)
            {
                CardCmd.Upgrade(fortunesArc);
            }

            List<CardModel> cards =
            [
                innocentShine,
                fortunesArc
            ];

            selectedCard =
                await CardSelectCmd.FromChooseACardScreen(
                    choiceContext,
                    cards,
                    Owner,
                    canSkip: false);
        }
        else
        {
            selectedCard = innocentShine;
        }

        selectedCard.ExhaustOnNextPlay = true;

        await CardCmd.AutoPlay(
            choiceContext,
            selectedCard,
            null);
    }

    protected override void OnUpgrade()
    {
    }
}