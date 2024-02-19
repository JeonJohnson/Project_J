using UnityEngine;
using Enums;
using Structs;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/Items/New Item Data")]
public class Item : ScriptableObject
{
    public Item_Type e_item_Type;
    public string item_name;
    public string item_description;
    public string item_explain;
    public int item_price;

    public Sprite item_sprite;
    public Sprite item_sprite_Big;

    [SerializeField]
    private BonusStatus bonusStatus;

    [HideInInspector]
    public BonusStatus BonusStatus { get { return bonusStatus; } }

    public virtual T Copy<T>() where T : Item
    {
        string name = this.name;
        T clone = Instantiate(this) as T;
        clone.name = name;
        return clone;
    }

    /// <summary>
    /// What happens when the object is picked - override this to add your own behaviors
    /// </summary>
    public virtual bool Pick(Player player) { return true; }

    /// <summary>
    /// What happens when the object is used - override this to add your own behaviors
    /// </summary>
    public virtual bool Use(Player player) { return true; }

    /// <summary>
    /// What happens when the object is equipped - override this to add your own behaviors
    /// </summary>
    public virtual bool Equip(Player player) { return true; }

    /// <summary>
    /// What happens when the object is unequipped (called when dropped) - override this to add your own behaviors
    /// </summary>
    public virtual bool UnEquip(Player player) { return true; }

    /// <summary>
    /// What happens when the object gets swapped for another object
    /// </summary>
    public virtual void Swap(Player player) { }
}