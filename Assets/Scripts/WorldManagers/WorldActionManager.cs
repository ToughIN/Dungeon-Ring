using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ToufFrame;
using UnityEngine.Serialization;

public class WorldActionManager : MonoSingletonBase<WorldActionManager>
{
    [FormerlySerializedAs("WeaponItemAction")] [Header("Weapon Item Actions")] 
    public WeaponItemAction[] weaponItemActions;

    private void Start()
    {
        for(int i=0;i<weaponItemActions.Length;i++)
        {
            weaponItemActions[i].actionID = i;
        }
    }

    public WeaponItemAction GetWeaponItemActionByID(int ID)
    {
        return weaponItemActions.FirstOrDefault(action => action.actionID == ID);
    }
    
}
