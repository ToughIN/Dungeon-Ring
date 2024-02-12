using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    private PlayerManager player;


    
    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    private void OnAnimatorMove()
    {
        if (player.playerAnimatorManager.applyRootMotion)
        {
            Vector3 velocity = player.animator.deltaPosition;
            player.characterController.Move(velocity);
            player.transform.rotation*=player.animator.deltaRotation;
        }
    }
    
    public override void EnableCanDoCombo()
    {
        base.EnableCanDoCombo();
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            player.playerCombatManager.canComboWithMainHandWeapon = true;
        }
        else
        {
            
        }
    }

    public  void SetIsMoving()
    {
        player.animator.SetBool(GameStrings.VARIABLE_ANIMATOR_IsMoving, player.playerNetworkManager.isMoving.Value);
    }
    public override void DisableCanDoCombo()
    {
        base.DisableCanDoCombo();
        player.playerCombatManager.canComboWithMainHandWeapon = false;
    }

}
