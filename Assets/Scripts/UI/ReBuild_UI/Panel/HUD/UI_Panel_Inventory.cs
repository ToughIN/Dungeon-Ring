using System;
using ToufFrame;
using UnityEngine;

public partial class UI_Panel_Inventory : BasePanel
{
    public EItemType curLable;

    
    private UI_Control_Content_Inventory _inventoryContent;
    private UI_Control_Content_Inventory InventoryContent
    {
        get
        {
            if (_inventoryContent == null)
            {
                _inventoryContent = BUC_Control_Content_InventorySlots.GetComponent<UI_Control_Content_Inventory>();
                if(_inventoryContent==null)Debug.LogError(GetType().Name+": _inventoryContent is null");
            }
            return _inventoryContent;
        }
    }

    private PlayerInventoryManager _playerInventoryManager;
    
    private UI_Control_ItemDescription_Scroll _itemDescriptionScroll;
    
    public SO_Item curItem;
    
    public PlayerInventoryManager playerInventoryManager
    {
        get
        {
            if (_playerInventoryManager == null)
            {
                _playerInventoryManager = PlayerInputMgr.Instance.player.playerInventoryManager;
                if(_playerInventoryManager==null)Debug.LogError(GetType().Name+": playerInventoryManager is null");
            }
            return _playerInventoryManager;
        }
    }
    
    public UI_Control_ItemDescription_Scroll itemDescriptionScroll
    {
        get
        {
            if (_itemDescriptionScroll == null)
            {
                _itemDescriptionScroll = BUC_Control_Item_Descriptioin.GetComponent<UI_Control_ItemDescription_Scroll>();
                if(_itemDescriptionScroll==null)Debug.LogError(GetType().Name+": itemDescriptionScroll is null");
            }
            return _itemDescriptionScroll;
        }
    }
    
    private void Start()
    {
        InventoryContent.InitializeSlots();
        ShowAllLable();
        
        // 给按钮添加监听事件
        BUC_Button_Lable_All.button.onClick.AddListener(ShowAllLable);
        BUC_Button_Lable_Weapons.button.onClick.AddListener(ShowWeaponLable);
        BUC_Button_Lable_Armors.button.onClick.AddListener(ShowArmorLable);
        BUC_Button_Lable_Consumable.button.onClick.AddListener(ShowConsumableLable);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ShowAllLable();
    }

    public void ShowAllLable()
    {
        if (InventoryContent == null) Debug.LogError(GetType().Name + ": inventoryContent is null");
        InventoryContent.LoadAllItemsAsync(playerInventoryManager.GetItems());
        curLable = EItemType.All;
    }

    public void ShowWeaponLable()
    {
        Debug.Log(" ShowWeaponLable");
        InventoryContent.LoadTargetItemsAsync(playerInventoryManager.GetItems(), EItemType.Weapon);
        curLable = EItemType.Weapon;
    }
    
    public void ShowArmorLable()
    {
        InventoryContent.LoadTargetItemsAsync(playerInventoryManager.GetItems(), EItemType.Armor);
        curLable=EItemType.Armor;
    }
    
    public void ShowConsumableLable()
    {
        InventoryContent.LoadTargetItemsAsync(playerInventoryManager.GetItems(), EItemType.Consumable);
        curLable=EItemType.Consumable;
    }
    
    public void ShowDescription(SO_Item item)
    {
          itemDescriptionScroll.ShowDescription(item);   
    }
}