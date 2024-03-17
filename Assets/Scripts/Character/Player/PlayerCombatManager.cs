using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCombatManager : CharacterCombatManager
{
    
    private PlayerManager player;
    
    [Header("Player Manager")]
    public SO_WeaponSoItem currentSoWeaponSoBeingUsed;
    public bool canComboWithMainHandWeapon = true;

    
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, SO_WeaponSoItem soWeaponSoPerformingAction)
    {
        if (player.IsOwner)
        {
            weaponAction.AttemptToPerformAction(player,soWeaponSoPerformingAction);
        
            // notify the server we have performed the action, so we perform it from there perspective also
            player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRPC( NetworkManager.Singleton.LocalClientId,weaponAction.actionID,soWeaponSoPerformingAction.itemID);
            
        }
    }

    public virtual void DrainStaminaBasedOnAttack()
    {
        if (!player.IsOwner)
        {
            return;
        }

        if(currentSoWeaponSoBeingUsed==null)
            return;
        
        float staminaDedutected = 0;

        switch (currentAttackType)
        {
            case EAttackType.LightAttack01:
                staminaDedutected = currentSoWeaponSoBeingUsed.baseStaminaCost *
                                    currentSoWeaponSoBeingUsed.lightAttackStaminaCostMultiplier;
                break;
            default:
                break;
        }

        player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDedutected);
    }

    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);
        if (player.IsOwner)
        {
            PlayerCamera.Instance.SetLockCameraHeight(); 
        }
    }


}