using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

public class PlayerInventoryManager : CharacterInventoryManager
{
    public SO_WeaponSoItem currentRightHandWeapon;
    public SO_WeaponSoItem currentLeftHandWeapon;
    
    [Header("Quick Slots")]
    public SO_WeaponSoItem[] weaponsInRightHandSlots = new SO_WeaponSoItem[3];
    public int rightHandWeaponIndex = 0;
    public SO_WeaponSoItem[] weaponsInLeftHandSlots = new SO_WeaponSoItem[3];
    public int leftHandWeaponIndex = 0;

    [Header("Items")] 
    [SerializeField]private List<SO_Item> items=new List<SO_Item>();

    private void Awake()
    {
        
    }
    
    public List<SO_Item> GetItems()
    {
        return items;
    }
    
    public List<string> GetItemsPahtList()
    {
        List<string> pathList= new List<string>();
        foreach (var item in items)
        {
            pathList.Add(item.addressablePath);
        }

        return pathList;
    }

    public void LoadItemsFromPathList(List<string> pathList)
    {
        foreach (var path in pathList)
        {
            Addressables.LoadAssetAsync<SO_Item>(path).Completed += handle =>
            {
                items.Add(handle.Result);
            };
        }
    }
    
}