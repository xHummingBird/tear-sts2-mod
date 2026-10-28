using BaseLib.Abstracts;
using Tear.TearCode.Extensions;
using Godot;

namespace Tear.TearCode.Character;

public class TearPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Tear.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}