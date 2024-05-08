﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YmirEngine
{
    public enum UI_STATE
    {
        DISABLED,
        NORMAL,
        FOCUSED,
        PRESSED,
        RELEASE,
        SELECTED,

        NONE
    };

    public enum ITEM_SLOT
    {
        ARMOR,
        CHIP,
        CONSUMABLE,
        MATERIAL,
        SAVE,
        NONE,

        SIZE
    }

    public enum ITEM_RARITY
    {
        COMMON,
        RARE,
        EPIC,
        MYTHIC,

        NONE
    }

    #region DEFINE ITEMS

    public class Item
    {
        public ITEM_SLOT currentSlot; // Where is the item placed in the inventory (manage inventory navigation/switch items), when picked up it should be NONE
        public ITEM_SLOT itemType;
        public ITEM_RARITY itemRarity;

        public string imagePath = ""; // Path to the image of the item itself, must include Assets/ 
        public string name = "";
        public string description = ""; // item description, name not included so that the font of the name can be different from the description, include stats
        public bool isEquipped = false;

        public bool inInventory = false;
        public string dictionaryName = "";

        public Item(
            ITEM_SLOT currentSlot = ITEM_SLOT.NONE,
            ITEM_SLOT itemType = ITEM_SLOT.NONE,
            ITEM_RARITY itemRarity = ITEM_RARITY.COMMON,
            bool isEquipped = false,
            string name = "",
            string description = "",
            string imagePath = "",
            string dictionaryName = "")
        {
            this.currentSlot = currentSlot;
            this.itemType = itemType;
            this.itemRarity = itemRarity;
            this.isEquipped = isEquipped;
            this.name = name;
            this.description = (itemRarity != ITEM_RARITY.NONE) ? ("Rarity: " + itemRarity.ToString() + "\n" + description) : "";
            this.imagePath = imagePath;
            this.dictionaryName = dictionaryName;
        }

        public virtual void UpdateStats()
        {
        }

        public virtual void LogStats()
        {
            Debug.Log("Item " +
             " currentSlot: " + currentSlot.ToString() +
             " itemType: " + itemType.ToString() +
             " itemRarity: " + itemRarity.ToString() +
             " isEquipped: " + isEquipped.ToString() +
             " name: " + name +
             " description: " + description +
             " imagePath: " + imagePath +
             " inInventory: " + inInventory.ToString());
        }
    }

    public class I_Equippable : Item
    {
        public float HP, armor, speed, fireRate, reloadSpeed, damageMultiplier;
        private string v1;
        private string v2;
        private string v3;

        public I_Equippable(
            ITEM_SLOT currentSlot = ITEM_SLOT.NONE,
            ITEM_SLOT itemType = ITEM_SLOT.NONE,
            ITEM_RARITY itemRarity = ITEM_RARITY.COMMON,
            bool isEquipped = false,
            string name = "Empty",
            string description = "Empty",
            string imagePath = "",
            string dictionaryName = "",

            // Equippables
            float HP = 0,
            float armor = 0,
            float speed = 0,
            float fireRate = 0,
            float reloadSpeed = 0,
            float damageMultiplier = 0 // skill dmg
        )
        {
            this.currentSlot = currentSlot;
            this.itemType = itemType;
            this.itemRarity = itemRarity;
            this.isEquipped = isEquipped;
            this.name = name;
            this.description = "Rarity: " + itemRarity.ToString() + "\n" + description;
            this.imagePath = imagePath;
            this.dictionaryName = dictionaryName;

            // Equippables
            this.HP = HP;
            this.armor = armor;
            this.speed = speed;
            this.fireRate = fireRate;
            this.reloadSpeed = reloadSpeed;
            this.damageMultiplier = damageMultiplier;
        }

        public override void UpdateStats()
        {
            int e = (isEquipped) ? 1 : -1;

            Debug.Log("isEquipped " + isEquipped.ToString());

            LogStats();

            Globals.GetPlayerHealthScript().currentHealth += (HP * e);
            Globals.GetPlayerHealthScript().maxHealth += (HP * e);
            Globals.GetPlayerHealthScript().armor += (armor * e);
            Globals.GetPlayerScript().movementSpeed += (speed * e);
            Globals.GetPlayerScript().currentWeapon.reloadTime += (reloadSpeed * e);
            Globals.GetPlayerScript().currentWeapon.fireRate += (fireRate * e);
            Globals.GetPlayerScript().damageMultiplier += (damageMultiplier * e);
        }

        public override void LogStats()
        {
            base.LogStats();

            Debug.Log("I_Equippable " +
             " HP: " + HP.ToString() +
             " armor: " + armor.ToString() +
             " speed: " + speed.ToString() +
             " fireRate: " + fireRate.ToString() +
             " reloadSpeed: " + reloadSpeed.ToString() +
             " damageMultiplier: " + damageMultiplier.ToString()
            );
        }
    }

    public class I_Consumables : Item
    {
        public float dmg, area, time;

        public I_Consumables(
            ITEM_SLOT currentSlot = ITEM_SLOT.NONE,
            ITEM_SLOT itemType = ITEM_SLOT.NONE,
            ITEM_RARITY itemRarity = ITEM_RARITY.COMMON,
            bool isEquipped = false,
            string name = "Empty",
            string description = "Empty",
            string imagePath = "",
            string dictionaryName = "",

            // Consumables
            float dmg = 0,
            float area = 0,
            float time = 0
            )
        {
            this.currentSlot = currentSlot;
            this.itemType = itemType;
            this.itemRarity = itemRarity;
            this.isEquipped = isEquipped;
            this.name = name;
            this.description = "Rarity: " + itemRarity.ToString() + "\n" + description;
            this.imagePath = imagePath;
            this.dictionaryName = dictionaryName;

            // Consumables
            this.dmg = dmg;
            this.area = area;
            this.time = time;
        }

        public override void UpdateStats()
        {
        }

        public override void LogStats()
        {
            base.LogStats();

            Debug.Log("I_Consumables " +
             " dmg: " + dmg.ToString() +
             " area: " + area.ToString() +
             " time: " + time.ToString()
             );
        }
    }
    #endregion

    public class Upgrade
    {
        public string name, description;
        public int cost;
        public bool isUnlocked;
        //UPGRADE upgrade;

        public Upgrade(string name, string description, int cost, bool isUnlocked)
        {
            this.name = name;
            this.description = description;
            this.cost = cost;
            this.isUnlocked = isUnlocked;
        }
    }
}