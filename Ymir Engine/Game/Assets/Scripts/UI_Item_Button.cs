using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using YmirEngine;

public class UI_Item_Button : YmirComponent
{
    public Item item = null;
    public ITEM_SLOT currentSlot;   // Slot type --> type of item that can be placed

    public string enumItem = "";
    public string enumSlot = "";
    public string enumRarity = "";
    public string menuName = "";

    private GameObject _menuReference;
    public bool updateStats;

    // Debug
    public ITEM_SLOT itemType;
    public ITEM_RARITY itemRarity;
    public float HP, armor, speed, fireRate, reloadSpeed, damageMultiplier;
    public bool isEquipped = false;

    public void Start()
    {
        _menuReference = InternalCalls.GetGameObjectByName(menuName);

        itemType = SetType(enumItem);
        currentSlot = SetType(enumSlot);
        itemRarity = SetRarity(enumRarity);

        item = CreateItemBase();
        //item = new Item(currentSlot, itemType, itemRarity, isEquipped,
        //    /*name*/"Empty",
        //    /*description*/ "Empty",
        //    /*imagePath*/ "");
    }

    public void Update()
    {
        if (updateStats && _menuReference.GetComponent<UI_Inventory>() != null)
        {
            if (item.currentSlot != ITEM_SLOT.NONE)
            {
                UpdateStats();
                _menuReference.GetComponent<UI_Inventory>().UpdateTextStats();
            }

            updateStats = false;
        }

        return;
    }

    public void UpdateInfo()
    {
        if (_menuReference.GetComponent<UI_Inventory>().goDescription != null)
        {
            UI.TextEdit(_menuReference.GetComponent<UI_Inventory>().goText, item.description);
        }

        if (_menuReference.GetComponent<UI_Inventory>().goName != null)
        {
            UI.TextEdit(_menuReference.GetComponent<UI_Inventory>().goName, item.name);
        }
    }

    private ITEM_SLOT SetType(string type)
    {
        ITEM_SLOT elementChanged = ITEM_SLOT.NONE;

        switch (type)
        {
            case "ARMOR":
                elementChanged = ITEM_SLOT.ARMOR;
                break;
            case "CHIP":
                elementChanged = ITEM_SLOT.CHIP;
                break;
            case "CONSUMABLE":
                elementChanged = ITEM_SLOT.CONSUMABLE;
                break;
            case "SAVE":
                elementChanged = ITEM_SLOT.SAVE;
                break;
            case "NONE":
                elementChanged = ITEM_SLOT.NONE;
                break;
            default:
                break;
        }

        return elementChanged;
    }

    private ITEM_RARITY SetRarity(string type)
    {
        ITEM_RARITY elementChanged = ITEM_RARITY.NONE;

        switch (type)
        {
            case "COMMON":
                {
                    elementChanged = ITEM_RARITY.COMMON;
                }
                break;
            case "RARE":
                {
                    elementChanged = ITEM_RARITY.RARE;
                }
                break;
            case "EPIC":
                {
                    elementChanged = ITEM_RARITY.EPIC;
                }
                break;
            case "NONE":
                {
                    elementChanged = ITEM_RARITY.NONE;
                }
                break;
            default:
                break;
        }

        return elementChanged;
    }

    private string SetInspectorType(ITEM_SLOT type) // Set values inspector when item is set
    {
        string elementChanged = " ";

        switch (type)
        {
            case ITEM_SLOT.ARMOR:
                elementChanged = "ARMOR";
                break;
            case ITEM_SLOT.CHIP:
                elementChanged = "CHIP";
                break;
            case ITEM_SLOT.CONSUMABLE:
                elementChanged = "CONSUMABLE";
                break;
            case ITEM_SLOT.SAVE:
                elementChanged = "SAVE";
                break;
            case ITEM_SLOT.NONE:
                elementChanged = "NONE";
                break;
            default:
                break;
        }

        return elementChanged;
    }

    private void UpdateStats() // TODO: cambiar cuando items funcionen en player
    {
        //if (_menuReference != null)
        //{
        //    _menuReference.GetComponent<UI_Inventory>().health.currentHealth += item.HP;
        //    _menuReference.GetComponent<UI_Inventory>().health.maxHealth += item.HP;
        //    _menuReference.GetComponent<UI_Inventory>().health.armor += item.armor;
        //    _menuReference.GetComponent<UI_Inventory>().player.movementSpeed += item.speed;
        //    _menuReference.GetComponent<UI_Inventory>().player.reloadDuration += item.reloadSpeed;
        //    _menuReference.GetComponent<UI_Inventory>().player.fireRate += item.fireRate;
        //    _menuReference.GetComponent<UI_Inventory>().player.damageMultiplier += item.damageMultiplier;
        //}
    }

    public bool SetItem(Item _item)
    {
        currentSlot = SetType(enumSlot);
        itemType = SetType(enumItem);

        item = CreateItemBase();

        //item = new Item(currentSlot, itemType, itemRarity, isEquipped,
        //    /*name*/"Empty",
        //    /*description*/ "Empty",
        //    /*imagePath*/ "");

        bool ret = false;
        Debug.Log("item currentSlot: " + item.currentSlot.ToString());
        Debug.Log("itemType: " + item.itemType.ToString());

        Debug.Log("itemType que le pasas: " + _item.itemType.ToString());
        Debug.Log("isEquipped: " + _item.isEquipped.ToString());
        Debug.Log("Rarity: " + _item.itemRarity.ToString());

        // is empty // is equipped // can be placed
        if (item.itemType == ITEM_SLOT.NONE &&
            ((_item.isEquipped && _item.itemType == item.currentSlot) ||
            item.currentSlot == ITEM_SLOT.NONE))
        {
            item = _item;
            enumSlot = SetInspectorType(item.currentSlot);
            enumItem = SetInspectorType(item.itemType);

            ret = true;

            UI.ChangeImageUI(InternalCalls.CS_GetChild(gameObject.parent, 1), item.imagePath, (int)UI_STATE.NORMAL);

            switch (item.itemRarity) // TODO: Rarity image crashes, error with meta file
            {
                case ITEM_RARITY.COMMON:
                    UI.ChangeImageUI(InternalCalls.CS_GetChild(gameObject.parent, 0), "Assets/UI/Items Slots/Iconos/AcidVesicleIconColor.png", (int)UI_STATE.NORMAL); ;
                    break;
                case ITEM_RARITY.RARE:
                    UI.ChangeImageUI(InternalCalls.CS_GetChild(gameObject.parent, 0), "Assets/UI/Items Slots/Iconos/ExocraniumIconColor.png", (int)UI_STATE.NORMAL);
                    break;
                case ITEM_RARITY.EPIC:
                    UI.ChangeImageUI(InternalCalls.CS_GetChild(gameObject.parent, 0), "Assets/UI/Items Slots/Iconos/BoneIconColor.png", (int)UI_STATE.NORMAL);
                    break;
                case ITEM_RARITY.NONE:
                    UI.ChangeImageUI(InternalCalls.CS_GetChild(gameObject.parent, 0), "Assets/UI/Items Slots/Unselected.png", (int)UI_STATE.NORMAL);
                    break;
                default:
                    break;
            }

            Debug.Log("aaa " + currentSlot.ToString() + " item: " + _item.itemType.ToString());
        }

        Debug.Log("return: " + ret.ToString());
        return ret;
    }

    public Item CreateItemBase()
    {
        Item _item;

        if (itemType == ITEM_SLOT.ARMOR || itemType == ITEM_SLOT.CHIP)
        {
            _item = new I_Equippable(currentSlot, itemType, itemRarity, isEquipped,
            /*name*/"Empty",
            /*description*/ "Empty",
            /*imagePath*/ "",
            HP, armor, speed, fireRate, reloadSpeed, damageMultiplier);
        }
        else if (itemType == ITEM_SLOT.CONSUMABLE)
        {
            _item = new I_Consumables(currentSlot, itemType, itemRarity, isEquipped,
            /*name*/"Empty",
            /*description*/ "Empty",
            /*imagePath*/ "");
        }
        else
        {
            _item = new Item(currentSlot, itemType, itemRarity, isEquipped,
            /*name*/"Empty",
            /*description*/ "Empty",
            /*imagePath*/ "");
        }

        return _item;
    }
}