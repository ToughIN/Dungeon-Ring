using Unity.VisualScripting;
using UnityEngine;

public class ResetActionFlag : StateMachineBehaviour
{
    private CharacterManager character;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);//ACTUALLY, THE BASE FUNCTION IS EMPTY
        if (character == null)
        {
            character = animator.GetComponentInParent<CharacterManager>();
        }
        
        //THIS CALLED WHEN AN ACTION ENDS, BECAUSE WE WILL SET EMPTY STATE
        character.isPerformingAction = false;
        character.animator.applyRootMotion = false;
        character.canRotate = true;
        character.canMove = true;
        character.characterLocomotionManager.isRolling = false;
        character.characterAnimatorManager.DisableCanDoCombo();
        if (character.IsOwner)
        {
            character.characterNetWorkManager.isJumping.Value = false;
        }

    }
}