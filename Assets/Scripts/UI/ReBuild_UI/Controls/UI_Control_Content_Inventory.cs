using System;
using System.Collections.Generic;
using ToufFrame;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;


public class UI_Control_Content_Inventory : BaseControl
{
    public UI_Panel_Inventory inventoryPanel;
    public List<UI_Control_Slot_Inventory> slots=new List<UI_Control_Slot_Inventory>();
    private GridLayoutGroup gridLayoutGroup;
    public int length;
    public int width
    {
        get => transform.childCount / length+1;
    }
    
    // 在Awake或初始化时调用此方法来初始化slots列表
    public void InitializeSlots()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        length = gridLayoutGroup.constraintCount;
        foreach (Transform child in transform)
        {
            var slot = child.GetComponent<UI_Control_Slot_Inventory>();
            if (slot != null)
            {
                slots.Add(slot);
            }
        }
    }

    public void EnsureFullRow()
    {
        int slotCountNeededToAdd=length-slots.Count%length;
        for (int i = 0; i < slotCountNeededToAdd; i++)
        {
            ExpansionASlotAsync();
        }
    }
    
    // 调用此方法来确保总是有整行的Slot
    public async Task EnsureFullRowAsync(int slotsCount)
    {
        int slotCountNeededToAdd = slotsCount - slots.Count;
        slotCountNeededToAdd += length - slotsCount % length;
        for (int i = 0; i < slotCountNeededToAdd; i++)
        {
            await ExpansionASlotAsync();
        }
    }
    
    // 调整ExpansionASlot方法以异步添加Slot
    public async Task<bool> ExpansionASlotAsync()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(GameStrings.ADDRESSABLES_UI_CONTROL_InventorySlot);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject go = Instantiate(handle.Result, transform);
            go.transform.SetAsLastSibling();
            slots.Add(go.GetComponent<UI_Control_Slot_Inventory>());
            slots.Last().SetPanel(inventoryPanel);
            return true;
        }
        return false;
    }
    
    // 调用此方法来删除多余的空行
    //1. 在此方法之前已经确保整行，从后往前
    //2.当空行数等于一整行时，不做处理，只有整行为空的算空行
    //3.当空行数小于一行，即最后一行全部不为空或只有部分为空时，添加一行空行
    //4. 当空行数大于一行时，删除到只剩一行
    public void AdjustEmptyRows()
    {
        int emptySlotsAtEnd = Enumerable.Reverse(slots).TakeWhile(slot=>slot.IsEmpty).Count();
        int totalEmptyRows = emptySlotsAtEnd / length;
        if (totalEmptyRows == 1) return;
        if (totalEmptyRows == 0)
        {
            for(int i = 0; i < length; i++)
            {
                ExpansionASlotAsync();
            }
            return;
        }
        for (int i = 0; i < totalEmptyRows - 1; i++)
        {
            for (int j = 0; j < length; j++)
            {
                
                Addressables.Release(slots[slots.Count - 1].gameObject);
                Destroy( slots[slots.Count - 1].gameObject);
                slots.RemoveAt(slots.Count - 1);
            }
        }

    }
    
    public async Task<bool> LoadAllItemsAsync(List<SO_Item> list)
    {
        if (list.Count > slots.Count)
        {
            await EnsureFullRowAsync(list.Count);
        }

        int i = 0;
        for (; i < list.Count; i++)
        {
            slots[i].LoadItem(list[i]);
        }
        for(;i<slots.Count;i++)
        {
            slots[i].Clear();
        }
        AdjustEmptyRows();
        return true;
    }
    
    public async Task<bool> LoadTargetItemsAsync(List<SO_Item> list,EItemType itemType)
    {
        List<SO_Item> tempItems = new List<SO_Item>();
        foreach (var item in list)
        {
            if (item.itemType == itemType)
            {
                tempItems.Add(item);
            }
        }
        if (tempItems.Count > slots.Count)
        {
            await EnsureFullRowAsync(tempItems.Count);
        }

        int i = 0;
        for (; i < tempItems.Count; i++)
        {
            slots[i].LoadItem(tempItems[i]);
        }
        for(;i<slots.Count;i++)
        {
            slots[i].Clear();
        }
        AdjustEmptyRows();
        return true;
    }
    
    
    
    public void ReloadSlots(List<SO_Item> list)
    {
        int i = 0;
        for (; i < list.Count; i++)
        {
            slots[i].LoadItem(list[i]);
        }
    }

    public UI_Control_Slot_Inventory GetFirstEmptySlot()
    {
        return slots.Find(s => s.IsEmpty);
    }
    
    public UI_Control_Content_Inventory GetSlot(int index)
    {
        if(index>=0 && index<slots.Count)
        {
            return transform.GetChild(index).GetComponent<UI_Control_Content_Inventory>();
        }

        return null;
    }
    
    public UI_Control_Content_Inventory GetSlot(int x, int y)
    {
        if(x>=0 && x<length && y>=0 && y<width)
        {
            return transform.GetChild(x+y*length).GetComponent<UI_Control_Content_Inventory>();
        }
        return null;
    }
    

   
    
    
    
}