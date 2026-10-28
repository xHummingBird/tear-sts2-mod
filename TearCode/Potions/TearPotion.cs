using BaseLib.Abstracts;
using BaseLib.Utils;
using Tear.TearCode.Character;

namespace Tear.TearCode.Potions;

[Pool(typeof(TearPotionPool))]
public abstract class TearPotion : CustomPotionModel;