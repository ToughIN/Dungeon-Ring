using ToufFrame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Control_Slot_Inventory : BaseControl, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private UI_Panel_Inventory _panelInventory;
    private SO_Item _item;
    
    public Image itemIcon;
    public Sprite nullSprite;
    public bool isEmpty = true;
    
    public Image background;
    public Color normalColor=Color.black;
    public Color chosenColor;
    
    public bool IsEmpty
    {
        get { return isEmpty; }
    }
    
    public bool LoadItem(SO_Item item)
    {
        if (item == null)
        {
            Debug.LogError(GetType().Name + ": item is null");
            return false;
        }
        isEmpty = false;
        itemIcon.sprite = item.itemIcon;
        _item = item;
        return true;
    }
    
    public bool SetPanel(UI_Panel_Inventory panelInventory)
    {
        if (panelInventory == null)
        {
            Debug.LogError(GetType().Name + ": panelInventory is null");
            return false;
        }
        _panelInventory = panelInventory;
        return true;
    }

    public bool Clear()
    {
        itemIcon.sprite = nullSprite;
        isEmpty = true;
        return true;
    }
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(IsEmpty)return;
        background.color = chosenColor;
        
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(IsEmpty)return;
        _panelInventory.ShowDescription(_item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isEmpty)return;
        background.color = normalColor;
    }
        
}