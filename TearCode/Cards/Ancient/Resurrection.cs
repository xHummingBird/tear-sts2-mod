using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Extensions;
using Tear.TearCode.Mechanics;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Ancient;

public class Resurrection() : TearCard(
    3,
    CardType.Skill,
    CardRarity.Ancient,
    TargetType.Self), IArtes
{
    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(5m)
    ];
    
    protected override bool ShouldGlowGoldInternal =>
        Owner.HasPower<WaterFononPower>() ||
        Owner.HasPower<DarkFononPower>() ||
        Owner.HasPower<OverlimitPower>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        TearStaticHoverTips.Arte,
        HoverTipFactory.FromPower<ResurrectionPower>(),
        HoverTipFactory.FromPower<LightFononPower>()
    ];

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (card != this)
            return false;

        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null)
            return false;

        var hasWaterFonon =
            ownerCreature.HasPower<WaterFononPower>() ||
            ownerCreature.HasPower<DarkFononPower>() ||
            ownerCreature.HasPower<OverlimitPower>();

        if (!hasWaterFonon)
            return false;

        modifiedCost = 0m;
        return true;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null)
            return;

        var hasWaterFonon =
            ownerCreature.HasPower<WaterFononPower>() ||
            ownerCreature.HasPower<DarkFononPower>() ||
            ownerCreature.HasPower<OverlimitPower>();

        if (Owner.Character is Character.Tear tear)
        {
            tear.PlayAnimation(ownerCreature, "cast_1");
            SfxCmd.Play("res://Tear/sounds/se/cast_1.mp3");

            await Task.Delay(500);

            tear.PlayAnimation(ownerCreature, "cast_2");
            SfxCmd.Play("res://Tear/sounds/se/cast_2.mp3");
            SfxCmd.Play("res://Tear/sounds/se/revitalise.mp3");
            if (hasWaterFonon)
                SfxCmd.Play("res://Tear/sounds/regenerate.wav", 0.7f);
            else
                SfxCmd.Play("res://Tear/sounds/raise_dead.wav", 0.7f);
        }

        await CreatureCmd.Heal(
            ownerCreature,
            DynamicVars.Heal.BaseValue);

        if (hasWaterFonon)
        {
            foreach (var ally in CombatState.PlayerCreatures)
            {
                if (ally.HasPower<ResurrectionPower>())
                    continue;

                if (ally.HasPower<ResurrectedPower>())
                    continue;

                await PowerCmd.Apply<ResurrectionPower>(
                    choiceContext,
                    ally,
                    1m,
                    ownerCreature,
                    this);
            }
        }
        else
        {
            if (!ownerCreature.HasPower<ResurrectionPower>() &&
                !ownerCreature.HasPower<ResurrectedPower>())
            {
                await PowerCmd.Apply<ResurrectionPower>(
                    choiceContext,
                    ownerCreature,
                    1m,
                    ownerCreature,
                    this);
            }
        }

        await PowerCmd.Apply<LightFononPower>(
            choiceContext,
            ownerCreature,
            1m,
            ownerCreature,
            this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}