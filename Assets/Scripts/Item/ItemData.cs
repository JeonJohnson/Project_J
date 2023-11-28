using UnityEngine;
using Enums;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/Items/New Item Data")]
public class ItemData : ScriptableObject
{
    public Item_Type e_item_Type;
    public string item_name;
    public string item_description;
    public string item_explain;

    public Sprite item_sprite;
    public Sprite item_sprite_Big;

    public int attackBonus;
    public int armorBonus;
    public float speedBonus;
    public float fireRateBonus;

    public delegate void SkillDelegate(Player player);
    public SkillDelegate onUseActiveEffect;

    public void EquipItem(Player player)
    {
        if (e_item_Type == Item_Type.Passive)
        {
            player.status.curArmor = armorBonus;
            player.status.walkSpeed += speedBonus;
        }
    }
    public void UnEquipItem(Player player)
    {
        if (e_item_Type == Item_Type.Passive)
        {
            player.status.curArmor = 0;
            player.status.walkSpeed -= speedBonus;
        }
    }
    public void UseActiveEffect(Player player)
    {
        onUseActiveEffect.Invoke(player);
    }
}