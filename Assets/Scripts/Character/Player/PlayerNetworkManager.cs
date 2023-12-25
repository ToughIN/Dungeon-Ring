
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerNetworkManager : CharacterNetWorkManager
{
    private PlayerManager player;
    
    public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character",NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    
    [Header("Equipment")]
    public NetworkVariable<int> currentWeaponBeingUsedID = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    [FormerlySerializedAs("isUsingRightHandWeapon")] public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    [FormerlySerializedAs("isUsingLeftHandWeapon")] public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();

    }

    public void SetCharacterActionHand(bool rightHandAction)
    {
        if (rightHandAction)
        {
            isUsingLeftHand.Value = false;
            isUsingRightHand.Value = true;
        }
        else
        {
            isUsingRightHand.Value = false;
            isUsingLeftHand.Value = true;
        }
    }

    public void SetNewMaxHealthValue(int oldValue, int newValue)
    {
        maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnVitalityLevel(newValue);
        player.PlayerHUDCanvas.uiPanelHudBasic.SetMaxHealthValue(maxHealth.Value);
        currentHealth.Value = maxHealth.Value;
    }
    
    public void SetNewMaxStaminaValue(int oldValue, int newValue)
    {
        maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(newValue);
        player.PlayerHUDCanvas.uiPanelHudBasic.SetMaxStaminaValue(maxStamina.Value);
        currentStamina.Value = maxStamina.Value;
    }

    public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
    {
        SO_WeaponSoItem newSoWeaponSo=Instantiate(WorldItemDataBase.Instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentRightHandWeapon = newSoWeaponSo;
        player.playerEquipmentManager.LoadRightWeapon();

        if (player.IsOwner)
        {
            player.PlayerHUDCanvas.uiPanelHudBasic.SetRightWeaponQuickSlotIcon(newID);
        }
    }
    
    public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
    {
        SO_WeaponSoItem newSoWeaponSo=Instantiate(WorldItemDataBase.Instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentLeftHandWeapon = newSoWeaponSo;
        player.playerEquipmentManager.LoadLeftWeapon();
        if (player.IsOwner)
        {
            player.PlayerHUDCanvas.uiPanelHudBasic.SetLeftWeaponQuickSlotIcon(newID);
        }
    }

    public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
    {
        if (WorldItemDataBase.Instance.GetWeaponByID(newID) == null)
        {
            Debug.Log("oldID: "+oldID+" newID: "+newID);   
        }
        SO_WeaponSoItem newSoWeaponSo=Instantiate(WorldItemDataBase.Instance.GetWeaponByID(newID));
        player.playerCombatManager.currentSoWeaponSoUsed = newSoWeaponSo;
    }
    
    //item actions
    [ServerRpc]
    public void NotifyTheServerOfWeaponActionServerRPC(ulong clientID,int actionID,int weaponID)
    {
        if (IsServer)
        {
            NotifyTheServerOfWeaponActionClientRpc(clientID,actionID,weaponID);
        }
    }

    [ClientRpc]
    public void NotifyTheServerOfWeaponActionClientRpc(ulong clientID,int actionID,int weaponID)
    {
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformWeaponBasedAction(actionID,weaponID);
        }
    }

    public void PerformWeaponBasedAction(int actionID,int weaponID)
    {
        WeaponItemAction weaponAction = WorldActionManager.Instance.GetWeaponItemActionByID(actionID);

        if (weaponAction != null)
        {
            weaponAction.AttemptToPerformAction(player,WorldItemDataBase.Instance.GetWeaponByID(weaponID));
        }
        else
        {
            Debug.LogError("ACTION IS NULL, CANNOT BE PERFORMED");
        }
        
    }
    
    
    
        
    
}