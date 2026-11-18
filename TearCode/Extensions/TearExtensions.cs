using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Tear.TearCode.Powers;
using Tear.TearCode.Relics;

namespace Tear.TearCode.Extensions;

public class TearExtensions
{
    public static class CombatHelpers
    {
        private const string DefaultHitVfxScene =
            "res://Tear/scenes/hit.tscn";

        private const string DefaultHitVfx = "hit";

        public const string NoHitVfx = "__NONE__";

        public static async Task TearFakeHit(
            Creature attacker,
            Creature target,
            string? attackSfx = null,
            string? hitSfx = null,
            string? hitVfxScene = null,
            string? hitVfx = null,
            bool playHitVfx = true)
        {
            if (target == null)
                return;

            if (attackSfx != null)
                SfxCmd.Play(attackSfx);

            if (hitSfx != null)
                SfxCmd.Play(hitSfx);

            var tear = attacker.Player.Character as Character.Tear;

            if (playHitVfx && tear != null)
            {
                tear.PlayVfxOnTarget(
                    target,
                    hitVfxScene ?? DefaultHitVfxScene,
                    hitVfx ?? DefaultHitVfx);
            }

            await CreatureCmd.TriggerAnim(
                target,
                "Hit",
                0f);

            if (target.Monster?.HasHurtSfx == true)
                SfxCmd.Play(target.Monster.HurtSfx);
        }
    }
}