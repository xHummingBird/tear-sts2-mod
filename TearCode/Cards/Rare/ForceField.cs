using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Rare;

public class ForceField() : TearCard(
    2,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self), IFonicHymn
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BufferPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.FonicHymn,
        HoverTipFactory.FromPower<BufferPower>(),
        HoverTipFactory.FromPower<EarthFononPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Tear tear)
        {
            SfxCmd.Play("res://Tear/sounds/hymns/second_hymn.mp3", 0.7f);
            tear.PlayAnimation(ownerCreature, "cast_1");
            SfxCmd.Play("res://Tear/sounds/se/cast_1.mp3");
            await Task.Delay(500);
            tear.PlayAnimation(ownerCreature, "cast_2");
            SfxCmd.Play("res://Tear/sounds/se/cast_2.mp3");
        }

        await PowerCmd.Apply<BufferPower>(
            choiceContext,
            CombatState.Players.Select(p => p.Creature),
            DynamicVars["BufferPower"].IntValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<EarthFononPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}