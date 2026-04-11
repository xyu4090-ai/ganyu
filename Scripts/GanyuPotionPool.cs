using BaseLib.Abstracts;
namespace Ganyu.Scripts;
public class GanyuPotionPool : CustomPotionPoolModel
{
    // 描述中使用的能量图标。大小为24x24。
    public override string? TextEnergyIconPath => "res://Ganyu/images/energy_Ganyu.png";
    // tooltip和卡牌左上角的能量图标。大小为74x74。
    public override string? BigEnergyIconPath => "res://Ganyu/images/energy_Ganyu_big.png";
}