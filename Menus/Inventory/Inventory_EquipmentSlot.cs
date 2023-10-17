using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Inventory_EquipmentSlot : MonoBehaviour
{
    private IEquipment _equipmentSourceActor;
    public int SlotIndex;
    public EquipmentItem EquipmentItem;
    private TextMeshProUGUI _stackSizeText;
    private Image _itemIcon;
    private SlotType _slotType;

    public virtual void UpdateSlotUI(IEquipment equipmentSource, EquipmentItem equipmentItem)
    {
        _itemIcon = GetComponent<Image>();
        _stackSizeText = GetComponentInChildren<TextMeshProUGUI>();

        if (equipmentSource is Actor_Base equipmentSourceActor)
        {
            _equipmentSourceActor = equipmentSourceActor;
        }
        
        EquipmentItem = equipmentItem;
        
        if (equipmentItem.ItemID == -1 || equipmentItem.StackSize == 0)
        {
            _itemIcon.sprite = null;
            _stackSizeText.enabled = false;
        }
        else
        {
            _itemIcon.sprite = List_Item.GetItemData(equipmentItem.ItemID).ItemStats.CommonStats.ItemIcon;
            equipmentItem.SlotIndex = SlotIndex;

            if (_stackSizeText != null)
            {
                if (equipmentItem.StackSize > 1)
                {
                    _stackSizeText.text = equipmentItem.StackSize.ToString();
                    _stackSizeText.enabled = true;
                }
                else
                {
                    _stackSizeText.enabled = false;
                }
            }
        }
    }

    public void OnPointerDown()
    {
        if (EquipmentItem.SlotIndex < 0)
        {
            Debug.Log($"EquipmentItem.Slot: {EquipmentItem.SlotIndex} is less than 0... Somehow."); return;
        }

        bool itemEquipped = false;

        if (EquipmentItem.ItemID != -1)
        {
            itemEquipped = true;
        }

        IEquipment equipmentSource = null;

        if (_equipmentSourceActor != null && _equipmentSourceActor is IEquipment equipmentSourceActor)
        {
            equipmentSource = equipmentSourceActor;
        }

        Menu_RightClick.Instance.SlotEquipment(equipmentSource: equipmentSource, equipmentSlot: equipmentSource.EquipmentSlotList[EquipmentItem.SlotIndex], item: List_Item.GetItemData(EquipmentItem.ItemID), itemEquipped: itemEquipped);
    }
}
