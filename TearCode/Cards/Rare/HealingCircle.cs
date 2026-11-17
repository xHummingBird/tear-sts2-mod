using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;
using Tear.TearCode.Relics;

namespace Tear.TearCode.Cards.Rare;

public class HealingCircle() : TearCard(
    2,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self), IArtes
{
    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new HealVar(2m)
    ];
    
    protected override bool ShouldGlowGoldInternal =>
        Owner.HasPower<WindFononPower>() ||
        Owner.HasPower<LightFononPower>() ||
        Owner.HasPower<OverlimitPower>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.Arte
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null)
            return;

        var hasWindFonon =
            ownerCreature.HasPower<WindFononPower>() ||
            ownerCreature.HasPower<LightFononPower>() ||
            ownerCreature.HasPower<OverlimitPower>();

        if (Owner.Character is Character.Tear tear)
        {
            tear.PlayAnimation(ownerCreature, "cast_1");
            SfxCmd.Play("res://Tear/sounds/se/cast_1.mp3");

            await Task.Delay(500);

            tear.PlayAnimation(ownerCreature, "cast_2");
            SfxCmd.Play("res://Tear/sounds/se/cast_2.mp3");
            SfxCmd.Play("res://Tear/sounds/se/revitalise.mp3");

            if (hasWindFonon)
                SfxCmd.Play(
                    "res://Tear/sounds/fairy_circle.wav",
                    0.7f);
            else
                SfxCmd.Play(
                    "res://Tear/sounds/hurtless_circle.wav",
                    0.7f);
        }

        var selectedCards = await CardSelectCmd.FromHand(
            context: choiceContext,
            player: Owner,
            prefs: new CardSelectorPrefs(
                SelectionScreenPrompt,
                0,
                DynamicVars.Cards.IntValue),
            filter: null,
            source: this);

        var cardsToExhaust = selectedCards.ToList();

        foreach (var card in cardsToExhaust)
        {
            await CardCmd.Exhaust(
                choiceContext,
                card);
        }

        decimal healAmount =
            cardsToExhaust.Count *
            DynamicVars.Heal.BaseValue;

        if (hasWindFonon)
            healAmount *= 2m;

        if (Owner.Character is Character.Tear)
        {
            var fonicRelic = Owner.Relics
                .OfType<FonicRelicBase>()
                .FirstOrDefault();

            if (fonicRelic != null)
            {
                decimal amountBelowStartingHp =
                    fonicRelic.StartingCombatHp -
                    ownerCreature.CurrentHp;

                healAmount = Math.Min(
                    healAmount,
                    Math.Max(0m, amountBelowStartingHp));
            }
        }

        if (healAmount > 0m)
        {
            await CreatureCmd.Heal(
                ownerCreature,
                healAmount);
        }

        if (hasWindFonon)
        {
            await PowerCmd.Remove<WindFononPower>(
                ownerCreature);

            await PowerCmd.Remove<LightFononPower>(
                ownerCreature);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}