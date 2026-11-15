using BaseLib.Abstracts;
using Tear.TearCode.Extensions;
using Godot;

namespace Tear.TearCode.Character;

public class TearCardPool : CustomCardPoolModel
{
    public override string Title => Tear.CharacterId; //This is not a display name.

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();


    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 0.58f;
    public override float S => 0.12f;
    public override float V => 1.0f;

    public override Color DeckEntryCardColor => new("D9DEE3");

    public override bool IsColorless => false;
}