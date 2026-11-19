using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tear.TearCode.Extensions;
using Tear.TearCode.Powers;

namespace Tear.TearCode.Cards.Uncommon;

public class CandleSupply() : TearCard(
    3,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FireFononPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null || play.Target == null)
            return;

        CenterCardCinematic.Start(
            RunManager.Instance.NetService.NetId);

        if (Owner.Character is Character.Tear tear)
        {
            tear.PlayAnimation(
                ownerCreature,
                "cast_1");

            SfxCmd.Play(
                "res://Tear/sounds/se/cast_1.mp3");

            await Task.Delay(500);

            tear.PlayAnimation(
                ownerCreature,
                "cast_2");

            SfxCmd.Play(
                "res://Tear/sounds/se/cast_2.mp3");

            SfxCmd.Play(
                "res://Tear/sounds/candle_supply.wav",
                0.7f);

            tear.PlayVfxOnTarget(
                play.Target,
                "res://Tear/scenes/vfx.tscn",
                "candle_supply");

            SfxCmd.Play(
                "res://Tear/sfx/from_killia/fire_impact (1).wav");

            await Task.Delay(133);
        }
        var vfx = NGroundFireVfx.Create(play.Target);
        if (vfx != null)
        {
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
            SfxCmd.Play("event:/sfx/characters/attack_fire");
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tear/sfx/tear_hit.wav")
            .Execute(choiceContext);

        await PowerCmd.Apply<FireFononPower>(
            choiceContext,
            ownerCreature,
            1m,
            ownerCreature,
            this);

        CenterCardCinematic.End(
            RunManager.Instance.NetService.NetId);
    }

    public override Task AfterCardEnteredCombat(
        CardModel card)
    {
        if (card != this)
            return Task.CompletedTask;

        if (IsClone)
            return Task.CompletedTask;

        var skillsPlayedThisTurn =
            CombatManager.Instance.History.CardPlaysFinished.Count(
                (CardPlayFinishedEntry entry) =>
                    entry.CardPlay.Card.Type == CardType.Skill &&
                    entry.CardPlay.Player == Owner &&
                    entry.HappenedThisTurn(CombatState));

        ReduceCostBy(skillsPlayedThisTurn);

        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;

        if (cardPlay.Card.Type != CardType.Skill)
            return Task.CompletedTask;

        ReduceCostBy(1);

        return Task.CompletedTask;
    }

    private void ReduceCostBy(
        int amount)
    {
        EnergyCost.AddThisTurn(-amount);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}