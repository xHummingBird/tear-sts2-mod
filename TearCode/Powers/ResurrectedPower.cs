using MegaCrit.Sts2.Core.Entities.Powers;

namespace Tear.TearCode.Powers;

public class ResurrectedPower : TearPower
{ 
    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.Single;
}