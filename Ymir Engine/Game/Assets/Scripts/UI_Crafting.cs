using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using YmirEngine;

public class UI_Crafting : YmirComponent
{
    private GameObject _selectedGO;
    public GameObject focusedGO, goDescription, goText, goName;

    private bool _show;

    public Player player = null;
    public Health health = null;
    public void Start()
	{
        focusedGO = UI.GetFocused();
        _selectedGO = UI.GetSelected();

        goDescription = InternalCalls.GetChildrenByName(gameObject, "Item Description Image"); // TODO: ARREGLAR-HO, FER SIGUI PARE TEXT

        goText = InternalCalls.GetChildrenByName(gameObject, "Item Description Text");
        goName = InternalCalls.GetChildrenByName(gameObject, "Item Description Name");
        goDescription.SetActive(false);// TODO: when menu opened

        goText.SetActive(false);
        goName.SetActive(false);

        _show = false;

        GetPlayerScript();
        GetHealthScript();

        //SetSlots();
    }

	public void Update()
	{
        if (player == null)
        {
            GetPlayerScript();
        }

        if (player != null && player.setHover)
        {
            Debug.Log("set first");
            goDescription.SetActive(false);// TODO: when menu opened
            goText.SetActive(false);
            goName.SetActive(false);

            player.setHover = false;
        }

        focusedGO = UI.GetFocused();// call this when menu starts or when changed, not efficient rn

        UI_Item_Button cs_UI_Item_Button = focusedGO.GetComponent<UI_Item_Button>();

        if (focusedGO != null)
        {
            if (Input.GetGamepadButton(GamePadButton.RIGHTSHOULDER) == KeyState.KEY_REPEAT)
            {
                if (!_show)
                {
                    _show = true;
                    ShowText(_show);
                }

                UpdateTextPos();
                cs_UI_Item_Button.UpdateInfo();
            }

            else if (Input.GetGamepadButton(GamePadButton.RIGHTSHOULDER) == KeyState.KEY_UP)
            {
                if (_show)
                {
                    _show = false;
                    ShowText(_show);
                }
            }

            if (Input.GetGamepadButton(GamePadButton.A) == KeyState.KEY_DOWN)
            {
                SwitchItems();
            }

            if (Input.GetGamepadButton(GamePadButton.X) == KeyState.KEY_DOWN)
            {
                UI.SetFirstFocused(gameObject);
            }

            //Debug.Log(_cs_UI_Item_Button.item.itemType.ToString());
            //Debug.Log(_cs_UI_Item_Button.item.currentSlot.ToString());
        }

        return;
	}

    private void SwitchItems()
    {
        _selectedGO = UI.GetSelected();

        if (_selectedGO != null)
        {
            //Debug.Log(_selectedGO.GetComponent<UI_Item_Button>().item.itemType.ToString());
            //Debug.Log(_selectedGO.GetComponent<UI_Item_Button>().item.currentSlot.ToString());

            if ((InternalCalls.CompareStringToName(focusedGO.parent, _selectedGO.GetComponent<UI_Item_Button>().item.name)) ||
                (focusedGO.GetComponent<UI_Item_Button>().item.currentSlot == ITEM_SLOT.NONE && focusedGO.GetComponent<UI_Item_Button>().item.itemType == ITEM_SLOT.NONE) ||
                (focusedGO.GetComponent<UI_Item_Button>().item.currentSlot == ITEM_SLOT.SAVE && _selectedGO.GetComponent<UI_Item_Button>().item.currentSlot == ITEM_SLOT.NONE) ||
                (focusedGO.GetComponent<UI_Item_Button>().item.currentSlot == ITEM_SLOT.NONE && _selectedGO.GetComponent<UI_Item_Button>().item.currentSlot == ITEM_SLOT.NONE))
            {
                UI.SwitchPosition(_selectedGO.parent, focusedGO.parent);

                //_show = false;
                //_focusedGO.GetComponent<UI_Item_Button>().ShowInfo(_show);
                //_selectedGO.GetComponent<UI_Item_Button>().ShowInfo(_show);

                ITEM_SLOT aux = _selectedGO.GetComponent<UI_Item_Button>().item.currentSlot;
                _selectedGO.GetComponent<UI_Item_Button>().item.currentSlot = focusedGO.GetComponent<UI_Item_Button>().item.currentSlot;
                focusedGO.GetComponent<UI_Item_Button>().item.currentSlot = aux;

                focusedGO.GetComponent<UI_Item_Button>().updateStats = true;
                _selectedGO.GetComponent<UI_Item_Button>().updateStats = true;
            }

            else
            {
                // maybe error sound?
            }

            UI.SetUIState(_selectedGO, (int)UI_STATE.NORMAL);
            //UI.SetUIState(focusedGO, (int)UI_STATE.NORMAL);
        }
    }

    void UpdateTextPos() // Place the descrition game object on the selected GO
    {
        UI.SetUIPosWithOther(goDescription, focusedGO.parent);// TODO: ARREGLAR - HO, FER SIGUI PARE TEXT
        UI.SetUIPosWithOther(goText, focusedGO.parent);
        UI.SetUIPosWithOther(goName, focusedGO.parent);
    }

    public void ShowText(bool isActive) // Show description of item when pressing R1
    {
        goDescription.SetActive(isActive);
        goText.SetActive(isActive);// TODO: ARREGLAR - HO, FER SIGUI PARE TEXT
        goName.SetActive(isActive);
    }

    private void GetPlayerScript()
    {
        GameObject gameObject = InternalCalls.GetGameObjectByName("Player");

        if (gameObject != null)
        {
            player = gameObject.GetComponent<Player>();
        }
    }

    private void GetHealthScript()
    {
        GameObject gameObject = InternalCalls.GetGameObjectByName("Player");

        if (gameObject != null)
        {
            health = gameObject.GetComponent<Health>();
        }
    }
}