using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
public class HeavyAttackWeaponItemAction : WeaponItemAction
{
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, SO_WeaponSoItem weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction,weaponPerformingAction);
        
        if(!playerPerformingAction.IsOwner)
        {
            return;
        }
        
        //CHECK FOR STOPS
        if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
        {
            return;
        }

        if (!playerPerformingAction.isGrounded .Value)
        {
            return;
        }
        
        PerfomingHeavyAttack(playerPerformingAction,weaponPerformingAction);
    }

    private void PerfomingHeavyAttack(PlayerManager playerPerformingAction, SO_WeaponSoItem soWeaponSoPerformingAction)
    {
        if (playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(EAttackType.HeavyAttack01,
                GameStrings.ANIMATION_MAIN_HEAVY_ATTACK_01, true);
        }

        if (playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
        {
            
        }
    }
        
}