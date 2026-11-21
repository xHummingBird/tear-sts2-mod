using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Tear.TearCode.Cards.Ancient;


public class PlayHymn() : TearCard(
    -1,
    CardType.Status,
    CardRarity.Status,
    TargetType.None)
{
    public override bool CanBeGeneratedInCombat => false;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
   
    }

    protected override void OnUpgrade()
    {
        
    }
}