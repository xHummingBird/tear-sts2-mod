using System;
using System.Collections.Generic;
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

public class Revitalise() : TearCard(
    2,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self), IFonicHymn
{
    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(50m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.FonicHymn,
        HoverTipFactory.FromPower<WaterFononPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null)
            return;

        if (Owner.Character is Character.Tear tear)
        {
            SfxCmd.Play("res://Tear/sounds/hymns/fourth_hymn.mp3", 0.7f);
            tear.PlayAnimation(ownerCreature, "cast_1");
            SfxCmd.Play("res://Tear/sounds/se/cast_1.mp3");

            await Task.Delay(500);

            tear.PlayAnimation(ownerCreature, "cast_2");
            SfxCmd.Play("res://Tear/sounds/se/cast_2.mp3");
            SfxCmd.Play("res://Tear/sounds/se/revitalise.mp3");
            SfxCmd.Play("res://Tear/sounds/revitalise.wav", 0.7f);
        }

        decimal missingHp;

        if (Owner.Character is Character.Tear)
        {
            var fonicRelic = Owner.Relics
                .OfType<FonicRelicBase>()
                .FirstOrDefault();

            if (fonicRelic != null)
            {
                missingHp = Math.Max(
                    0m,
                    fonicRelic.StartingCombatHp - ownerCreature.CurrentHp);
            }
            else
            {
                missingHp = Math.Max(
                    0m,
                    ownerCreature.MaxHp - ownerCreature.CurrentHp);
            }
        }
        else
        {
            missingHp = Math.Max(
                0m,
                ownerCreature.MaxHp - ownerCreature.CurrentHp);
        }

        decimal healAmount =
            missingHp * (DynamicVars.Heal.BaseValue / 100m);

        if (healAmount > 0m)
        {
            await CreatureCmd.Heal(
                ownerCreature,
                healAmount);
        }

        await PowerCmd.Apply<WaterFononPower>(
            choiceContext,
            ownerCreature,
            1m,
            ownerCreature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(10m);
        AddKeyword(CardKeyword.Retain);
    }
}