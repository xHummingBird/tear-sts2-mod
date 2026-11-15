using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Tear.TearCode.Powers;

public class ResurrectionPower : TearPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(20m)
    ];

    public override bool ShouldDieLate(Creature creature)
    {
        if (creature != Owner)
        {
            return true;
        }

        return false;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        Flash();

        decimal amount = Math.Max(
            1m,
            (decimal)creature.MaxHp *
            (DynamicVars.Heal.BaseValue / 100m));

        await CreatureCmd.Heal(
            creature,
            amount);
        
        await PowerCmd.Apply<ResurrectedPower>(new ThrowingPlayerChoiceContext(), creature, 1m, Owner, null);

        PowerCmd.Remove(this);
    }
}