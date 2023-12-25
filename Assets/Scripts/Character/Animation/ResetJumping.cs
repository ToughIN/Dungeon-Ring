using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetJumping : StateMachineBehaviour
{
    private CharacterManager character;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);//ACTUALLY, THE BASE FUNCTION IS EMPTY
        if (character == null)
        {
            character = animator.GetComponentInParent<CharacterManager>();
        }

        if (character.IsOwner)
        {
            character.characterNetWorkManager.isJumping.Value = false;
        }
        //THIS CALLED WHEN AN ACTION ENDS, BECAUSE WE WILL SET EMPTY STATE
        character.isPerformingAction = false;
        character.animator.applyRootMotion = false;
        character.canRotate = true;
        character.canMove = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
