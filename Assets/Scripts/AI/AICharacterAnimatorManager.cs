using System;
using UnityEngine;

public class AICharacterAnimatorManager : CharacterAnimatorManager
{
    private AICharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();
        aiCharacter= GetComponent<AICharacterManager>();
    }

    private void OnAnimatorMove()
    {
        //HOST
        if (aiCharacter.IsOwner)
        {
            if(!aiCharacter.aiCharacterLocomotionManager.isGrounded.Value)
                return;
            Vector3 velocity = aiCharacter.animator.deltaPosition;
            aiCharacter.characterController.Move(velocity);
            aiCharacter.transform.rotation*=aiCharacter.animator.deltaRotation;
        }
        //CLIENT
        else
        {
            if(!aiCharacter.aiCharacterLocomotionManager.isGrounded.Value)
                return;
            Vector3 velocity = aiCharacter.animator.deltaPosition;
            aiCharacter.transform.position=Vector3.SmoothDamp(transform.position,
                aiCharacter.characterNetWorkManager.networkPosition.Value,
                ref aiCharacter.characterNetWorkManager.networkPositionVelocity,
                aiCharacter.characterNetWorkManager.networkPositionSmoothTime);
            aiCharacter.transform.rotation*=aiCharacter.animator.deltaRotation;
        }
        
    }
}