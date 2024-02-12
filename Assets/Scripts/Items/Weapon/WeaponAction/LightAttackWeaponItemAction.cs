using UnityEngine;


[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
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

        if (!playerPerformingAction.playerLocomotionManager.isGrounded .Value)
        {
            return;
        }
        
        PerfomingLightAttack(playerPerformingAction,weaponPerformingAction);
    }

    private void PerfomingLightAttack(PlayerManager playerPerformingAction, SO_WeaponSoItem soWeaponSoPerformingAction)
    {
        if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon &&
            playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            
            //TODO: More effecient combat system
            if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed ==
                GameStrings.ANIMATION_MAIN_LIGHT_ATTACK_01)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(EAttackType.LightAttack02,
                    GameStrings.ANIMATION_MAIN_LIGHT_ATTACK_02, true);
            }
            else
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(EAttackType.LightAttack01,
                    GameStrings.ANIMATION_MAIN_LIGHT_ATTACK_01, true);
            }
            
        }
        else if(!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(EAttackType.LightAttack01,
                GameStrings.ANIMATION_MAIN_LIGHT_ATTACK_01, true);
        }
        
       
    }
}