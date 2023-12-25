using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToufFrame;
using UnityEngine;
using UnityEngine.Serialization;

public class WorldItemDataBase : MonoSingletonBase<WorldItemDataBase>
{
    [FormerlySerializedAs("unarmedWeaponSo")] [FormerlySerializedAs("unarmedWeapon")] public SO_WeaponSoItem unarmedSoWeaponSo;
    
    [Header("Weapons")]
    [SerializeField] private List<SO_WeaponSoItem> weapons = new List<SO_WeaponSoItem>();
    
    
    [Header("Items")]
    private List<SO_Item> items = new List<SO_Item>();
    
    protected override void Awake()
    {
        base.Awake();

        foreach (var weapon in weapons)
        {
            items.Add(weapon);
        }
        for(int i=0;i<items.Count;i++)
        {
            items[i].itemID = i;
        }
    }

    public SO_WeaponSoItem GetWeaponByID(int ID)
    {
        return weapons.FirstOrDefault(weapon=>weapon.itemID==ID) as SO_WeaponSoItem;
    }
}
