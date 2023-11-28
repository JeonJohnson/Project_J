using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventroy : MonoBehaviour
{
    private Player player;
    public ItemData curActiveItem;
    public List<ItemData> lastPassiveItem;

    private void Awake()
    {
        player = GetComponent<Player>();

        lastPassiveItem = new List<ItemData>();
    }

    private void Equip(ItemData data)
    {
        switch (data.e_item_Type)
        {
            case Enums.Item_Type.Active:
                {
                    if (curActiveItem != null)
                    {
                        curActiveItem.UnEquipItem(player);
                    }
                    curActiveItem = data;
                    curActiveItem.EquipItem(player);
                    UiController_Proto.Instance.playerUiView.UpdateActiveItem(curActiveItem.item_sprite);
                }
                break;
            case Enums.Item_Type.Passive:
                {
                    lastPassiveItem.Add(data);
                    curActiveItem.EquipItem(player);
                }
                break;
        }
    }

    private void UnEquip(ItemData data)
    {
        switch (data.e_item_Type)
        {
            case Enums.Item_Type.Active:
                {
                    if (curActiveItem != null)
                    {
                        curActiveItem.UnEquipItem(player);
                        curActiveItem = null;
                    }
                }
                break;
            case Enums.Item_Type.Passive:
                {

                    lastPassiveItem.Remove(data);
                }
                break;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            collision.gameObject.GetComponent<Item>().ShowInteractButton(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("À½ ´­·È±º");
                ItemData itemData = collision.gameObject.GetComponent<Item>().itemData;
                Equip(itemData);
                collision.gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            collision.gameObject.GetComponent<Item>().ShowInteractButton(false);
        }
    }
}
